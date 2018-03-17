using System.Collections;
using System.Collections.Generic;
using System.Net;
using RC3;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;

public class CubicGrowthManager : MonoBehaviour
{
    private SharedGraph CubicGrid;

    private Graph _graph;
    List<TensegrityObject> TenVertex;
    private Queue <int>_queue;
    private List<int> Sources;
    private bool Grow = false;

    private int Frame = 0;

    private int[] Depth;


    // Use this for initialization
    void Start ()
	{
	    CubicGrid = transform.GetComponent<GraphManager>().CubicGrid();
	    _graph = CubicGrid.Graph;
	    Depth = new int[_graph.VertexCount];
	    TenVertex = CubicGrid.TensegrityObjects;
	    _queue = new Queue <int>();
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
                 if(t.State ==0)
                {
                    if (nc >= 1&& nc <= 2)
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


    

    void  CreatSource()
    {
        Sources = new List<int>();
        System.Random Random = new System. Random();
        int SourceCount = (int)TenVertex .Count /100;

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
            if (TenVertex[v].State !=0)
            {
                count++;
            }
        }
        return count;
    }


	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.S))
	    {
	        CreatSource();
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

	    if (Input.GetKeyDown(KeyCode.Alpha1 ))
	    {
	        ChangeLook(1);
        }
	    if (Input.GetKeyDown(KeyCode.Alpha2 ))
	    {
	        ChangeLook(2);
	    }
	    if (Input.GetKeyDown(KeyCode.Alpha3 ))
	    {
	        ChangeLook(3);
	    }
	    if (Input.GetKeyDown(KeyCode.Alpha0 ))
	    {
	        ChangeLook(0);
	    }

        UpdateGrowth();

    }
}
