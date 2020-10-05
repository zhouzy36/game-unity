using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 20;
    public int flg = 0; //0:不动 两阶段移动
    private Vector3 end;//
    private Vector3 middle;//
    
    void Update()
    {
        if (flg == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, middle, speed * Time.deltaTime);
            if (transform.position == middle) flg = 2;
        }
        else if (flg == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            if (transform.position == end) flg = 0;
        }
    }

    public void MovePosition(Vector3 position)
    {
        end = position;
        if (end.y == transform.position.y) flg = 2; // boat
        if (end.y < transform.position.y) //character->boat
        {
            flg = 1;
            middle = new Vector3(end.x, transform.position.y, transform.position.z);
        }
        if (end.y > transform.position.y) //boat->character
        {
            flg = 1;
            middle = new Vector3(transform.position.x, end.y, transform.position.z);
        }
    }
}
