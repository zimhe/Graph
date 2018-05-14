using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Button = UnityEngine.UI.Button;

public class ApplicationControl : MonoBehaviour
{





    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Application.Quit);
    }

}
