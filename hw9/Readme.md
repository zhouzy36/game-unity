# 作业九 UI效果制作之背包系统
> 18342144 周子扬

## 任务
参考NGUI官方例子，使用Unity实现一个背包系统。
![](https://ww3.sinaimg.cn/large/6177e8b1gw1f42sxsw6drg20z80i9npm.gif)
## 制作过程
1. 创建一个Canvas和UI Camera，设置Canvas的Reander Mode为World Space，Event Camera为创建的UI Camera
2. 在创建的Canvas中新建一个Canvas子对象，命名为Inventory，用于存放背包栏和装备栏的画布。
3. 在Inventory中创建两个Panel子对象，命名为Bag和Wear表示物品栏和装备栏，分别添加Gird LayOut Group组件用于自动布局。可以通过Cell Size设置grid大小，通过Spacing设置grid间隔，通过修改Constraint可以固定行行数或列数。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw9/images/1.jpg)

4. 使用Image填充背包格子和装备，根据需要添加文本。
5. 在场景下创建一个空对象命名为SF Scene Elements，在该对象下放置主摄像机，并创建一个空的子对象命名为Backgound，并添加Sprite Rendera组件。选择一张图片作为背景，记得要将图片格式设置为Sprite（2D and UI）后才能拖入Sprite Rendera的Sprite。
6. 调整两个摄像机的参数
    - UI Camera：设置Clear Flags为Depth only，Culling Mask为UI，Depth为0
    - Main Camera：设置Clear Flags为Skybox，Culling Mask选取除UI外的所有，Depth为-1。
其他大小自行调整，到这一步效果如下：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw9/images/2.jpg)

7. 添加角色模型，这里我是从unity资源库下载的[人物模型](https://assetstore.unity.com/packages/3d/characters/humanoids/humans/fantasy-chess-rpg-character-arthur-160647),将预制放入场景中调整位置和大小，并且更改其动画控制器为：删除原先的死亡等动画。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw9/images/3.jpg)

8. 添加窗口随鼠标移动的效果，创建一个脚本window并挂载在Bag和Wear上，脚本代码如下：
```C#
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class window : MonoBehaviour
{
	public Vector2 range = new Vector2(5f, 3f);
	Transform mTrans;
	Quaternion mStart;
	Vector2 mRot = Vector2.zero;

	void Start()

	{
		mTrans = transform;
		mStart = mTrans.localRotation;
	}

	void Update()

	{
		Vector3 pos = Input.mousePosition;
		float halfWidth = Screen.width * 0.5f;
		float halfHeight = Screen.height * 0.5f;
		float x = Mathf.Clamp((pos.x - halfWidth) / halfWidth, -1f, 1f);
		float y = Mathf.Clamp((pos.y - halfHeight) / halfHeight, -1f, 1f);
		mRot = Vector2.Lerp(mRot, new Vector2(x, y), Time.deltaTime * 5f);
		mTrans.localRotation = mStart * Quaternion.Euler(-mRot.y * range.y, mRot.x * range.x, 0f);
	}
}

```
此脚本是参考的，实现原理大概是根据鼠标的位置将Canvas进行小角度的旋转。
9. 在Inventory中新建一个空对象collections用于存放物品，类型为Image。每一个Image对象表示一个物品，在Image组件中选择相应的Source Image作为装备贴图。每一个Image还要添加一个Canvas Group组件，作用在之后会讲解。

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw9/images/4.jpg)

10. 实现物品的拖拽功能:这是一个比较复杂的功能，我们需要实现这几个接口IBeginDragHandler, IDragHandler, IEndDragHandler。要实现的功能是让物品跟随鼠标一起移动，当物品移动到背包的格子时将物品对其格子，在空白区域释放时将物品复位，在另一物品上释放时交换两物品的位置。具体代码如下：
```C#
using UnityEngine;

using System.Collections;

using UnityEngine.EventSystems;

using UnityEngine.UI;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler

{

    private Transform myTransform;
    private RectTransform myRectTransform;

    // 用于event trigger对自身检测的开关
    private CanvasGroup canvasGroup;

    // 拖拽操作前的有效位置，拖拽到有效位置时更新
    public Vector3 originalPosition;

    // 记录上一帧所在物品格子
    private GameObject lastEnter = null;

    // 记录上一帧所在物品格子的正常颜色
    private Color lastEnterNormalColor;

    // 拖拽至新的物品格子时，该物品格子的高亮颜色
    private Color highLightColor = Color.cyan;

    void Start()

    {

        myTransform = this.transform;
        myRectTransform = this.transform as RectTransform;

        canvasGroup = GetComponent<CanvasGroup>();

        originalPosition = myTransform.position;

    }

    void Update()

    {

    }

    public void OnBeginDrag(PointerEventData eventData)

    {

        canvasGroup.blocksRaycasts = false;//让event trigger忽略自身，这样才可以让event trigger检测到它下面一层的对象,如包裹或物品格子等

        lastEnter = eventData.pointerEnter;
        lastEnterNormalColor = lastEnter.GetComponent<Image>().color;

        originalPosition = myTransform.position;//拖拽前记录起始位置

        gameObject.transform.SetAsLastSibling();//保证当前操作的对象能够优先渲染，即不会被其它对象遮挡住

    }

    public void OnDrag(PointerEventData eventData)

    {

        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(myRectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            myRectTransform.position = globalMousePos;
        }

        GameObject curEnter = eventData.pointerEnter;

        bool inItemGrid = EnterItemGrid(curEnter);
        if (inItemGrid)
        {
            Image img = curEnter.GetComponent<Image>();

            lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
            if (lastEnter != curEnter)

            {

                lastEnter.GetComponent<Image>().color = lastEnterNormalColor;
                lastEnter = curEnter;//记录当前物品格子以供下一帧调用

            }

            //当前格子设置高亮
            img.color = highLightColor;

        }

    }

    public void OnEndDrag(PointerEventData eventData)

    {

        GameObject curEnter = eventData.pointerEnter;

        //拖拽到的空区域中（如包裹外），恢复原位
        if (curEnter == null)
        {
            myTransform.position = originalPosition;
        }
        else
        {
            //移动至物品格子上
            if (curEnter.name == "UI_ItemGrid")
            {
                myTransform.position = curEnter.transform.position;
                originalPosition = myTransform.position;

                curEnter.GetComponent<Image>().color = lastEnterNormalColor;//当前格子恢复正常颜色
            }
            else

            {
                //移动至包裹中的其它物品上
                if (curEnter.name == eventData.pointerDrag.name && curEnter != eventData.pointerDrag)
                {
                    Vector3 targetPostion = curEnter.transform.position;
                    curEnter.transform.position = originalPosition;
                    myTransform.position = targetPostion;
                    originalPosition = myTransform.position;
                }
                else//拖拽至其它对象上面（包裹上的其它区域）
                {
                    myTransform.position = originalPosition;
                }

            }

        }

        lastEnter.GetComponent<Image>().color = lastEnterNormalColor;//上一帧的格子恢复正常颜色
        canvasGroup.blocksRaycasts = true;//确保event trigger下次能检测到当前对象

    }

    // 判断鼠标指针是否指向包裹中的物品格子
    bool EnterItemGrid(GameObject go)
    {
        if (go == null)
        {
            return false;
        }
        return go.name == "UI_ItemGrid";
    }

}
```
一个关键的地方：物品跟随鼠标移动的过程中，鼠标一般是不会检测到它所经过的格子区域的，因为鼠标视线被物品挡住了。组件Canvas Group中有一个属性Blocks Raycasts，将它的值设置为false就可以让鼠标透过物品，看到物品下面的格子了，从而就可以实现格子的高亮显示了。
## 制作效果
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw9/images/5.gif)

## 参考博客
[UGUI 实现Inventory(背包系统)](https://blog.kinpzz.com/2016/05/21/unity3d-ugui-Inventory/)

[利用UGUI制作的包裹系统（一）](http://www.manew.com/thread-39589-1-1.html)
