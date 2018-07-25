using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Block_Breakable : Scr_Entity
{

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

    #region Skill Interactions
    public override void Skill_Push(Scr_CharacterController _Character, Vector3 _HitPoint)
    {
        //base.Skill_Push();
        Debug.Log("Breakable block Got Pushed");
        Destroy(this.gameObject);
    }

    public override void Skill_Pull(Scr_CharacterController _Character, Vector3 _HitPoint)
    {
        base.Skill_Pull(_Character, _HitPoint);
    }

    public override void Skill_Jump()
    {
        base.Skill_Jump();
    }

    #endregion


}
