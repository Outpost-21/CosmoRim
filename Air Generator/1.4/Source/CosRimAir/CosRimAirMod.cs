using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CosRimAir
{
    public class CosRimAirMod : Mod
    {
        public static CosRimAirMod mod;
        public static CosRimAirSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public CosRimAirMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<CosRimAirSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            Harmony harmony = new Harmony("Neronix17.CosRimAir.RimWorld");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Air Generator";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            if (listing.ButtonText("Default Settings"))
            {
                settings.rainfallRange = new IntRange(500, 1800);
                settings.excludedBiomes.Clear();
                CosRimAirStartup.SetDefaultBiomeExclusions();
            }
            listing.GapLine();
            listing.LabelBacked("Rainfall Values", Color.white);
            listing.IntRange(ref settings.rainfallRange, 0, 3000);
            listing.LabelBacked("Biome Restriction", Color.white);
            listing.Note("The list below allows you to restrict the functionality of air generators in specified biomes. Vanilla biomes are accounted for automatically by default, in those the air generator isn't disabled completely but it's significantly less effective and reaches 20% power total.", GameFont.Tiny);
            foreach(BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
            {
                DoBiomeCheckbox(listing, biome);
            }
        }

        public void DoBiomeCheckbox(Listing_Standard listing, BiomeDef biome)
        {
            bool bufferBool = settings.excludedBiomes[biome.defName];
            listing.CheckboxLabeled(biome.LabelCap, ref bufferBool);
            settings.excludedBiomes[biome.defName] = bufferBool;
        }
    }
}
