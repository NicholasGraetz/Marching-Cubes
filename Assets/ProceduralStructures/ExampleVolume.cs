using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MarchingCubesProject;
using ProceduralNoiseProject;

public class ExampleVolume : MonoBehaviour
{
    [SerializeField]
    private Material material;

    public MARCHING_MODE mode = MARCHING_MODE.CUBES;

    MeshFilter target;

    //The size of voxel array.
    const int width     = 16;
    const int height    = 16;
    const int length    = 16;

    float[] voxels = new float[width * height * length];

    List<Vector3> verts = new List<Vector3>();
    List<int> indices = new List<int>();

    Marching marching = null;

    void Start()
    {
        for(int i = 0; i < voxels.Length; i++)
        {
            voxels[i] = 1f;//Random.Range(-1f, 1f);

            int idx = i;
            int x = idx % 16;
            idx /= 16;
            int y = idx % 16;
            idx /= 16;
            int z = idx;

            if (Vector3.Distance(new Vector3(x, y, z), new Vector3(8f, 8f, 8f)) < 5f)
                voxels[i] = -1f;
        }

        target = new GameObject().AddComponent<MeshFilter>();
        MeshRenderer mrnd = target.gameObject.AddComponent<MeshRenderer>();
        mrnd.material = material;

        //Set the mode used to create the mesh.
        //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
        if (mode == MARCHING_MODE.TETRAHEDRON)
            marching = new MarchingTertrahedron();
        else
            marching = new MarchingCubes();

        //Surface is the value that represents the surface of mesh
        //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
        //The target value does not have to be the mid point it can be any value with in the range.
        marching.Surface = 0.0f;
    }

    private void Update()
    {
        verts.Clear();
        indices.Clear();

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);
        target.mesh.SetVertices(verts); 
        target.mesh.SetTriangles(indices, 0);
        target.mesh.RecalculateBounds();
        target.mesh.RecalculateNormals();
    }
}
