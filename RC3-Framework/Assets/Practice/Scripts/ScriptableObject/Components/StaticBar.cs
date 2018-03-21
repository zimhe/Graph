using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticBar : MonoBehaviour
{
    private Vector3 [] P = new Vector3[2];

    private GameObject pivot;

    float  Length;

    private int index;

    private float Thickness;

    public void SetupBar(Vector3 p0, Vector3  p1,float _thickness,int _index,string _type)
    {

        Thickness = _thickness;

        P[0] = p0;
        P[1] = p1;

        var d = P[1] - P[0];

        var L = d.magnitude;

        Length = L;

        var T = gameObject.GetComponent<Transform>();

        T.localScale = new Vector3(Thickness, (L)/2, Thickness);
        T.localPosition  = (P [0]+P [1]) * 0.5f;
        T.localRotation  =Quaternion.FromToRotation(T.up , d);
        index = _index;
        gameObject.name = "Bar" + index+":"+_type;
    }

    public void UpdateBar(Vector3 p0, Vector3 p1)
    {

        if (!float.IsNaN(p0.x) && !float.IsNaN(p1.x) && !float.IsNaN(p0.y) && !float.IsNaN(p1.y) &&
            !float.IsNaN(p0.z) && !float.IsNaN(p1.z))
        {
            P[0] = p0;
            P[1] = p1;

            var d = P[1] - P[0];
            var L = d.magnitude;

            Length = L;

            var T = gameObject.GetComponent<Transform>();

            T.localScale = new Vector3(Thickness, (L) / 2, Thickness);
            T.localPosition = (P[0] + P[1]) * 0.5f;



            Vector3 fwd0 = Vector3.Cross(Vector3.left * 3f, d);

            if (fwd0 != Vector3.zero)
            {
                T.transform.localRotation = Quaternion.LookRotation(fwd0, d);
            }
            else
            {
                Vector3 fwd1 = Vector3.Cross(Vector3.down * 3f, d);
                T.transform.localRotation = Quaternion.LookRotation(fwd1, d);
            }
        }

    }


    public float  GetLength()
    {
        return Length;
    }

    public int GetIndex()
    {
        return index;
    }

    public Vector3 GetVector(int _index)
    {
        return P [_index ];
    }

} 
