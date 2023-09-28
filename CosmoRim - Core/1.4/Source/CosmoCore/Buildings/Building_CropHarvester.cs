using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CosmoCore
{
    public class Building_CropHarvester : Building
    {
        public List<IntVec3> validSpots = new List<IntVec3>();
        public CompPowerTrader powerComp => GetComp<CompPowerTrader>();

        public bool IsPowered => powerComp == null || powerComp.PowerOn;

        public bool outputIsBlocked = false;

        public override void Tick()
        {
            base.Tick();
            if(Find.TickManager.TicksGame % 360 == 0)
            {
                AttemptHarvest();
            }
        }

        public void UpdateValidSpots()
        {
            validSpots.Clear();
            validSpots = CropHarvestUtil.GenCells(Position, Rotation, def.size).ToList();
        }

        public void AttemptHarvest()
        {
            UpdateValidSpots();
            if (IsPowered && !validSpots.NullOrEmpty())
            {
                foreach (IntVec3 p in validSpots)
                {
                    if (p.IsValid)
                    {
                        Plant plant = p.GetPlant(Map);
                        if (plant != null && plant.HarvestableNow)
                        {
                            int num = plant.YieldNow();
                            if (num > 0)
                            {
                                IntVec3? outputCell = GetOutputCell();
                                if(outputCell is null)
                                {
                                    outputIsBlocked = true;
                                    return;
                                }
                                outputIsBlocked = false;
                                Thing thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef);
                                thing.stackCount = num;
                                Find.QuestManager.Notify_PlantHarvested(null, thing);
                                GenPlace.TryPlaceThing(thing, (IntVec3)outputCell, base.Map, ThingPlaceMode.Near);
                            }
                            plant.Destroy();
                        }
                        IPlantToGrowSettable settable = CropHarvestUtil.GetIPlantToGrowSettable(p, Map);
                        if (settable != null)
                        {
                            AttemptToPlantInCell(settable, p);
                        }
                    }
                }
            }
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            yield return new Command_Action
            {
                action = MakeMatchingGrowZone,
                hotKey = KeyBindingDefOf.Misc2,
                defaultDesc = "CommandSunLampMakeGrowingZoneDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Designators/ZoneCreate_Growing"),
                defaultLabel = "CommandSunLampMakeGrowingZoneLabel".Translate()
            };
        }

        public void MakeMatchingGrowZone()
        {
            UpdateValidSpots();
            if (validSpots.NullOrEmpty())
            { return; }
            Designator_ZoneAdd_Growing designator = new Designator_ZoneAdd_Growing();
            designator.DesignateMultiCell(from c in validSpots
                                          where designator.CanDesignateCell(c).Accepted
                                          select c);
        }

        public override string GetInspectString()
        {
            if (outputIsBlocked)
            {
                return "\n" + "CosmoRim.CropHarvesterIsFucked".Translate();
            }
            return base.GetInspectString();
        }

        public IntVec3? GetOutputCell(Building origin = null)
        {
            IntVec3? result = null;
            foreach(Thing t in InteractionCell.GetThingList(Map))
            {
                if(t is Building_CropHarvester harvester)
                {
                    if(origin != null && origin == harvester)
                    {
                        return null;
                    }
                    harvester.GetOutputCell(origin ?? this);
                }
                if(t.def?.IsEdifice() ?? false)
                {
                    return null;
                }
            }
            if(result == null)
            {
                result = InteractionCell;
            }
            return result;
        }

        public void AttemptToPlantInCell(IPlantToGrowSettable settable, IntVec3 p)
        {
            ThingDef plantDef = settable.GetPlantDefToGrow();
            if (plantDef != null && plantDef.CanNowPlantAt(p, Map) && PlantUtility.AdjacentSowBlocker(plantDef, p, Map) == null)
            {
                foreach (Thing t in p.GetThingList(Map))
                {
                    if (t is Plant)
                    {
                        continue;
                    }
                }
                Thing thing = ThingMaker.MakeThing(plantDef);
                GenPlace.TryPlaceThing(thing, p, Map, ThingPlaceMode.Direct);
            }
        }
    }
}
