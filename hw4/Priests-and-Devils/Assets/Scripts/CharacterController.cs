using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController
{
    public GameObject character;
    public int role; // 0: priest 1: devil
    public bool on_boat; // True: on boat
    public Click click;
    public float speed = 20;

    public Vector3 getPositon()
    {
        return character.transform.position;
    }

    //public Move move;

    /*
    public void characterMove(Vector3 des)
    {
        move.MovePosition(des);
    }
    */
}
