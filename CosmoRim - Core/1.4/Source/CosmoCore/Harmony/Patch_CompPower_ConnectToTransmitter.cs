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
    [HarmonyPatch(typeof(CompPower), "ConnectToTransmitter")]
    public static class Patch_CompPower_ConnectToTransmitter
    {
        [HarmonyPostfix]
        public static void Postfix(CompPower __instance)
        {
            if (__instance.parent != null && __instance.parent.def.HasComp(typeof(Comp_EnhancedBeacon)))
            {
                __instance.parent.GetComp<Comp_EnhancedBeacon>().UpdateTradeRegion();
            }
        }
    }
}
