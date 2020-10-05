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

    private void Awake()
    {
        Director director = Director.getInstance();
        director.setFPS(60);
        director.CurrentScenceController = this;
        director.CurrentScenceController.LoadResource();
        gui = gameObject.AddComponent<UserGUI>() as UserGUI;
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
        Boat.move = Boat.boat.AddComponent(typeof(Move)) as Move;
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
            tmp.move = tmp.character.AddComponent(typeof(Move)) as Move;
            tmp.click = tmp.character.AddComponent(typeof(Click)) as Click;
            tmp.click.SetCharacter(tmp);
            Characters.Add(tmp);
        }
    }

    public void MoveBoat()
    {
        if (Boat.isEmpty() || gui.flg != 0) return;
        //check
        int leftPriest = 0, leftDevil = 0, rightPriest = 0, rightDevil = 0;
        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = Characters[i].character.transform.position;
            if (Characters[i].on_boat) //在船上
            {
                if (Boat.flg == 1) //要开往左岸
                {
                    if (Characters[i].role == 0) leftPriest++;
                    else leftDevil++;
                }
                else
                {
                    if (Characters[i].role == 0) rightPriest++;
                    else rightDevil++;
                }
            }
            else
            {
                if (pos.x > 0) //在右岸
                {
                    if (Characters[i].role == 0) rightPriest++;
                    else rightDevil++;
                }
                else //在左岸
                {
                    if (Characters[i].role == 0) leftPriest++;
                    else leftDevil++;
                }
            }
        }
        if (leftPriest != 0 && leftPriest < leftDevil) gui.flg = 1; //Debug.Log("Game over");
        else if (rightPriest != 0 && rightPriest < rightDevil) gui.flg = 1; //Debug.Log("Game over");
        if (leftDevil + leftPriest == 6) gui.flg = 2;//Debug.Log("You Win");
        //
        Boat.boatMove();
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
        if (Boat.flg == -1) Boat.boatMove();
        for (int i = 0; i < 2; i++) Boat.empty[i] = true;
        //characters
        for (int i = 0; i < 6; i++)
        {
            if(Characters[i].on_boat)
            {
                Characters[i].character.transform.parent = null;
                Characters[i].characterMove(new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3));
                Characters[i].on_boat = false;
            }
            else Characters[i].characterMove(new Vector3((float)(2.5 + 0.6 * i), 0.2f, -3));
        }
    }

    public void MoveCharacter(CharacterController Character)
    {
        if (Boat.isMoving()) return; //船在移动
        if (Character.on_boat) //在船上
        {
            bool isLeft = Character.character.transform.localPosition.x < 0 ? true : false;//判断乘客是否在左边
            Character.character.transform.parent = null; //解除父子关系
            if (Boat.flg == 1) //在右岸
            {
                Vector3 des = RightBank.getEmptyPosition();
                Character.characterMove(des);
                RightBank.empty[RightBank.index(des)] = false;
            }
            else if (Boat.flg == -1) //在左岸
            {
                Vector3 des = LeftBank.getEmptyPosition();
                Character.characterMove(des);
                LeftBank.empty[LeftBank.index(des)] = false;
            }
            Boat.empty[isLeft ? 0 : 1] = true; //船上产生空位
            Character.on_boat = false;
        }
        else //在岸上
        {
            Vector3 charPos = Character.character.transform.position;
            if (charPos.x * Boat.flg < 0) return; //船和人物不在同一边
            else
            {
                Vector3 des= Boat.getEmptyPosition();
                if (des == Vector3.zero) return; //没有空位
                Character.characterMove(des);
                if (charPos.x > 0) RightBank.empty[RightBank.index(charPos)] = true;
                else LeftBank.empty[LeftBank.index(charPos)] = true;
                Character.character.transform.parent = Boat.boat.transform; //成为船的子对象
                Boat.empty[des.x < Boat.boat.transform.position.x ? 0 : 1] = false;
                Character.on_boat = true;
            }
        }
    }
}


