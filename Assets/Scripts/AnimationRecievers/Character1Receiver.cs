using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character1Receiver : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void ActivateFromAnimation()
    {
        transform.parent.GetComponent<Scr_Skill_Push>().ActivateFromAnimation();
    }

}
