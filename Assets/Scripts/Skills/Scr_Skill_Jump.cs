using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Skill_Jump : Scr_Skill
{
    public float JumpRange;

    enum JumpState
    {
        Neutural,
        Aiming,
        Jumping
    }

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
    }

    IEnumerator AimingLoop(Scr_CharacterController _Character)
    {
        _Character.Busy = true;
        yield return null;
        _Character.Busy = false;
    }
}
