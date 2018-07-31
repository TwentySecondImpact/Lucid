using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Skill_Phase : Scr_Skill
{

    Scr_Entity PhasedEntity;

    // Use this for initialization
    protected override void Start()
    {
        //Runs the parent scripts Start function
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        //Runs the parent scripts Update function
        base.Update();
    }

    public override void Activate(Scr_CharacterController _Character)
    {
        //Runs the parent scripts Activate function
        base.Activate(_Character);

        if(PhasedEntity == null)
        {
            //Create a forward ray from the center of the objects collider
            Ray GrabRay = new Ray(transform.position + new Vector3(0, -0.7f, 0), _Character.transform.forward);
            RaycastHit GrabHit = new RaycastHit();
            //Cast the ray
            if (Physics.Raycast(GrabRay, out GrabHit, 2))
            {
                Debug.Log("Shot ray");
                Debug.Log(GrabHit.transform.gameObject.name);
                //If it hits something Check that it is an entity 
                if (GrabHit.transform.gameObject.GetComponent<Scr_Entity>() != null)
                {
                    Debug.Log("Hit Entity");
                    //If its an entity, run the entities "Hit by push" function
                    PhasedEntity = GrabHit.transform.gameObject.GetComponent<Scr_Entity>();
                    Debug.Log("Phasing Object");
                    PhasedEntity.Skill_Phase();
                    //Toggle on channeling
                    _Character.Channeling = true;
                    //Also make the portal creator independant
                    Scr_PlayerController.inst.SeperateCharacter(Scr_PlayerController.inst.GetPartyIndex(_Character));
                }
            }
        }
        else
        {
            //Check if there is anything in the collider

            Collider EntityCollider = PhasedEntity.EntityCollider;

            if(Physics.BoxCast(EntityCollider.bounds.center, EntityCollider.bounds.extents, new Vector3(0,0,0)))
            {
                Debug.Log("Something blocking th ephase");
            }
            else
            {
                //Turn off channeling
                _Character.Channeling = false;
                Debug.Log("Unphasing Object");
                PhasedEntity.Skill_Phase();
                PhasedEntity = null;
            }


            
        }
        
    }
}
