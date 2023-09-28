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
    public class Comp_MachineSounds : ThingComp
    {
        public CompProperties_MachineSounds Props => (CompProperties_MachineSounds)this.props;

        public CompPowerTrader compPowerTrader;

        public Sustainer ambientSustainer = null;

        public int pingTick = 3000;

        public bool activeLastTick = false;

        public bool Active => compPowerTrader == null || compPowerTrader.PowerOn;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref pingTick, "pingTick");
            Scribe_Values.Look(ref activeLastTick, "activeLastTick");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if(Props.pingSound != null)
            {
                pingTick = Find.TickManager.TicksGame + 3000;
            }
            compPowerTrader = this.parent.TryGetComp<CompPowerTrader>();
        }

        public override void CompTick()
        {
            base.CompTick();

            TickAudio();
        }

        public void TickAudio()
        {
            if (Props.pingSound != null && Active)
            {
                if (Find.TickManager.TicksGame > pingTick)
                {
                    SoundStarter.PlayOneShot(Props.pingSound, SoundInfo.InMap(this.parent));
                    pingTick = Find.TickManager.TicksGame + 3000;
                }
            }
            if (activeLastTick && !Active)
            {
                if (Props.shutdownSound != null)
                {
                    SoundStarter.PlayOneShot(Props.shutdownSound, SoundInfo.InMap(this.parent));
                }
                if (ambientSustainer != null)
                {
                    ambientSustainer.End();
                }
            }
            if (!activeLastTick && Active)
            {
                if (Props.startupSound != null)
                {
                    SoundStarter.PlayOneShot(Props.startupSound, SoundInfo.InMap(this.parent));
                }
                if (Props.ambientSound != null)
                {
                    ambientSustainer = Props.ambientSound.TrySpawnSustainer(SoundInfo.InMap(this.parent));
                }
            }
            activeLastTick = Active;
        }
    }
}
