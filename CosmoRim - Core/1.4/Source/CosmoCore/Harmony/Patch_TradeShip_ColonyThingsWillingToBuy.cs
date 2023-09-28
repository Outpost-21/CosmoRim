using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

using HarmonyLib;

namespace CosmoCore
{
    [HarmonyPatch(typeof(TradeShip), "ColonyThingsWillingToBuy")]
    public static class Patch_TradeShip_ColonyThingsWillingToBuy
    {
        [HarmonyPostfix]
        public static void Postfix(TradeShip __instance, ref IEnumerable<Thing> __result)
        {
            if (__instance.Map.listerBuildings.allBuildingsColonist.Any(b => b.def.HasComp(typeof(Comp_EnhancedBeacon))))
            {
                List<Thing> things = new List<Thing>();
                List<Zone> tradeZones = __instance.Map.zoneManager.AllZones.Where(z => z is Zone_Stockpile).ToList();
                //IEnumerable<Thing> enumerable = from thing in __instance.Map.listerThings.AllThings
                //                                where thing.def.category == ThingCategory.Item
                //                                && TradeUtility.PlayerSellableNow(thing)
                //                                && !thing.Position.Fogged(thing.Map)
                //                                && (thing.Map.areaManager.Home[thing.Position] || thing.IsInAnyStorage())
                //                                select thing;
                //foreach (Thing thing in enumerable)
                //{
                //    things.Add(thing);
                //}
                foreach (Zone zone in tradeZones)
                {
                    things.AddRange(zone.AllContainedThings);
                }
                __result = things.AsEnumerable<Thing>();
            }
        }
    }
}
