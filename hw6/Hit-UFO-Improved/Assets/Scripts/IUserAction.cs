using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserAction
{
    void Hit(Vector3 position);
    void Restart();
    void GameBegin();
    void GameOver();
    //获取当前分数
    int GetScore();

    //获取当前回合
    int GetRound();

    //回合升级，增加难度
    void RoundUp();
    //实现两种运动管理器的切换
    void SetActionMode();
}
