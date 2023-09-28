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

namespace CosmoCore
{
    public class CosmoCoreMod : Mod
    {
        public static CosmoCoreMod mod;
        public static CosmoCoreSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;
        public Dictionary<string, bool> collapsedCategories = new Dictionary<string, bool>();

        internal static string VersionDir => Path.Combine(mod.Content.ModMetaData.RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public CosmoCoreMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<CosmoCoreSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            collapsedCategories.Clear();
            collapsedCategories.Add("Cat_Thumper", false);
            collapsedCategories.Add("Cat_MacroComms", false);
            collapsedCategories.Add("Cat_AirGenerator", false);

            Harmony harmony = new Harmony("Neronix17.CosmoCore.RimWorld");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "CosmoRim";

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
            listing.Note("You will need to restart the game for most of these settings to take effect.", GameFont.Tiny);
            listing.GapLine();
            DoOptions_Defaults(listing);
            DoOptions_Thumper(listing);
            DoOptions_OrbitalBeacon(listing);
            DoOptions_AirGenerator(listing);
        }

        public void DoOptions_Defaults(Listing_Standard listing)
        {
            if (listing.ButtonTextLabeled("Default Settings", "Reset"))
            {
                // Thumper
                settings.soundEnabled = true;
                settings.thumperRange = 24f;
                // Macro Comms
                settings.powerCostOverride = 2;
                settings.pingSound = false;
                settings.ambientSound = true;
                // Air Generator
                settings.rainfallRange = new IntRange(500, 1800);
                settings.excludedBiomes.Clear();
                CosmoCoreStartup.SetDefaultBiomeExclusions(settings);
            }
        }

        public void DoOptions_Thumper(Listing_Standard listing)
        {
            listing.GapLine();
            bool collapsed = collapsedCategories["Cat_Thumper"];
            listing.LabelBackedHeader("Insectoid Thumper", Color.white, ref collapsed);
            collapsedCategories["Cat_Thumper"] = collapsed;
            if (!collapsedCategories["Cat_Thumper"])
            {
                listing.Label("Some changes may require a restart for them to take effect.");
                listing.CheckboxLabeled("Thumper Sound Effect", ref settings.soundEnabled, "Whether or not the continuous 'Thump' is played.");
                listing.AddLabeledSlider($"Range: {settings.thumperRange}", ref settings.thumperRange, 6f, 45f, "Min: 6.0", "Max: 45.0", 0.1f);
                CosmoCoreStartup.RunAdjustments_Thumper(settings);
            }
        }

        public void DoOptions_OrbitalBeacon(Listing_Standard listing)
        {
            listing.GapLine();
            bool collapsed = collapsedCategories["Cat_MacroComms"];
            listing.LabelBackedHeader("Macro Comms Console", Color.white, ref collapsed);
            collapsedCategories["Cat_MacroComms"] = collapsed;
            if (!collapsedCategories["Cat_MacroComms"])
            {
                listing.Label("Energy Cost Per Home Tile:");
                listing.Label("Min: 0, Max: 10, Current: " + settings.powerCostOverride.ToString());
                settings.powerCostOverride = (int)listing.Slider(settings.powerCostOverride, 0, 10);
                listing.CheckboxLabeled("Beacon Ping Sound", ref settings.pingSound, "Enable/Disable ping sound effect, plays occasionally, but could undestandably be annoying for some people.");
                listing.CheckboxLabeled("Beacon Ambient Sound", ref settings.ambientSound, "Enable/Disable ambient sound effect, plays constantly while powered.");
            }
        }

        public void DoOptions_AirGenerator(Listing_Standard listing)
        {
            listing.GapLine();
            bool collapsed = collapsedCategories["Cat_AirGenerator"];
            listing.LabelBackedHeader("Air Generator", Color.white, ref collapsed);
            collapsedCategories["Cat_AirGenerator"] = collapsed;
            if (!collapsed)
            {
                listing.LabelBacked("Rainfall Values", Color.white);
                listing.IntRange(ref settings.rainfallRange, 0, 3000);
                listing.LabelBacked("Biome Restriction", Color.white);
                listing.Note("The list below allows you to restrict the functionality of air generators in specified biomes. Vanilla biomes are accounted for automatically by default, in those the air generator isn't disabled completely but it's significantly less effective and reaches 20% power total.", GameFont.Tiny);
                foreach (BiomeDef biome in DefDatabase<BiomeDef>.AllDefs)
                {
                    DoBiomeCheckbox(listing, biome);
                }
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
