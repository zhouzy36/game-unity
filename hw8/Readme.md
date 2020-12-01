# 第八次作业
运行方式：将Asserts中的magic ball预制放入场景即可看到效果，sea预制则要运行才能看到效果。
## 简单粒子制作
要求：按[参考资源](https://www.cnblogs.com/CaomaoUnity3d/p/5983730.html)要求制作一个一个粒子系统。除了完成参考资源中的粒子系统外，我还把课上实现的爆炸效果和粒子海洋也放到作业中了。

## 粒子系统概述

### 粒子系统及作用
粒子系统可以模拟并渲染许多称为粒子的小图像或网格以产生视觉效果。系统中的每个粒子代表效果中的单个图形元素。系统共同模拟每个粒子以产生完整效果的印象。我们经常使用粒子系统模拟的现象有火、爆炸、烟、水流、火花、落叶、云、雾、雪、尘、流星尾迹或者象发光轨迹这样的抽象视觉效果Unity 提供两种粒子系统解决方案：内置粒子系统和Visual Effect Graph，我们使用的是前者。
### 粒子系统组件和属性
使用 菜单 -> GameObject -> Effects -> Particle System 在游戏对象场景中添加一个 Particle System。在 Inspector 中见到 Particle System 有许多组件：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/1.jpg)

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/2.jpg)

常用的也就是Emission、Shape、Renderer组件。本次作业中还用到了Color over Lifetim、Texure Sheet Animation。
### 主属性
具体用法见[官方文档](https://docs.unity.cn/cn/2020.2/Manual/PartSysMainModule.html)

### Emission
此模块中的属性会影响粒子系统发射的速率和时间。具体属性有
| 属性               | 功能                           |
| ------------------ | ------------------------------ |
| Rate over Time     | 每个时间单位发射的粒子数。     |
| Rate over Distance | 每个移动距离单位发射的粒子数。 |
| Bursts             | 爆发式生成粒子的               |
如果激活 Rate over Distance 模式，则父对象移动的每个距离单位将释放一定数量的粒子。对于模拟实际由对象运动产生的粒子非常有用（例如，泥路上车轮留下的尘土）。

### Shape
此模块用于定义可发射粒子的体积或表面以及起始速度的方向。Shape 属性定义发射体积的形状（默认为Cone），其余模块属性根据您选择的 Shape 值而变化。如果想向空间四周发射粒子，常用Sphere和Hemisphere；若想向平面四周发射粒子常用Circle，具体情况具体分析。

### Renderer
Renderer 模块的设置决定了粒子的图像或网格如何被其他粒子变换、着色和过度绘制。默认粒子物体是面片，可以通过设置Material 属性来设置粒子的材质。

---

除了上述默认有的一些组件，常用的一些模块还有
### Color over Lifetime

### Texture Sheet Animation
我们常常见到这样的材料
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/3.jpg)

其释放出来的效果为：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/4.jpg)

此模块允许您将纹理视为可作为动画帧进行播放的一组单独子图像。
具体的属性和作用见[官方文档](https://docs.unity.cn/cn/2020.2/Manual/PartSysTexSheetAnimModule.html)
此处我们将Tiles的x和y设置为2，表示将纹理划分为2x2的区块，可以根据曲线调整我们要使用的纹理，例如将curve设置为下图：
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/5.jpg)

得到的效果为：粒子只显示最后一种纹理效果

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/6.jpg)

可以通过调整曲线来设置粒子随着生命周期所显示的纹理变化。

## 制作粒子系统的一般步骤
1. 选择粒子的材料：Render 模块和Texture Sheet Animation 模块；
2. 设置粒子的初始状态和生命周期：主模块；
3. 发射器设置：Emission模块和Shape模块；
4. 运动和变化管理：… over Lifetime、… by Speed 等，通常是通过调整曲线来实现。

## 效果
### Mageic Ball
静止状态：

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/7.jpg)

移动状态：会有星光跟随

![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/8.jpg)

### Particle Sea
[参考博客](http://www.manew.com/thread-47123-1-1.html)
![](https://raw.githubusercontent.com/ShunShunNeverGiveUp/game-unity/master/hw8/images/9.jpg)

---

Ps:上次作业还在制作过程中！最近事情略多，上次作业又比较难，望TA理解！
