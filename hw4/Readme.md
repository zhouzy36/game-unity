# 第四次作业
## 1、基本操作演练
### 下载 Fantasy Skybox FREE， 构建自己的游戏场景
1. 创建地形：GameObject->3D Object->Terrain
2. paint terrain，这里我直接使用了该资源中的地形，效果如下：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw4/images/terrain.jpg)
3. 种树：选中terrain游戏对象，Inspector->Terrain->Paint Tree。选择Edit Trees->Add Tree，然后选择prefab，点击鼠标左键即可在地形上绘制，shift+右键擦除已绘制的树。可以从Assets Store下载资源，如我下载的Free SpeedTrees Package。可以在设置调整画笔的大小（bush size），树的密度（density）等。效果如下：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw4/images/tree.jpg)
4. 种草：和种树类似，选择New Bush，然后选择Texture（Fantasy Skybox有相关资源），点击鼠标左键即可在地形上绘制。效果如下：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw4/images/grass.jpg)
5. skybox
* 制作天空盒：天空盒本质是material，可以通过六面体构建一个天空盒。在Assets 上下文菜单 -> create -> Material。在 Inspector 视图中选择 Shader -> Skybox -> 6Sided，结果如下图：
![](https://pmlpml.gitee.io/game-unity/post/images/ch04/ch04-skybox-creation.png)
在资源贴图中选择合适的图片，拖放到对应位置即可。
* 将天空盒分配给当前场景:直接将天空盒拖放到场景中。我使用了资源包中制作好的天空盒，效果如下：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw4/images/skybox.jpg)
* 使用天空盒：在Camera 对象中添加部件 Rendering -> Skybox并将天空盒拖放入 Skybox！对应摄像机就使用自定义天空盒,而不使用场景的天空盒，效果如下：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw4/images/skybox1.jpg)
可以看到场景中的天空盒和Main Camera看到的不一样了。
### 写一个简单的总结，总结游戏对象的使用
游戏对象是Unity场景中所有实体的基类。（Unity官方文档对GameObject的描述）
我们在游戏中看到所有东西，包括光照、地形、物体等，以及我们的“眼睛”（Camera）都是游戏对象。除了看得见的东西，一些看不见的东西也算是游戏对象，例如我们之前写的游戏控制脚本要挂载在一个空游戏对象上运行。因此，游戏对象更确切的定义是component的容器，例如最常用的Transform。Unity中我们能创建的一些游戏对象例如Cube，是软件为我们添加好一些component的GameObject，我们还能根据需要继续为其添加或删除component。
我们可以在软件中直接实例化创建游戏对象，Unity已经为我们预制好了许多游戏对象，详情见GameObject菜单。我们也可以先预制，通过运行代码来实例化的方式创建游戏对象。在Unity中，我们可以选中游戏对象，在Inspector中点击Add Component来为游戏对象添加组件。在代码中，我们可以调用AddComponent方法来给游戏对象添加Component。特别的，我们可以用此方法为游戏对象挂载Mono脚本。
## 2、编程实践 牧师与魔鬼动作分离版
项目运行方法：将将Priests-and-Devils的Asserts文件夹拖入Project窗口中，创建一个空对象，将GameController.cs代码拖入空对象中即可运行。
### 动作管理器的设计
设计图（直接使用老师给的类图）：
![](https://pmlpml.gitee.io/game-unity/post/images/ch04/ch04-oo-design.png)
设计要点：
* 使用模板方法：SSActionManager作为所有运动管理者的基类
* 使用回调机制实现管理者和被管理者的解耦：组合对象实现一个事件抽象接口（ISSCallback），作为监听器（listener）监听子动作的事件；被组合对象使用监听器传递消息给管理者。
* 通过组合模式实现动作的组合：SSAction为所有动作的基类，用户可以设置基本的动作类（如本例中的CCMoveToAction），再通过组合基本动作实现组合动作（如本例中的CCSequenceAction）。
### 核心代码
1. 动作基类（SSAction）
代码：
```C#
public class SSAction : ScriptableObject
{
    public bool enable = true;  //是否正在进行此动作
    public bool destroy = false;    //是否需要被销毁
    public GameObject gameobject { get; set; } //动作对象
    public Transform transform { get; set; } //动作对象的transform
    public ISSActionCallback callback { get; set; } //回调函数

    protected SSAction() { } //保证SSAction不会被new
    public virtual void Start()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Update()
    {
        throw new System.NotImplementedException();
    }
}
```
设计要点：
* 官方文档对ScriptableObject的描述：一个类，如果需要创建无需附加到游戏对象的对象时，可从该类派生。这说明SSAction是不需要绑定游戏对象的，它是所有动作的基类
* 使用 virtual 将Start和Update申明为虚方法，继承者通过重写这两个方法实现不同的运动从而实现多态。
* 利用接口ISSACtionCallback实现消息通知，通常是在动作完成时调用以通知动作管理者，避免了与动作管理者直接依赖，相当于回调函数。
2. 简单动作——直线运动类（CCMoveToAction）
```C#
public class CCMoveToAction : SSAction
{
    public Vector3 target; //运动目标
    public float speed;

    public static CCMoveToAction GetSSAction(Vector3 target, float speed)
    {
        CCMoveToAction action = ScriptableObject.CreateInstance<CCMoveToAction>();
        action.target = target;
        action.speed = speed;
        return action;
    }

    public override void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
        if (this.transform.position == target) //完成动作
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }

    public override void Start()
    {
        //在本游戏中无动作
    }
}
```
设计要点：
* 实现了一个public静态方法用于在其他类中创建该运动控制类。使用了Unity的方法，确保内存正确回收。
* 重写Update和Start代码，Update中调用了我们熟悉的MoveTowards方法实现直线运动。
3. 顺序组合动作类（CCSequenceAction）
代码：
```C#
public class CCSequenceAction : SSAction, ISSActionCallback
{
    public List<SSAction> sequence; //动作的列表
    public int repeat = -1; //-1：无限循环组合中的动作
    public int start = 0;   //当前做的动作的索引

    public static CCSequenceAction GetSSAction(int repeat, int start, List<SSAction> sequence)
    {
        CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
        action.repeat = repeat;
        action.sequence = sequence;
        action.start = start;
        return action;
    }


    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        source.destroy = false;     //先保留这个动作，如果是无限循环动作组合之后还需要使用
        this.start++;
        if (this.start >= sequence.Count)
        {
            this.start = 0;
            if (repeat > 0) repeat--; //做完一次动作后将repeat减1
            if (repeat == 0)
            {
                this.destroy = true;
                this.callback.SSActionEvent(this); //告诉组合动作的管理对象组合做完了
            }
        }
    }

    public override void Start()
    {
        foreach (SSAction action in sequence)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this; //把sequence里的动作的回调函数设置为CCSequenceAction实现的这一个
            action.Start();
        }
    }

    public override void Update()
    {
        if (sequence.Count == 0) return;
        if (start < sequence.Count) sequence[start].Update();//这里不用也不能用start++，而是通过上面的回调函数实现
    }

    void OnDestory()
    {
        //TODO: something
    }
}
```
设计要点：
* CCSequenceAction其实也可以算作一个动作管理者了，它顺序执行sequence中的一系列动作。因此该类实现ISSActionCallback接口，在Start中将回调函数设置为当前实现的这一个。
* Update方法执行当前的动作。在这里，sequence[start].Update()调用的是CCMoveToAction类中的Update。
* SSActionEvent 收到当前动作执行完成的回调后，将start++推到下一个动作，如果完成一次sequence内的所有动作，递减repeat。如果repeat减为0，通知该动作的管理者（下面将提到的CCActionManager）。
4. 动作事件接口（ISSActionCallback） 
代码：
```C#
public enum SSActionEventType : int { Started, Competeted }
public interface ISSActionCallback
{
    void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0, string strParam = null, Object objectParam = null);
}
```
设计要点:
* 使用枚举变量定义事件类型
* 所有事件管理者都必须实现这个接口来实现事件调度，因此组合动作类和动作管理类都必须实现该接口
5. 动作管理基类（SSActionManager）
代码：
```C#
public class SSActionManager : MonoBehaviour
{
    private Dictionary数据结构管理动作集合
    * Update函数用于添加和删除<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    protected void Start()
    {
        
    }

    [System.Obsolete]
    protected void Update()
    {
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();

        foreach(KeyValuePair<int,SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy) waitingDelete.Add(ac.GetInstanceID()); 
            else if (ac.enable) ac.Update();
        }

        foreach(int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }

}
```
设计要点：
* 使用Dictionary数据结构管理动作集合，使用List数据结构保存待添加和删除的动作。
* Update函数用于添加和删除动作。Update所做的事情具体为：
1. waitingAdd中的动作对象通过调用GetInstanceID函数获得唯一的ID，和对象本身一起作为键值对添加至动作Dictionary。
2. 遍历动作Dictionary，如果动作是待销毁的（destroy为true），则将其添加至waitingDelete中，否则调用动作的update进行动作。
3. 将waitingDelete中的动作从动作Dictionary中移除，并调用DestroyObject函数销毁该对象。
* 提供了添加新动作的方法 RunAction。该方法把游戏对象与动作绑定，并绑定该动作事件的消息接收者。
### 本例中的动作管理类实现（CCActionManager）
代码：
```C#
public class CCActionManager : SSActionManager, ISSActionCallback
{
    public GameController gameController;
    public CCMoveToAction boatMove;
    public CCSequenceAction characterMove;

    public void SSActionEvent(SSAction source, SSActionEventType events = SSActionEventType.Competeted, int intParam = 0, string strParam = null, object objectParam = null)
    {
        //do nothing
        //Debug.Log("Sequence action finished");
    }

    // Start is called before the first frame update
    protected new void Start()
    {
        gameController = (GameController)Director.getInstance().CurrentScenceController;
        gameController.actionManager = this;    //将场景的动作管理者设置为自己
    }

    public void moveBoat(GameObject boat, Vector3 target, float speed)
    {
        boatMove = CCMoveToAction.GetSSAction(target, speed);
        this.RunAction(boat, boatMove, this);
    }

    public void moveCharacter(GameObject character, Vector3 middle_pos, Vector3 end_pos, float speed)
    {
        SSAction action1 = CCMoveToAction.GetSSAction(middle_pos, speed);
        SSAction action2 = CCMoveToAction.GetSSAction(end_pos, speed);
        characterMove = CCSequenceAction.GetSSAction(1, 0, new List<SSAction> { action1, action2 }); //1表示是做一次动作组合，0代表从action1开始
        this.RunAction(character, characterMove, this);
    }
}
```
设计要点：
* 该类继承了SSActionManager并且要实现ISSActionCallback接口。
* 由于船只进行水平移动，所以船动作类为CCMoveToAction；角色会进行一个两阶段的移动，所以角色的动作类为CCSequenceAction。
* 在Start中将当前场景的动作控制类设置为CCActionManager。
* moveBoat和moveCharacter是移动的具体方法，都先调用对应动作类的GetSSAction来实例化一个对象，并且调用基类中的RunAction函数将游戏对象与动作以及回调函数绑定。 
### 【2019开始的新要求】：设计一个裁判类
在阅读了许多师兄师姐的博客后，发现裁判类主要有两种写法：
1. 将check函数单独拿出来作为一个独立的类
2. 利用Update函数实时检查游戏获胜和失败的条件
我选择了第一种方法，因为我觉得实时检查游戏获胜和失败的条件会增加开销，其实只要在移动完船后检查即可实现目的，所以何乐而不为？代码：
```C#
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
```
调用的地方：
```C#
//GameController中
    public void MoveBoat()
    {
        if (Boat.isEmpty() || gui.flg != 0) return;        
        gui.flg = judgeController.check();
        actionManager.moveBoat(Boat.boat, Boat.positionToMove(), Boat.speed);
    }
```
设计要点：通过当前角色的位置预判开船后两岸角色的数量而不是在移动船后调用函数来检查当前的状态。（踩过坑的）因为在游戏运行时，调用完moveBoat函数后马上就能执行check，而等不到运动结束后，因此获取的位置都是运动前时的。除非使用第二种实现，利用Update实时检查游戏对象的状态。

---

最终运行效果和上一次相同