using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController
{
    public GameObject character;
    public int role; // 0: priest 1: devil
    public bool on_boat; // True: on boat
    public Move move;
    public Click click;

    public void characterMove(Vector3 des)
    {
        move.MovePosition(des);
    }
}
