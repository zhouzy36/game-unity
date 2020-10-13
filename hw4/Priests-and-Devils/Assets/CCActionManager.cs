using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCActionManager : SSActionManager, ISSActionCallback
{
    public GameController gameController;
    public CCMoveToAction boatMove;
    public CCSequenceAction characterMove;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //do nothing
        //Debug.Log("Sequence action finished");
    }

    // Start is called before the first frame update
    protected new void Start()
    {
        gameController = (GameController)Director.getInstance().CurrentScenceController;
        gameController.actionManager = this;    //将场景的动作管理者设置为自己
    }

    public void moveBoat(GameObject boat, Vector3 target, float speed)
    {
        boatMove = CCMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, boatMove, this);
    }

    public void moveCharacter(GameObject character, Vector3 middle_pos, Vector3 end_pos, float speed)
    {
        SSAction action1 = CCMoveToAction.GetSSAction(middle_pos, speed);
        SSAction action2 = CCMoveToAction.GetSSAction(end_pos, speed);
        characterMove = CCSequenceAction.GetSSAction(1, 0, new List<SSAction> { action1, action2 }); //1表示是做一次动作组合，0代表从action1开始
        this.RunAction(character, characterMove, this);
    }
}
