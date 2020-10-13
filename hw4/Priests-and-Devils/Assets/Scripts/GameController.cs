using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour, ISceneController, IUserAction
{
    public BankController LeftBank;
    public BankController RightBank;
    public BoatController Boat;
    public List<CharacterController> Characters = new List<CharacterController>(6);
    private UserGUI gui;
    //新增
    public CCActionManager actionManager;
    public JudgeController judgeController;

    private void Awake()
    {
        Director director = Director.getInstance();
        director.setFPS(60);
        director.CurrentScenceController = this;
        director.CurrentScenceController.LoadResource();
        gui = gameObject.AddComponent<UserGUI>() as UserGUI;

        actionManager = gameObject.AddComponent<CCActionManager>() as CCActionManager;
        judgeController = gameObject.AddComponent<JudgeController>() as JudgeController;
    }

    public void LoadResource()
    {
        //bank
        LeftBank = new BankController();
        LeftBank.bank = Object.Instantiate(Resources.Load("Prefabs/bank", typeof(GameObject)), new Vector3(-4, -1, -3), Quaternion.identity) as GameObject;
        LeftBank.flg = -1;
        for (int i = 0; i < 6; i++) LeftBank.empty[i] = true;
        RightBank = new BankController();
        RightBank.bank = Object.Instantiate(Resources.Load("Prefabs/bank", typeof(GameObject)), new Vector3(4, -1, -3), Quaternion.identity) as GameObject;
        RightBank.flg = 1;
        for (int i = 0; i < 6; i++) RightBank.empty[i] = false; //初始都在右岸

        //river
        GameObject river = Object.Instantiate(Resources.Load("Prefabs/river", typeof(GameObject)), new Vector3(0, -1.5f, -3), Quaternion.identity) as GameObject;

        //boat
        Boat = new BoatController();
        Boat.boat = Object.Instantiate(Resources.Load("Prefabs/boat", typeof(GameObject)), new Vector3(1.2f, -0.9f, -3), Quaternion.identity) as GameObject;
        Boat.flg = 1;
        for (int i = 0; i < 2; i++) Boat.empty[i] = true;

        //Boat.move = Boat.boat.AddComponent(typeof(Move)) as Move;

        Boat.click = Boat.boat.AddComponent(typeof(Click)) as Click;
        Boat.click.SetBoat(Boat);

        //characters
        for (int i = 0; i < 6; i++)
        {
            CharacterController tmp = new CharacterController();
            if (i < 3)
            {
                tmp.character = Object.Instantiate(Resources.Load("Prefabs/priest", typeof(GameObject)), new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3), Quaternion.identity) as GameObject;
                tmp.role = 0;
            }
            else
            {
                tmp.character = Object.Instantiate(Resources.Load("Prefabs/devil", typeof(GameObject)), new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3), Quaternion.identity) as GameObject;
                tmp.role = 1;
            }
            tmp.on_boat = false;
            tmp.click = tmp.character.AddComponent(typeof(Click)) as Click;
            tmp.click.SetCharacter(tmp);
            Characters.Add(tmp);
        }
    }

    public void MoveBoat()
    {
        //Debug.Log(gui.flg);

        if (Boat.isEmpty() || gui.flg != 0) return;
        
        gui.flg = judgeController.check();

        actionManager.moveBoat(Boat.boat, Boat.positionToMove(), Boat.speed);
    }

    
    public void Restart()
    {
        
        //bank
        for (int i = 0; i < 6; i++)
        {
            RightBank.empty[i] = false;
            LeftBank.empty[i] = true;
        }

        //boat
        actionManager.moveBoat(Boat.boat, Boat.originPosition(), Boat.speed);
        Boat.flg = 1;
        for (int i = 0; i < 2; i++) Boat.empty[i] = true;

        //characters
        for (int i = 0; i < 6; i++)
        {
            if(Characters[i].on_boat)
            {
                Characters[i].character.transform.parent = null;
                Vector3 middle = Characters[i].getPositon();
                middle.y = 0.2f;
                actionManager.moveCharacter(Characters[i].character, middle, new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3), Characters[i].speed);
                Characters[i].on_boat = false;
            }
            else
            {
                Vector3 middle = Characters[i].getPositon();
                actionManager.moveCharacter(Characters[i].character, middle, new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3), Characters[i].speed);//相当于直接平移
            }
        }
        
    }
    

    public void MoveCharacter(CharacterController Character)
    {
        if (Boat.isMoving() || gui.flg != 0) return; //船在移动
        if (Character.on_boat) //在船上
        {
            bool isLeft = Character.character.transform.localPosition.x < 0 ? true : false;//判断乘客是否在左边
            Character.character.transform.parent = null; //解除父子关系
            if (Boat.flg == 1) //在右岸
            {
                Vector3 des = RightBank.getEmptyPosition();
                Vector3 middle = Character.getPositon();
                middle.y = des.y;
                actionManager.moveCharacter(Character.character, middle, des, Character.speed);
                RightBank.empty[RightBank.index(des)] = false;
            }
            else if (Boat.flg == -1) //在左岸
            {
                Vector3 des = LeftBank.getEmptyPosition();
                Vector3 middle = Character.getPositon();
                middle.y = des.y;
                actionManager.moveCharacter(Character.character, middle, des, Character.speed);
                LeftBank.empty[LeftBank.index(des)] = false;
            }
            Boat.empty[isLeft ? 0 : 1] = true; //船上产生空位
            Character.on_boat = false;
        }
        else //在岸上
        {
            Vector3 charPos = Character.getPositon();
            if (charPos.x * Boat.flg < 0) return; //船和人物不在同一边
            else
            {
                Vector3 des= Boat.getEmptyPosition();
                if (des == Vector3.zero) return; //没有空位
                Vector3 middle = charPos;
                middle.x = des.x;
                actionManager.moveCharacter(Character.character, middle, des, Character.speed);
                if (charPos.x > 0) RightBank.empty[RightBank.index(charPos)] = true;
                else LeftBank.empty[LeftBank.index(charPos)] = true;
                Character.character.transform.parent = Boat.boat.transform; //成为船的子对象
                Boat.empty[des.x < Boat.boat.transform.position.x ? 0 : 1] = false;
                Character.on_boat = true;
            }
        }
    }
}


