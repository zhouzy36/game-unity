using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalFlyAction : SSAction
{
    public float speed; //初速度
    public Vector3 direction; //方向，值为（cos，sin，0）
    public Rigidbody rb;

    public static PhysicalFlyAction GetSSAction(Rigidbody rb, float speed, Vector3 direction)
    {
        PhysicalFlyAction action = ScriptableObject.CreateInstance<PhysicalFlyAction>();
        action.speed = speed;
        action.direction = direction;
        action.rb = rb;
        return action;
    }

    public override void Start()
    {
        rb.velocity = direction * speed;
    }

    public override void Update()
    {
        if (Mathf.Abs(this.transform.position.y) > 6)
        {
            this.destroy = true;
            this.enable = false;
            this.callback.SSActionEvent(this);
        }
    }
}
