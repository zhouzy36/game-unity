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
## 2. 编程实践
* 游戏规则
> 
Priests and Devils
Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck!
* 游戏中提及的事物（Objects)： 3 priests, 3 devils, river, boat, side.

* 玩家动作表（规则表）：
|玩家动作|执行条件|执行结果|
| ---- | ---- | --- |
| 点击牧师/魔鬼 |游戏未结束，船停在同一边且船上的人数少于2|牧师/魔鬼上船|
|点击船|游戏未结束且船上至少有一人|船从河的一端移动到另一端|

  
