using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController
{
    public GameObject boat;
    public int flg; //-1: left bank  1: right bank
    public bool[] empty = new bool[2];// 0:船上的靠左的乘客 1:船上靠右的乘客
    private Vector3 right_pos = new Vector3(1.2f, -0.9f, -3); //船在右岸的位置
    private Vector3 bank_passenger = new Vector3(1.52f, -0.6f, -3); //右岸靠岸乘客位置
    private Vector3 river_passenger = new Vector3(0.88f, -0.6f, -3); //右岸靠河乘客位置
    public Move move;
    public Click click;

    public Vector3 getEmptyPosition()
    {
        Vector3 emptyPosition = Vector3.zero;
        if (empty[0])
        {
            emptyPosition = (flg == 1) ? river_passenger : bank_passenger;
            emptyPosition.x *= flg;
        }
        else if (empty[1])
        {
            emptyPosition = (flg == 1) ? bank_passenger : river_passenger;
            emptyPosition.x *= flg;
        }
        return emptyPosition;
    }

    public bool isEmpty()
    {
        return empty[0] && empty[1];
    }

    public void boatMove()
    {
        Vector3 des = right_pos;
        if (flg == 1) des.x = -des.x;
        move.MovePosition(des);
        flg = -flg;
    }

    public bool isMoving()
    {
        Vector3 left_pos = right_pos;
        left_pos.x = -right_pos.x;
        return (boat.transform.position != right_pos && boat.transform.position != left_pos);
    }
}
