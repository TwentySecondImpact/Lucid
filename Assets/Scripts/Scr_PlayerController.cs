using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scr_PlayerController : MonoBehaviour
{
    public static Scr_PlayerController inst;
    public List<Scr_CharacterController> Characters;

    //The amount of TimeStamps delay there is behind the character
    public int InputDelay;
    public float MaximumReuniteDistance;

    public Transform LeaderCap;

    private void Awake()
    {
        inst = this;
    }

    private void Start ()
    {
        InitializeCharacters();
	}
	
	private void Update ()
    {
        UpdateInput();
	}

    void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCharacters();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SeperateCharacter();
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            ReuniteCharacters();
        }
    }

    private void InitializeCharacters()
    {
        //Set the initial controls
        Characters[0].UnderPlayerControl = true;
        Characters[1].UnderPlayerControl = false;
        Characters[2].UnderPlayerControl = false;

        DetermineLeaders();
        ////Set the initial Leaders
        //Characters[2].Leader = Characters[1];
        //Characters[1].Leader = Characters[0];
        //Characters[0].Leader = null;
    }

    private void ToggleCharacters()
    {
        //Adjusts The order in a temp list
        List<Scr_CharacterController> TempCharacterList = new List<Scr_CharacterController>();
        TempCharacterList.Add(Characters[1]);
        TempCharacterList.Add(Characters[2]);
        TempCharacterList.Add(Characters[0]);
        //Applies the list to the original list
        Characters = TempCharacterList;

        //Sets player controls
        Characters[0].UnderPlayerControl = true;
        Characters[1].UnderPlayerControl = false;
        Characters[2].UnderPlayerControl = false;

        //Change the position and parent of the Leader Cap
        LeaderCap.transform.parent = Characters[0].transform;
        LeaderCap.transform.localPosition = new Vector3(0, 1, 0);

        //Determine the leaders
        DetermineLeaders();
    }

    private void DetermineLeaders()
    {


        #region Position Leader Calculations
        if(Characters[0].Independent == true)
        {
            Characters[0].Leader = null;
            Characters[1].Leader = null;
            Characters[2].Leader = null;
        }
        else
        {
            #region Bottom Character
            //Characters 2 Bottom
            //If the next one up isnt independant 
            if (Characters[1].Independent == false)
            {
                Characters[2].Leader = Characters[1];
            }
            //If the one up 2 isnt independant
            else if (Characters[0].Independent == false)
            {
                Characters[2].Leader = Characters[0];
            }
            else
            {
                Characters[2].Leader = null;
            }
            #endregion

            #region Middle Character
            //Characters 1 Middle
            //First check if your current leader is still appropriate
            //if(Characters[1].Leader != null)
            //{
            //    if(Characters[1].Leader.Independent == false)
            //    {
            //        //Remain the same
            //    }
            //}
            //else

            //If the next one up isnt independant 
            if (Characters[0].Independent == false)
            {
                Characters[1].Leader = Characters[0];
            }
            else
            {
                Characters[1].Leader = null;
            }


            #endregion

            #region Top Character
            //Characters 0 Top
            Characters[0].Leader = null;
            #endregion

        }


        int IndependantCount = 0;

        //Resets all leaders to null if they are independant
        foreach (Scr_CharacterController Character in Characters)
        {
            if (Character.Independent == true)
            {
                Character.Leader = null;
            }

            //Counts the current independant characters
            if (Character.Independent == true)
            {
                IndependantCount++;
            }
        }

        //The final indepent switch
        //If you have no leader and are not independant, you are now independant
        foreach (Scr_CharacterController Character in Characters)
        {
            if (Character.Independent == false && IndependantCount == 2)
            {
                Character.Independent = true;
            }
        }


        #endregion

    }

    private void SeperateCharacter()
    {
        Characters[0].Independent = true;
        DetermineLeaders();
    }

    private void ReuniteCharacters()
    {
        //Loop through every character, if they are within range, remove the independant and refresh leaders
        for (int i = 1; i < Characters.Count; i++)
        {
            //Debug.Log((Characters[0].transform.position - Characters[i].transform.position).magnitude);
            //If the distance is within the bounds of the reunite variable
            if((Characters[0].transform.position - Characters[i].transform.position).magnitude < MaximumReuniteDistance)
            {
                //Disable their independance
                Characters[0].Independent = false;
                Characters[i].Independent = false;
            }
        }
        DetermineLeaders();
    }

    public Scr_PositionTimeStamp GetPositionTimeStampFromCharacter(Scr_CharacterController _Leader)
    {
        //Grab the leaders timestamp
        return _Leader.PositionTimeStamps[_Leader.PositionTimeStamps.Count - 1];

    }
}
