using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    
    //每个GUI的style
    GUIStyle bold_style = new GUIStyle();
    GUIStyle score_style = new GUIStyle();
    GUIStyle text_style = new GUIStyle();
    GUIStyle over_style = new GUIStyle();
    private int high_score = 0; //最高分
    public bool inGame = false;
    private bool beforeGame = true;
    private string mode = "Physical";

    void Start()
    {
        action = SSDirector.getInstance().CurrentScenceController as IUserAction;
    }

    // Update is called once per frame
    void OnGUI()
    {
        bold_style.normal.textColor = new Color(1, 0, 0);
        bold_style.fontSize = 16;
        text_style.normal.textColor = new Color(0, 0, 0, 1);
        text_style.fontSize = 16;
        score_style.normal.textColor = new Color(1, 0, 1, 1);
        score_style.fontSize = 16;
        over_style.normal.textColor = new Color(1, 0, 0);
        over_style.fontSize = 25;
        
        //游戏前
        if (beforeGame)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 100), "Hit UFO!", over_style);
            GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 100, 100), "Click the UFO to destroy it！", text_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 50), "Start"))
            {
                inGame = true;
                beforeGame = false;
                action.GameBegin();
            }
        }
        //游戏中
        else if (inGame)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                action.Hit(Input.mousePosition);
            }

            GUI.Label(new Rect(10, 5, 200, 50), "score:", text_style);
            GUI.Label(new Rect(55, 5, 200, 50), action.GetScore().ToString(), score_style);
            GUI.Label(new Rect(10, 25, 200, 50), "round:", text_style);
            GUI.Label(new Rect(55, 25, 200, 50), action.GetRound().ToString(), score_style);

            if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 50), "Stop"))
            {
                action.GameOver();
                inGame = false;
            }
            if (GUI.Button(new Rect(Screen.width - 200, 0, 100, 50), "Round Up"))
            {
                action.RoundUp();
            }
            if (GUI.Button(new Rect(Screen.width - 300, 0, 100, 50), mode))
            {
                action.SetActionMode();
                mode = mode == "Physical" ? "kinematic" : "Physical";
            }
        }
        //游戏结束
        else
        {
            high_score = high_score > action.GetScore() ? high_score : action.GetScore();
            GUI.Label(new Rect(Screen.width / 2 - 20, Screen.height / 2 - 50, 100, 100), "GameOver", over_style);
            GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2, 50, 50), "Best:", text_style);
            GUI.Label(new Rect(Screen.width / 2 + 50, Screen.height / 2, 50, 50), high_score.ToString(), text_style);
            GUI.Label(new Rect(Screen.width / 2 - 10, Screen.height / 2 + 25, 50, 50), "score:", text_style);
            GUI.Label(new Rect(Screen.width / 2 + 50, Screen.height / 2 + 25, 50, 50), action.GetScore().ToString(), score_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.height / 2 + 50, 100, 50), "Restart"))
            {
                inGame = true;
                action.Restart();
                return;
            }
            action.GameOver();
        }
    }
}
