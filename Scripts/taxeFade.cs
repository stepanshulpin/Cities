using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class taxeFade : MonoBehaviour {


    private Text txt;
    private Outline oLine;


	// Use this for initialization
	void Start () {
        txt = GetComponent <Text> ();
        oLine = GetComponent <Outline> ();
		
	}
	
	// Update is called once per frame
	void Update () {
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, Mathf.PingPong(Time.time, 1.0f));
	}
}
