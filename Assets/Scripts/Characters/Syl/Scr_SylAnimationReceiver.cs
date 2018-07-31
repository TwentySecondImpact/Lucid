using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_SylAnimationReceiver : MonoBehaviour
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ActivateFromAnimation(string _SkillName)
    {
        Debug.Log("Activate Ability from animation");
        switch (_SkillName)
        {
            case "PortalOne":
                Debug.Log("Activate Portal One");
                transform.parent.GetComponent<Scr_Skill_Portal_One>().ActivateFromAnimation();
                break;
            case "PortalTwo":
                Debug.Log("Activate Portal Two");
                transform.parent.GetComponent<Scr_Skill_Portal_Two>().ActivateFromAnimation();
                break;
        }
    }
}
