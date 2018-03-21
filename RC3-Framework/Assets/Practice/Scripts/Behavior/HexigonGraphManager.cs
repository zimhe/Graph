using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private Button btn;
    [SerializeField] private Button btn2;

    private TensileEdge[,] EdgeObj2d;

    List<TensileEdge> verticalEdges=new List<TensileEdge>();

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


    private List<Vector3> currentPositions;
    private List<Vector3> savedPositions;

    private bool awake;

    private int sizeHolderX = 5;
    private int sizeHolderY = 3;
    private int sizeHolderZ = 5;

    public void SetXSize(string _x)
    {

        int IntX;

        int.TryParse(_x, out IntX);

        if (IntX != 0)
        {
            int xValue = Mathf.Clamp(IntX, 2, 15);
            sizeHolderX = xValue;
        }
    }
    public void SetYSize(string _y)
    {
        int IntY;

        int.TryParse(_y, out IntY);

        print(IntY);

        if (IntY != 0)
        {
            int yValue = Mathf.Clamp(IntY, 1, 15);
            sizeHolderY = yValue;
        }
      
    }
    public void SetZSize(string _z)
    {
        int IntZ;

        int.TryParse(_z, out IntZ);

        if (IntZ != 0)
        {
            int zValue = Mathf.Clamp(IntZ, 2, 15);
            sizeHolderZ = zValue;
        }
    }

    private void UpdateSize()
    {
        countX = sizeHolderX;
        countY = sizeHolderY;
        countZ = sizeHolderZ;
    }

    public void GraphToggleAll()
    {
        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.GraphToggle();
        }

        foreach (var te in _edgeGraph.TensileEdgeObjects)
        {
            te.GraphToggle();
        }

        foreach (var ve in verticalEdges)
        {
            ve.GraphToggle();
        }
    }


    private void Awake()
    {
        //initializeGraph();
        btn.interactable = false;
        btn2.interactable = false;
    }

    public bool GraphReady()
    {
        return awake;
    }

    public void initializeGraph()
    {
        if (awake == false)
        {
            UpdateSize();

            currentPositions = new List<Vector3>(countX * countZ * countY);
            savedPositions = new List<Vector3>(countX * countZ * countY);
            _edgeGraph = new SharedEdgeGraph();
            _edgeGraph.Initialize(EdgeGraph.Factory.CreateTriangleGrid3D(countX, countZ, countY));

            savedPositions.AddRange(SetVertexPosition());
            currentPositions.AddRange(savedPositions);
            _edgeGraph.TensileVertexObjects.AddRange(CreatVertexObjects());
            _edgeGraph.TensileEdgeObjects.AddRange(CreatEdgeObjects());
            _edgeGraph.TensileTriangle.AddRange(CreateTriangle());
            ConnectEdgeToVert();
            ConnectVertexs();
            Triangle = _edgeGraph.TensileTriangle.ToArray();

            enableVertexes();

            getNoise();

            transform.position = new Vector3(-countX * 0.5f, 0f, -countZ * 0.5f);
            awake = true;

            GetComponent<HexigonGrowthManager>().initializeGrowther();
            btn.interactable = true;
            btn2.interactable = true;
        }
   
    }

    void enableVertexes()
    {
        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.EnableVertex();
        }
    }


    void Start()
    {

    }


    private List<float> NoiseX;
    private List<float> NoiseY;
    private List<float> NoiseZ;
    private float Degree = 0f;
    private bool LerpOnOff;

    public void LerpToggle()
    {

       
        if (LerpOnOff == false)
        {
            LerpOnOff = true;
        }
        else
        {
            LerpOnOff = false;
        }
    }

    private int frame = 0;
    public void LerpPosition()
    {
      
        if (LerpOnOff == true)
        {
            if (frame < currentPositions.Count - 1)
            {
                List<Vector3> toLerpPosition = new List<Vector3>(currentPositions.Count);

                for (int i = 0; i < currentPositions.Count; i++)
                {
                    var p = savedPositions[i];

                    float PDx = NoiseX[i] * Degree;
                    float PDy = NoiseY[i] * Degree;
                    float PDz = NoiseZ[i] * Degree;

                    float x = p.x + PDx;
                    float y = p.y + PDy;
                    float z = p.z + PDz;

                    p = new Vector3(x, y, z);
                    toLerpPosition.Add(p);
                }

                for (int i = 0; i < currentPositions.Count; i++)
                {
                    currentPositions[i] = Vector3.Lerp(currentPositions[i], toLerpPosition[i], Time.deltaTime*1.5f);

                    var d = currentPositions[i] - toLerpPosition[i];

                    if (d.magnitude<0.003f)
                    {
                        LerpOnOff = false;
                        updateNoise();
                        LerpOnOff = true;
                    }
                }
            }
            else
            {
                //updateNoise();
            }
          
        }
    }

    void getNoise()
    {
        NoiseX = new List<float>(currentPositions.Count);
        NoiseY = new List<float>(currentPositions.Count);
        NoiseZ = new List<float>(currentPositions.Count);
        for (int i = 0; i < currentPositions.Count; i++)
        {

            float valueX = Random.Range(-0.3f, 0.3f);
            float valueY = Random.Range(-0.3f, 0.3f);
            float valueZ = Random.Range(-0.3f, 0.3f);

            NoiseX.Add(valueX);
            NoiseY.Add(valueY);
            NoiseZ.Add(valueZ);

          
        }
    }

    void updateNoise()
    {
        for (int i = 0; i < currentPositions.Count; i++)
        {

            float valueX = Random.Range(-0.3f, 0.3f);
            float valueY = Random.Range(-0.3f, 0.3f);
            float valueZ = Random.Range(-0.3f, 0.3f);

            NoiseX[i] = valueX;
            NoiseY[i] = valueY;
            NoiseZ[i] = valueZ;
        }
    }

    public  void adjustPosition(float _degree)
    {
        Degree = _degree;
        if (awake == true && LerpOnOff == false)
        {
            for (int i = 0; i < currentPositions.Count; i++)
            {
                var p = savedPositions[i];



                float PDx = NoiseX[i] * _degree;
                float PDy = NoiseY[i] * _degree;
                float PDz = NoiseZ[i] * _degree;

                float x = p.x + PDx;
                float y = p.y + PDy;
                float z = p.z + PDz;

                p = new Vector3(x, y, z);

                currentPositions[i] = p;
            }
        }
    }



    IEnumerable<TensileVertex> CreatVertexObjects()
    {
        List<TensileVertex> Obj = new List<TensileVertex>();
        int index = 0;
        
        foreach (var p in currentPositions)
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

        var vc = _edgeGraph.edgeGraph.VertexCount;

        EdgeObj2d=new TensileEdge[3*vc,3*vc];


        var graph = _edgeGraph.edgeGraph;

        for (int i = 0; i < graph.EdgeCount; i++)
        {
            int S = graph.GetStartVertex(i);
            int E = graph.GetEndVertex(i);

            int V = S - countX * countZ;

            if (E!=V)
            {
                var eObj = Instantiate(TGEdgePrefab, transform);
                eObj.SetupStructure(Scale, DefState, S, E, currentPositions);
                EObj.Add(eObj);
               EdgeObj2d[S, E] = eObj;
            }
            else
            {
                var eObj = Instantiate(TGEdgePrefab, transform);
                eObj.SetupStructure(Scale, DefState, S, E, currentPositions);
                verticalEdges.Add(eObj);
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
        for (int y = 0, ti = 0, vi = 0; y < countY; y++,vi+=countX)
        {
            for (int z = 0; z < zSize; z++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    if (z % 2 == 0)
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

            //print(v0 + "," + v1 + "," + v2);

            Tri.SetupTriangle(v0, v1, v2, currentPositions);
            foreach (var te in Tri.GetTriEdges())
            {
                int S = te[0];
                int E = te[1];

                int D = Tri.GiveDiagonalVertice(S, E);

                Tri.addTensileEdgeObjs(EdgeObj2d[S, E]);
                EdgeObj2d[S, E].AddTriangle(Tri);
                EdgeObj2d[S,E].AddDiagonalVertex(_edgeGraph.TensileVertexObjects[D]);
                
            }
            T.Add(Tri);
        }
        return T;
    }

    void ConnectEdgeToVert()
    {
        var Verts = _edgeGraph.TensileVertexObjects;

        foreach (var Eg in _edgeGraph.TensileEdgeObjects)
        {
            int S = Eg.StartVertice;
            int E = Eg.EndVertice;

            Verts[S].AddEdgeObjs(Eg);
            Verts[E].AddEdgeObjs(Eg);
            Eg.AddConnectedVertex(Verts[S]);
            Eg.AddConnectedVertex(Verts[E]);
        }

        foreach (var ve in verticalEdges)
        {
            int S = ve.StartVertice;
            int E = ve.EndVertice;

            ve.AddConnectedVertex(Verts[S]);
            ve.AddConnectedVertex(Verts[E]);
        }
    }

    void ConnectVertexs()
    {
        var Verts = _edgeGraph.TensileVertexObjects;
        foreach (var V in Verts)
        {
            int i = V.Index;
            if (i >= countX * countZ)
            {
                int blw = i - countX * countZ;
                V.SetVertBelow(Verts[blw]);
            }


            if (i < _edgeGraph.edgeGraph.VertexCount-countX*countZ)
            {
                int abv = i + countX * countZ;
                V.SetVertAbove(Verts[abv]);
            }

            var cv = _edgeGraph.edgeGraph.GetConnectedVertices(i);

            foreach (var v in cv)
            {
                if (v == i - countX * countZ || v == i + countX * countZ)
                {
                    continue;
                }
                else
                {
                    V.AddConnectedVertObjInLayer(Verts[v]);
                }
            }
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
                        w = x * Scale + 0.5f * Scale;
                    }

                    float h = z * Mathf.Sqrt(Mathf.Pow(Scale, 2) - Mathf.Pow(Scale * 0.5f, 2));

                    float t = y * (0.42552f/1f)*Scale;
                    _positions.Add(new Vector3(w,t,h));
                }
            }
        }

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
	void Update ()
	{
	    if (awake == true)
	    {
	        updateVertexPosition();
	        updateTriangle();
	        LerpPosition();
        }
    }

    void updateVertexPosition()
    {
        //foreach (var tv in _edgeGraph.TensileVertexObjects)
        //{
        //    int v = tv.Index;
        //    tv.transform.localPosition = currentPositions[v];
        //}

        for (int i = 0; i < _edgeGraph.edgeGraph.VertexCount; i++)
        {
            _edgeGraph.TensileVertexObjects[i].transform.localPosition = currentPositions[i];
        }
    }

    void updateTriangle()
    {
        foreach (var tt in _edgeGraph.TensileTriangle)
        {
            tt.updateTriangle(currentPositions);
        }
    }

    public void ResetGraph()
    {
        if (awake == true)
        {
            foreach (var tv in _edgeGraph.TensileVertexObjects)
            {
                tv.SetState(0);
                DestroyImmediate(tv.gameObject);
            }

            foreach (var te in _edgeGraph.TensileEdgeObjects)
            {
                te.SetState(0);
                DestroyImmediate(te.gameObject);
            }

            foreach (var ve in verticalEdges)
            {
                ve.SetState(0);
                DestroyImmediate(ve.gameObject);
            }
            _edgeGraph.TensileVertexObjects.Clear();
            _edgeGraph.TensileEdgeObjects.Clear();
            _edgeGraph.TensileTriangle.Clear();
            verticalEdges.Clear();

            btn.interactable = false;
            btn2.interactable = false;
            LerpOnOff = false;
            btn.GetComponent<ButtonImageHandler>().SetTexture(true);
            btn2.GetComponent<ButtonImageHandler>().SetTexture(true);
        }

        awake = false;
    }



}
