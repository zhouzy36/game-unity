# 第六次作业
## 1、改进飞碟（Hit UFO）游戏：
项目运行方法：将Hit-UFO-Improved的Asserts文件夹拖入Project窗口中，创建一个空对象，将GameController.cs代码拖入空对象中即可运行。
### 游戏内容要求： 
1. 按 adapter模式 设计图修改飞碟游戏
2. 使它同时支持物理运动与运动学（变换）运动
### 程序设计
#### Adapter 模式
适配器模式（Adapter）的定义如下：将一个类的接口转换成客户希望的另外一个接口，使得原本由于接口不兼容而不能一起工作的那些类能一起工作。

该模式的主要优点：
* 客户端通过适配器可以透明地调用目标接口。
* 复用了现存的类，程序员不需要修改原有代码而重用现有的适配者类。
* 将目标类和适配者类解耦，解决了目标类和适配者类接口不一致的问题。

使用场景：例如对于电商网站，它有一个实用简单同一的支付接口，现在这个接口要对接工行、农行、… 、微信、支付宝等不同支付接口。

在本次作业中，我们想保留之前实现的基于运动学的运动管理类CCActionManager，同时加入基于物理引擎的运动管理类PhysicalActionManager，我们就可以使用该设计模式。类图：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw6/images/adapterUML.png)

#### 新增的类
##### IActionManager.cs
该接口是适配器设计模式下的中间接口，该接口声明了所有运动管理类要实现的方法UFOFly，代码如下：
```C#
public interface IActionManager
{
    void UFOFly(GameObject disk, float speed, Vector3 direction);
}
```
##### PhysicalFlyAction.cs
该类使用了飞碟的Rigidbody组件，使用了刚体的重力属性使飞碟具有向下的加速度，使用了刚体的velocity变量为飞碟设置初始的速度，其余的运动则交给物理引擎控制。具体代码如下：
```C#
public class PhysicalFlyAction : SSAction
{
    public float speed; //初速度大小
    public Vector3 direction; //方向，值为（cos，sin，0）
    public Rigidbody rb;

    public static PhysicalFlyAction GetSSAction(Rigidbody rb, float speed, Vector3 direction)
    {
        PhysicalFlyAction action = ScriptableObject.CreateInstance<PhysicalFlyAction>();
        action.speed = speed;
        action.direction = direction;
        action.rb = rb;
        return action;
    }

    public override void Start()
    {
        rb.velocity = direction * speed;
    }

    public override void Update()
    {
        if (Mathf.Abs(this.transform.position.y) > 6)
        {
            this.destroy = true;
            this.enable = false;
            this.callback.SSActionEvent(this);
        }
    }
}
```
##### PhysicalActionManager.cs
该类的写法和CCActionManager一样，唯一的不同在于在UFOFly中，在使用PhysicalFlyAction的方法前要将飞碟刚体属性中的isKinematic属性设置为false，这样物理引擎才能控制飞碟。代码如下：
```C#
public class PhysicalActionManager : SSActionManager, ISSActionCallback, IActionManager
{
    public GameController gameController;
    public PhysicalFlyAction fly;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //完成飞行动作后就回收飞碟
        gameController.factory.FreeDisk(source.gameobject);
    }

    protected new void Start()
    {
        gameController = (GameController)SSDirector.getInstance().CurrentScenceController;
    }

    public void UFOFly(GameObject disk, float speed, Vector3 direction)
    {
        disk.GetComponent<Rigidbody>().isKinematic = false;
        fly = PhysicalFlyAction.GetSSAction(disk.GetComponent<Rigidbody>(), speed, direction);
        this.RunAction(disk, fly, this);
    }
}
```
##### Singleton.cs
该类为场景单实例模板。运用该模板，可以为每个 MonoBehaviour子类创建一个对象的实例。场景单实例的使用很简单，仅需要将 MonoBehaviour子类对象挂载任何一个游戏对象上即可。然后在任意位置使用代码 Singleton< YourMonoType >.Instance 获得该对象。代码如下：
```C#
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

	protected static T instance;

	public static T Instance {  
		get {  
			if (instance == null) { 
				instance = (T)FindObjectOfType (typeof(T));  
				if (instance == null) {  
					Debug.LogError ("An instance of " + typeof(T) +
					" is needed in the scene, but there is none.");  
				}  
			}  
			return instance;  
		}  
	}
}
```
#### 修改部分
##### 飞的预制
首先要修改的就是飞碟预制，我们要为飞碟预制加上刚体组件，各项属性使用其默认值。
##### GameController.cs
场景控制类GameController有较多的改动：
1. DiskFactory、Scorer等类使用了场景单实例模板。
2. 用IActionManager代替之前版本的CCActionManager，默认设置为PhysicalActionManager。增加一个isPhysics变量用于指示当前使用的动作管理者的类型。
3. 之前版本会在每次回合升级前将本回合所需的飞碟全部生成好放置在队列中，待计时器timer为0时将其发送。但是当飞碟加上了刚体组件后，这个方法就不行了。因为刚体会受到重力且具有碰撞检测功能，如果一次性生成所有要发射的飞碟，飞碟之间就会相互作用而弹开。所以新版本飞碟发射的机制变为要发射时才生成，并用一个计数器记录发射的数量。
4. 由于学习了粒子系统，所以在Hit方法中增加了爆炸效果的实现。大致思路就是在点击位置用一个协程（类似多线程的一个机制，代码是参考师兄的）生成一个爆炸预制并销毁。
完整代码如下：
```C#
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
```
##### CCActionManager.cs
在UFOFly方法中增加了一行代码用于让飞碟不受物理引擎控制
```C#
public void UFOFly(GameObject disk, float speed, Vector3 direction)
    {
        disk.GetComponent<Rigidbody>().isKinematic = true;//新增
        fly = CCFlyAction.GetSSAction(speed, direction);
        this.RunAction(disk, fly, this);
    }
```
##### IUserAction.cs
增加了切换运动管理器的方法：
```C#
void SetActionMode();
```
对应在GameController中的实现
```C#
public void SetActionMode()
    {
        actionManager = isPhysics ? Singleton<PhysicalActionManager>.Instance : Singleton<CCActionManager>.Instance as IActionManager;
        isPhysics = !isPhysics;
    }
}
```
##### UserGUI.cs
在OnGUI方法中增加了一个切换运动管理器的按钮（建议还是看完整代码，这里只粘贴了修改部分）：
```C#
if (GUI.Button(new Rect(Screen.width - 300, 0, 100, 50), mode))
            {
                action.SetActionMode();
                mode = mode == "Physical" ? "kinematic" : "Physical";
            }
```
### 运行效果
使用PhysicalActionManager
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw6/images/physical.jpg)

使用CCActionManager
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw6/images/kinematic.jpg)

爆炸效果
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw6/images/explosion.jpg)