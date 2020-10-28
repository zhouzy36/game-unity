using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, ISceneController, IUserAction
{
    public CCActionManager actionManager;
    public DiskFactory factory;
    public UserGUI gui;
    public Scorer scorer;
    public int round;
    public float timer;
    public bool inGame = false;

    private Queue<GameObject> disks = new Queue<GameObject>();
    private int initialDiskNumber = 5; //初始飞碟数量
    private float initialTimer = 2; //初始发射时间间隔
    private float factor = 0.18f; //
    private bool ready = true; //在回合升级时使用
    

    private void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.setFPS(60);
        director.CurrentScenceController = this;
        director.CurrentScenceController.LoadResource();

        factory = DiskFactory.getInstance();
        actionManager = gameObject.AddComponent<CCActionManager>() as CCActionManager;
        gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        scorer = gameObject.AddComponent<Scorer>() as Scorer;
    }

    void Start()
    {
        round = 1;
        timer = initialTimer;
        for (int i = 0; i < initialDiskNumber + (round - 1) * 5; i++)
        {
            disks.Enqueue(factory.GetDisk(round));
        }
    }

    void Update()
    {
        if (inGame)
        {
            if (disks.Count == 0)
            {
                ready = false;
                NextRound();
                Debug.Log("next round");
            }
            else if (ready)
            {
                if (timer <= 0)
                {
                    ThrowDisk();
                    timer = initialTimer - factor * round;
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
        }
    }

    //载入场景
    public void LoadResource()
    {
    }

    private void NextRound()
    {
        if (round == 10)
        {
            GameOver();
            gui.inGame = false;
            //Debug.Log("Game Over");
        }
        else
        {
            round++;
            for (int i = 0; i < initialDiskNumber + (round-1) * 5; i++)
            {
                disks.Enqueue(factory.GetDisk(round));
            }
            ready = true;
        }
    }

    //发射飞碟
    private void ThrowDisk()
    {
          
        if (disks.Count > 0)
        {
            GameObject disk = disks.Dequeue();          
            actionManager.UFOFly(disk, disk.GetComponent<DiskData>().speed, disk.GetComponent<DiskData>().direction);
            
        }
    }

    public void Hit(Vector3 position)
    {
        Camera ca = Camera.main;
        Ray ray = ca.ScreenPointToRay(position);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            GameObject disk = hit.collider.gameObject;

            if (disk.GetComponent<DiskData>() != null)
            {
                //将命中的飞碟的Active设置为false使其从屏幕上消失待其完成飞行后会通过动作管理的回调函数回收
                disk.SetActive(false);
                //计分
                scorer.Record(disk);
            }
        }
    }

    public void Restart()
    {
        //回合置为1，重置分数
        round = 0;
        scorer.Reset();
        inGame = true;
        NextRound();
    }

    public void GameBegin()
    {
        inGame = true;
    }

    public void GameOver()
    {
        inGame = false;
    }

    public int GetScore()
    {
        return scorer.getScore();
    }

    public int GetRound()
    {
        return round;
    }

    public void RoundUp()
    {
        ready = false;
        NextRound();
    }
}