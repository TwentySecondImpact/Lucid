using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Skill : MonoBehaviour
{
    #region Tracking variables

    #endregion

    // Use this for initialization
    protected virtual void Start ()
    {
		
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
		
	}

    public virtual void Activate(Scr_CharacterController _Character)
    {
        Debug.Log("Base Skill Activate");
    }
}
