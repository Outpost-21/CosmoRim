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
    public class CosmoCoreSettings : ModSettings
    {
        public bool verboseLogging = false;

        // Insectoid Thumper
        public bool soundEnabled = true;
        public float thumperRange = 24f;
        // Orbital Beacon
        public int powerCostOverride = 2;
        public bool pingSound = false;
        public bool ambientSound = true;
        // Air Generator
        public IntRange rainfallRange = new IntRange(500, 1800);
        public Dictionary<string, bool> excludedBiomes = new Dictionary<string, bool>();

        public override void ExposeData()
        {
            base.ExposeData();

            // Insectoid Thumper
            Scribe_Values.Look<bool>(ref soundEnabled, "soundEnabled", true);
            Scribe_Values.Look<float>(ref thumperRange, "thumperRange", 24f);
            // Orbital Beacon
            Scribe_Values.Look<int>(ref powerCostOverride, "powerCostOverride", 2);
            Scribe_Values.Look<bool>(ref pingSound, "pingSound", false);
            Scribe_Values.Look<bool>(ref ambientSound, "ambientSound", true);
            // Air Generator
            Scribe_Values.Look(ref rainfallRange, "rainfallRange", new IntRange(500, 1800));
            Scribe_Collections.Look(ref excludedBiomes, "excludedBiomes");
        }

        public bool IsValidSetting(string input)
        {
            if (GetType().GetFields().Where(p => p.FieldType == typeof(bool)).Any(i => i.Name == input))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<string> GetEnabledSettings
        {
            get
            {
                return GetType().GetFields().Where(p => p.FieldType == typeof(bool) && (bool)p.GetValue(this)).Select(p => p.Name);
            }
        }
    }
}
