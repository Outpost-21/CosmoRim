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
    [HarmonyPatch(typeof(PassingShip), "CommFloatMenuOption")]
    public static class Patch_PassingShip_CommFloatMenuOption
    {
        [HarmonyPrefix]
        public static bool Prefix(PassingShip __instance, Building_CommsConsole console, Pawn negotiator, ref FloatMenuOption __result)
        {
            string label = "CallOnRadio".Translate(__instance.GetCallLabel());
            if (!__instance.Map.listerBuildings.allBuildingsColonist.Any(b => b.def.HasComp(typeof(Comp_EnhancedBeacon)) && b.TryGetComp<CompPowerTrader>().PowerOn))
            {
                return true;
            }
            else
            {
                Action action = delegate ()
                {
                    console.GiveUseCommsJob(negotiator, __instance);
                };
                __result = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.InitiateSocial, null, null, 0f, null, null), negotiator, console, "ReservedBy");
                return false;
            }
        }
    }
}
