using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]private GameObject Bars;

    [SerializeField ]private GameObject Strings;

    private GameObject Obj;

    private GameObject A;

    private GameObject B;
	// Use this for initialization
	void Start ()
	{
	    A = Instantiate(Bars, transform);
	    B = Instantiate(Strings, transform);

	   
        Obj = A;
	    print("B");
        //  GameObject Bar = Instantiate(StaticBar, transform);

    }
	
	// Update is called once per frame
	void Update () {

	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        //ChangeObje();
	        print("a");

	        A.SetActive(false);
	    }

	}

    void ChangeObje()
    {
        if (Obj == A)
        {
            
            Obj = B;
            print("B");
        }
        else
        {
            Obj = A;
            print("A");
        }
    }

}
