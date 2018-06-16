using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;
using UnityEngine.UI;

public class HexigonIcosaGraphManager : MonoBehaviour
{
    [SerializeField] private SharedEdgeGraph _edgeGraph;
    [SerializeField] private TensileIcosaVertex TivPrefab;

    [SerializeField] private float Scale = 1;

    private int countX = 3;
    private int countZ = 3;
    private int countY = 3;


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

    private int[] Depths;

    private List<Transform> PositiveFactors=new List<Transform>();
    private List<Transform> NegativeFactors=new List<Transform>();

    private List<Vector3> currentPositions;
    private List<Vector3> savedPositions;


    private bool awake;

    private int sizeHolderX = 10;
    private int sizeHolderY = 3;
    private int sizeHolderZ = 10;

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


    private bool displayGraph = true;

    public void GraphToggleAll()
    {
        if (displayGraph == false)
        {
            displayGraph = true;
        }
        else
        {
            displayGraph = false;
        }

        foreach (var tv in _edgeGraph.TensileVertexObjects)
        {
            tv.GraphToggle(displayGraph);
        }

        foreach (var te in _edgeGraph.TensileEdgeObjects)
        {
            te.GraphToggle(displayGraph);
        }
    }


    private void Awake()
    {

        initializeGraph();
    }

    public bool GraphDone()
    {
        return awake;
    }

    /// <summary>
    /// 
    /// </summary>
    public void initializeGraph()
    {
        if (awake == false)
        {
            UpdateSize();



            currentPositions = new List<Vector3>(countX * countZ * countY);
            savedPositions = new List<Vector3>(countX * countZ * countY);
            _edgeGraph = new SharedEdgeGraph();
            _edgeGraph.Initialize(EdgeGraph.Factory.CreateTriangleGrid3D(countX, countY, countZ));

            savedPositions.AddRange(SetVertexPosition());
            currentPositions.AddRange(savedPositions);
            _edgeGraph.TensileIcosahedron.AddRange(CreatVertexObjects());

            Depths=new int[_edgeGraph.edgeGraph.VertexCount];

            
            
            transform.position = new Vector3(-countX *Scale* 0.5f, -countY * Scale*0.5f, -countZ * Scale*0.5f);
            awake = true;


            GetComponent<IcosaUnitGrowthManager>().initializeGrowther();
        }
        else
        {
            ResetGraph();

            initializeGraph();
        }
    }

    void Start()
    {

    }


    IEnumerable<TensileIcosaVertex> CreatVertexObjects()
    {
        List<TensileIcosaVertex> Objs = new List<TensileIcosaVertex>();
        int index = 0;
        
        foreach (var p in savedPositions)
        {
            var tObj = Instantiate(TivPrefab, transform);
           
            tObj.transform.localPosition = p;

            tObj.SetupStructure(index,0,0.01f*Scale,Scale,false );

            Objs.Add(tObj);
            index++;
        }
        SortVertexs(Objs);
        return Objs;
    }

    void SortVertexs(List<TensileIcosaVertex> _objs)
    {
        int index = 0;
        for (int y = 0; y < countY; y++)
        {
            //even rows
            for (int z = 0; z < countZ; z+=2)
            {
                for (int x = 0; x < countX; x++)
                {
                    int i = x + z * countX + y * countX * countZ;

                    if (z > 0 && x > 0) // z-1,x-1
                    _objs[i].AddNeighbor(0, _objs[i - countX-1 ]);

                    if (z > 0) // z-1
                        _objs[i].AddNeighbor(1, _objs[i - countX]);

                    if (x > 0) // x-1
                        _objs[i].AddNeighbor(2, _objs[i - 1]);

                    if (x < countX - 1) // x+1
                        _objs[i].AddNeighbor(3, _objs[i + 1]);

                    if(z<countZ-1&&x>0) //z+1,x-1
                        _objs[i].AddNeighbor(4, _objs[i + countZ-1]);

                    if (z < countZ - 1)//z+1
                        _objs[i].AddNeighbor(5,_objs[i+countZ]);
                    
                    if (y < countY-1) //y+1
                        _objs[i].AddNeighbor(6, _objs[i + countZ*countX]);

                    if(y>0)//y-1
                        _objs[i].AddNeighbor(7, _objs[i -countZ*countX]);

                    if (y % 2 != 0)
                    {
                        _objs[i].setFlip(true);
                    }
                    else
                    {
                        _objs[i].setFlip(false);
                    }

                }
            }
            //odd rows
            for (int z = 1; z < countZ; z += 2)
            {
                for (int x = 0; x < countX; x++)
                {
                    int i = x + z * countX + y * countX * countZ;

                    if (z > 0) // z-1
                        _objs[i].AddNeighbor(0, _objs[i - countX]);

                    if (z > 0 && x < countX-1) // z-1
                        _objs[i].AddNeighbor(1, _objs[i - countX+1]);

                    if (x > 0) // x-1
                        _objs[i].AddNeighbor(2, _objs[i - 1]);

                    if (x < countX - 1) // x+1
                        _objs[i].AddNeighbor(3, _objs[i + 1]);

                    if (z < countZ - 1)//z+1
                        _objs[i].AddNeighbor(4, _objs[i + countZ]);

                    if (z < countZ - 1 && x <countX-1) //z+1
                        _objs[i].AddNeighbor(5, _objs[i + countZ+1]);

                    if (y < countY - 1) //y+1
                        _objs[i].AddNeighbor(6, _objs[i + countZ * countX]);

                    if (y > 0)//y-1
                        _objs[i].AddNeighbor(7, _objs[i - countZ * countX]);


                    if (y % 2 != 0)
                    {
                        _objs[i].setFlip(true);
                    }
                    else
                    {
                        _objs[i].setFlip(false);
                    }

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

                    var edgeLength = Scale / ((1f + Mathf.Sqrt(5f)) / 2f);

                    float t =2f* y*Mathf.Sqrt(Mathf.Pow(Scale / 2f, 2) - Mathf.Pow(Mathf.Tan(Mathf.PI / 6f) * edgeLength / 2f, 2));

                    _positions.Add(new Vector3(w,t,h));
                }
            }
        }

        return _positions;
    }


	// Update is called once per frame
	void Update ()
	{
	    if (awake == true)
	    {
	        
        }

        UpdateVertex();
    }

    void CreateSource()
    {

    }


  

    public void ResetGraph()
    {
        if (awake == true)
        {
            foreach (var tv in _edgeGraph.TensileIcosahedron)
            {
                tv.SetState(0);
                DestroyImmediate(tv.gameObject);
            }

          
            _edgeGraph.TensileIcosahedron.Clear();

            GetComponent<IcosaUnitGrowthManager>().ResetGrowth();
           
        }

        awake = false;
    }

    int GetClosestVertex(Vector3 point)
    {
        int minVert = -1;
        float minDist = float.MaxValue;
        for (int i = 0; i < _edgeGraph.edgeGraph.VertexCount; i++)
        {
            Vector3 d0 = _edgeGraph.TensileIcosahedron[i].transform.localPosition;
            Vector3 d = d0 - point;
            var m = d.magnitude;
            if (m < minDist)
            {
                minVert = i;
                minDist = m;
            }
        }
        return minVert;
    }

    private IEnumerable<int> GetSourceIndices()
    {
        List<Transform>factors=new List<Transform>();
        factors.AddRange(PositiveFactors);
        factors.AddRange(NegativeFactors);

        foreach (var f in factors)
        {
            var v = GetClosestVertex(transform.InverseTransformPoint(f.position));

            yield return v;
        }
    }


    void UpdateVertex()
    {
        PositiveFactors = GetComponent<SourceGenerator>().GetPosiSources();
        NegativeFactors = GetComponent<SourceGenerator>().GetNegaSources();

        //GraphUtil.GetVertexEdgeDepths(_edgeGraph.edgeGraph,GetSourceIndices(),Depths);

        ChangePositionBySource();
        updateVertexPositions();
    }

    float[] Distance(Transform f)
    {
        float [] d=new float[savedPositions.Count];

        for (int i = 0; i < savedPositions.Count; i++)
        {
            var dis = (savedPositions[i] - f.localPosition).magnitude;

            d[i] = dis;
        }

        return d;

    }

    void ChangePositionBySource()
    {
        foreach (var sp in savedPositions)
        {
            int i = Array.IndexOf(savedPositions.ToArray(), sp);

            float MinDis = float.MaxValue;
            float MaxDis = float.MinValue;

            Vector3 cp=Vector3.zero;
            ;
            foreach (var pf in PositiveFactors)
            {
                var dis = Distance(pf);
                float min = Mathf.Min(dis);
                float Max = Mathf.Max(dis);

                var t=(dis[i]-min)/(Max-min);

                if(dis[i]<MinDis) MinDis=dis[i];

                cp += Vector3.Lerp(pf.localPosition, sp, t)/PositiveFactors.Count;

            }

            if (PositiveFactors.Count != 0)
            {
                currentPositions[i] = cp;
            }
            else
            {
                currentPositions[i] = savedPositions[i];
            }
        }
    }

    void updateVertexPositions()
    {
        foreach (var v in _edgeGraph.TensileIcosahedron)
        {
            var i = Array.IndexOf(_edgeGraph.TensileIcosahedron.ToArray(), v);

            v.transform.localPosition = Vector3.Lerp(v.transform.localPosition, currentPositions[i], Time.deltaTime);

        }
    }



    List<int> GetClosestVertexSet(Vector3 point,float _range)
    {
        List<int> vertSet = new List<int>();

        for (int i = 0; i < _edgeGraph.edgeGraph.VertexCount; i++)
        {
            Vector3 d0 = savedPositions[i];//_edgeGraph.TensileIcosahedron[i].transform.localPosition;

        
            var m = (d0 - point).magnitude;

            

            if (m < _range*Scale)
            {
                vertSet.Add(i);
            }
        }
        return vertSet;
    }


}
