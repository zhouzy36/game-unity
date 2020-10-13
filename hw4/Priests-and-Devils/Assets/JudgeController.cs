using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeController : MonoBehaviour
{
    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = (GameController)Director.getInstance().CurrentScenceController;
        gameController.judgeController = this;
    }

    public int check() //0:in game  1:lose  2:win
    {
        int leftPriest = 0, leftDevil = 0, rightPriest = 0, rightDevil = 0;

        for (int i = 0; i < 6; i++)
        {
            Vector3 pos = gameController.Characters[i].character.transform.position;
            if (gameController.Characters[i].on_boat) //在船上
            {
                if (gameController.Boat.flg == 1) //要开往左岸
                {
                    if (gameController.Characters[i].role == 0) leftPriest++;
                    else leftDevil++;
                }
                else
                {
                    if (gameController.Characters[i].role == 0) rightPriest++;
                    else rightDevil++;
                }
            }
            else
            {
                if (pos.x > 0) //在右岸
                {
                    if (gameController.Characters[i].role == 0) rightPriest++;
                    else rightDevil++;
                }
                else //在左岸
                {
                    if (gameController.Characters[i].role == 0) leftPriest++;
                    else leftDevil++;
                }
            }
        }

        if (leftPriest != 0 && leftPriest < leftDevil) return 1; //Debug.Log("Game over");
        else if (rightPriest != 0 && rightPriest < rightDevil) return 1; //Debug.Log("Game over");
        if (leftDevil + leftPriest == 6) return 2;//Debug.Log("You Win");
        else return 0;
    }
}
