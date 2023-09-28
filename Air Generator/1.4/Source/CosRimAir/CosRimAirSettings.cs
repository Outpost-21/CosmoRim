using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CosRimAir
{
    public class CosRimAirSettings : ModSettings
    {
        public bool verboseLogging = false;

        public IntRange rainfallRange = new IntRange(500, 1800);

        public Dictionary<string, bool> excludedBiomes = new Dictionary<string, bool>();

        public override void ExposeData()
        {
            base.ExposeData();
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
