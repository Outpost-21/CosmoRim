using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using RimWorld;
using Verse;

using HarmonyLib;
using System.Reflection;

namespace CosmoCore
{

    [HarmonyPatch(typeof(InfestationCellFinder), "GetScoreAt")]
    public static class Patch_InfestationCellFinder_GetScoreAt
    {
        [HarmonyPostfix]
        public static void Postfix(ref float __result, IntVec3 cell, Map map)
        {
            if (__result != 0f)
            {
                Thing thumper = GenClosest.ClosestThing_Global(cell, map.listerThings.ThingsOfDef(CosmoCoreDefOf.CosRim_InsectoidThumper), CosmoCoreDefOf.CosRim_InsectoidThumper.specialDisplayRadius, (Thing t) => ((t as Building).PowerComp as CompPowerTrader).PowerOn);

                if (thumper != null)
                {
                    __result = 0f;
                }
            }
        }
    }
}
