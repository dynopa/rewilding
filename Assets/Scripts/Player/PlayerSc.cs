using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(PlayerController))]
public class PlayerSc : MonoBehaviour
{
    private Player player; // The Rewired Player
    private PlayerController p_ctrl;
    private PlayerState p_state;
    public Vector3 moveVector;
    private bool fire;
    private bool fireDown;
    private bool fireUp;
    private bool fire2Down;
    private bool sprintDown;
    private bool sprintUp;
    private bool NextPlantDown;
    private bool PrevPlantDown;
    
    // Start is called before the first frame update
    void Awake()
    {
        player = ReInput.players.GetPlayer(0);
        p_ctrl = GetComponent<PlayerController>();
        p_state = GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ProcessInput();
    }

    void GetInput()
    {
        // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
        // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

        moveVector.x = player.GetAxis("MoveH"); // get input by name or action id
        moveVector.y = player.GetAxis("MoveV");
        sprintDown = player.GetButtonDown("Sprint");
        sprintUp = player.GetButtonUp("Sprint");

        fire = player.GetButton("Fire");
        fireDown = player.GetButtonDown("Fire");
        fireUp = player.GetButtonUp("Fire");
        fire2Down = player.GetButtonDown("Fire2");
        NextPlantDown = player.GetButtonDown("NextPlant");
        PrevPlantDown = player.GetButtonDown("PrevPlant");
       
    }
    void ProcessInput()
    {
        p_ctrl.StartMove(moveVector.normalized); //Todo: unnormalize this and set up for smooth joystick movement in the playercontroller
        if (sprintDown) p_ctrl.Run(true);
        if (sprintUp) p_ctrl.Run(false);

        if (fireDown)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            switch (p_state.CastCheck())
            {
                default: 
                    break;
                case lookObject.door:
                    p_ctrl.HitDoor();
                    break;
                case lookObject.ground:
                    p_ctrl.Plant();
                    break;
                case lookObject.narrObj:
                    p_ctrl.HitNarrObj();
                    break;
                case lookObject.plant:
                    break;
                case lookObject.none:
                    break;
            }
        }
        //if (fire) p_ctrl.HoldPlant();
        if (fire2Down)
        {
            p_ctrl.DestroyPlant();
        }    
        if (NextPlantDown)
        {
            p_ctrl.SwitchPlant(true);
        }
        else if (PrevPlantDown)
        {
            p_ctrl.SwitchPlant(false);
        }
    }
}
