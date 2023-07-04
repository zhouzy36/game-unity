# 第十次作业 坦克对战游戏 AI 设计

## 任务
1. 使用“感知-思考-行为”模型，建模 AI 坦克
2. 场景中要放置一些障碍阻挡对手视线
3. 坦克需要放置一个矩阵包围盒触发器，以保证 AI 坦克能使用射线探测对手方位
4. AI 坦克必须在有目标条件下使用导航，并能绕过障碍。（失去目标时策略自己思考）
5. 实现人机对战

## 制作过程
大部分制作过程参考了该[博客](https://blog.csdn.net/Jenny_Shirunhao/article/details/103337423)
### 场景布置
1. 在[资源商店](https://assetstore.unity.com/packages/3d/vehicles/land/kawaii-tanks-free-version-1-1-54604)中下载Kawaii Tank资源包，并将资源包导入项目。在导入时会报错，此时导入标准资源包中的CrossPlatformInput就解决了。
2. 使用Kawaii Tank 的包Scenes中的Test_Field，自己布置场景，并放置AI坦克，AI坦克的prefab是SD_Firefly_1.1。到这一步的场景如下：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw10/images/1.jpg)

### AI寻路
1. 选中地形Terrian，在Navigation窗口中选择Navigation Static，设置Navigation Area为Walkable。
2. 在 Navigation 窗口选中Bake一栏，然后点击Bake，生成导航网格图。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw10/images/2.jpg)

3. 编写寻路脚本AIroute，代码如下：
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIroute : MonoBehaviour
{
    public GameObject target;  //获取目标点
    NavMeshAgent agent;   //声明变量

    void Start()
    {
        //获取自身的NavMeshAgent组件
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //设置目标
        agent.SetDestination(target.transform.position);
    }
}
```
4. 将脚本挂载在AI坦克预制的MainBody上，并为其添加NavMeshAgent 组件，设置target为玩家坦克的MainBody，此时AI坦克已经能自动寻路了。
### 自动开火
按照师兄[博客](https://blog.csdn.net/Jenny_Shirunhao/article/details/103337423)修改Fire_Control_CS脚本，让AI坦克能每隔3s发射一次炮弹。

修改Damage_Control_CS脚本让玩家一直能看到自己的血条。当血条为0时，AI坦克会消失。
到此步基本可以实现要求了。

### 碰撞触发
1. 为AI坦克添加一个Sphere Collider，勾选Is Trigger，设置适当的Radius（我设置的是70）。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw10/images/3.jpg)

2. 修改玩家坦克的MainBody游戏对象名字为PlayerMainBody用于区分。
3. 编写碰撞事件检测脚本，当玩家坦克进入Sphere Collider时触发AI寻路，当玩家离开碰撞体时就关闭AI寻路，代码如下：
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Check : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
    	//当玩家进入时使能AI坦克的自动寻路功能
        if (other.gameObject.name == "PlayerMainBody")
        {
            GameObject obj = this.gameObject;
            obj.GetComponent<NavMeshAgent>().enabled = true;
            obj.GetComponent<AIroute>().enabled = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "PlayerMainBody")
        {
            GameObject obj = this.gameObject;
            obj.GetComponent<NavMeshAgent>().enabled = false;
            obj.GetComponent<AIroute>().enabled = false;
        }
    }
}
```
4. 设置AI坦克的AIroute脚本和NavMeshAgent组件的初始enabled为false。

## 最终效果
运行方式：将Asset->Scene->SampleScene放入场景点击运行。

玩家进入AI坦克的“警戒范围”时，AI坦克驶向玩家。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw10/images/4.jpg)

## 不足
AI坦克发射出的子弹会“误伤”其他AI坦克。
