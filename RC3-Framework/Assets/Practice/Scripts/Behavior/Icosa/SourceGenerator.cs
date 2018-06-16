using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourceGenerator : MonoBehaviour
{

    [SerializeField] private Transform PosiSrcPrefab;
    [SerializeField] private Transform NegaSrcPrefab;


    List<Transform> PS=new List<Transform>();
    List<Transform> NS=new List<Transform>();

    private int Seed()
    {
        System.Random rdm=new System.Random();

        int s = rdm.Next(0, 2);
        return s;
    }

	// Use this for initialization
	void Start ()
	{
	  //GenerateSource();
	}

    public void GenerateSource(int _seed)
    {
        if (_seed == 0)
        {
            var ps = Instantiate(PosiSrcPrefab, transform);
            ps.transform.localPosition = new Vector3(Rdm(), 0f, Rdm());
            PS.Add(ps);
        }
        else if (_seed == 1)
        {
            var ns = Instantiate(NegaSrcPrefab, transform);

            ns.transform.localPosition = new Vector3(Rdm(), 0f, Rdm());

            NS.Add(ns);
        }

    }

    float Rdm()
    {
        float f = Random.Range(0f, 10f);
        return f;
    }

    public List<Transform> GetPosiSources()
    {
        return PS;
    }

    public List<Transform> GetNegaSources()
    {
        return NS;
    }

    public void toggle(bool enabled)
    {
        foreach (var n in NS)
        {
            n.GetComponent<MeshRenderer>().enabled = enabled;
        }

        foreach (var p in PS)
        {
            p.GetComponent<MeshRenderer>().enabled = enabled;
        }
    }

    void clearSource()
    {
        if (PS.Count != 0)
        {
            PS.ForEach(s=>Destroy(s.gameObject));

            PS.Clear();
        }

        if (NS.Count != 0)
        {
            NS.ForEach(s=>Destroy(s.gameObject));

            NS.Clear();
        }
    }

    private bool Enable;

	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.Space))
	    {
	        GenerateSource(0);
	    }

	    if (Input.GetKeyDown(KeyCode.V))
	    {
	        Enable = !Enable;
            toggle(Enable);
	    }

	    if (Input.GetKeyDown(KeyCode.C))
	    {
            clearSource();
	    }

	}
}
