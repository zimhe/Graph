using System;
using System.Collections;
using System.Collections.Generic;
using SpatialSlur.SlurCore;
using UnityEngine;
using UnityEngine.UI;

public class InputSwitcher : MonoBehaviour
{
    private List<InputField> input;


	// Use this for initialization
	void Start ()
    {

        input =new List<InputField>(transform.childCount);

      
        input.AddRange(GetComponentsInChildren<InputField>());

	}
	
	// Update is called once per frame
	void Update ()
    {
		inputDetecet();
	}

    void inputDetecet()
    {
        foreach (var ip in input)
        {
            if (ip.isFocused == true)
            {
                int id = Array.IndexOf(input.ToArray(), ip);

                int moveTo;

                int moveBack;
                if (id < input.Count - 1)
                {
                 
                    moveTo = id + 1;
                }
                else
                {
                    moveTo = 0;
                }
                if (id > 0)
                {
                    moveBack = id - 1;
                }
                else
                {
                    moveBack = input.Count - 1;
                }

                if (Input.GetKeyDown(KeyCode.Tab)||Input.GetKeyDown(KeyCode.RightArrow))
                {
                   
                    input[moveTo].ActivateInputField();
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {

                    input[moveBack].ActivateInputField();
                }
            }
        }
    }


}
