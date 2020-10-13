using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click : MonoBehaviour
{
    IUserAction action;
    CharacterController Character = null;
    BoatController Boat = null;
    public void SetCharacter(CharacterController Character)
    {
        this.Character = Character;
    }
    public void SetBoat(BoatController Boat)
    {
        this.Boat = Boat;
    }

    void Start()
    {
        action = Director.getInstance().CurrentScenceController as IUserAction;
    }

    private void OnMouseDown()
    {
        if (Character == null && Boat == null) return;
        if (Boat != null) action.MoveBoat();
        else if (Character != null) action.MoveCharacter(Character);
    }
}
