using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Skill_PushPull : Scr_Skill
{
    public float PullRange;

	// Use this for initialization
	protected override void Start ()
    {
        //Runs the parent scripts Start function
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update ()
    {
        //Runs the parent scripts Update function
        base.Update();
	}

    public override void Activate(Scr_CharacterController _Character)
    {
        //Runs the parent scripts Activate function
        base.Activate(_Character);

        Debug.Log("Push Pull Activate");
        _Character.CharacterAnimator.SetTrigger("Skill_OneShot");

        //Create a forward ray from the center of the objects collider
        Ray PushRay = new Ray(transform.position + new Vector3(0, -0.7f, 0), _Character.transform.forward);
        RaycastHit PushHit = new RaycastHit();
        //Cast the ray
        if (Physics.Raycast(PushRay, out PushHit, 2))
        {
            Debug.Log("Shot ray");
            Debug.Log(PushHit.transform.gameObject.name);
            //If it hits something Check that it is an entity 
            if (PushHit.transform.gameObject.GetComponent<Scr_Entity>() != null)
            {
                Debug.Log("Hit Entity");
                //If its an entity, run the entities "Hit by push" function
                PushHit.transform.gameObject.GetComponent<Scr_Entity>().Skill_Push(_Character, PushHit.point);
            }
        }
        else
        {
            //Create a forward ray from the center of the objects collider
            Ray PullRay = new Ray(transform.position + new Vector3(0, -0.7f, 0), _Character.transform.forward);
            RaycastHit PullHit = new RaycastHit();
            //Cast the ray
            if (Physics.Raycast(PullRay, out PullHit, PullRange))
            {
                Debug.Log("Shot ray");
                Debug.Log(PullHit.transform.gameObject.name);

                //If the object was at least 2 units away
                float Distance = (PullHit.transform.position - _Character.transform.position).magnitude;
                Debug.Log("Distance from block: " + Distance);
                if(Distance > 2)
                {
                    //If it hits something Check that it is an entity 
                    if (PullHit.transform.gameObject.GetComponent<Scr_Entity>() != null)
                    {
                        Debug.Log("Hit Entity");
                        //If its an entity, run the entities "Hit by push" function
                        PullHit.transform.gameObject.GetComponent<Scr_Entity>().Skill_Pull(_Character, PullHit.point);
                    }
                }
            }
        }
    }

    

    public void PushImpact()
    {
        Debug.Log("Impact Pushed");
    }
}
