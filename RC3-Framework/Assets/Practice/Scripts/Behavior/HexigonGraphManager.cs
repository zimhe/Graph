using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;

public class HexigonGraphManager : MonoBehaviour
{

    [SerializeField] private SharedEdgeGraph _edgeGraph;
    [SerializeField] private TensileVertex TGVertexPrefab;
    [SerializeField] private TensileEdge TGEdgePrefab;
    [SerializeField] private int countX = 5;
    [SerializeField] private int countZ = 5;
    [SerializeField] private int countY = 5;
    [SerializeField] private int Scale = 1;
    [SerializeField] int DefState;

    private TensileEdge[,] EdgeObj2d;

    private TensileTriangle[] Triangle;
    private int[] TriangleVerts;

    private List<Vector3> P=new List<Vector3>();

    public SharedEdgeGraph _hexGrid()
    {
        return _edgeGraph;
    }

    public int GetCountX()
    {
        return countX;
    }
    public int GetCountY()
    {
        return countY;
    }
    public int GetCountZ()
    {
        return countZ;
    }

    private void Awake()
    {
        _edgeGraph=new SharedEdgeGraph();
        _edgeGraph.Initialize(EdgeGraph.Factory.CreateTriangleGrid3D(countX,countZ,countY));

        _edgeGraph.TensileVertexObjects.AddRange(CreatVertexObjects());
        _edgeGraph.TensileEdgeObjects.AddRange(CreatEdgeObjects());
        ConnectEdgeToVert();

        

        transform.position = new Vector3(-countX * 0.5f, -countY * 0.5f, -countZ * 0.5f);
    }

    void Start()
    {
        Triangle = CreateTriangle().ToArray();
        LineRenderer lr = new LineRenderer();
        for (int i = 0; i < TriangleVerts.Length; i += 2)
        {
            int v0 = TriangleVerts[i];
            int v1 = TriangleVerts[i + 1];

            Debug.DrawLine(P[v0],P[v1]);

   
        }
    }

    IEnumerable<TensileVertex> CreatVertexObjects()
    {
        List<TensileVertex> Obj = new List<TensileVertex>();
        int index = 0;
        foreach (var p in SetVertexPosition())
        {
            var tObj = Instantiate(TGVertexPrefab, transform);
            tObj.transform.localPosition = p;
            tObj.SetupStructure(Scale, DefState, index);
            Obj.Add(tObj);
            index++;
        }
        return Obj;
    }

    IEnumerable<TensileEdge> CreatEdgeObjects()
    {
        List< TensileEdge> EObj=new List<TensileEdge>();


        var graph = _edgeGraph.edgeGraph;

        for (int i = 0; i < graph.EdgeCount; i++)
        {
            int S = graph.GetStartVertex(i);
            int E = graph.GetEndVertex(i);

            int V = S - countX * countZ;

            if (E!=V)
            {
                var eObj = Instantiate(TGEdgePrefab, transform);
                eObj.SetupStructure(Scale, DefState, S, E, SetVertexPosition());
                EObj.Add(eObj);
               // EdgeObj2d[S, E] = eObj;
            }
        }
        return EObj;
    }

    IEnumerable<TensileTriangle> CreateTriangle()
    {
        List<TensileTriangle> T = new List<TensileTriangle>();
      

        int xSize = countX - 1;
        int zSize = countZ - 1;
        int[] triangles = new int[xSize * zSize * 6 * countY];
        for (int y = 0, ti = 0, vi = 0; y < countY; y++)
        {
            for (int z = 0; z < zSize; z++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    if (y % 2 == 0)
                    {
                        //if (x < xSize - 1)
                        {
                            triangles[ti] = vi;
                            triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                            triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                            triangles[ti + 5] = vi + xSize + 2;
                        }
                    }
                    else
                    {
                        triangles[ti] = triangles[ti + 3] = vi;
                        triangles[ti + 1] = vi + xSize + 1;
                        triangles[ti + 2] = triangles[ti + 4] = vi + xSize + 2;
                        triangles[ti + 5] = vi + 1;
                    }
                }
            }
        }

        TriangleVerts = triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            TensileTriangle Tri = new TensileTriangle();
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            print(v0 + "" + v1 + "" + v2);

            Tri.SetupTriangle(v0, v1, v2, P);

           
            T.Add(Tri);
        }
        return T;
    }

    void ConnectEdgeToVert()
    {
        var Verts = _edgeGraph.TensileVertexObjects;

        foreach (var Eg in _edgeGraph.TensileEdgeObjects)
        {
            int S = Eg.Start;
            int E = Eg.End;

            Verts[S].AddEdgeObjs(Eg);
            Verts[E].AddEdgeObjs(Eg);
        }
    }


    IEnumerable<Vector3> SetVertexPosition()
    {
        List<Vector3> _positions = new List<Vector3>();
        for (int y = 0; y < countY; y++)
        {
            // add even row position
            for (int z = 0; z < countZ; z++)
            {
                for (int x = 0; x < countX; x++)
                {
                    float w;
                    if (z % 2 == 0)//even
                    {
                         w = x * Scale;
                    }
                    else//odd
                    {
                         w = x * Scale+0.5f*Scale;
                    }
                   
                    float h = z*Mathf.Sqrt(Mathf.Pow(Scale, 2) - Mathf.Pow(Scale*0.5f, 2));

                    float t = y * (0.42552f/1f)*Scale;//2f * (Scale/(0.5f*Mathf.Sqrt(5f)+2f*Mathf.Sqrt(3f)+0.5f));
                    _positions.Add(new Vector3(w,t,h));
                }
            }
        }
        P.AddRange(_positions);

        return _positions;
    }

    public int[,,] remap3D()
    {
        int[,,] remap=new int[countX,countZ,countY];
        for (int y = 0; y < countY; y++)
        {
            for (int z = 0; z < countZ; z++)
            {
                for (int x = 0; x < countX; x++)
                {
                    int index = x + z * countX + y * countX * countZ;
                    remap[x, z, y] = index;
                }
            }
        }
        return remap;
    }

    public TensileVertex[,,] Vert3D()
    {
        TensileVertex [,,] v3d=new TensileVertex[countX, countZ, countY];
        var VObj = _hexGrid().TensileVertexObjects;

        for (int y = 0; y < countY; y++)
        {
            for (int z = 0; z < countZ; z++)
            {
                for (int x = 0; x < countX; x++)
                {
                    int id = remap3D()[x, z, y];
                    v3d[x, z, y] = VObj[id];
                }
            }
        }

        return v3d;
    }

    // Use this for initialization
   
	
	// Update is called once per frame
	void Update () {
		
	}
}
