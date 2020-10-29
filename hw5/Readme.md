# 第五次作业
## 编写一个简单的鼠标打飞碟（Hit UFO）游戏
运行方式：将Hit-UFO的Asserts文件夹拖入Project窗口中，创建一个空对象，将GameController.cs代码拖入空对象中即可运行。
### 游戏内容要求
1. 游戏有 n 个 round，每个 round 都包括10 次 trial；
2. 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制；
3. 每个 trial 的飞碟有随机性，总体难度随 round 上升；
4. 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定。
### 游戏要求
1. 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类；
2. 近可能使用前面 MVC 结构实现人机交互与游戏模型分离。
### 程序设计
#### 飞碟工厂
***工厂方法模式***
工厂方法模式中，类一个方法能够得到一个对象实例，使用者不需要知道该实例如何构建、初始化等细节。这满足创建型模式中所要求的“创建与使用相分离”的特点。在本次游戏中会产生不同速度、方向和颜色的飞碟，我们可以写一个专门生产不同飞碟的工厂类。
***对象池***
创建与销毁需要耗费大量资源，因此对象销毁时，我们仅做一个标记，等需要时再重新初始化并投入使用，该技术称为对象池。在本次游戏中，每一轮都会产生多个飞碟。在回合结束后，我们可以不急着销毁这些游戏对象，而是将其放在对象池中。当下一回合开始时，我们可以为其赋予新的属性，再次使用这些飞碟。
***工厂方法 + 对象池 + 单实例 = DiskFactory.cs***
##### DiskData.cs
飞碟的相关属性，用于挂载在飞碟游戏对象上。具体有：大小、速度、颜色、分值、速度方向。代码如下：
```C#
public class DiskData : MonoBehaviour
{
    public int score;
    public Vector3 size;
    public Color color;
    public float speed;
    public Vector3 direction;
}
```
##### DiskFactory.cs
生产飞碟的工厂类，用于生产、使用、回收飞碟，使用单例模式。主要的方法：
1. GetDisk：获取飞碟。可以是从对象池中获取，也可以是新实例化。根据规则（此处用回合数为参数）给飞碟设置速度、方向、颜色和分值。我的设计思路是：飞碟有红、绿、蓝三种颜色，分值分别为1、2、3分。第一回合只有红色飞碟，第二回合加入绿色飞碟，第三回合以后三种飞碟都有。速度随着回合数增加而变快，发射角度也随着速度发生改变，速度越快角度越小以防止飞碟只从画面边缘划过。（规则部分在写的时候花了些时间调整参数，其实这是交给游戏策划师来设计的了，我做的就马虎一点，不是重点）。
ps：发射角度的参数为一个方向向量(cosθ,sinθ,0)，这只允许飞碟在xy平面上运动，速度与水平面的夹角为θ。由于我设置的飞碟可能从两侧飞出来，具体的一些细节见代码实现。
2. FreeDisk：将飞碟回收进对象池。
代码如下：
```C#
public class DiskFactory : MonoBehaviour
{
    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public GameObject diskPrefab;
    private List<DiskData> used = new List<DiskData>(); //使用中的飞碟列表
    private List<DiskData> free = new List<DiskData>(); //未使用的飞碟列表

    //游戏的一些参数
    private const float speed = 10;
    private const float factor1 = 0.15f; //加速因子
    private const float factor2 = 0.02f; //控制发射角度下限
    private const float factor3 = 0.04f; //控制发射角度上限

    //单例模式
    private static DiskFactory factory; 

    public static DiskFactory getInstance()
    {
        if (factory == null)
        {
            factory = new DiskFactory();
        }
        return factory;
    }

    //根据回合生产飞碟，当然也可以设计一个规则类作为参数来生产飞碟
    public GameObject GetDisk(int round)
    {
        Assert.IsTrue(round > 0 && round <= 10);
        diskPrefab = null;

        int colorSelector;
        if (round <= 3) colorSelector = Random.Range(0, round);
        else colorSelector = Random.Range(0,3);
        //Debug.Log(colorSelector);

        //如果有空闲的飞碟就直接拿来使用，没有的话就实例化新的飞碟
        int x = Random.Range(0, 2) > 0.5 ? 20 : -20;
        if (free.Count > 0)
        {
            diskPrefab = free[0].gameObject;
            diskPrefab.transform.position = new Vector3(x, 0, 0);
            //将该飞碟从free列表中删除
            free.Remove(free[0]);
        }
        else
        { 
            diskPrefab = Instantiate(Resources.Load<GameObject>("Prefabs/disk"), new Vector3(x, 0, 0), Quaternion.identity);
            diskPrefab.AddComponent<DiskData>();
        }

        diskPrefab.GetComponent<DiskData>().size = diskPrefab.transform.localScale;
        //根据round设置速度
        diskPrefab.GetComponent<DiskData>().speed = speed * Mathf.Pow(1 + factor1, round);

        //根据round设置发射角度
        float angle = Random.Range(Mathf.PI / 12 - factor2 * round, Mathf.PI / 4 - factor3 * round);
        angle = diskPrefab.transform.position.x < 0 ? angle : (Mathf.PI - angle); 
        diskPrefab.GetComponent<DiskData>().direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);

        //根据colorSelector给飞碟上色，并为其指定分数
        switch (colorSelector)
        {
            case (int)Color.Red:
                diskPrefab.GetComponent<DiskData>().score = 1;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                break;
            case (int)Color.Green:
                diskPrefab.GetComponent<DiskData>().score = 2;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.green;
                break;
            case (int)Color.Blue:
                diskPrefab.GetComponent<DiskData>().score = 5;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.blue;
                break;
            default:
                diskPrefab.GetComponent<DiskData>().score = 1;
                diskPrefab.GetComponent<Renderer>().material.color = UnityEngine.Color.red;
                break;
        }
        diskPrefab.GetComponent<DiskData>().color = diskPrefab.GetComponent<Renderer>().material.color;

        diskPrefab.SetActive(true);
        //将该飞碟添加到used列表中
        used.Add(diskPrefab.GetComponent<DiskData>());
        return diskPrefab;
    }

    public void FreeDisk(GameObject disk)
    {
        for (int  i = 0; i < used.Count; i++)
        {
            if (disk.GetInstanceID() == used[i].gameObject.GetInstanceID())
            {
                used[i].gameObject.SetActive(false);
                free.Add(used[i]);
                used.Remove(used[i]);
                break;
            }
        }
    }
}
```
#### 动作部分
##### CCActionManager.cs
在该游戏中，飞碟不需要一直执行运动，只要飞碟飞出玩家视角即可对其回收（在回调中实现）。具体代码如下：
```C#
public class CCActionManager : SSActionManager, ISSActionCallback
{
    public GameController gameController; //场景控制器
    public CCFlyAction fly;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //完成飞行动作后就回收飞碟
        gameController.factory.FreeDisk(source.gameobject);
    }

    protected new void Start()
    {
        gameController = (GameController)SSDirector.getInstance().CurrentScenceController;
        gameController.actionManager = this; //将场景的动作管理者设置为自己
    }

    //飞碟飞行
    public void UFOFly(GameObject disk, float speed, Vector3 direction)
    {
        fly = CCFlyAction.GetSSAction(speed, direction);
        this.RunAction(disk, fly, this);
    }
}
```
##### CCFlyAction.cs
飞碟的运动轨迹为一个抛物线，对于抛物线运动，只需要给定飞碟的初速度和方向即可确定运动轨迹。具体代码如下：
```C#
public class CCFlyAction : SSAction
{   
    public float speed; //初速度
    public Vector3 direction; //方向，值为（cos，sin，0）

    private float gravity = 9.8f;
    private float x_speed;
    private float y_speed;

    public static CCFlyAction GetSSAction(float speed, Vector3 direction)
    {
        CCFlyAction action = ScriptableObject.CreateInstance<CCFlyAction>();
        action.speed = speed;
        action.direction = direction;
        return action;
    }

    public override void Start()
    {
        direction = direction.normalized;
        x_speed = speed * direction.x;
        y_speed = speed * direction.y;
    }

    public override void Update()
    {
        y_speed = y_speed - gravity * Time.deltaTime;
        this.transform.position += x_speed * Vector3.right * Time.deltaTime;
        this.transform.position += y_speed * Vector3.up * Time.deltaTime;
        //飞碟飞出玩家视角就销毁动作并调用回调函数回收
        if (Mathf.Abs(this.transform.position.y) > 10)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }
}
```
#### 核心框架
游戏主体框架大部分沿用以前的代码，改动部分如下
##### IUserAction.cs
在打飞碟游戏中，用户能执行的基本动作有开始游戏、结束游戏、打飞碟、获取得分。在我的设计中，用户可以终止游戏、手动提升难度即关卡升级。代码如下：
```C#
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
}
```
##### GameController.cs
该类为该游戏的控制器，主要是要实现IUserAction接口中的方法，控制飞碟的发送以及回合的升级。实现该类的主要的难点有：
1. 飞碟的发送：我用一个队列保存每回合要发送的飞碟。设定一个timer，在Update中更新timer的值，当timer的值小于或等于0时调用动作管理器的方法发送飞碟，当队列为空时则进行回合升级。为了使难度随回合升级增加，我会递增飞碟的数量和减短飞碟发送的间隔。
2. Hit方法的实现：我们想实现的效果就是我们点击飞碟，飞碟消失，分数增加。此处参考了老师上课给过的光标拾取物体的代码实现，老师的代码如下：
```C#
public class PickupObject : MonoBehaviour {

	public GameObject cam;

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Debug.Log ("Fired Pressed");
			Debug.Log (Input.mousePosition);

			Vector3 mp = Input.mousePosition; //get Screen Position

			//create ray, origin is camera, and direction to mousepoint
			Camera ca;
			if (cam != null ) ca = cam.GetComponent<Camera> (); 
			else ca = Camera.main;

			Ray ray = ca.ScreenPointToRay(Input.mousePosition);

			//Return the ray's hit
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)) {
				print (hit.transform.gameObject.name);
				if (hit.collider.gameObject.tag.Contains("Finish")) { //plane tag
					Debug.Log ("hit " + hit.collider.gameObject.name +"!" ); 
				}
				Destroy (hit.transform.gameObject);
			}
		}
	}
}
```
完整的GameController.cs代码：
```C#
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
```
##### UserGUI.cs
该类用于实现游戏界面，设计思路为：总共有3个状态：游戏前、游戏中和游戏结束。游戏前提供一个开始按钮并简单介绍游戏规则；游戏中则显示得分和当前回合，并提供两个按钮：游戏终止按钮和关卡升级按钮；游戏结束时显示玩家得分和最高分，提供一个重新开始游戏的按钮。思路清晰后就可以写代码了：
```C#
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
            if (GUI.Button(new Rect(Screen.width - 100, 50, 100, 50), "Round Up"))
            {
                action.RoundUp();
            }
        }
        //游戏结束
        else
        {
            high_score = high_score > action.GetScore() ? high_score : action.GetScore();
            GUI.Label(new Rect(Screen.width / 2 - 20, Screen.width / 2 - 250, 100, 100), "GameOver", over_style);
            GUI.Label(new Rect(Screen.width / 2 - 10, Screen.width / 2 - 200, 50, 50), "Best:", text_style);
            GUI.Label(new Rect(Screen.width / 2 + 50, Screen.width / 2 - 200, 50, 50), high_score.ToString(), text_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 20, Screen.width / 2 - 150, 100, 50), "Restart"))
            {
                inGame = true;
                action.Restart();
                return;
            }
            action.GameOver();
        }
    }
}
```
#### 其他功能类
##### Scorer.cs
该类顾名思义就是游戏得分的管理类，具体的方法有：获取得分、记录得分、得分清零。代码实现如下：
```C#
public class Scorer: MonoBehaviour
{
	//将得分设置为private以封装
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
```
## 游戏效果
开始界面：
![]()
游戏中界面：
![]()
游戏结束界面：
![]()