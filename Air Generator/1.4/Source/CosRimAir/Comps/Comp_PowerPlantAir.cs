using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace CosRimAir
{

	[StaticConstructorOnStartup]
	public class Comp_PowerPlantAir : CompPowerPlant
	{
		public int updateAirEveryXTicks = 250;

		public int ticksSinceAirUpdate;

		public float cachedPowerOutput;

		public float spinPosition;

		public static float SpinRateFactor = 0.035f;

		public List<IntVec3> airPathCells = new List<IntVec3>();

		public List<Thing> airPathBlockedByThings = new List<Thing>();

		public List<IntVec3> airPathBlockedCells = new List<IntVec3>();

		public const float PowerReductionPercentPerObstacle = 0.2f;

		public const string TranslateWindPathIsBlockedBy = "WindTurbine_WindPathIsBlockedBy";

		public const string TranslateWindPathIsBlockedByRoof = "WindTurbine_WindPathIsBlockedByRoof";

		public static Vector2 BarSize;

		public static readonly Material WindTurbineBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));

		public static readonly Material WindTurbineBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));

		public static readonly Material TurbineGraphic = MaterialPool.MatFrom("CosmoRim/Buildings/AirGenerator_Fan");

		public static readonly SimpleCurve tempCurve = new SimpleCurve() 
		{ 
			points = new List<CurvePoint>() 
			{ 
				new CurvePoint(0f, 0f), 
				new CurvePoint(1f, 12),
				new CurvePoint(1f, 32f),
				new CurvePoint(0.25f, 42f),
				new CurvePoint(0f, 82f)
			} 
		};

		public override float DesiredPowerOutput => cachedPowerOutput;

		public float PowerPercent => base.PowerOutput / 1000f;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			BarSize = new Vector2(0.65f, 0.14f);
			RecalculateBlockages();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref ticksSinceAirUpdate, "updateCounter", 0);
			Scribe_Values.Look(ref cachedPowerOutput, "cachedPowerOutput", 0f);
		}

		public override void CompTick()
		{
			base.CompTick();
			if (!base.PowerOn)
			{
				cachedPowerOutput = 0f;
				return;
			}
			ticksSinceAirUpdate++;
			if (ticksSinceAirUpdate >= updateAirEveryXTicks)
			{
				float num = Mathf.InverseLerp(CosRimAirMod.settings.rainfallRange.min, CosRimAirMod.settings.rainfallRange.max, parent.Map.TileInfo.rainfall);
                if (!CosRimAirMod.settings.excludedBiomes[parent.Map.Biome.defName])
                {
					num = Mathf.Min(num, 0.2f);
                }
				float num2 = tempCurve.EvaluateInverted(parent.AmbientTemperature);
				ticksSinceAirUpdate = 0;
				cachedPowerOutput = 0f - Props.PowerConsumption * num * num2;
				RecalculateBlockages();
				if (airPathBlockedCells.Count > 0)
				{
					float num3 = 0f;
					for (int i = 0; i < airPathBlockedCells.Count; i++)
					{
						num3 += cachedPowerOutput * 0.2f;
					}
					cachedPowerOutput -= num3;
					if (cachedPowerOutput < 0f)
					{
						cachedPowerOutput = 0f;
					}
				}
			}
			if (cachedPowerOutput > 0.01f)
			{
				spinPosition += PowerPercent * SpinRateFactor;
				if (spinPosition > 2f)
				{
					spinPosition = 0f;
				}
				if (spinPosition > 1f)
                {
					spinPosition -= 1f;
                }
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			DrawFanSpin();
			DrawOutputBar();
		}

		public void DrawFanSpin()
        {
			MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
			propertyBlock.SetColor(ShaderPropertyIDs.Color, Color.white);
			Matrix4x4 matrix = default(Matrix4x4);
			matrix.SetTRS(parent.DrawPos + new Vector3(0, 0.05f, 0.1f), Quaternion.AngleAxis(Mathf.Lerp(0, 350, spinPosition), Vector3.up), new Vector3(3f, 1f, 3f));
			Graphics.DrawMesh(MeshPool.plane10, matrix, TurbineGraphic, 0, null, 0, propertyBlock);
			
        }

		public void DrawOutputBar()
		{
			GenDraw.FillableBarRequest fillableBarRequest = default(GenDraw.FillableBarRequest);
			fillableBarRequest.center = parent.DrawPos + new Vector3(0, 0, -0.35f) + Vector3.up * 0.1f;
			fillableBarRequest.size = BarSize;
			fillableBarRequest.fillPercent = PowerPercent;
			fillableBarRequest.filledMat = WindTurbineBarFilledMat;
			fillableBarRequest.unfilledMat = WindTurbineBarUnfilledMat;
			fillableBarRequest.margin = 0.15f;
			fillableBarRequest.rotation = parent.Rotation;
			GenDraw.DrawFillableBar(fillableBarRequest);
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompInspectStringExtra());
			if (airPathBlockedCells.Count > 0)
			{
				stringBuilder.AppendLine();
				Thing thing = null;
				if (airPathBlockedByThings != null)
				{
					thing = airPathBlockedByThings[0];
				}
				if (thing != null)
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedBy".Translate() + " " + thing.Label);
				}
				else
				{
					stringBuilder.Append("WindTurbine_WindPathIsBlockedByRoof".Translate());
				}
			}
			return stringBuilder.ToString();
		}

		private void RecalculateBlockages()
		{
			if (airPathCells.Count == 0)
			{
				IEnumerable<IntVec3> collection = GenAdjFast.AdjacentCells8Way(parent.Position);
				airPathCells.AddRange(collection);
			}
			airPathBlockedCells.Clear();
			airPathBlockedByThings.Clear();
			for (int i = 0; i < airPathCells.Count; i++)
			{
				IntVec3 intVec = airPathCells[i];
				if (parent.Map.roofGrid.Roofed(intVec))
				{
					airPathBlockedByThings.Add(null);
					airPathBlockedCells.Add(intVec);
					continue;
				}
				List<Thing> list = parent.Map.thingGrid.ThingsListAt(intVec);
				for (int j = 0; j < list.Count; j++)
				{
					Thing thing = list[j];
					if (thing.def.blockWind)
					{
						airPathBlockedByThings.Add(thing);
						airPathBlockedCells.Add(intVec);
						break;
					}
				}
			}
		}
	}
}
