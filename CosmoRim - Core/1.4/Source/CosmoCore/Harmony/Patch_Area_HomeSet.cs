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
    [HarmonyPatch(typeof(Area_Home), "Set")]
    public static class Patch_Area_Home_Set
    {
        [HarmonyPostfix]
        public static void Postfix(Area_Home __instance)
        {
            foreach (Building building in __instance.Map.listerBuildings.allBuildingsColonist)
            {
                if (building.def.HasComp(typeof(Comp_EnhancedBeacon)))
                {
                    building.GetComp<Comp_EnhancedBeacon>().UpdateTradeRegion();
                }
            }
        }
    }
}
