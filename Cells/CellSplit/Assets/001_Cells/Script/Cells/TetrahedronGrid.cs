using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RC3;
using RC3.Unity;
using UnityEngine;

public class TetrahedronGrid : MonoBehaviour
{
    [SerializeField] private Cells CellPrefab;
    [SerializeField] private CellEdge CellEdgePrefab;

    [SerializeField] private float GridScale = 1f;

    [SerializeField] private int CountX = 5;
    [SerializeField] private int CountY = 5;
    [SerializeField] private int CountZ = 5;


    List<Vector3> SavedPositions=new List<Vector3>();

    List<Cells> allCells=new List<Cells>();

    private SharedEdgeGraph<Cells,CellEdge> CG;

    private void Awake()
    {
        CG=new SharedEdgeGraph<Cells,CellEdge>();
        CG.Initialize(EdgeGraph.Factory.CreateTetrahedronGrid(CountX,CountY,CountZ));
     


        SavedPositions = SetPosition(CountX, CountY, CountZ,GridScale).ToList();

        CG.VertexObjects.AddRange(CreateVertex());
        CG.EdgeObjects.AddRange(CreateEdge());

        //foreach (var p in SavedPositions)
        //{
        //    var c = Instantiate(CellPrefab,transform);
        //    c.transform.localPosition = p;
        //    c.transform.localScale *= GridScale;
        //}

    }
    // Use this for initialization
    void Start ()
    {
	
        

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerable<Cells> CreateVertex()
    {
        foreach (var sp in SavedPositions)
        {
            var c = Instantiate(CellPrefab, transform);
            c.transform.localPosition =sp;
            c.transform.localScale *= GridScale;
            yield return c;
        }
    }

    IEnumerable<CellEdge> CreateEdge()
    {
        for (int i = 0; i < CG.edgeGraph.EdgeCount; i++)
        {
            var s = CG.edgeGraph.GetStartVertex(i);
            var e = CG.edgeGraph.GetEndVertex(i);

            var ce = Instantiate(CellEdgePrefab,transform);
            ce.SetEdge(SavedPositions[s],SavedPositions[e]);
            yield return ce;
        }
    }

    IEnumerable<Vector3> SetPosition(int X, int Y, int Z,float _scale)
    {
        float Scale = 0.7f * _scale;

        List<Vector3> Positions = new List<Vector3>();

        float Vz = Mathf.Sqrt(Mathf.Pow(Scale, 2) - Mathf.Pow(Scale / 2, 2));

        float Vy= Mathf.Sqrt(Mathf.Pow(Scale, 2) - Mathf.Pow((Scale / 2) / Mathf.Cos(Mathf.PI / 6), 2));

        for (int y = 0; y < Y; y++)
        {
            for (int z = 0; z < Z - y; z++)
            {
                for (int x = 0; x < X - y - z; x++)
                {
                    if (y % 2 == 0)
                    {
                        var p = new Vector3(Scale * x + Scale / 2 * z + Scale / 2 * y, Vy * y, (Vz) * z + (Scale / 2 * Mathf.Tan(Mathf.PI / 6)) * y);

                        Positions.Add(p);
                    }
                    else
                    {
                        var p = new Vector3(Scale * x + Scale / 2 * z + Scale / 2 * y, Vy * y, (Vz) * z + (Scale / 2 * Mathf.Tan(Mathf.PI / 6)) * y);

                        Positions.Add(p);
                    }
                }
            }
        }



        return Positions;
    }

    

  




}
