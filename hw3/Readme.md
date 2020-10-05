# 第三次作业
## 1. 简答并用程序验证
### 游戏对象运动的本质
游戏对象运动的本质就是游戏对象在游戏的坐标系统中位置（通常是一个三维坐标）、角度（Unity中有欧拉角和四元数两种表示方法）和大小比例的变化。在Unity中，游戏的运动可以通过改变Transform这一部件中Position、Rotation和Scale这三个属性的值来实现。
### 请用三种以上的方法实现物体的抛物线运动
1. 最简单的抛物线方程为y=x^2， 因此我们可以通过直接修改Transform属性来实现抛物线运动。
```C#
public class Parabola : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x + 1 * Time.deltaTime;
        this.transform.position = new Vector3(x, x*x, 0);
    }
}
```
2. 使用Vector3的方法：以平抛运动为例子，将速度分解到x和y两个方向，x方向做匀速直线运动，y方向做匀加速直线运动。利用了Vector3中的Vector3.right和Vector3.down这两个变量。
```C#
public class FlatThrow : MonoBehaviour
{
    public float xspeed, yspeed, g = 9.8f;
    // Start is called before the first frame update
    void Start()
    {
        xspeed = 1;
        yspeed = 0;
        this.transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        yspeed += g * Time.deltaTime;
        transform.position += Vector3.right * Time.deltaTime * xspeed;
        transform.position += Vector3.down * Time.deltaTime * yspeed;
    }
}
```
3. 利用上题中的思想，通过两个脚本来实现平抛运动，一个脚本实现x方向的运动，一个脚本实现y方向上的运动，代码就不在此赘述。
4. 使用Translate函数来改变坐标：再次以平抛运动为例，我们计算得到一个变换矩阵mv，然后调用Translate来进行变换：
```C#
public class FlatThrow : MonoBehaviour
{
    public float xspeed, yspeed, g = 9.8f;
    // Start is called before the first frame update
    void Start()
    {
        xspeed = 1;
        yspeed = 0;
        this.transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        yspeed += g * Time.deltaTime;
        Vector3 mv = new Vector3(xspeed * Time.deltaTime, -Time.deltaTime * yspeed, 0);
        this.transform.Translate(mv);
    }
}
```
### 写一个程序，实现一个完整的太阳系，其他星球围绕太阳的转速必须不一样，且不在一个法平面上
#### GameObject
为了实现一个完整的太阳系模拟，我们需要使用球体（Sphere）模拟太阳系中的星球。为了使得星体更为逼真，我上网查找了一些太阳系星球的贴图并将其制作成material。再通过改变球体的大小和间距得到下图中的效果。
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw3/images/solarsystem1.jpg)
#### 公转程序：
该程序设置了一个public变量center用于设置公转的中心，在Start函数中随机设置了公转的速度以及公转转轴向量（0，y，z），在Update函数中调用了RotateAround函数。
```C#
public class revolution : MonoBehaviour
{
    public Transform center;
    private float speed;
    private Vector3 normal;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(10, 30);
        normal.y = Random.Range(-50, 0);
        normal.z = Random.Range(-50, 50);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(center.position, normal, speed * Time.deltaTime);
    }
}
```
#### 自转程序
自转程序就非常简单，同样在Update函数中调用RotateAround函数，只不过旋转中心为自己的位置，转轴为（0，0，1）。
```C#
public class rotation : MonoBehaviour
{
    private float speed;
    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(30, 60);
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
```
#### 项目运行方式和效果
运行方式：将solar-system的Asserts文件夹拖入Project窗口中，将场景solar system拖入Scene窗口点击运行。
为了记录星体的运动轨迹，我为各个星体添加了Trail Renderer组件。其需要设置Material 才可显示所设置的颜色，可以根据需要更改轨迹的宽度值width。最终实现效果如下
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw3/images/solarsystem2.jpg)
## 2. 编程实践
### 游戏规则
> 
Priests and Devils
Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck!
* 游戏中提及的事物（Objects)： 3 priests, 3 devils, river, boat, bank.

### 玩家动作表（规则表）：
|玩家动作|执行条件|执行结果|
| ---- | ---- | --- |
| 点击牧师/魔鬼 |游戏进行中，船停在同一边且船上的人数少于2|牧师/魔鬼上船|
|点击船|游戏进行中且船上至少有一人|船从河的一端移动到另一端|
|重置|玩家成功或失败|将游戏对象设置为初始状态|

### 游戏设计框架
本游戏使用面向对象设计和MVC架构。
#### 面向对象设计
先直接给出游戏框架图：
![](https://pmlpml.gitee.io/game-unity/post/images/ch03/ch03-oo-game-architecture.png)
按照Cocos 2d的设计理念，设计一个游戏如同组织一场话剧，至少需要3个角色：
1. 导演（Director）：仅需要1个，所以使用单例模式。其职责有：获取当前游戏的场景；控制场景运行、切换、入栈与出栈；管理游戏全局状态；设定游戏的配置；设定游戏全局视图等。其代码如下：（基本照搬老师的代码）
```C#
public class Director : System.Object
{
    private static Director _instance;
    public ISceneController CurrentScenceController { get; set; }
    public static Director getInstance()
    {
        if (_instance == null)
        {
            _instance = new Director();
        }
        return _instance;
    }
    public int getFPS()
    {
        return Application.targetFrameRate;
    }

    public void setFPS(int fps)
    {
        Application.targetFrameRate = fps;
    }
}
```
2. 场记（SceneController）：可以有多个，但是该游戏只有一个场景，所以只有一个场记。其职责大致如下：管理本次场景所有的游戏对象；响应外部输入事件；管理本场次的规则等。代码大致如下：
```C#
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
    
    //实现ISceneController和IUserAction中的接口
    //可添加一些辅助函数
}
```
3. 吃瓜群众（玩家）：职责就是玩游戏啦！程序中的IUserAction接口定义了玩家能够进行的动作，这些接口在本场景的SceneController中实现。对于本次游戏，IUserAction代码为：
```C#
public interface IUserAction
{
    void MoveBoat();
    void Restart();
    void MoveCharacter(CharacterController character);
}
```
这和我们在上面玩家动作表里的分析是一致的。
#### MVC架构
MVC架构把程序分为三个部分：
* 模型（Model）：数据对象及关系。在该游戏中，我们需要制作的模型有：河岸（bank）、河水（river）、牧师（priest）、恶魔（devil）和船（boat），制作完毕后将其保存为预制。我们还需要预先在unity中布置好场景，并记录下他们在不同状态下的坐标，方便接下来使用。
* 控制器（Controller）：除了场景的主控制器（GameController），针对每一个 Model ，我实现了对应的Controller，用于控制对应游戏对象的运动：
1. 河岸控制器（bankcontroller）：应该具备的属性：标识是左岸还是右岸，空位的信息。应该具备的函数：获取空位，根据坐标返回其在河岸上的第几个位置。代码如下：
```C#
public class BankController
{
    public GameObject bank;
    public int flg; // 1: right  -1: left
    public bool[] empty = new bool[6];
    private Vector3 first_pos = new Vector3(2.5f, 0.2f, -3);

    public Vector3 getEmptyPosition() //返回值为(0,0,0)时表示无空位
    {
        Vector3 emptyPositon = first_pos;
        int i = 0;
        for (; i < 6; i++)
        {
            if (empty[i]) break;
        }
        if (i < 6) emptyPositon.x = (float)((emptyPositon.x + i * 0.6) * flg);
        else emptyPositon = Vector3.zero;
        return emptyPositon;
    }

    public int index(Vector3 pos)
    {
        double v = ((flg * pos.x - 2.5) / 0.6);
        int index = (int)v;
        if (v - index > 0.5) index += 1;
        return index;
    }
}
```
2. 船控制器（BoatController）：应具备的属性：标识船在左岸还是右岸，空位信息。应具备的函数：获取船上的空位，判断是否有乘客，判断船是否在移动，移动船。代码如下：
```C#
public class BoatController
{
    public GameObject boat;
    public int flg; //-1: left bank  1: right bank
    public bool[] empty = new bool[2];// 0:船上的靠左的乘客 1:船上靠右的乘客
    private Vector3 right_pos = new Vector3(1.2f, -0.9f, -3); //船在右岸的位置
    private Vector3 bank_passenger = new Vector3(1.52f, -0.6f, -3); //右岸靠岸乘客位置
    private Vector3 river_passenger = new Vector3(0.88f, -0.6f, -3); //右岸靠河乘客位置
    public Move move;
    public Click click;

    public Vector3 getEmptyPosition()
    {
        Vector3 emptyPosition = Vector3.zero;
        if (empty[0])
        {
            emptyPosition = (flg == 1) ? river_passenger : bank_passenger;
            emptyPosition.x *= flg;
        }
        else if (empty[1])
        {
            emptyPosition = (flg == 1) ? bank_passenger : river_passenger;
            emptyPosition.x *= flg;
        }
        return emptyPosition;
    }

    public bool isEmpty()
    {
        return empty[0] && empty[1];
    }

    public void boatMove()
    {
        Vector3 des = right_pos;
        if (flg == 1) des.x = -des.x;
        move.MovePosition(des);
        flg = -flg;
    }

    public bool isMoving()
    {
        Vector3 left_pos = right_pos;
        left_pos.x = -right_pos.x;
        return (boat.transform.position != right_pos && boat.transform.position != left_pos);
    }
}
```
3. 角色模型（CharacterController）：应具备属性：角色标识，是否在船上。应具备的函数：角色移动。具体代码如下：
```C#
public class CharacterController
{
    public GameObject character;
    public int role; // 0: priest 1: devil
    public bool on_boat; // True: on boat
    public Move move;
    public Click click;

    public void characterMove(Vector3 des)
    {
        move.MovePosition(des);
    }
}
```
* 界面（View）：显示模型，将人机交互事件交给控制器处理。具体的代码如下
```C#
public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    public int flg = 0;//0:gaming 1:lose 2:win
    bool isShow = false;

    // Start is called before the first frame update
    void Start()
    {
        action = Director.getInstance().CurrentScenceController as IUserAction;
    }

    // Update is called once per frame
    void OnGUI()
    {
        GUIStyle text_style;
        GUIStyle button_style;
        text_style = new GUIStyle()
        {
            fontSize = 30
        };
        button_style = new GUIStyle("button")
        {
            fontSize = 15
        };
        if (GUI.Button(new Rect(10, 10, 60, 30), "Rule", button_style))
        {
            if (isShow)
                isShow = false;
            else
                isShow = true;
        }
        if (isShow)
        {
            GUI.Label(new Rect(Screen.width / 2 - 85, 10, 200, 50), "让全部牧师和恶魔都过河");
            GUI.Label(new Rect(Screen.width / 2 - 120, 30, 250, 50), "每一边恶魔数量都不能多于牧师数量");
            GUI.Label(new Rect(Screen.width / 2 - 85, 50, 250, 50), "点击牧师、恶魔、船移动");
        }
        if (flg == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 90, Screen.height / 2 - 120, 100, 50), "Gameover!", text_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Restart", button_style))
            {
                action.Restart();
                flg = 0;
            }
        }
        else if (flg == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 - 120, 100, 50), "You Win!", text_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 50, 100, 50), "Restart", button_style))
            {
                action.Restart();
                flg = 0;
            }
        }
    }
}
```
PS：该UI设计代码借鉴了师兄设计，[传送门](https://blog.csdn.net/c486c/article/details/79795708)
#### 辅助脚本
这些脚本挂载预制体上(挂载方式是使用AddComponent函数)，帮助实现一些动作。

可以观察到，船控制器和角色控制器都有两个属性：Click和Move。Click脚本是用来检测船和角色是否被点击，是通过调用OnMouseDown方法来实现。代码如下：
```C#
public class Click : MonoBehaviour
{
    IUserAction action;
    CharacterController Character = null;
    BoatController Boat = null;
    public void SetCharacter(CharacterController Character)
    {
        this.Character = Character;
    }
    public void SetBoat(BoatController Boat)
    {
        this.Boat = Boat;
    }

    void Start()
    {
        action = Director.getInstance().CurrentScenceController as IUserAction;
    }

    private void OnMouseDown()
    {
        if (Character == null && Boat == null) return;
        if (Boat != null) action.MoveBoat();
        else if (Character != null) action.MoveCharacter(Character);
    }
}
```
Move脚本是用来让角色移动的，在实现时可以不用逻辑细节，只用实现移动的功能就行了。代码如下：
```C#
public class Move : MonoBehaviour
{
    public float speed = 20;
    public int flg = 0; //0:不动 两阶段移动
    private Vector3 end;//
    private Vector3 middle;//
    
    void Update()
    {
        if (flg == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, middle, speed * Time.deltaTime);
            if (transform.position == middle) flg = 2;
        }
        else if (flg == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);
            if (transform.position == end) flg = 0;
        }
    }

    public void MovePosition(Vector3 position)
    {
        end = position;
        if (end.y == transform.position.y) flg = 2; // boat
        if (end.y < transform.position.y) //character->boat
        {
            flg = 1;
            middle = new Vector3(end.x, transform.position.y, transform.position.z);
        }
        if (end.y > transform.position.y) //boat->character
        {
            flg = 1;
            middle = new Vector3(transform.position.x, end.y, transform.position.z);
        }
    }
}
```

