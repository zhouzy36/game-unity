using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorer: MonoBehaviour
{
    private int score;

    void Start()
    {
        score = 0;
    }

    //记录分数
    public void Record(GameObject disk)
    {
        score += disk.GetComponent<DiskData>().score;
        //Debug.Log(score);
    }

    //重置分数
    public void Reset()
    {
        score = 0;
    }

    //获取分数
    public int getScore()
    {
        return score;
    }
}
