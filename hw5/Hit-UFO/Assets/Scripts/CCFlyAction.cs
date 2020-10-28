using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CCFlyAction : SSAction
{
    
    public float speed; //初速度
    public Vector3 direction; //方向，值为（cos，sin，0）

    private float gravity = 9.8f;
    private float x_speed;
    private float y_speed;

    public static CCFlyAction GetSSAction(float speed, Vector3 direction)
    {
        CCFlyAction action = ScriptableObject.CreateInstance<CCFlyAction>();
        action.speed = speed;
        action.direction = direction;
        return action;
    }

    public override void Start()
    {
        direction = direction.normalized;
        x_speed = speed * direction.x;
        y_speed = speed * direction.y;
    }

    public override void Update()
    {
        y_speed = y_speed - gravity * Time.deltaTime;
        this.transform.position += x_speed * Vector3.right * Time.deltaTime;
        this.transform.position += y_speed * Vector3.up * Time.deltaTime;
        if (Mathf.Abs(this.transform.position.y) > 10)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }
}
