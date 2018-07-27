﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Portal_Entity : Scr_Entity
{

    public GameObject LatestEntity;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();

    }

   

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGERED");
        //if there are 2 portals
        if(GameObject.FindObjectsOfType<Scr_Portal_Entity>().Length == 2)
        {
            //If the object is a character
            if (other.tag == "Player")
            {
                //Teleport them
                other.transform.position = GetPartnerPortal().transform.position;
            }
        }


    }

    public bool IsBlocked()
    {
        return false;
    }

    public Scr_Portal_Entity GetPartnerPortal()
    {
        //Search through all portals in scene
        foreach(Scr_Portal_Entity Portal in GameObject.FindObjectsOfType<Scr_Portal_Entity>())
        {
            //Return the first one that isnt you
            if(Portal != this)
            {
                return Portal;
            }
        }

        return null;
    }
}