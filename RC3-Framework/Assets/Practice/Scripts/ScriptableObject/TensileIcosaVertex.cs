using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SpatialSlur.SlurCore;

using UnityEngine;


public class TensileIcosaVertex : MonoBehaviour
{

    [SerializeField] private GameObject EdgeIndicatorPfb;
    [SerializeField] private GameObject ShapePfb;
    [SerializeField] private Transform PointPfb;

    [SerializeField] private Color defualtColor;
    [SerializeField] private Color state1Color;
    [SerializeField] private Color state2Color;
    [SerializeField] private Color state3Color;

    int [] Neighbors=new int[6];

   

    private Transform[] APoints = new Transform[6];
    Vector3 [] SavedAPos=new Vector3[6];

    private Transform[] BPoints = new Transform[6];
    Vector3[] SavedBPos = new Vector3[6];

    public float Scale { get; set; }

    public bool Flip { get; set; }

    private float edgeLength;

    private float H1;

    private float H2;

    


    Vector3 GetCircumCenter(Vector3 _a, Vector3 _b, Vector3 _c)
    {
        Vec3d A=new Vec3d(_a.x,_a.y,_a.z);
        Vec3d B = new Vec3d(_b.x, _b.y, _b.z);
        Vec3d C = new Vec3d(_c.x, _c.y, _c.z);

        var cc = GeometryUtil.GetCircumcenter(A, B, C);

        return new Vector3((float)cc.X,(float)cc.Y,(float)cc.Z);
    }


    public void SetPoints(float _scale,bool _flip)
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

        switch (_flip)
        {
            case true:
                H1 = -H1;
                H2 = -H2;
                break;

            case false:
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

    void UpdatePoints()
    {



    }

}
