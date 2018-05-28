using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcosaTest : MonoBehaviour {

	// Use this for initialization
    public bool flip;

    public int SubState;

    private Vector3 v;

	void Start () {
		
        GetComponent<TensileIcosaVertex>().SetUpStructure(0,1,0.1f,10f,flip);
    
        GetComponent<TensileIcosaVertex>().SetState(1,SubState);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
