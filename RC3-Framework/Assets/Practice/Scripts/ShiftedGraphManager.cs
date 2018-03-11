using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity.Examples.DendriticGrowth;
using UnityEngine;

public class ShiftedGraphManager : MonoBehaviour
{
    private SharedGraph _cubicGrid;

    [SerializeField]private TensegrityObject TenPrefab;
    [SerializeField] private int CountX = 5;
    [SerializeField] private int CountY = 5;
    [SerializeField] private int CountZ = 5;
    [SerializeField] private int Scale = 10;


    private void Awake()
    {
        _cubicGrid = new SharedGraph();
        _cubicGrid.Initialize(Graph.Factory.CreateCubicGrid(CountX, CountY, CountZ));
        _cubicGrid.TensegrityObjects.AddRange(CreatTensegrityObjects());

        transform.position = new Vector3(-CountX  * 0.5f, -CountY  * 0.5f, -CountZ * 0.5f);
    }

    public SharedGraph CubicGrid()
    {
        return _cubicGrid;
    }

    IEnumerable<TensegrityObject> CreatTensegrityObjects()
    {
        List<TensegrityObject> Obj = new List<TensegrityObject>();
        int index = 0;
        foreach (var p in SetObjectsPosition())
        {
            var tObj = Instantiate(TenPrefab, transform);
            tObj.transform.localPosition = p;
            tObj.SetupStructure(Scale, 0, index);
            Obj.Add(tObj);
            index++;
        }
        return Obj;
    }

    IEnumerable<Vector3> SetObjectsPosition()
    {
        List<Vector3> _positions = new List<Vector3>();
        for (int z = 0; z < CountZ; z++)
        {
            for (int y = 0; y < CountY; y++)
            {
                for (int x = 0; x < CountX; x++)
                {
                    Vector3 p;
                    if (x % 2 == 0)
                    {
                         p = new Vector3(x * Scale, y * Scale, z * Scale);
                    }
                    else
                    {
                        p = new Vector3(x * Scale, y * Scale, z * Scale+0.5f*Scale);
                    }
                    
                    _positions.Add(p);
                }
            }
        }

        return _positions;
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
