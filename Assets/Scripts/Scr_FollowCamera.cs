using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FollowCamera : MonoBehaviour
{

    public Transform TargetTransform;
    public Vector3 TargetOffset;

	// Use this for initialization
	void Start ()
    {
        TargetOffset = transform.position - TargetTransform.position;
        transform.parent = null;
	}
	
	// Update is called once per frame
	void Update ()
    {
        TargetTransform = Scr_PlayerController.inst.Characters[0].transform;

        UpdatePosition();
	}

    void UpdatePosition()
    {

        transform.position = Vector3.LerpUnclamped(transform.position, TargetTransform.position +  TargetOffset, 6 * Time.deltaTime);
    }
}
