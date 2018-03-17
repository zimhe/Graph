using System.Collections;
using System.Collections.Generic;
using RC3;
using RC3.Unity;
using RC3.Unity.Examples.DendriticGrowth;
using SpatialSlur.SlurData;
using UnityEngine;

public class DirectedGrowthManager : MonoBehaviour
{
    [SerializeField]private SharedGraph CubicGrid;
    private Graph _graph;
    List<TensegrityObject> TenVertex;

    private Queue<int> _sourceQueue;
    private Queue<int> _queue;
    private PriorityQueue<float, int> _proQueue = new PriorityQueue<float, int>();
    private List<int> Sources;
    private bool Grow = false;  

    private int Frame = 0;

    private int[] Depth;

    int S_Inx = 0;
    void  getShortestSourcePath()
    {
        Queue<int> _tempQueue = new Queue<int>();
        _sourceQueue = new Queue<int>();

        List<int> _tempSources = new List<int>(Sources.ToArray());

        //_sourceQueue.Enqueue(Sources [0]);
        _tempQueue.Enqueue(_tempSources [0]);
        _tempSources.Remove(_tempSources[0]);

        for (int i = 0; i <Sources .Count -1; i++)
        { 
            
            int minIndex = 0;
            float minDistance = float.MaxValue;

            int v = _tempQueue.Dequeue();

            foreach (var src in _tempSources)
            {
                var p0 = TenVertex[v].transform.position;
                var p1 = TenVertex[src].transform.position;
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

    }

   

    // Use this for initialization
    void Start()
    {
        //CubicGrid = transform.GetComponent<ShiftedGraphManager>().CubicGrid();
        //CubicGrid = transform.GetComponent<GraphManager>().CubicGrid();
        CubicGrid = transform.GetComponent<DeformedGraphManager>().CubicGrid();
        //CubicGrid = transform.GetComponent<HexigonGraphManager>().();

        _graph = CubicGrid.Graph;
        Depth = new int[_graph.VertexCount];
        TenVertex = CubicGrid.TensegrityObjects;
        _queue = new Queue<int>();
        _sourceQueue = new Queue<int>();
    }

    void ResetGrowth()
    {
        _queue.Clear();

        foreach (var t in TenVertex)
        {
            t.SetState(0);
        }

        Grow = false;
        Sources.Clear();
        _sourceQueue.Clear();
        _proQueue = new PriorityQueue<float, int>();
        S_Inx = 0;

    }


    void UpdateGrowth()
    {
        if (Grow == true)
        {
            if (_queue.Count == 0)
            {
                return;
            }

            foreach (int v in _graph.GetConnectedVertices(_queue.Dequeue()))
            {
                var t = TenVertex[v];
                if (t.State != 0)
                {
                    continue;
                }

                var depth = Depth[v];

                if (depth > 3)
                {
                    return;
                }

                int nc = NeighborCount(v);
                /*  if (t.State !=0)
                  {
                      if (nc >= 1 && nc <= 2)
                      {
                          t.SetState(2);
                          _queue.Enqueue(v);
                      }
                      if (nc > 4)
                      {
                          t.SetState(0);
                      }
                  }*/
                if (t.State == 0)
                {
                    if (nc >= 1 && nc <= 2)
                    {
                        t.SetState(2);
                        _queue.Enqueue(v);
                    }
                }

                //t.UpdateToFuture();
                Frame++;

            }
        }
    }

    void ChangeLook(int _subState)
    {
        foreach (var t in TenVertex)
        {
            if (t.State == 2)
            {
                t.SetSubState(_subState);
            }
        }
    }

    void getDepth()
    {
        GraphUtil.GetVertexDepths(_graph, SourceRO(), Depth);
    }




    void CreatSource()
    {
        Sources = new List<int>();
        System.Random Random = new System.Random();
        int SourceCount = (int)TenVertex.Count / 200;

        for (int i = 0; i < SourceCount; i++)
        {
            int _source = Random.Next(0, TenVertex.Count);

            Sources.Add(_source);
        }

        foreach (int src in Sources)
        {
            _queue.Enqueue(src);
            TenVertex[src].SetState(1);
        }

        
    }

    IEnumerable<int> SourceRO()
    {
        return Sources;
    }


    int NeighborCount(int vertex)
    {
        int count = 0;
        foreach (var v in _graph.GetConnectedVertices(vertex))
        {
            if (TenVertex[v].State != 0)
            {
                count++;
            }
        }
        return count;
    }

    float GetKey(int vertex, int target)
    {
        float _key = 0f;

        var p0 = TenVertex[vertex].transform.position;
        var p1 = TenVertex[target].transform.position;

        _key = Mathf .Abs(( p1 - p0).magnitude);

        return _key;
    }

    void UpdateDirectedGrowth()
    {
        if (Grow == true&&_sourceQueue .Count !=0)
        {
          
           //for (int i = 0; i < _sourceQueue.Count; i++)
            {
                float key;
               
                int vertex;
                _proQueue.RemoveMin(out key, out vertex);

                var T = _sourceQueue.ToArray();

                foreach (var vi in _graph.GetConnectedVertices(vertex))
                {
                    var target = T[S_Inx];

                    if (vi == target)
                    {
                        print("reached");

                        _proQueue =new PriorityQueue<float, int>();
                        _proQueue.Insert(0f, target);

                  
                        if (S_Inx < T.Length-1)
                        {
                            S_Inx++;
                            target = T[S_Inx];
                        }
                        else
                        {
                            Grow = false;
                        }
                    }

                    if (Depth[vi] > 4)
                    {
                        //return;
                    }
                    var t = TenVertex[vi];
                    int nc = NeighborCount(vi);
                    if ( nc <4)
                    {
                        if(t.State == 0) 
                        t.SetState(2);
                        _proQueue.Insert(GetKey(vi, target), vi);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreatSource();
            getShortestSourcePath();
            getDepth();
        }

        if (Input.GetKeyDown((KeyCode.Space)))
        {
            if (Grow == false)
            {
                Grow = true;
            }
            else
            {
                Grow = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ResetGrowth();

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeLook(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeLook(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeLook(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ChangeLook(0);
        }

       // UpdateGrowth();

        UpdateDirectedGrowth();

    }





}

