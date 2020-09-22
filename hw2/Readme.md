# 第二次作业
## 1、简答题
### 1.1 解释 游戏对象（GameObjects） 和 资源（Assets）的区别与联系。
资源和游戏对象本质上都是数据，资源存储在硬盘上，游戏对象在游戏运行时存储在内存中。资源可以包括模型、声音、图片、脚本等文件，是在游戏中可能用用到的各种数据。而游戏对象可以认为是运行中的资源，是Unity场景中所有实体的基类。它可以包含许多组件和属性，也可以挂载脚本，因此游戏对象更像一个组件的容器。

二者的联系：资源可以作为模板，实例化为具体的游戏对象，可以作为游戏对象中的属性。我们创建的游戏对象也可以存储为资源，例如：在Unity中，我们可以创建一个游戏对象，并把它保存为资源以供后续多次使用。
### 1.2 下载几个游戏案例，分别总结资源、对象组织的结构（指资源的目录组织结构与游戏对象树的层次结构）
以unity hub提供的学习项目John Lemon's Haunted Jaunt:3D Beginner为例：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/1.jpg)

资源的目录通常包括Materials（材料）、Models（模型）、Scripts（脚本）、Textures（包括人物角色、场景、用户界面）等文件夹。还常常包括animation（动画）、Audio（音频文件）、Gizmos(好像是一个调试工具)。由此可见，资源常常按照文件类型来进行分类。

### 1.3 编写一个代码，使用 debug 语句来验证 MonoBehaviour 基本行为或事件触发的条件 
官方文档中对各个事件触发条件的描述：
1. Awake()：在加载脚本实例时调用， 在脚本实例的生命周期内仅调用 Awake 一次。
2. Start()：在首次调用任何 Update 方法之前启用脚本时，类似于 Awake 函数，Start 在脚本生命周期内仅调用一次。但是在调用任何对象的 Start 函数之前，将在场景中的所有对象上调用 Awake 函数。
3. Update()：每帧调用 Update。
4. FixedUpdate()：每个固定帧率帧调用该函数，默认时间为 0.02 秒（50 次调用/秒）。可以在Edit->Project Settings->Time的Fixed Timestep更改该值。
5. LateUpdate()：LateUpdate 在调用所有 Update 函数后调用。
6. OnGUI()：每帧可能会多次调用 OnGUI 实现（每个事件调用一次）。
7. OnDisable()：该函数在行为被禁用时调用，当对象销毁时也会调用该函数。
8. OnEnable()：该函数在对象变为启用和激活状态时调用。
测试代码：
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Update");
    }

    void Awake()
    {
        Debug.Log("Awake");
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate");
    }

    void LateUpdate()
    {
        Debug.Log("LateUpdate");
    }

    void OnGUI()
    {
        Debug.Log("OnGUI");
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
    }
}
```
我将该脚本挂载在一个cube上，下图是测试结果：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/2.jpg)

可以看到Awake和Start只出现一次，LateUpdate一定出现在Update后面，OnGUI也不定时出现。由于我调整了Fixed Timestep为3s，所以FixedUpdate没3s出现一次，具体如下图：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/3.jpg)

为了测试OnDisable和OnEnable，我注释了Update类的事件，并勾选activeSelf属性，测试结果如下：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/4.jpg)

在我不勾选时，触发OnDisable，选中时触发OnEnable。

### 1.4 查找脚本手册，了解 GameObject，Transform，Component 对象 
#### 1.4.1 分别翻译官方对三个对象的描述（Description）
* GameObject：Unity 场景中所有实体的基类。
* Transform：对象的位置、旋转和缩放。
* Component：附加到 GameObject 的所有内容的基本类。
#### 1.4.2 描述下图中 table 对象（实体）的属性、table 的 Transform 的属性、 table 的部件 

![](https://pmlpml.gitee.io/game-unity/post/images/ch02/ch02-prefabs.png)

table 的对象是 GameObject，第一个选择框是activeSelf属性，用于标识此游戏对象的本地活动状态；选择框右边是name属性，为该游戏对象的名称；isStatic指定游戏对象是否为静态；Layer标识该游戏对象所在的层；第三行的Prefab属性用于设置该对象的预设。

table的Transform的属性包括三部分：Position表示游戏对象的位置坐标为（0，0，0）；Rotation表示旋转角度为（0，0，0）；Scale表示游戏对象的在X、Y、Z方向的拉伸程度分别为（1，1，1）。

除上述部件外，table还有Cube（Mesh Filter）、Box Collider、Mesh Renderer等部件。Mesh是用于通过脚本创建或修改网格的类；Collider是所有碰撞体的基类，Box Collider表示盒体形状的原始碰撞体，center表示在该对象本地空间中测量的盒体中心，size表示在该对象本地空间中测量的盒体大小。
#### 1.4.3 用 UML 图描述三者的关系

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/5.jpg)

### 1.5 资源预设（Prefabs）与 对象克隆 (clone) 
* 预设有什么好处？

  预设或称预置，是将基本的游戏对象组合起来，当作一个游戏对象使用。文件存储的游戏对象与属性的组合，可一次性方便地加载到内存，而不用每次都从基础游戏对象构建游戏。

* 预设与对象克隆 (clone or copy or Instantiate of Unity Object) 关系？

  预设创建的实体会根据预设的变化而变化，而克隆的实体不会因为原实体的变化而变化。

* 制作 table 预制，写一段代码将 table 预制资源实例化成游戏对象。
table预制：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/6.jpg)

实例化代码：
```C#
public class prefabs : MonoBehaviour
{
    public GameObject table;

    // Start is called before the first frame update
    void Start()
    {
        GameObject instance = Instantiate(table);
        instance.transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
```
## 2、编程实践，小游戏
游戏内容：井字棋
使用了IMGUI构建UI，游戏界面如下

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw2/images/7.jpg)

代码关键部分:

1. 检查胜利条件：我采用了比较愚蠢的方法，通过两个二重循环分别检查行、列；通过两个单重循环检查对角线是否满足胜利条件。
```C#
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
```
2. 棋盘的更新：这部分代码放置在OnGUI函数中，根据二维数组的值决定按钮中的字符。
```C#
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
```
具体代码见TicTacToe.cs

运行方法：在Unity中将TicTacToe.cs代码挂载在空对象上即可运行游戏。
