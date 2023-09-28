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
    public class CompProperties_MachineSounds : CompProperties
    {
        public CompProperties_MachineSounds()
        {
            this.compClass = typeof(Comp_MachineSounds);
        }

        /// <summary>
        /// Sound while on
        /// </summary>
        public SoundDef ambientSound = null;
        /// <summary>
        /// Sound randomly plays every so often.
        /// </summary>
        public SoundDef pingSound = null;
        /// <summary>
        /// Sound plays when powered up.
        /// </summary>
        public SoundDef startupSound = null;
        /// <summary>
        /// Sound plays when powered down.
        /// </summary>
        public SoundDef shutdownSound = null;
    }
}
