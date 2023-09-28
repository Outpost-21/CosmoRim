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
    [StaticConstructorOnStartup]
    public static class CosmoCoreStartup
    {
        static CosmoCoreStartup()
        {
            CosmoCoreSettings s = CosmoCoreMod.settings;

            RunAdjustments_Thumper(s);
            SetDefaultBiomeExclusions(s);
        }

        public static void RunAdjustments_Thumper(CosmoCoreSettings s)
        {
            CosmoCoreDefOf.CosRim_InsectoidThumper.specialDisplayRadius = s.thumperRange;
        }

        public static void SetDefaultBiomeExclusions(CosmoCoreSettings s)
        {
            if (s.excludedBiomes.NullOrEmpty())
            {
                s.excludedBiomes = new Dictionary<string, bool>();
            }
            foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                if (!s.excludedBiomes.ContainsKey(biome.defName))
                {
                    if (biome.defName == "AridShrubland" || biome.defName == "Desert" || biome.defName == "ExtremeDesert")
                    {
                        s.excludedBiomes.Add(biome.defName, false);
                    }
                    else
                    {
                        s.excludedBiomes.Add(biome.defName, true);
                    }
                }
            }
        }
    }
}
