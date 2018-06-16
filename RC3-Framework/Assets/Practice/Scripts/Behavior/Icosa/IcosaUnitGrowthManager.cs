using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity.Examples.DendriticGrowth;
using SpatialSlur.SlurData;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IcosaUnitGrowthManager : MonoBehaviour {

    private SharedEdgeGraph HexigonGrid;
    private HexigonIcosaGraphManager GraphManager;
    private EdgeGraph _graph;
    List<TensileIcosaVertex> TenVertex;

    [SerializeField] private LineRenderer DrawSourceLine;

    [SerializeField] private ButtonImageHandler directBtn;

    private bool GraphDone;

    private Queue<int> _sourceQueue;
    private Queue<int> _queue;
    private PriorityQueue<float, int> _proQueue = new PriorityQueue<float, int>();
    private List<int> Sources;
    private bool GrowDirect = false;

    private int Frame = 0;

    private int[] Depth;

    int qIndex = 0;

    private int countX;
    private int countY;
    private int countZ;

    void EvaluateSource()
    {
        if (GraphDone == true)
        {
            Queue<int> _tempQueue = new Queue<int>();
            _sourceQueue = new Queue<int>();

            List<int> _tempSources = new List<int>(Sources.Count);
            _tempSources.AddRange(Sources);

            var next = _tempSources.First();
            _sourceQueue.Enqueue(Sources [0]);
            _tempQueue.Enqueue(next);
            _tempSources.Remove(next);

            for (int i = 0; i < Sources.Count - 1; i++)
            {

                int minIndex = 0;
                float minDistance = float.MaxValue;

                int v = _tempQueue.Dequeue();

                foreach (var src in _tempSources)
                {
                    var p0 = TenVertex[v].transform.localPosition;
                    var p1 = TenVertex[src].transform.localPosition;
                    var d = p1 - p0;
                    var _d = d.magnitude;

                    if (minDistance > _d && _d != 0)
                    {
                        minDistance = _d;
                        minIndex = src;
                    }
                }
                _tempQueue.Enqueue(minIndex);
                _sourceQueue.Enqueue(minIndex);
                _tempSources.Remove(minIndex);
            }

            _proQueue.Insert(0f, Sources[0]);

            SourceCreated = true;
        }
    }



    // Use this for initialization
    void Start()
    {
        //initializeGrowther();
    }

    public void initializeGrowther()
    {
        GraphManager = transform.GetComponent<HexigonIcosaGraphManager>();

        GraphDone = GraphManager.GraphDone();

        displayed = false;

        HexigonGrid = GraphManager._hexGrid();
        _graph = HexigonGrid.edgeGraph;
        Depth = new int[_graph.VertexCount];
        TenVertex = HexigonGrid.TensileIcosahedron;
        _queue = new Queue<int>();
        _sourceQueue = new Queue<int>();


        countX = GraphManager.GetCountX();
        countY = GraphManager.GetCountY();
        countZ = GraphManager.GetCountZ();
    }


    public void ResetGrowth()
    {
        _queue.Clear();

        foreach (var t in TenVertex)
        {
            t.SetState(0, 0);
        }

        GrowDirect = false;
        Frame = 0;
        if (Sources != null)
        {
            Sources.Clear();
        }

        if (_sourceQueue.Count!=0)
        {
            _sourceQueue.Clear();
        }

        SourceCreated = false;

        _proQueue = new PriorityQueue<float, int>();

        qIndex = 0;
        displayed = false;
    }


    void UpdateGrowth()
    {
        if (GrowSpread == true)
        {
            if (_queue.Count == 0)
            {
                return;
            }
            foreach (int v in _graph.GetConnectedVertices(_queue.Dequeue()))
            {
                var t = TenVertex[v];
                if (t.GetState() == 2)
                {
                    continue;
                }

                var depth = Depth[v];

                if (depth > 3)
                {
                    return;
                }

                int nc = AliveNeighborCount(v);
               
                if (t.GetState() == 0)
                {
                    if (nc >= 1 && nc <= 2)
                    {

                        System.Random rdm=new System.Random();

                        int SS = rdm.Next(0, 3);
                        t.SetState(2,SS);
                        _queue.Enqueue(v);
                    }
                }

        
                Frame++;
              
            }
        }
    }

    public void ChangeLook(int _subState)
    {
        if (GraphDone == true)
        {
            foreach (var t in TenVertex)
            {
                if (t.GetState() != 0)
                {

                    t.SetState(1, _subState);

                }
            }
           
        }
    }

    void getDepth()
    {
        if (GraphDone == true)
        {
            GraphUtil.GetVertexEdgeDepths(_graph, SourceRO(), Depth);
        }
    }

    void CreatSourceRdm(int _percentage)
    {
        if (GraphDone == true)
        {
            System.Random Random = new System.Random();
            int SourceCount = Mathf.FloorToInt(TenVertex.Count * (_percentage * 0.01f));

            qIndex = 0;

            if (Sources != null && Sources.Count != 0)
            {

                foreach (var tv in TenVertex)
                {
                    if (tv.GetState() != 0)
                    {
                        tv.SetState(0, 0);
                    }
                }
                Sources = new List<int>(SourceCount);
            }
            else
            {
                Sources = new List<int>(SourceCount);
            }

            for (int i = 0; i <= SourceCount; i++)
            {

                int _source = Random.Next(0, TenVertex.Count);

                Sources.Add(_source);
                TenVertex[_source].SetState(1);
                _queue.Enqueue(_source);
            }

        }
    }

    bool displayed = false;


    [SerializeField] Button turnOnAllBtn;

    public void TurnOnAll()
    {

        if (GraphDone == true && GrowSpread == false && GrowDirect == false)
        {
            if (displayed == false)
            {
                System.Random rdm = new System.Random();
                foreach (var tv in TenVertex)
                {
                    

                    int substate = rdm.Next(0, 3);

                    tv.SetState(2, substate);
                }

            
                displayed = true;
                //turnOnAllBtn.GetComponent<ButtonImageHandler>().SetTexture(true);
            }
            else
            {
                foreach (var tv in TenVertex)
                {
                    tv.SetState(0);
                }
                displayed = false;
                //turnOnAllBtn.GetComponent<ButtonImageHandler>().SetTexture(false);

            }
        }
    }

    IEnumerable<int> SourceRO()
    {
        return Sources;
    }

    int AliveNeighborCount(int vertex)
    {
        int count = 0;
        foreach (var v in _graph.GetConnectedVertices(vertex))
        {
            if (TenVertex[v].GetState() != 0)
            {
                count++;
            }
        }
        return count;
    }

    float GetKey(int vertex, int target)
    {
        float _key = 0f;

        var p0 = TenVertex[vertex].transform.localPosition;
        var p1 = TenVertex[target].transform.localPosition;

        _key = Mathf.Abs((p1 - p0).magnitude);
        return _key;
    }

    void UpdateDirectedGrowth()
    {
        if (GrowDirect == true && _sourceQueue.Count != 0)
        {
               

            if (_proQueue.Count > 0)
            {
                float key;
                int vertex;
                _proQueue.RemoveMin(out key, out vertex);


            
                var T = _sourceQueue.ToArray();

                foreach (var vi in _graph.GetConnectedVertices(vertex))
                {
                    int target = T[qIndex];

                    if (vi == target)
                    {
                        _proQueue = new PriorityQueue<float, int>();
                        _proQueue.Insert(0f, target);

                        if (qIndex < T.Length-1)
                        {
                            qIndex++;
                            target = T[qIndex];
                        }
                        else
                        {
                            GrowDirect = false;
                            //directBtn.SetTexture(true);
                            print("done");
                        }
                    }

                    var t = TenVertex[vi];
                    int nc = AliveNeighborCount(vi);
                    if (nc < 4)
                    {
                        //if (nc < 3)
                        {
                            if (t.GetState() != 2)
                            {
                                System.Random rdm = new System.Random();
                                int Ss = rdm.Next(0, 3);
                                t.SetState(2, Ss);
                            }
                            _proQueue.Insert(GetKey(vi, target), vi);
                        }
                    }
                }
            }
        }
    }

    private bool SourceCreated;

    public void GenerateSource()
    {
        CreatSourceRdm(3);
       EvaluateSource();
        getDepth();


    }

    void Update()
    {
        if (GraphDone == true)
        {
            UpdateGrowth();
            UpdateDirectedGrowth();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            rotateUnits();
        }

        DrawLine();
    }

    private bool GrowSpread;

    public void GrowthToggleDirect()
    {
        GrowSpread = false;
        GrowDirect = !GrowDirect;
    }

    public void GrowthToggleSpread()
    {
        GrowDirect = false;
        GrowSpread = !GrowSpread;
    }

    public void rotateUnits()
    {
        foreach (var tv in TenVertex)
        {
            if (tv.GetState() == 2)
            {
                float i = Random.Range(0f, 3f);

                int s = Mathf.FloorToInt(i);
                tv.SetSubstate(s);
            }
        
        }
    }

    void DrawLine()
    {
        if (SourceCreated)
        {
            var sq = _sourceQueue.ToArray();

            DrawSourceLine.positionCount = sq.Length;
            for (int i = 0; i < sq.Length; i++)
            {

                DrawSourceLine.SetPosition(i, TenVertex[sq[i]].transform.position);
            }
        }
    }
}
