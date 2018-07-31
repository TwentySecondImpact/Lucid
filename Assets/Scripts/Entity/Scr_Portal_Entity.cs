using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Portal_Entity : Scr_Entity
{
    public bool Active = false;
    float ArmingTime = 0;
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

    public override void Skill_Phase()
    {
        //Do nothing so it overrides the phasing, making this entity unable to be phased
    }

    public bool IsBlocked()
    {
        for (int i = 0; i < GetPartnerPortal().transform.childCount; i++)
        {
            if(GetPartnerPortal().transform.GetChild(i).tag != "Player")
            {
                return true;
            }
        }
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
