using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Entity : MonoBehaviour
{
    public bool Pushable;
    public bool Pullable;
    public bool Breakable;
    public Collider EntityCollider;
    public int UnitScale;
    public bool Busy = false;

    public bool PhasedOut = false;

    public GameObject FloorObject;
    public GameObject PreviousFloorObject;

    public bool WaitingToTeleport = false;
    public Vector3 TeleportTarget;

	// Use this for initialization
	protected virtual void Start ()
    {
        EntityCollider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        UpdateTransformParenting();
    }

    private void LateUpdate()
    {
        PreviousFloorObject = FloorObject;
    }

    #region Skill interactions
    public virtual void Skill_Push(Scr_CharacterController _Character, Vector3 _HitPoint)
    {
        if (!Busy)
        {
            Debug.Log("Breakable block Got Pushed");
            Vector3 Direction = _HitPoint - _Character.transform.position;
            Direction.y = 0;
            Direction.Normalize();
            Debug.Log(Direction);
            //Debug.Log("Is object next to: " + HasObjectNextTo(collider.bounds.center, Direction, 2));
            if (HasObjectNextTo(EntityCollider.bounds.center, SnapVectorToGrid(Direction), UnitScale) == false)
            {
                StartCoroutine(ShiftPosition(transform.position + SnapVectorToGrid(Direction) * 2));
            }
        }
    }

    public virtual void Skill_Pull(Scr_CharacterController _Character, Vector3 _HitPoint)
    {
        if (!Busy)
        {
            Debug.Log("Breakable block Got Pulled");
            Vector3 Direction = _HitPoint - _Character.transform.position;
            Direction.y = 0;
            Direction.Normalize();
            Debug.Log(Direction);

            if (HasObjectNextTo(EntityCollider.bounds.center, -Direction, UnitScale) == false)
            {
                StartCoroutine(ShiftPosition(transform.position - SnapVectorToGrid(Direction) * 2));
            }
        }
    }

    public virtual void Skill_Phase()
    {
        Debug.Log("Object Phased");

        if (!Busy)
        {
            if(PhasedOut)
            {
                PhaseIn();
            }
            else
            {
                PhaseOut();
            }
        }
    }

    public void PhaseOut()
    {
        PhasedOut = true;
        EntityCollider.enabled = false;
    }

    public void PhaseIn()
    {
        PhasedOut = false;
        EntityCollider.enabled = true;
    }

    public static Vector3 SnapVectorToGrid(Vector3 _Vector)
    {
        Vector3 SnappedVector = Vector3.zero;
        
        //Determine if the X or Z axis is larger
        if(Mathf.Abs(_Vector.x) > Mathf.Abs(_Vector.z))
        {
            //X is larger
            SnappedVector.x = _Vector.x;
        }
        else
        {
            //Z is larger
            SnappedVector.z = _Vector.z;
        }

        SnappedVector.Normalize();
        return (SnappedVector);
    }

    public bool HasObjectNextTo(Vector3 _Origin,  Vector3 _DirectionToCheck, int UnitScale)
    {
        //Slightly further than 2 units to check extents
        Ray DropRay = new Ray(_Origin + (_DirectionToCheck * (UnitScale + 0.1f)), new Vector3(0, -1, 0));
        RaycastHit BoxHit = new RaycastHit();
        //Check if anything is behind the object (the 2.1 is so its slightly less than half, so it doesnt clip the floor
        if (Physics.BoxCast(_Origin, new Vector3(UnitScale / 2.1f, UnitScale / 2.1f, UnitScale / 2.1f), _DirectionToCheck, out BoxHit, Quaternion.identity, UnitScale))
        {
            Debug.Log("Hit Object: " + BoxHit.transform.gameObject);
            return true;
        }
        else if (!Physics.Raycast(DropRay, UnitScale / 2 + 0.1f))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public virtual void UpdateTransformParenting()
    {
        #region Update Platform Parenting
        //Create a downward ray
        Ray DownRay = new Ray(transform.position + new Vector3(0, EntityCollider.bounds.extents.y, 0), new Vector3(0, -1, 0));
        RaycastHit DownHit = new RaycastHit();
        //Check what you are currently standing on
        if (Physics.Raycast(DownRay, out DownHit, EntityCollider.bounds.extents.y + 0.1f))
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
        if (PreviousFloorObject != null && FloorObject != null)
        {
            //If the previous floor object was not a portal
            if (PreviousFloorObject.GetComponent<Scr_Portal_Entity>() == null)
            {
                //And the current one is
                if (FloorObject.GetComponent<Scr_Portal_Entity>() != null)
                {
                    if (FloorObject.GetComponent<Scr_Portal_Entity>().GetPartnerPortal() != null)
                    {
                        if (FloorObject.GetComponent<Scr_Portal_Entity>().IsBlocked() == false)
                        {
                            WaitingToTeleport = true;
                            //Teleport the character
                            Scr_Portal_Entity TargetPortal = FloorObject.GetComponent<Scr_Portal_Entity>().GetPartnerPortal();
                            Vector3 TargetPosition = TargetPortal.transform.position + new Vector3(0, -TargetPortal.EntityCollider.bounds.extents.y / 2, 0);
                            transform.position = TargetPosition;
                            TeleportTarget = TargetPosition;
                            TargetPortal.LatestEntity = this.gameObject;
                        }
                    }
                }
            }
        }

        #endregion
    }

    IEnumerator ShiftPosition(Vector3 _TargetPosition)
    {
        //Sets busy to true so it cant be messed with during transition
        Busy = true;
        Vector3 FinalPosition = _TargetPosition;

        //The movement Loop
        for (int i = 0; i < 100; i++)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, _TargetPosition, 7 * Time.deltaTime);

            yield return null;

            if (((transform.position - _TargetPosition).magnitude) < 0.01f)
            {
                break;
            }

            if (WaitingToTeleport)
            {
                WaitingToTeleport = false;
                FinalPosition = TeleportTarget;
                break;
            }

            
        }

        //Final safe snap
        transform.position = FinalPosition;
        Busy = false;
    }

    #endregion

}
