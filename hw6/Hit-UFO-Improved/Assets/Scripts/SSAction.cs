using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject
{
    public bool enable = true;  //是否正在进行此动作
    public bool destroy = false;    //是否需要被销毁
    public GameObject gameobject { get; set; } //动作对象
    public Transform transform { get; set; } //动作对象的transform
    public ISSActionCallback callback { get; set; } //回调函数

    protected SSAction() { } //保证SSAction不会被new
    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}
