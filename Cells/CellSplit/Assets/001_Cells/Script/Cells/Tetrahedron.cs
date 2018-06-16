using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrahedron
{
    private List<Triangle> T=new List<Triangle>(3);


    public Triangle GetTriangle(int index)
    {

        return T[index];

    }

    public void addTriangle(Triangle t)
    {
        T.Add(t);
    }
}
