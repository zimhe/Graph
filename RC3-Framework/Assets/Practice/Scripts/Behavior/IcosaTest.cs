using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcosaTest : MonoBehaviour {

	// Use this for initialization
    public bool flip;

	void Start () {
		
        GetComponent<TensileIcosaVertex>().SetPoints(10f,flip);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
