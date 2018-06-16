using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    List<Cells> C=new List<Cells>(3);

    public Cells getCell(int index)
    {
        return C[index];
    }

    public void addCell(Cells c)
    {
        C.Add(c);
    }


}
