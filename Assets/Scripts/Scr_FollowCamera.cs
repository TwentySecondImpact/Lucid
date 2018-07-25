using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_FollowCamera : MonoBehaviour
{

    public Transform TargetTransform;

    public Vector3 TargetOffset;
    public Quaternion TargetRotation;

	// Use this for initialization
	void Start ()
    {
        TargetOffset = transform.position - TargetTransform.position;
        TargetRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdatePosition();
	}

    void UpdatePosition()
    {
        transform.position = TargetTransform.position +  TargetOffset;
        transform.rotation = TargetRotation;
    }
}
