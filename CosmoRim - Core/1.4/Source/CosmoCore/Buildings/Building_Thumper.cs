using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace CosmoCore
{
    public class Building_Thumper : Building
    {
        public CompPowerTrader BetterPowerComp => this.GetComp<CompPowerTrader>();

        public CompFlickable FlickableComp => this.GetComp<CompFlickable>();

        public ThumperStatus status = ThumperStatus.Inactive;

        public int thumpRate = 1200;

        public int thumpTick = 0;

        public int preventedAttacks = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref thumpTick, "thumpTick");
            Scribe_Values.Look<int>(ref preventedAttacks, "preventedAttacks");
            Scribe_Values.Look<ThumperStatus>(ref status, "ThumperStatus");
        }

        public override void Tick()
        {
            base.Tick();

            if(status == ThumperStatus.Active && !BetterPowerComp.PowerOn)
            {
                ShutdownThumper();
            }
            else if(status == ThumperStatus.Inactive && BetterPowerComp.PowerOn)
            {
                StartupThumper();
            }

            if (ThumperRunning())
            {
                thumpTick++;
                if (thumpTick >= thumpRate)
                {
                    ThumpNow();
                    thumpTick = 0;
                }
            }
        }

        public bool ThumperRunning()
        {
            if (BetterPowerComp.PowerOn)
            {
                return true;
            }
            return false;
        }

        public void ThumpNow()
        {
            CellRect radius = GenAdj.OccupiedRect(this.Position, this.Rotation, new IntVec2(2, 2));
            radius = radius.ExpandedBy(1);
            if (CosmoCoreMod.settings.soundEnabled)
            {
                SoundStarter.PlayOneShot(CosmoCoreDefOf.CosRim_Sound_ThumperHit, SoundInfo.InMap(this));
            }

            foreach (IntVec3 cell in radius.Cells)
            {
                FleckMaker.ThrowDustPuff(cell, this.Map, 2f);
            }
        }

        public void StartupThumper()
        {
            SoundStarter.PlayOneShot(CosmoCoreDefOf.CosRim_Sound_ThumperStartup, SoundInfo.InMap(this));
            status = ThumperStatus.Active;
        }

        public void ShutdownThumper()
        {
            SoundStarter.PlayOneShot(CosmoCoreDefOf.CosRim_Sound_ThumperShutdown, SoundInfo.InMap(this));
            status = ThumperStatus.Inactive;
        }

        public enum ThumperStatus
        {
            Inactive,
            Active
        }
    }
}
