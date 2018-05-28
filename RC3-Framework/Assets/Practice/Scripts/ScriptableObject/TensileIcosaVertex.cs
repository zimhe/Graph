using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpatialSlur.SlurCore;
using UnityEditor;
using UnityEngine;


public class TensileIcosaVertex : MonoBehaviour
{
    #region Constructors

    [SerializeField] private GameObject VertexPrefab;
    [SerializeField] private Transform PointPfb;
    [SerializeField] private StaticBar barPrefab;

    [SerializeField] private Color defualtColor;
    [SerializeField] private Color state1Color;
    [SerializeField] private Color state2Color;
    [SerializeField] private Color state3Color;

    List<StaticBar> AllBars=new List<StaticBar>(6);


    private GameObject Vertex;


    TensileIcosaVertex [] Neighbors=new TensileIcosaVertex[8];
    Vector3[] LP = new Vector3[8];

    public void SetNeighbor(int index, TensileIcosaVertex _v)
    {
        Neighbors[index] = _v;
    }

    public TensileIcosaVertex[] GetNeighbors()
    {
        return Neighbors;
    }
   

    private Transform[] APoints = new Transform[6];
    Vector3 [] SavedAPos=new Vector3[6];
    Vector3[] currentAPos = new Vector3[6];

    private Transform[] BPoints = new Transform[6];
    Vector3[] SavedBPos = new Vector3[6];
    Vector3[] currentBPos = new Vector3[6];

    public int Index { get; set; }

    private float Thickness;

    public float Scale { get; set; }

    public bool Flip { get; set; }

    private float edgeLength;

    private float H1;

    private float H2;

    #endregion


    Vector3 GetCircumCenter(Vector3 _a, Vector3 _b, Vector3 _c)
    {
        Vec3d A=new Vec3d(_a.x,_a.y,_a.z);
        Vec3d B = new Vec3d(_b.x, _b.y, _b.z);
        Vec3d C = new Vec3d(_c.x, _c.y, _c.z);

        var cc = GeometryUtil.GetCircumcenter(A, B, C);

        return new Vector3((float)cc.X,(float)cc.Y,(float)cc.Z);
    }


    public void SetPoints(float _scale)
    {
        Scale = _scale;

        edgeLength = _scale /( (1f + Mathf.Sqrt(5f)) / 2f);

        float a = Mathf.Sqrt(Mathf.Pow(_scale / 2f, 2) + Mathf.Pow(edgeLength / 2f, 2));

        float b = (_scale / 2f) / Mathf.Cos(Mathf.PI / 6f);

        float za0 = Mathf.Tan(Mathf.PI / 6f) * _scale / 2f;
        float za1 = _scale / 2f / Mathf.Cos(Mathf.PI / 6f);

        float zb0 = Mathf.Tan(Mathf.PI / 6f) * edgeLength / 2f;
        float zb1 = edgeLength / 2 / Mathf.Cos(Mathf.PI / 6f);

         H1 = Mathf.Sqrt(Mathf.Pow(a, 2) - Mathf.Pow(b, 2));

        H2 = Mathf.Sqrt(Mathf.Pow(_scale / 2f, 2) - Mathf.Pow(Mathf.Tan(Mathf.PI / 6f) * edgeLength / 2f,2));

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

        APoints[0].transform.localPosition=SavedAPos[0]=new Vector3(-_scale/2f,H1, - za0);

        APoints[1] = Instantiate(PointPfb, transform);

        APoints[1].transform.localPosition = SavedAPos[1]=new Vector3(0f,-H1, -za1);

        APoints[2] = Instantiate(PointPfb, transform);

        APoints[2].transform.localPosition= SavedAPos[2] = new Vector3(_scale / 2f, H1, -za0);

        APoints[3] = Instantiate(PointPfb, transform);

        APoints[3].transform.localPosition = SavedAPos[3] = new Vector3(-_scale / 2f, -H1, za0);

        APoints[4] = Instantiate(PointPfb, transform);

        APoints[4].transform.localPosition = SavedAPos[4] = new Vector3(0f, H1, za1);

        APoints[5] = Instantiate(PointPfb, transform);

        APoints[5].transform.localPosition = SavedAPos[5] = new Vector3(_scale / 2f, -H1, za0);

        BPoints[0] = Instantiate(PointPfb, transform);
        BPoints[0].transform.localPosition= SavedBPos[0] = new Vector3(-edgeLength/2f,-H2, -zb0);

        BPoints[1] = Instantiate(PointPfb, transform);
        BPoints[1].transform.localPosition= SavedBPos[1] = new Vector3(0f, H2, -zb1);

        BPoints[2] = Instantiate(PointPfb, transform);
        BPoints[2].transform.localPosition = SavedBPos[2] = new Vector3(edgeLength / 2f, -H2, -zb0);

        BPoints[3] = Instantiate(PointPfb, transform);
        BPoints[3].transform.localPosition = SavedBPos[3] = new Vector3(-edgeLength / 2f, H2, zb0);

        BPoints[4] = Instantiate(PointPfb, transform);
        BPoints[4].transform.localPosition = SavedBPos[4] = new Vector3(0f, -H2, zb1);

        BPoints[5] = Instantiate(PointPfb, transform);
        BPoints[5].transform.localPosition = SavedBPos[5] = new Vector3(edgeLength / 2f, H2, zb0);
    }


    /// <summary>
    /// 
    /// </summary>
    void UpdatePoints()
    {
        Vector3[] NP = new Vector3[8];

        float Lt = edgeLength / Scale;
        float Ht = H1 / H2;

        if (Neighbors[0] != null && Neighbors[2] != null)
        {
            var n0 = Neighbors[0].transform.localPosition;
            var n2 = Neighbors[2].transform.localPosition;
            NP[0] = GetCircumCenter(n0, n2, transform.localPosition);
        }
        else
        {
         NP[0]=new Vector3(SavedAPos[0].x,transform.localPosition.y,SavedAPos[0].z);
        }

        if (Neighbors[0] != null && Neighbors[1] != null)
        {
            var n0 = Neighbors[0].transform.localPosition;
            var n1 = Neighbors[1].transform.localPosition;
            NP[1] = GetCircumCenter(n0, n1, transform.localPosition);
        }
        else
        {
            NP[1] = new Vector3(SavedAPos[1].x, transform.localPosition.y, SavedAPos[1].z);
        }
        if (Neighbors[1] != null && Neighbors[3] != null)
        {
            var n1 = Neighbors[1].transform.localPosition;
            var n3 = Neighbors[3].transform.localPosition;
            NP[2] = GetCircumCenter(n1, n3, transform.localPosition);
        }
        else
        {
            NP[2] = new Vector3(SavedAPos[2].x, transform.localPosition.y, SavedAPos[2].z);
        }
        if (Neighbors[2] != null && Neighbors[4] != null)
        {
            var n2 = Neighbors[2].transform.localPosition;
            var n4 = Neighbors[4].transform.localPosition;
            NP[3] = GetCircumCenter(n2, n4, transform.localPosition);
        }
        else
        {
            NP[3] = new Vector3(SavedAPos[3].x, transform.localPosition.y, SavedAPos[3].z);
        }
        if (Neighbors[4] != null && Neighbors[5] != null)
        {
            var n4 = Neighbors[4].transform.localPosition;
            var n5 = Neighbors[5].transform.localPosition;
            NP[4] = GetCircumCenter(n4, n5, transform.localPosition);
        }
        else
        {
            NP[4] = new Vector3(SavedAPos[4].x, transform.localPosition.y, SavedAPos[4].z);
        }
        if (Neighbors[3] != null && Neighbors[5] != null)
        {
            var n3 = Neighbors[3].transform.localPosition;
            var n5 = Neighbors[5].transform.localPosition;
            NP[5] = GetCircumCenter(n3, n5, transform.localPosition);
        }
        else
        {
            NP[5] = new Vector3(SavedAPos[5].x, transform.localPosition.y, SavedAPos[5].z);
        }


        if (Neighbors[6] != null)
        {
            NP[6] = 0.5f * (Neighbors[7].transform.localPosition + transform.localPosition);
        }
        else
        {
            NP[6]=new Vector3(0f,transform.localPosition.y+H2,0f);
        }

        if (Neighbors[7] != null)
        {
            NP[7] = 0.5f * (Neighbors[8].transform.localPosition + transform.localPosition);
        }
        else
        {
            NP[7] = new Vector3(0f, transform.localPosition.y - H2, 0f);
        }

        foreach (var p in NP)
        {
            LP[Array.IndexOf(NP,p)] = transform.InverseTransformPoint(transform.parent.TransformPoint(p));
        }

        float py0;
        float py1;

        switch (Flip)
        {
            case true:
                py0 = LP[7].y;
                py1 = LP[6].y;
                break;
            case false:
                py0 = LP[6].y;
                py1 = LP[7].y;
                break;
            default:
                py0 = LP[6].y;
                py1 = LP[7].y;
                break;
        }

        currentAPos[0]=new Vector3(LP[0].x,py0*Ht,LP[0].z);
        currentBPos[0]=new Vector3(LP[0].x*Lt,py1,LP[0].z*Lt);

        currentAPos[1]=new Vector3(LP[1].x,py1*Ht,LP[1].z);
        currentBPos[1]=new Vector3(LP[1].x*Lt,py0,LP[1].z*Lt);

        currentAPos[2] = new Vector3(LP[2].x, py0 * Ht, LP[2].z);
        currentBPos[2] = new Vector3(LP[2].x * Lt, py1, LP[2].z * Lt);

        currentAPos[3] = new Vector3(LP[3].x, py1 * Ht, LP[3].z);
        currentBPos[3] = new Vector3(LP[3].x * Lt, py0, LP[3].z * Lt);

        currentAPos[4] = new Vector3(LP[4].x, py0 * Ht, LP[4].z);
        currentBPos[4] = new Vector3(LP[4].x * Lt, py1, LP[4].z * Lt);

        currentAPos[5] = new Vector3(LP[5].x, py1 * Ht, LP[5].z);
        currentBPos[5] = new Vector3(LP[5].x * Lt, py0, LP[5].z * Lt);
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

    #region barsConstruction

    Transform[] b0 = new Transform[2];
    Transform[] b1 = new Transform[2];
    Transform[] b2 = new Transform[2];
    Transform[] b3 = new Transform[2];
    Transform[] b4 = new Transform[2];
    Transform[] b5 = new Transform[2];

    void setBars()
    {
        var bar0 = Instantiate(barPrefab, transform);
        var bar1 = Instantiate(barPrefab, transform);
        var bar2 = Instantiate(barPrefab, transform);
        var bar3 = Instantiate(barPrefab, transform);
        var bar4 = Instantiate(barPrefab, transform);
        var bar5 = Instantiate(barPrefab, transform);

        bar0.SetupBar(b0[0].localPosition,b0[1].localPosition,Thickness,0,"vertex"+Index);
        bar1.SetupBar(b1[0].localPosition, b1[1].localPosition, Thickness, 1, "vertex" + Index);
        bar2.SetupBar(b2[0].localPosition, b2[1].localPosition, Thickness, 2, "vertex" + Index);
        bar3.SetupBar(b3[0].localPosition, b3[1].localPosition, Thickness, 3, "vertex" + Index);
        bar4.SetupBar(b4[0].localPosition, b4[1].localPosition, Thickness, 4, "vertex" + Index);
        bar5.SetupBar(b5[0].localPosition, b5[1].localPosition, Thickness, 5, "vertex" + Index);

        AllBars.AddRange(new List<StaticBar>{ bar0, bar1, bar2, bar3, bar4, bar5 });
    }
    void updateBars()
    {
       AllBars[0].UpdateBar(b0[0].localPosition,b0[1].localPosition);
        AllBars[1].UpdateBar(b1[0].localPosition, b1[1].localPosition);
        AllBars[2].UpdateBar(b2[0].localPosition, b2[1].localPosition);
        AllBars[3].UpdateBar(b3[0].localPosition, b3[1].localPosition);
        AllBars[4].UpdateBar(b4[0].localPosition, b4[1].localPosition);
        AllBars[5].UpdateBar(b5[0].localPosition, b5[1].localPosition);

    }
    #endregion


    #region StateManage

    

 
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
                break;

            case 1:

                if (Vertex.GetComponent<MeshRenderer>().enabled)
                    Vertex.GetComponent<MeshRenderer>().enabled = false;

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

                if (Vertex.GetComponent<MeshRenderer>().enabled)
                    Vertex.GetComponent<MeshRenderer>().enabled = false;

                if (AllBars.Count == 0)
                {
                    setBars();
                    print("setbars");
                }
                else
                {
                    updateBars();
                }

                break;
        }
    }
    #endregion


    #region public Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_state"></param>
    /// <param name="_thickness"></param>
    /// <param name="_scale"></param>
    public void SetUpStructure(int _index,int _state,float _thickness,float _scale,bool _flip)
    {
        Index = _index;
        State = _state;
        SetPoints(_scale);
        Flip = _flip;

        Thickness = _thickness;

        Vertex = Instantiate(VertexPrefab, transform);
        Vertex.transform.localPosition=Vector3.zero;

        UpdateState();

    }

    /// <summary>
    /// 
    /// </summary>
    public void UpdatePosition()
    {

        for (int i = 0; i < APoints.Length; i++)
        {
            APoints[i].transform.localPosition =
                Vector3.Lerp(APoints[i].transform.localPosition, currentAPos[i], Time.deltaTime);
            BPoints[i].transform.localPosition =
                Vector3.Lerp(BPoints[i].transform.localPosition, currentBPos[i], Time.deltaTime);
        }
        UpdateState();
    }


  

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_state"></param>
    public void SetState(int _state)
    {
        State = _state;
        UpdateState();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_state"></param>
    /// <param name="_subState"></param>
    public void SetState(int _state,int _subState)
    {
        State = _state;
        SubState = _subState;
        UpdateState();
    }
    #endregion
}
