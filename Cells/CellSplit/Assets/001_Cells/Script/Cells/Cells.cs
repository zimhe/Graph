using System.Collections;
using System.Collections.Generic;
using RC3.Unity;
using UnityEngine;

public class Cells : VertexObject
{
    private float Radius;

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

    


}
