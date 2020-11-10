using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalActionManager : SSActionManager, ISSActionCallback, IActionManager
{
    public GameController gameController;
    public PhysicalFlyAction fly;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //完成飞行动作后就回收飞碟
        gameController.factory.FreeDisk(source.gameobject);
    }

    protected new void Start()
    {
        gameController = (GameController)SSDirector.getInstance().CurrentScenceController;
    }

    public void UFOFly(GameObject disk, float speed, Vector3 direction)
    {
        disk.GetComponent<Rigidbody>().isKinematic = false;
        fly = PhysicalFlyAction.GetSSAction(disk.GetComponent<Rigidbody>(), speed, direction);
        this.RunAction(disk, fly, this);
    }
}
