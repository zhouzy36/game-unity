using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCSequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence; //动作的列表
    public int repeat = -1; //-1：无限循环组合中的动作
    public int start = 0;   //当前做的动作的索引

    public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence)
    {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }


    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    { //CCSequenceAction其实也可以算作一个动作管理者了，它顺序执行sequence中的一系列动作
        source.destroy = false;     //先保留这个动作，如果是无限循环动作组合之后还需要使用
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--; //做完一次动作后将repeat减1
            if (repeat == 0)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this); //告诉组合动作的管理对象组合做完了
            }
        }
    }

    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this; //把回调函数设置为当前实现的这一个
            action.Start();
        }
    }

    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (start < sequence.Count) sequence[start].Update();//这里不用start++，而是通过上面的回调函数实现
    }

    void OnDestory()
    {
        //TODO: something
    }
}
