using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_GarrethAnimationReceiver : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
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
            case "Push":
                Debug.Log("Activate Push");
                transform.parent.GetComponent<Scr_Skill_Push>().ActivateFromAnimation();
                break;
            case "Pull":
                Debug.Log("Activate Pull");
                transform.parent.GetComponent<Scr_Skill_Pull>().ActivateFromAnimation();
                break;
        }
    }
}
