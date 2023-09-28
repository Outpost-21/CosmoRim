using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CosmoCore
{
    public class Comp_EnhancedBeacon : ThingComp
    {
        public CompProperties_EnhancedBeacon Props => (CompProperties_EnhancedBeacon)this.props;

        public CompPowerTrader compPowerTrader;

        public bool Active => compPowerTrader == null || compPowerTrader.PowerOn;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            UpdateTradeRegion();

            compPowerTrader = parent.TryGetComp<CompPowerTrader>();
        }

        public void UpdateTradeRegion()
        {
            if (parent.Spawned)
            {
                if (compPowerTrader != null)
                {
                    int tradeZoneCount = 0;
                    List<Zone> tradeZones = parent.Map.zoneManager.AllZones.Where(z => z is Zone_Stockpile).ToList();
                    if (!tradeZones.NullOrEmpty())
                    {
                        foreach (Zone zone in tradeZones)
                        {
                            tradeZoneCount += zone.Cells.Count;
                        }
                    }
                    if (tradeZoneCount < 193)
                    {
                        return;
                    }
                    compPowerTrader.PowerOutput = -(compPowerTrader.Props.basePowerConsumption + (CosmoCoreMod.settings.powerCostOverride * (tradeZoneCount - 193)));
                }
            }
        }
    }
}
