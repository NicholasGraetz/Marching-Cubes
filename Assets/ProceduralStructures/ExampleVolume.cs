using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MarchingCubesProject;
using ProceduralNoiseProject;
using static SpatialHelpers;

public class ExampleVolume : MonoBehaviour
{
    public static ExampleVolume Instance { get; private set; }

    [SerializeField]
    private Material material;

    [SerializeField]
    private GameObject visualizationPrefab;

    [SerializeField, Range(0, 16)]
    private int sphereSize = 4;

    public MARCHING_MODE mode = MARCHING_MODE.CUBES;

    MeshFilter target;

    //The size of voxel array.
    const int width     = 16;
    const int height    = 16;
    const int length    = 16;

    public Bounds Bounds => new Bounds() { min = transform.position, max = transform.position + new Vector3(width, height, length)};

    float[] voxels = new float[width * height * length];

    List<Vector3> verts = new List<Vector3>();
    List<int> indices = new List<int>();

    Marching marching = null;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        target = new GameObject().AddComponent<MeshFilter>();
        MeshRenderer mrnd = target.gameObject.AddComponent<MeshRenderer>();
        mrnd.material = material;

        if(visualizationPrefab)
        {
            var instance = Instantiate(visualizationPrefab);
            instance.transform.SetParent(target.transform, false);
            instance.transform.localScale = new Vector3(width, height, length);
        }

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

        Vector3Int bounds = new Vector3Int(width, height, length);
        
        for (int i = 0; i < voxels.Length; i++)
        {
            voxels[i] = 1f;//Random.Range(-1f, 1f);
        
            Vector3Int index3D = LinearIndexTo3D(i, bounds);
        
            if (Vector3Int.Distance(index3D, bounds / 2) < sphereSize)
                voxels[i] = -1f;
        }
    }

    private void Update()
    {
        verts.Clear();
        indices.Clear();

        //Vector3Int bounds = new Vector3Int(width, height, length);
        //
        //for (int i = 0; i < voxels.Length; i++)
        //{
        //    voxels[i] = 1f;//Random.Range(-1f, 1f);
        //
        //    Vector3Int index3D = LinearIndexTo3D(i, bounds);
        //
        //    if (Vector3Int.Distance(index3D, bounds / 2) < sphereSize)
        //        voxels[i] = -1f;
        //}

        //The mesh produced is not optimal. There is one vert for each index.
        //Would need to weld vertices for better quality mesh.
        marching.Generate(voxels, width, height, length, verts, indices);
        target.mesh.SetVertices(verts); 
        target.mesh.SetTriangles(indices, 0);
        target.mesh.RecalculateBounds();
        target.mesh.RecalculateNormals();
    }

    public void PaintVoxel(int index, float value)
    {
        if (index < 0 || index >= voxels.Length) return;
        voxels[index] = Mathf.Clamp(voxels[index] + value, -1f, 1f);
    }

    public void PaintVoxel(Vector3 pos, float value)
    {
        PaintVoxel(Index3DToLinear(new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z), new Vector2Int(width, height)), value);
    }

    public void SpherePaint(Vector3 center, float radiusSqr, float color)
    {
        PaintVoxel(center, color);

        Bounds volumeBounds = Bounds;
        for(float x = Bounds.min.x; x < Bounds.max.x; ++x)
            for(float y = Bounds.min.y; y < Bounds.max.y; ++y)
                for(float z = Bounds.min.z; z < Bounds.max.z; ++z)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if ((center - pos).sqrMagnitude < radiusSqr)
                        PaintVoxel(pos, color);
                }

        return;
    }
}
