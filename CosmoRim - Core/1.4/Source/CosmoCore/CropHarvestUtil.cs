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
    public static class CropHarvestUtil
    {
        public static IPlantToGrowSettable GetIPlantToGrowSettable(IntVec3 c, Map map)
        {
            Zone zone = c.GetZone(map);
            IPlantToGrowSettable plantToGrowSettable = (from t in c.GetThingList(map)
                                                        where t.def.category == ThingCategory.Building
                                                        where t is IPlantToGrowSettable
                                                        select (IPlantToGrowSettable)t).FirstOrDefault();
            if (plantToGrowSettable != null)
            {
                IPlantToGrowSettable result = plantToGrowSettable;
                return result;
            }
            if (zone is IPlantToGrowSettable result2)
            {
                return result2;
            }
            return null;
        }

        public static IEnumerable<IntVec3> GenCells(IntVec3 center, Rot4 rot, IntVec2 size)
        {
            CellRect rectA = default(CellRect);
            CellRect rectB = default(CellRect);
            if (rot == Rot4.North)
            {
                rectA.minX = center.x - 1;
                rectA.maxX = center.x - 1;
                rectA.minZ = center.z - 1;
                rectA.maxZ = center.z + 2;
                rectB.minX = center.x + 1;
                rectB.maxX = center.x + 1;
                rectB.minZ = center.z - 1;
                rectB.maxZ = center.z + 2;
            }
            else if (rot == Rot4.South)
            {
                rectA.minX = center.x - 1;
                rectA.maxX = center.x - 1;
                rectA.minZ = center.z - 2;
                rectA.maxZ = center.z + 1;
                rectB.minX = center.x + 1;
                rectB.maxX = center.x + 1;
                rectB.minZ = center.z - 2;
                rectB.maxZ = center.z + 1;
            }
            else if (rot == Rot4.East)
            {
                rectA.minZ = center.z - 1;
                rectA.maxZ = center.z - 1;
                rectA.minX = center.x - 1;
                rectA.maxX = center.x + 2;
                rectB.minZ = center.z + 1;
                rectB.maxZ = center.z + 1;
                rectB.minX = center.x - 1;
                rectB.maxX = center.x + 2;
            }
            else
            {
                rectA.minZ = center.z - 1;
                rectA.maxZ = center.z - 1;
                rectA.minX = center.x - 2;
                rectA.maxX = center.x + 1;
                rectB.minZ = center.z + 1;
                rectB.maxZ = center.z + 1;
                rectB.minX = center.x - 2;
                rectB.maxX = center.x + 1;
            }
            for (int z2 = rectA.minZ; z2 <= rectA.maxZ; z2++)
            {
                for (int x = rectA.minX; x <= rectA.maxX; x++)
                {
                    yield return new IntVec3(x, 0, z2);
                }
            }
            for (int z2 = rectB.minZ; z2 <= rectB.maxZ; z2++)
            {
                for (int x = rectB.minX; x <= rectB.maxX; x++)
                {
                    yield return new IntVec3(x, 0, z2);
                }
            }
        }
    }
}
