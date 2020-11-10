using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, ISceneController, IUserAction
{
    //public CCActionManager actionManager;
    public DiskFactory factory;
    public UserGUI gui;
    public Scorer scorer;
    public int round;
    public float timer;
    public bool inGame = false;
    public int diskCnt;
    public IActionManager actionManager;

    private int initialDiskNumber = 5;//初始飞碟数量
    private float initialTimer = 2; //初始发射时间间隔
    private float factor = 0.18f;
    public bool isPhysics = true;
    

    private void Awake()
    {
        SSDirector director = SSDirector.getInstance();
        director.setFPS(60);
        director.CurrentScenceController = this;
        director.CurrentScenceController.LoadResource();
        gameObject.AddComponent<DiskFactory>();
        gameObject.AddComponent<CCActionManager>();
        gameObject.AddComponent<PhysicalActionManager>();
        //improved
        factory = Singleton<DiskFactory>.Instance;
        actionManager = Singleton<PhysicalActionManager>.Instance as IActionManager;
        gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        scorer = gameObject.AddComponent<Scorer>() as Scorer;
    }

    void Start()
    {
        round = 1;
        timer = initialTimer;
        diskCnt = initialDiskNumber;
    }

    void Update()
    {
        if (inGame)
        {
            if (diskCnt == 0)
            {
                NextRound();
            }
            else
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
        }
        else
        {
            round++;
            diskCnt = initialDiskNumber + (round - 1) * 5;
        }
    }

    //发射飞碟
    private void ThrowDisk()
    {
        GameObject disk = factory.GetDisk(round);
        actionManager.UFOFly(disk, disk.GetComponent<DiskData>().speed, disk.GetComponent<DiskData>().direction);
        diskCnt--;
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
                //创建协程，用于控制飞碟爆炸效果的延续时间。
                StartCoroutine("DestroyExplosion", disk);
                //将命中的飞碟的Active设置为false使其从屏幕上消失待其完成飞行后会通过动作管理的回调函数回收
                disk.SetActive(false);
                //计分
                scorer.Record(disk);

            }
        }
    }

    // 该协程用于控制飞碟爆炸效果。
    private IEnumerator DestroyExplosion(GameObject disk)
    {
        //实例化预制
        GameObject explosion = Instantiate(Resources.Load<GameObject>("Prefabs/Exploson6"), disk.transform.position, Quaternion.identity);
        //爆炸效果持续 1.2 秒
        yield return new WaitForSeconds(1.2f);
        //销毁爆炸效果对象
        Destroy(explosion);
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
        NextRound();
    }

    public void SetActionMode()
    {
        actionManager = isPhysics ? Singleton<PhysicalActionManager>.Instance : Singleton<CCActionManager>.Instance as IActionManager;
        isPhysics = !isPhysics;
    }
}