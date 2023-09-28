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
    [StaticConstructorOnStartup]
    public static class CosRimAirStartup
    {
        static CosRimAirStartup()
        {
            SetDefaultBiomeExclusions();
        }

        public static void SetDefaultBiomeExclusions()
        {
            CosRimAirSettings settings = CosRimAirMod.settings;
            if (settings.excludedBiomes.NullOrEmpty())
            {
                settings.excludedBiomes = new Dictionary<string, bool>();
            }
            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                if (!settings.excludedBiomes.ContainsKey(biome.defName))
                {
                    if (biome.defName == "AridShrubland" || biome.defName == "Desert" || biome.defName == "ExtremeDesert")
                    {
                        settings.excludedBiomes.Add(biome.defName, false);
                    }
                    else
                    {
                        settings.excludedBiomes.Add(biome.defName, true);
                    }
                }
            }
        }
    }
}
