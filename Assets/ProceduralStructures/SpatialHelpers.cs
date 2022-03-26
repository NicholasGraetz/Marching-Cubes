using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpatialHelpers
{
    public static Vector3Int LinearIndexTo3D(int linearIndex, Vector3Int maxIndices)
    {
        Vector3Int index3D = new Vector3Int();

        int idx = linearIndex;
        index3D.x = idx % maxIndices.x;
        idx /= maxIndices.x;
        index3D.y = idx % maxIndices.y;
        idx /= maxIndices.y;
        index3D.z = idx;

        return index3D;
    }

    public static int Index3DToLinear(Vector3Int index3D, Vector2Int maxIndicesXY)
    {
        return index3D.x + maxIndicesXY.x * index3D.y + maxIndicesXY.x * maxIndicesXY.y * index3D.z;
    }
}
