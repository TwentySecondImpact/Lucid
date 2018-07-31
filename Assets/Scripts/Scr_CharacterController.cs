using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Scr_CharacterController : MonoBehaviour
{
    #region Tracking Variables
    [HideInInspector] public Vector3 InputVector;
    [HideInInspector] public Quaternion TargetRotation;
    [HideInInspector] public Vector3 TargetPosition;
    public float CurrentAccelerationPercentage;
    List<float> AccelerationHistory = new List<float>();
    public int AccelerationHistoryLength;
    public List<Scr_PositionTimeStamp> PositionTimeStamps = new List<Scr_PositionTimeStamp>();
    public Vector3 PreviousPosition;
    public float TimeStampDistance;
    public float DistanceTracker;
    public bool Independent = false;

    public bool Busy;
    public bool Channeling = false;
    //This is a list of stings that represent all the things this object is waiting to resolve before contorl if returned
    public List<string> WaitingList = new List<string>();

    public GameObject FloorObject;
    public GameObject PreviousFloorObject;

    #endregion

    #region Tweaking Variables
    public bool UnderPlayerControl;
    public float HightDelta;
    [Tooltip("The time in seconds it would take a character to make a full 360 degree spin")]  public float TurnTime;
    public float AccelerationSpeed;
    public float DecelerationSpeed;
    public float RunSpeed;
    //The time between input snapshots
    public float InputWaitTime;
    float InputWaitTimer;
    public int MaxTimeStamps;
    public float MinimumDistance;
    public float MaxFollowSpeed;
    public float UnPartyDistance;
    public float CharacterHeight;
    #endregion

    #region External Object References
    public Scr_CharacterController Leader;
    public GameObject AimingReticle;
    public Animator CharacterAnimator;
    #endregion

    #region Internal Component References
    Rigidbody rb;
    public Scr_Skill SkillOne;
    public Scr_Skill SkillTwo;
    public Collider CharacterCollider;
    #endregion

    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        PreviousPosition = transform.position;

        //Put an initial entry into the position time stamps
        Scr_PositionTimeStamp TimeStamp = new Scr_PositionTimeStamp(Time.time, transform.position);
        PositionTimeStamps.Insert(0, TimeStamp);

        //Adds the initial acceleratoin to the list
        AccelerationHistory.Insert(0, CurrentAccelerationPercentage);

        //Set the floor object
        UpdateTransformParenting();
        //Set the previous floor object
        PreviousFloorObject = FloorObject;

    }
	
	void Update ()
    {
        UpdateBusy();

        if (UnderPlayerControl)
        {
            UpdateInput();
            if (!Channeling)
            {
                UpdateDirection();
                UpdatePosition();
            }
        }
        else
        {
            UpdateFollowPosition();
        }


        UpdateTransformParenting();
        UpdateInputTimeStamp();
        UpdateAnimatorParamaters();
        
	} 

    private void LateUpdate()
    {
        DistanceTracker += (transform.position - PreviousPosition).magnitude;
        PreviousPosition = transform.position;

        //Add to the acceleration history
        AccelerationHistory.Insert(0, CurrentAccelerationPercentage);
        //Trim if too large
        if(AccelerationHistory.Count > AccelerationHistoryLength)
        {
            AccelerationHistory.RemoveAt(AccelerationHistory.Count - 1);
        }

        //Save the previous FloorObject
        PreviousFloorObject = FloorObject;
    }

    void UpdateBusy()
    {
        if(WaitingList.Count == 0)
        {
            Busy = false;
        }
        else
        {
            Busy = true;
        }
    }

    void UpdateInput()
    {
        //Determine the input vector from unity input
        InputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Ability Inputs
        if(UnderPlayerControl == true && Busy == false)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                SkillOne.Activate(this);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SkillTwo.Activate(this);
            }
        }
    }

    void UpdateDirection()
    {
        #region Update Target Rotation
        //If there is any direction input detected
        if (InputVector.sqrMagnitude > 0)
        {
            //Determine the target direction the character wants to be facing
            TargetRotation = Quaternion.LookRotation(InputVector);
        }
        #endregion

        #region Slerp character towards rotation
        //Dirty Slerp from current rotation to target rotation
        //transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);

        //Rotate towards the target. Better Way
        transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, ((360 / TurnTime) * Time.deltaTime));
        #endregion

    }

    void UpdatePosition()
    {
        #region Update Acceleration
        //If there is any direction input detected
        if(InputVector.sqrMagnitude > 0)
        {
            //Increase the acceleration percentage
            CurrentAccelerationPercentage = Mathf.Clamp01(CurrentAccelerationPercentage + (AccelerationSpeed * Time.deltaTime));
        }
        //If there is no directoin input
        else
        {
            //Reduce the acceleration
            CurrentAccelerationPercentage = Mathf.Clamp01(CurrentAccelerationPercentage - (DecelerationSpeed * Time.deltaTime));
        }
        #endregion

        #region Update Horizontal Position

        #region Collision Rays

        Vector3 CollisionVelocity = transform.forward;
        float DropDistance = 0.7f;
        float DropReach = 0.5f;
        float ReachDistance = 0.5f;

        #region Forward Collision

        //Check Forward
        Ray CollisionRay = new Ray(transform.position + new Vector3(0, -0.70f, 0), new Vector3(0,0,1));
        RaycastHit CollisionHit = new RaycastHit();
        if ((Physics.Raycast(CollisionRay, out CollisionHit, ReachDistance)))
        {
            //Debug.Log("Hitting forward Wall");
            //Moving Forward
            if(CollisionHit.transform.tag != "Player")
            {
                if (CollisionVelocity.z > 0)
                {
                    CollisionVelocity.z = 0;
                }
            }
        }

        //Check Forward & Down
        Ray DropRay = new Ray(transform.position + new Vector3(0, -0.70f, DropReach), new Vector3(0, -1, 0));
        RaycastHit DropHit = new RaycastHit();
        if(!Physics.Raycast(DropRay, out DropHit, DropDistance))
        {
            if (CollisionVelocity.z > 0)
            {
                CollisionVelocity.z = 0;
            }
        }
        #endregion

        #region Backward Collision
        //Check Forward
        CollisionRay = new Ray(transform.position + new Vector3(0, -0.70f, 0), new Vector3(0, 0, -1));
        CollisionHit = new RaycastHit();

        //If collidiong forward
        if (Physics.Raycast(CollisionRay, out CollisionHit, ReachDistance))
        {
            //Debug.Log("Hitting forward Wall");
            //Moving Forward
            if (CollisionHit.transform.tag != "Player")
            {
                if (CollisionVelocity.z < 0)
                {
                    CollisionVelocity.z = 0;
                }
            }
        }

        //Check Forward Down
        DropRay = new Ray(transform.position + new Vector3(0, -0.70f, -DropReach), new Vector3(0, -1, 0));
        DropHit = new RaycastHit();

        if (!Physics.Raycast(DropRay, out DropHit, DropDistance))
        {
            if (CollisionVelocity.z < 0)
            {
                CollisionVelocity.z = 0;
            }
        }
        #endregion

        #region Right Collision
        //Check Forward
        CollisionRay = new Ray(transform.position + new Vector3(0, -0.70f, 0), new Vector3(1, 0, 0));
        CollisionHit = new RaycastHit();

        //If collidiong forward
        if (Physics.Raycast(CollisionRay, out CollisionHit, ReachDistance))
        {
            //Debug.Log("Hitting forward Wall");
            //Moving Forward
            if (CollisionHit.transform.tag != "Player")
            {
                if (CollisionVelocity.x > 0)
                {
                    CollisionVelocity.x = 0;
                }
            }
        }

        //Check Foward Down
        DropRay = new Ray(transform.position + new Vector3(DropReach, -0.70f, 0), new Vector3(0, -1, 0));
        DropHit = new RaycastHit();

        if (!Physics.Raycast(DropRay, out DropHit, DropDistance))
        {
            if (CollisionVelocity.x > 0)
            {
                CollisionVelocity.x = 0;
            }
        }
        #endregion

        #region Left Collision
        //Check Forward
        CollisionRay = new Ray(transform.position + new Vector3(0, -0.70f, 0), new Vector3(-1, 0, 0));
        CollisionHit = new RaycastHit();

        //If collidiong forward
        if (Physics.Raycast(CollisionRay, out CollisionHit, ReachDistance))
        {
            //Debug.Log("Hitting forward Wall");
            //Moving Forward
            if (CollisionHit.transform.tag != "Player")
            {
                if (CollisionVelocity.x < 0)
                {
                    CollisionVelocity.x = 0;
                }
            }
        }

        //Check Forward Down
        DropRay = new Ray(transform.position + new Vector3(-DropReach, -0.70f, 0), new Vector3(0, -1, 0));
        DropHit = new RaycastHit();

        if (!Physics.Raycast(DropRay, out DropHit, DropDistance))
        {
            //Moving Forward
            if (CollisionVelocity.x < 0)
            {
                CollisionVelocity.x = 0;
            }
        }
        #endregion

        #endregion

            //Move the character forward based on rotation
            transform.position += CollisionVelocity * RunSpeed * CurrentAccelerationPercentage * Time.deltaTime;
        

        #endregion

        #region Update Vertical Position

        //Create a downwards rayd from the center of the objects collider
        Ray ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), new Vector3(0, -1, 0));
        RaycastHit hit = new RaycastHit();
        //Cast the ray
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag != "Player")
            {
                //Snap to floor
                transform.position = SetTransformY(transform.position, hit.point.y + HightDelta);
            }
            
        }
        #endregion
    }

    void UpdateFollowPosition()
    {
        if(Leader != null)
        {
            #region Distance Detachment
            //If they are on a moveable block
            if(Leader.transform.parent.GetComponent<Scr_Block_Movable>() != null)
            {
                //And they are too far away
                float distance = (transform.position - Leader.transform.position).magnitude;
                //If they are too far away
                if(distance > UnPartyDistance)
                {
                    //Break up the party
                    Scr_PlayerController.inst.SeperateCharacter(Scr_PlayerController.inst.GetPartyIndex(this));
                    //Get out of the Follow function, now that you dont have a leadrr
                    return;
                }
            }
            #endregion

            //Gets the oldest TimeStamp from the leader and sets it to the target
            Scr_PositionTimeStamp TargetTimeStamp = Scr_PlayerController.inst.GetPositionTimeStampFromCharacter(Leader);

            //Set the target rotation
            TargetRotation = Quaternion.LookRotation(TargetTimeStamp.Position - transform.position);

            //Update the target position
            TargetPosition = TargetTimeStamp.Position;

            #region Simulated Acceleration
            //Simulated Acceleration


            //If you are holind down a movement then use the archived acceleration
            CurrentAccelerationPercentage = Leader.AccelerationHistory[Leader.AccelerationHistory.Count - 1];

            //Update Acceleration
            //CurrentAccelerationPercentage = Leader.CurrentAccelerationPercentage;
            #endregion

            //Update Rotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, ((360 / TurnTime) * Time.deltaTime));
            

            //Update Position     //This bit is intentional, as it scales the follow speed based on the distance to the target point
            //Determine the full vector
            Vector3 FollowVector = (TargetPosition - transform.position) * RunSpeed * CurrentAccelerationPercentage;
            //Determine the magnitude
            float FollowVectorMagnitude = Mathf.Clamp(FollowVector.magnitude, 0, MaxFollowSpeed);
            //Clamp the magnitude
            FollowVector = FollowVector.normalized * FollowVectorMagnitude * Time.deltaTime;
            //Normalize and reapply new magnatude

            transform.position += FollowVector;
        }
        //If you dont have a leader, slow down consistently still
        else
        {
            //update Decelration
            CurrentAccelerationPercentage = Mathf.Clamp01(CurrentAccelerationPercentage - (DecelerationSpeed * Time.deltaTime));
            //Move the character forward based on rotation
            transform.position += transform.forward * RunSpeed * CurrentAccelerationPercentage * Time.deltaTime;
        }
    }

    void UpdateInputTimeStamp()
    {
        if(DistanceTracker > TimeStampDistance)
        {
            DistanceTracker = 0;

            Scr_PositionTimeStamp TimeStamp = new Scr_PositionTimeStamp(Time.time, transform.position);
            PositionTimeStamps.Insert(0, TimeStamp);


            if (PositionTimeStamps.Count > MaxTimeStamps)
            {
                PositionTimeStamps.RemoveAt(MaxTimeStamps);
            }
        }
    }

    void UpdateTransformParenting()
    {
        #region Update Platform Parenting
        //Create a downward ray
        Ray DownRay = new Ray(transform.position, new Vector3(0, -1, 0));
        RaycastHit DownHit = new RaycastHit();
        //Check what you are currently standing on
        if (Physics.Raycast(DownRay, out DownHit, (CharacterHeight / 2) + 1))
        {
            FloorObject = DownHit.transform.gameObject;

            if (FloorObject.layer == 8)
            {
                transform.parent = FloorObject.transform;
            }
            else
            {
                transform.parent = Scr_PlayerController.inst.transform;
            }
        }
        #endregion

        #region Portal Check
        if(PreviousFloorObject != null && FloorObject != null)
        {
            //If the previous floor object was not a portal
            if (PreviousFloorObject.GetComponent<Scr_Portal_Entity>() == null)
            {
                //And the current one is
                if (FloorObject.GetComponent<Scr_Portal_Entity>() != null)
                {
                    if(FloorObject.GetComponent<Scr_Portal_Entity>().GetPartnerPortal() != null)
                    {
                        if(FloorObject.GetComponent<Scr_Portal_Entity>().IsBlocked() == false)
                        {
                            //Teleport the character
                            Scr_Portal_Entity TargetPortal = FloorObject.GetComponent<Scr_Portal_Entity>().GetPartnerPortal();
                            Vector3 TargetPosition = TargetPortal.transform.position + new Vector3(0, CharacterHeight / 2, 0);
                            transform.position = TargetPosition;

                            //Make the character independant
                            Scr_PlayerController.inst.SeperateCharacter(Scr_PlayerController.inst.GetPartyIndex(this));
                        }
                        
                    }
                }
            }
        }

        #endregion
    }

    void UpdateAnimatorParamaters()
    {
        //ACP_AccelerationPercentage.defaultFloat = CurrentAccelerationPercentage;
        CharacterAnimator.SetFloat("AccelerationPercentage", CurrentAccelerationPercentage);
    }
    public Vector3 SetTransformY(Vector3 _Vector, float newY)
    {
        //Grab Vector
        Vector3 modifiedVector = _Vector;

        //Change Vector
        modifiedVector.y = newY;

        //Return Vector
        return modifiedVector;
    }

    private void OnDrawGizmos()
    {
        //foreach(Scr_PositionTimeStamp timeStamp in PositionTimeStamps)
        //{
        //    Gizmos.DrawCube(timeStamp.Position, (Vector3.one / 5));
        //}
    }
}

[Serializable]
public class Scr_PositionTimeStamp
{
    public float TimeStamp;
    public Vector3 Position;

    public Scr_PositionTimeStamp(float _timeStamp, Vector3 _position)
    {
        TimeStamp = _timeStamp;
        Position = _position;
    }
}


