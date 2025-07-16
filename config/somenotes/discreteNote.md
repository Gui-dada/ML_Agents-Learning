### 离散行为定义

~~~ C#
enum ItemType { Sword, Shield, Bow }
public class HeroAgent : Agent
{
    [Observable]
    ItemType m_CurrentItem;
}
~~~


是在 **Unity ML-Agents 2.x+ 的“自动观察系统”**中使用 [Observable] 属性，告诉训练系统：让 m_CurrentItem 自动变成 Observation 数据的一部分。

🔍 分析各部分

~~~c#
enum ItemType { Sword, Shield, Bow } 
~~~

这是一个枚举类型（enum），表示三个物品类型：

~~~
Sword = 0

Shield = 1

Bow = 2
~~~

它是一个离散变量，通常用于状态、类型选择。

~~~c#
[Observable] ItemType m_CurrentItem;
~~~

这是 ML-Agents 引入的新特性：自动观察注解系统。
在 ML-Agents 2.0 起，你可以用 [Observable] 标记一个字段（field），系统就会自动将它序列化为观察值，而不用手动写进 CollectObservations()。

对于枚举，默认行为是将其转换成 one-hot 编码：

m_CurrentItem	观察值（Observation 向量）
Sword (0)	[1, 0, 0]
Shield (1)	[0, 1, 0]
Bow (2)	[0, 0, 1]

✅ 效果总结
你只需要声明 [Observable]，不需要写 sensor.AddOneHotObservation()。

ML-Agents 会自动检测枚举值，转换为对应的 one-hot 向量。

枚举的数量 = Observation 向量的维度（比如这里是 3）。

🔧 补充说明
你也可以写成：

~~~c# 
[Observable(numStackedObservations: 1)]

~~~

ItemType m_CurrentItem;
来手动指定 observation 堆叠次数，或者用 displayName 重命名：

~~~c#
[Observable(displayName: "CurrentItem")]
ItemType m_CurrentItem;
~~~

🚀 推荐使用场景
适用于强化学习中：

当前角色的职业、状态

当前场景类型

手中持有的道具

怪物种类等







~~~c#
public class Ball3DHardAgent : Agent {
    [Observable(numStackedObservations: 9)]
	Vector3 PositionDelta
	{
    	get
    		{
       			 return ball.transform.position - gameObject.transform.position;
    		}
	}
}
~~~

**为什么返回的是9**
这是因为你使用了 ML-Agents 的 [Observable(numStackedObservations: 9)] 特性，它的参数含义是：

🔍 numStackedObservations: 9 的含义：
它的作用是：

**在每一步训练中，保留当前 observation 和过去的 8 个历史 observation，一共 堆叠 9 次 observation 值，作为当前的输入状态。**

📦 举个例子：
你定义的 PositionDelta 是一个 Vector3，即：
// Vector3 = (x, y, z)
默认这就是一个 3 维的观察值。

设置 numStackedObservations: 9 后：

会在 observation buffer 中 堆叠最近的 9 个 Vector3

所以最终 observation 是 3 × 9 = 27 维的浮点向量

这就像给 AI 留了个「短时记忆」，让它知道过去 9 步 ball 相对于自己的移动趋势，从而学会预测和追踪。

✅ 为什么这么做？
堆叠历史状态是强化学习中一种提升智能体表现的常用技巧，原因包括：

原因				说明
弥补部分可观察性	当前状态可能不足以推断完整环境状态，历史信息提供上下文
更好判断动态趋势	比如判断 ball 是正在靠近还是远离
增强时序感知能力	适用于 RNN-free 的时序建模

🧠 所以总结：
[Observable(numStackedObservations: 9)]
Vector3 PositionDelta
不是说返回的是 9，而是：

将 PositionDelta 这个 Vector3（3 维），在训练时自动堆叠最近 9 帧，总共传入神经网络的是 27 个值。

如果你希望减少模型输入维度、或者历史信息不重要，也可以设置为：
[Observable(numStackedObservations: 1)]