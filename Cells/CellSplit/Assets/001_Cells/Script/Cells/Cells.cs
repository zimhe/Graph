using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using RC3.Unity;
using UnityEngine;

using System.Linq;
using UnityScript.Lang;
using Array = System.Array;

public class Cells : VertexObject
{
    #region Constructors
   
    [SerializeField] private Transform PointPfb;
    [SerializeField] private StaticBar barPrefab;
    [SerializeField] private StaticStrings StringPrefab;
    [SerializeField] private GameObject _vertexPrefab;

    [SerializeField] private Color defualtColor;
    [SerializeField] private Color SourceColor;
    [SerializeField] private Color state1Color;
    [SerializeField] private Color state2Color;
    [SerializeField] private Color state3Color;

    Transform[] b0 = new Transform[2];
    Transform[] b1 = new Transform[2];
    Transform[] b2 = new Transform[2];
    Transform[] b3 = new Transform[2];
    Transform[] b4 = new Transform[2];
    Transform[] b5 = new Transform[2];

    private float SourceScale = 0.1f;


    List<StaticBar> AllBars = new List<StaticBar>(6);
    List<StaticStrings> AllSrings = new List<StaticStrings>(24);

    private GameObject Vertex;

    private Transform[] APoints = new Transform[6];
    Vector3[] SavedAPos = new Vector3[6];
    Vector3[] currentAPos = new Vector3[6];

    private Transform[] BPoints = new Transform[6];
    Vector3[] SavedBPos = new Vector3[6];
    Vector3[] currentBPos = new Vector3[6];

    public int Index { get; set; }

    private float Thickness;

    public float Scale { get; set; }

    private bool Flip;

    public void setFlip(bool _flip)
    {
        Flip = _flip;
        Vertex.GetComponent<MeshRenderer>().material.color = defualtColor;
        gameObject.name = "IcosaVertex :" + Index + "/ Flip :" + Flip;
    }

    private float edgeLength;

    private float H1;

    private float H2;

    #endregion


    public void SetSize(float size)
    {
        
        _radius = size;
    }

    private float _radius;

   List<Cells> ConnectedCells=new List<Cells>();

    public void addConnctedCell(Cells c)
    {
        ConnectedCells.Add(c);
    }

    public List<Cells> getConnectedCells()
    {
        return ConnectedCells;
    }


    private void OnCollisionStay(Collision other)
    {
        if (other.transform.GetComponent<Cells>())
        {

        }
    }

    public void SetPoints(float _scale)
    {
        Scale = _scale;

        edgeLength = _scale / ((1f + Mathf.Sqrt(5f)) / 2f);

        float a = Mathf.Sqrt(Mathf.Pow(_scale / 2f, 2) + Mathf.Pow(edgeLength / 2f, 2));

        float b = (_scale / 2f) / Mathf.Cos(Mathf.PI / 6f);

        float za0 = Mathf.Tan(Mathf.PI / 6f) * _scale / 2f;
        float za1 = _scale / 2f / Mathf.Cos(Mathf.PI / 6f);

        float zb0 = Mathf.Tan(Mathf.PI / 6f) * edgeLength / 2f;
        float zb1 = edgeLength / 2 / Mathf.Cos(Mathf.PI / 6f);

        H1 = Mathf.Sqrt(Mathf.Pow(a, 2) - Mathf.Pow(b, 2));

        H2 = Mathf.Sqrt(Mathf.Pow(_scale / 2f, 2) - Mathf.Pow(Mathf.Tan(Mathf.PI / 6f) * edgeLength / 2f, 2));

        switch (Flip)
        {
            case true:
                H1 = -H1;
                H2 = -H2;
                break;

            case false:
                H1 = H1;
                H2 = H2;
                break;
            default:
                H1 = H1;
                H2 = H2;
                break;
        }


        APoints[0] = Instantiate(PointPfb, transform);

        APoints[0].transform.localPosition = SavedAPos[0] = new Vector3(-_scale / 2f, H1, -za0);

        APoints[1] = Instantiate(PointPfb, transform);

        APoints[1].transform.localPosition = SavedAPos[1] = new Vector3(0f, -H1, -za1);

        APoints[2] = Instantiate(PointPfb, transform);

        APoints[2].transform.localPosition = SavedAPos[2] = new Vector3(_scale / 2f, H1, -za0);

        APoints[3] = Instantiate(PointPfb, transform);

        APoints[3].transform.localPosition = SavedAPos[3] = new Vector3(-_scale / 2f, -H1, za0);

        APoints[4] = Instantiate(PointPfb, transform);

        APoints[4].transform.localPosition = SavedAPos[4] = new Vector3(0f, H1, za1);

        APoints[5] = Instantiate(PointPfb, transform);

        APoints[5].transform.localPosition = SavedAPos[5] = new Vector3(_scale / 2f, -H1, za0);

        BPoints[0] = Instantiate(PointPfb, transform);
        BPoints[0].transform.localPosition = SavedBPos[0] = new Vector3(-edgeLength / 2f, -H2, -zb0);

        BPoints[1] = Instantiate(PointPfb, transform);
        BPoints[1].transform.localPosition = SavedBPos[1] = new Vector3(0f, H2, -zb1);

        BPoints[2] = Instantiate(PointPfb, transform);
        BPoints[2].transform.localPosition = SavedBPos[2] = new Vector3(edgeLength / 2f, -H2, -zb0);

        BPoints[3] = Instantiate(PointPfb, transform);
        BPoints[3].transform.localPosition = SavedBPos[3] = new Vector3(-edgeLength / 2f, H2, zb0);

        BPoints[4] = Instantiate(PointPfb, transform);
        BPoints[4].transform.localPosition = SavedBPos[4] = new Vector3(0f, -H2, zb1);

        BPoints[5] = Instantiate(PointPfb, transform);
        BPoints[5].transform.localPosition = SavedBPos[5] = new Vector3(edgeLength / 2f, H2, zb0);

        //APoints.ToList().ForEach(p => { p.localScale = Scale * Vector3.one * 0.01f; });
        //BPoints.ToList().ForEach(p => { p.localScale = Scale * Vector3.one * 0.01f; });
    }
    void setBars()
    {
        var bar0 = Instantiate(barPrefab, transform);
        var bar1 = Instantiate(barPrefab, transform);
        var bar2 = Instantiate(barPrefab, transform);
        var bar3 = Instantiate(barPrefab, transform);
        var bar4 = Instantiate(barPrefab, transform);
        var bar5 = Instantiate(barPrefab, transform);

        bar0.SetupBar(b0[0].localPosition, b0[1].localPosition, Thickness, 0, "vertex" + Index);
        bar1.SetupBar(b1[0].localPosition, b1[1].localPosition, Thickness, 1, "vertex" + Index);
        bar2.SetupBar(b2[0].localPosition, b2[1].localPosition, Thickness, 2, "vertex" + Index);
        bar3.SetupBar(b3[0].localPosition, b3[1].localPosition, Thickness, 3, "vertex" + Index);
        bar4.SetupBar(b4[0].localPosition, b4[1].localPosition, Thickness, 4, "vertex" + Index);
        bar5.SetupBar(b5[0].localPosition, b5[1].localPosition, Thickness, 5, "vertex" + Index);

        AllBars.AddRange(new List<StaticBar> { bar0, bar1, bar2, bar3, bar4, bar5 });
    }
    public int GetState()
    {
        return State;
    }
    public int GetSubState()
    {
        return SubState;
    }

    private int State;

    private int SubState;

    void UpdateState()
    {
        switch (State)
        {
            case 0:

                clean();
                if (!Vertex.GetComponent<MeshRenderer>().enabled)
                    Vertex.GetComponent<MeshRenderer>().enabled = true;

                Vertex.GetComponent<MeshRenderer>().material.color = defualtColor;
                //Vertex.transform.localScale = 0.05f * Vector3.one;

                APoints.ToList().ForEach(p => { p.GetComponent<MeshRenderer>().enabled = false; });
                BPoints.ToList().ForEach(p => { p.GetComponent<MeshRenderer>().enabled = false; });

                break;

            case 1:
                clean();

                if (!Vertex.GetComponent<MeshRenderer>().enabled)
                    Vertex.GetComponent<MeshRenderer>().enabled = true;

                Vertex.GetComponent<MeshRenderer>().material.color = SourceColor;
                //Vertex.transform.localScale = Vector3.one * SourceScale;

                APoints.ToList().ForEach(p => { p.GetComponent<MeshRenderer>().enabled = false; });
                BPoints.ToList().ForEach(p => { p.GetComponent<MeshRenderer>().enabled = false; });



                break;

            case 2:

                //if (Vertex.GetComponent<MeshRenderer>().enabled)
                //    Vertex.GetComponent<MeshRenderer>().enabled = false;

                switch (SubState)
                {
                    case 0:
                        b0[0] = APoints[2];
                        b0[1] = APoints[0];
                        b1[0] = APoints[5];
                        b1[1] = APoints[3];
                        b2[0] = APoints[1];
                        b2[1] = BPoints[4];
                        b3[0] = BPoints[1];
                        b3[1] = APoints[4];
                        b4[0] = BPoints[2];
                        b4[1] = BPoints[5];
                        b5[0] = BPoints[0];
                        b5[1] = BPoints[3];

                        break;
                    case 1:

                        b0[0] = APoints[4];
                        b0[1] = APoints[2];
                        b1[0] = APoints[3];
                        b1[1] = APoints[1];
                        b2[0] = APoints[5];
                        b2[1] = BPoints[0];
                        b3[0] = BPoints[5];
                        b3[1] = APoints[0];
                        b4[0] = BPoints[4];
                        b4[1] = BPoints[3];
                        b5[0] = BPoints[2];
                        b5[1] = BPoints[1];

                        break;
                    case 2:
                        b0[0] = APoints[0];
                        b0[1] = APoints[4];
                        b1[0] = APoints[1];
                        b1[1] = APoints[5];
                        b2[0] = APoints[3];
                        b2[1] = BPoints[2];
                        b3[0] = BPoints[3];
                        b3[1] = APoints[2];
                        b4[0] = BPoints[0];
                        b4[1] = BPoints[1];
                        b5[0] = BPoints[4];
                        b5[1] = BPoints[5];

                        break;

                    default:

                        b0[0] = APoints[2];
                        b0[1] = APoints[0];
                        b1[0] = APoints[5];
                        b1[1] = APoints[3];
                        b2[0] = APoints[1];
                        b2[1] = BPoints[4];
                        b3[0] = BPoints[1];
                        b3[1] = APoints[4];
                        b4[0] = BPoints[2];
                        b4[1] = BPoints[5];
                        b5[0] = BPoints[0];
                        b5[1] = BPoints[3];

                        break;
                }

                //if (Vertex.GetComponent<MeshRenderer>().enabled)
                //    Vertex.GetComponent<MeshRenderer>().enabled = false;

                if (AllBars.Count == 0)
                {
                    setBars();
                    setString();
                }
                else
                {
                    updateBars();
                }

                break;
        }
    }


    Vector3[] TranformToVector(Transform[] xform)
    {
        Vector3[] vecs = new Vector3[xform.Length];

        foreach (var t in xform)
        {
            int i = Array.IndexOf(xform, t);

            vecs[i] = t.localPosition;
        }



        return vecs;
    }
    void setString()
    {
        List<Vector3[]>pointCloud=new List<Vector3[]>();
       
        pointCloud.Add(TranformToVector(b0));
        pointCloud.Add(TranformToVector(b1));
        pointCloud.Add(TranformToVector(b2));
        pointCloud.Add(TranformToVector(b3));
        pointCloud.Add(TranformToVector(b4));
        pointCloud.Add(TranformToVector(b5));


        if (pointCloud.Count != 0)
        {
            int index = 0;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int ToPoint;
                    int ToIndex0;
                    int ToIndex1;

                    Vector3 _start;
                    Vector3 _end0;
                    Vector3 _end1;

                    if (i % 2 == 0)
                    {
                        ToPoint = 0;
                        ToIndex0 = 2;
                        ToIndex1 = 3;
                    }
                    else
                    {
                        ToPoint = 1;
                        ToIndex0 = 1;
                        ToIndex1 = 2;
                    }
                    _start = pointCloud[i][j];
                    if (i < 4)
                    {
                        _end0 = pointCloud[i + ToIndex0][ToPoint];
                        _end1 = pointCloud[i + ToIndex1][ToPoint];
                    }
                    else
                    {
                        _end0 = pointCloud[0][ToPoint];
                        _end1 = pointCloud[1][ToPoint];
                    }

                    StaticStrings str0 = Instantiate(StringPrefab, transform);
                    StaticStrings str1 = Instantiate(StringPrefab, transform);
                    str0.ConnectString(_start, _end0, Thickness * 0.3f, index, "edge");
                    str1.ConnectString(_start, _end1, Thickness * 0.3f, index + 1, "edge");
                    AllSrings.Add(str0);
                    AllSrings.Add(str1);
                    index += 2;
                }
            }
        }
    }

    void updateBars()
    {
        AllBars[0].UpdateBar(b0[0].localPosition, b0[1].localPosition);
        AllBars[1].UpdateBar(b1[0].localPosition, b1[1].localPosition);
        AllBars[2].UpdateBar(b2[0].localPosition, b2[1].localPosition);
        AllBars[3].UpdateBar(b3[0].localPosition, b3[1].localPosition);
        AllBars[4].UpdateBar(b4[0].localPosition, b4[1].localPosition);
        AllBars[5].UpdateBar(b5[0].localPosition, b5[1].localPosition);
    }


    void clean()
    {
        if (AllBars.Count != 0)
        {
            foreach (var b in AllBars)
            {
                if (b != null)
                {
                    DestroyImmediate(b.gameObject);
                }
            }
            AllBars.Clear();
        }
    }

    public void VertexSwitch( bool swh)
    {
        Vertex.GetComponent<MeshRenderer>().enabled = swh;
    }

    public void Initialize(int index, int state,int subState, float thickness, float scale, bool flip)
    {
        Index = index;
        State = state;
        SubState = subState;
        SetSize(scale);

        Vertex = Instantiate(_vertexPrefab,transform);
        Vertex.transform.localScale = scale*Vector3.one;
        SetPoints(scale);
        Flip = flip;

        Thickness = thickness;
        gameObject.name = "IcosaVertex :" + Index + "/ Flip :" + Flip;

        UpdateState();
    }
}
