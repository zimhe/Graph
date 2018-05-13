using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTriangeGrid : MonoBehaviour
{

    [SerializeField] private int SegmentCount = 3;


    void Awake()
    {

    }


	// Use this for initialization
	void Start ()
	{
	    int vertexCount = 0;
	    for (int y = 0; y < 5; y++)
	    {
	        for (int z = 0; z < SegmentCount; z++)
	        {
	            for (int x = 0; x < SegmentCount - z; x++)
	            {
	                vertexCount++;
	                var rowVertexCount = ((z * SegmentCount - (SegmentCount - z + 1) * z) / 2 + (SegmentCount - z + 1) * z);
                    var layerVertexCount= y * ((SegmentCount * SegmentCount - SegmentCount) / 2 + SegmentCount);

	                int id = x + rowVertexCount + layerVertexCount;
	                print(id);
	            }
	        }
        }
	
     



	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
