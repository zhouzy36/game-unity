using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCActionManager : SSActionManager, ISSActionCallback
{
    public GameController gameController;
    public CCFlyAction fly;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //完成飞行动作后就回收飞碟
        gameController.factory.FreeDisk(source.gameobject);
    }

    protected new void Start()
    {
        gameController = (GameController)SSDirector.getInstance().CurrentScenceController;
        gameController.actionManager = this; //将场景的动作管理者设置为自己
    }

    //飞碟飞行
    public void UFOFly(GameObject disk, float speed, Vector3 direction)
    {
        fly = CCFlyAction.GetSSAction(speed, direction);
        this.RunAction(disk, fly, this);
    }
}
