using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BankController
{
    public GameObject bank;
    public int flg; // 1: right  -1: left
    public bool[] empty = new bool[6];
    private Vector3 first_pos = new Vector3(2.5f, 0.2f, -3);

    public Vector3 getEmptyPosition() //返回值为(0,0,0)时表示无空位
    {
        Vector3 emptyPositon = first_pos;
        int i = 0;
        for (; i < 6; i++)
        {
            if (empty[i]) break;
        }
        if (i < 6) emptyPositon.x = (float)((emptyPositon.x + i * 0.6) * flg);
        else emptyPositon = Vector3.zero;
        return emptyPositon;
    }

    public int numberOfCharacter()
    {
        int cnt = 0;
        for (int i = 0; i < 6; i++)
        {
            if (!empty[i]) cnt++;
        }
        return cnt;
    }

    public int index(Vector3 pos)
    {
        double v = ((flg * pos.x - 2.5) / 0.6);
        int index = (int)v;
        if (v - index > 0.5) index += 1;
        return index;
    }
}
