using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Entity : MonoBehaviour
{
    public bool Pushable;
    public bool Pullable;
    public bool Breakable;
    public Collider collider;

    public bool Busy = false;

	// Use this for initialization
	protected virtual void Start ()
    {
        collider = GetComponent<Collider>();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
    {
		
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
            if (HasObjectNextTo(collider.bounds.center, Direction, 2) == false)
            {
                StartCoroutine(ShiftPosition(transform.position + SnapVectorToGrid(Direction) * 2));
                //transform.position += SnapVectorToGrid(Direction) * 2;
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

            if (HasObjectNextTo(collider.bounds.center, -Direction, 2) == false)
            {
                StartCoroutine(ShiftPosition(transform.position - SnapVectorToGrid(Direction) * 2));
                //transform.position -= SnapVectorToGrid(Direction) * 2;
            }
        }
    }

    public virtual void Skill_Jump()
    {

    }

    public Vector3 SnapVectorToGrid(Vector3 _Vector)
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

        RaycastHit BoxHit = new RaycastHit();
        //Check if anything is behind the object (the 2.1 is so its slightly less than half, so it doesnt clip the floor
        if (Physics.BoxCast(_Origin, new Vector3(UnitScale/2.1f, UnitScale/2.1f, UnitScale/2.1f),  _DirectionToCheck, out BoxHit, Quaternion.identity, UnitScale))
        {
            Debug.Log("Hit Object: " + BoxHit.transform.gameObject);
            return true;
        }
        else
        {
            return false;
        }
        
    }

    IEnumerator ShiftPosition(Vector3 _TargetPosition)
    {
        //Sets busy to true so it cant be messed with during transition
        Busy = true;

        //The movement Loop
        for (int i = 0; i < 100; i++)
        {
            transform.position = Vector3.LerpUnclamped(transform.position, _TargetPosition, 7 * Time.deltaTime);

            if (((transform.position - _TargetPosition).magnitude) < 0.01f)
            {
                break;
            }

            yield return null;
        }

        //Final safe snap
        transform.position = _TargetPosition;
        Busy = false;
    }

    #endregion

}
