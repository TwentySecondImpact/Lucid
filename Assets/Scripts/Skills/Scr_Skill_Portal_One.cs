using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_Skill_Portal_One : Scr_Skill
{

    public GameObject PortalPrefab;
    public List<GameObject> Portals = new List<GameObject>();

    public GameObject CurrentPortalOne;

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


        if (!_Character.CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName("PortalOne"))
        {
            //Debug.Log("Push Pull Activate");
            _Character.CharacterAnimator.SetTrigger("Skill_PortalOne");

            //_Character.Channeling = true;
        }

    }

    public void ActivateFromAnimation()
    {

        Scr_CharacterController _Character = GetComponent<Scr_CharacterController>();

        //_Character.Channeling = false;

        //If they are not standing on a portal 
        if (_Character.FloorObject.GetComponent<Scr_Portal_Entity>() == null)
        {
            //If the character is on flat ground
            if (_Character.FloorObject.layer == 8)
            {
                List<Vector3> PossiblePositions = DeterminePosition(SnapToGrid(_Character.transform.position + new Vector3(0, -0.9f, 0)));
                List<Vector3> GroundedPositions = new List<Vector3>();

                //Check through all possible positions to see what is grounded
                for (int i = 0; i < PossiblePositions.Count; i++)
                {
                    if (CheckPortalOverLand(PossiblePositions[i]))
                    {
                        Debug.Log("Postal Grounded: " + PossiblePositions[i]);
                        GroundedPositions.Add(PossiblePositions[i]);
                    }
                }

                //Figure out the closest position from the remaining grounded positions
                Vector3 ClosestPosition = CheckClosestPosition(GroundedPositions, _Character.transform.position + new Vector3(0, -0.9f, 0));


                //Destroy the old portal if it exists
                if (CurrentPortalOne != null)
                {
                    List<Transform> PortalChildren = new List<Transform>();

                    //Create a seperate list so yo udont take items out of a list youre working with
                    for (int i = 0; i < CurrentPortalOne.transform.childCount; i++)
                    {
                        PortalChildren.Add(CurrentPortalOne.transform.GetChild(i));
                    }

                    //Hands the objects parented over to their parent so they dont take their children with them
                    for (int i = 0; i < PortalChildren.Count; i++)
                    {
                        //Set the floor objects to the things on top of the portal
                        if (PortalChildren[i].gameObject.GetComponent<Scr_CharacterController>() != null)
                        {
                            Debug.Log("Unparenting Character Object");
                            PortalChildren[i].gameObject.GetComponent<Scr_CharacterController>().FloorObject = transform.parent.gameObject;
                            PortalChildren[i].gameObject.GetComponent<Scr_CharacterController>().PreviousFloorObject = transform.parent.gameObject;
                            PortalChildren[i].parent = transform.parent;
                        }

                        if (PortalChildren[i].gameObject.GetComponent<Scr_Block_Movable>() != null)
                        {
                            Debug.Log("Unparenting Entity Object");
                            PortalChildren[i].gameObject.GetComponent<Scr_Entity>().FloorObject = transform.parent.gameObject;
                            PortalChildren[i].gameObject.GetComponent<Scr_Entity>().PreviousFloorObject = transform.parent.gameObject;
                            PortalChildren[i].parent = transform.parent;
                        }

                        //Debug.Log("Moving Object: " + transform.GetChild(i) + " From parent: " + transform.GetChild(i).parent + " To Parent: " + transform.parent);
                    }

                    //The collider needs to be disabled in order to stop reparenting in the next frame before destroying
                    CurrentPortalOne.GetComponent<Scr_Entity>().EntityCollider.enabled = false;
                    Destroy(CurrentPortalOne);
                }

                //Create portal
                GameObject NewPortal = GameObject.Instantiate(PortalPrefab);

                //Set the new portal
                CurrentPortalOne = NewPortal;

                //Set the new portals partent to the characters parent
                NewPortal.transform.parent = _Character.transform.parent;
                //NewPortal.GetComponent<Scr_Portal_Entity>().FloorObject = _Character.transform.parent;

                //Set position Lowering it to be in line with the floor
                NewPortal.transform.position = ClosestPosition + new Vector3(0, -0.1f + 0.005f, 0);

                //Sets this portal to both current and previous floor objects to avoid instant porting
                _Character.FloorObject = NewPortal;
                _Character.PreviousFloorObject = NewPortal;

                //Also make the portal creator independant
                Scr_PlayerController.inst.SeperateCharacter(Scr_PlayerController.inst.GetPartyIndex(_Character));

                //Adds the new portal to the from of the list
                Portals.Insert(0, NewPortal);

                ////If the portal list is now greater than 2, remove the oldest one
                //if (Portals.Count > 2)
                //{
                //    GameObject PortalToDelete = Portals[Portals.Count - 1];
                //    Portals.RemoveAt(Portals.Count - 1);

                //    //Pass over parenting
                //    if (PortalToDelete.transform.childCount > 0)
                //    {
                //        for (int i = 0; i < PortalToDelete.transform.childCount; i++)
                //        {
                //            PortalToDelete.transform.GetChild(i).parent = PortalToDelete.transform.parent;
                //        }
                //    }

                //    Destroy(PortalToDelete);
                //}
            }
        }
        
    }

    public Vector3 SnapToGrid(Vector3 _Position)
    {
        Vector3 SnappedPosition = _Position;
        SnappedPosition.x = Mathf.RoundToInt(SnappedPosition.x);
        if(SnappedPosition.x % 2 != 0)
        {
            Debug.Log("Placed on odd grid");
            SnappedPosition.x += 1;
        }
        SnappedPosition.z = Mathf.RoundToInt(SnappedPosition.z);
        if (SnappedPosition.z % 2 != 0)
        {
            Debug.Log("Placed on odd grid");
            SnappedPosition.z += 1;
        }
        return SnappedPosition;
    }

    public List<Vector3> DeterminePosition(Vector3 _Position)
    {
        List<Vector3> PossiblePositions = new List<Vector3>();

        //Closest
        Vector3 ClosestPosition = _Position;
        ClosestPosition.x = Mathf.RoundToInt(ClosestPosition.x);
        ClosestPosition.z = Mathf.RoundToInt(ClosestPosition.z);
        PossiblePositions.Add(ClosestPosition);

        //X Inverted
        Vector3 XInvertedPosition = ClosestPosition;
        //If the closest position is smaller, add 1 to it
        if(ClosestPosition.x < _Position.x)
        {
            XInvertedPosition.x += 2;
        }
        //Otherwise take 1 from it
        else
        {
            XInvertedPosition.x -= 2;
        }
        PossiblePositions.Add(XInvertedPosition);

        //Z Inverted
        Vector3 ZInvertedPosition = ClosestPosition;
        //If the closest position is smaller, add 1 to it
        if (ClosestPosition.z < _Position.z)
        {
            ZInvertedPosition.z += 2;
        }
        //Otherwise take 1 from it
        else
        {
            ZInvertedPosition.z -= 2;
        }
        PossiblePositions.Add(ZInvertedPosition);

        //XZ Inverted
        Vector3 XZInvertedPosition = ClosestPosition;
        XZInvertedPosition.x = XInvertedPosition.x;
        XZInvertedPosition.z = ZInvertedPosition.z;
        PossiblePositions.Add(XZInvertedPosition);

        return PossiblePositions;
    }

    public bool CheckPortalOverLand(Vector3 _Position)
    {
        bool OnGround = true;

        //Debug.Log(_Position);

        //Top Right
        Ray DropRay = new Ray(_Position + new Vector3(0.5f, 0, 0.5f), new Vector3(0, -1, 0));
        if(!Physics.Raycast(DropRay, 0.3f))
        {
            Debug.Log("Top Right not grounded");
            OnGround = false;
        }

        //Top Left
        DropRay = new Ray(_Position + new Vector3(-0.5f, 0, 0.5f), new Vector3(0, -1, 0));
        if (!Physics.Raycast(DropRay, 0.3f))
        {
            Debug.Log("Top Left not grounded");
            OnGround = false;
        }

        //Bottom Right
        DropRay = new Ray(_Position + new Vector3(0.5f, 0, -0.5f), new Vector3(0, -1, 0));
        if (!Physics.Raycast(DropRay, 0.3f))
        {
            Debug.Log("Bottom Right not grounded");
            OnGround = false;
        }

        //Bottom Left
        DropRay = new Ray(_Position + new Vector3(-0.5f, 0, -0.5f), new Vector3(0, -1, 0));
        if (!Physics.Raycast(DropRay, 0.3f))
        {
            Debug.Log("Bottom Left not grounded");
            OnGround = false;
        }

        return OnGround;
    }

    public Vector3 CheckClosestPosition(List<Vector3> _PossiblePositions, Vector3 _OriginPosition)
    {
        Vector3 ClosestPosition = new Vector3(0,0,0);
        float CurrentClosestDistance = 9999;

        for (int i = 0; i < _PossiblePositions.Count; i++)
        {
            //Debug.Log("Grounded Position " + i + ": " + _PossiblePositions[i]);
            if(Vector3.Distance(_PossiblePositions[i], _OriginPosition) < CurrentClosestDistance)
            {
                //Set the new closest distance
                CurrentClosestDistance = Vector3.Distance(_PossiblePositions[i], _OriginPosition);
                //Set the new closest position
                ClosestPosition = _PossiblePositions[i];
            }
        }

        return ClosestPosition;
    }
}
