using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToe : MonoBehaviour
{
    private int turn = 0;//0:player1 1:player2
    private int result = 0;
    private int[,] board = new int[3, 3];

    void Reset()
    {
        turn = 0;
        result = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = 0;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        result = check();
        if (result == 0) GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 4 - 50, 100, 20), "In game...");
        if (result == 1) GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 4 - 50, 100, 20), "Player1(X) Win!");
        if (result == 2) GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 4 - 50, 100, 20), "Player2(O) Win!"); 
        if (result == 3) GUI.Label(new Rect(Screen.width / 2 - 25, Screen.height / 4 - 50, 100, 20), "Tie!");

        if (GUI.Button(new Rect(Screen.width / 2 - 200, Screen.height / 4 + 20, 100, 50), "Reset")) Reset();
        if (GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height / 4 + 20, 100, 50), "Quit")) Application.Quit();

        if (result == 0)
        {
            if (turn == 0) GUI.Label(new Rect(Screen.width / 2 - 190, Screen.height / 4 + 100, 100, 50), "Player1(X)'s Turn");
            else GUI.Label(new Rect(Screen.width / 2 - 190, Screen.height / 4 + 100, 100, 50), "Player2(O)'s Turn");
        }
        else GUI.Label(new Rect(Screen.width / 2 - 190, Screen.height / 4 + 100, 100, 50), "Gameover!");

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 1)
                {
                    GUI.Button(new Rect(Screen.width / 2 - 75 + 50 * i, Screen.height / 4 + 50 * j, 50, 50), "X");
                }
                if (board[i, j] == 2)
                {
                    GUI.Button(new Rect(Screen.width / 2 - 75 + 50 * i, Screen.height / 4 + 50 * j, 50, 50), "O");
                }
                if (GUI.Button(new Rect(Screen.width / 2 - 75 + 50 * i, Screen.height / 4 + 50 * j, 50, 50), ""))
                {
                    if (result == 0)
                    {
                        if (turn == 1) board[i, j] = 2;
                        else board[i, j] = 1;
                        turn = 1 - turn;
                    }
                }
            }
        }

    }

    int check()
    {
        bool flg = false;
        int cnt1, cnt2;
        //check row
        for (int i = 0; i < 3; i++)
        {
            cnt1 = 0;
            cnt2 = 0;
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == 0) flg = true;//还有空位
                if (board[i, j] == 1) cnt1++;
                if (board[i, j] == 2) cnt2++;
            }
            if (cnt1 == 3) return 1;
            if (cnt2 == 3) return 2;
        }
        //check column
        for (int i = 0; i < 3; i++)
        {
            cnt1 = 0;
            cnt2 = 0;
            for (int j = 0; j < 3; j++)
            {
                if (board[j, i] == 1) cnt1++;
                if (board[j, i] == 2) cnt2++;
            }
            if (cnt1 == 3) return 1;
            if (cnt2 == 3) return 2;
        }
        //check diagonal
        cnt1 = cnt2 = 0;
        for (int i = 0; i < 3; i++)
        {
            if (board[i, i] == 1) cnt1++;
            if (board[i, i] == 2) cnt2++;
        }
        if (cnt1 == 3) return 1;
        if (cnt2 == 3) return 2;

        cnt1 = cnt2 = 0;
        for (int i = 0; i < 3; i++)
        {
            if (board[0+i, 2-i] == 1) cnt1++;
            if (board[0+i, 2-i] == 2) cnt2++;
        }
        if (cnt1 == 3) return 1;
        if (cnt2 == 3) return 2;
        //
        if (flg) return 0;
        else return 3;
    }
}
