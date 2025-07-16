### 多智能体组（Multi-Agent Group） 机制

## 🌐 背景

在某些训练场景（例如团队协作、对抗、组队打怪等），单个 Agent 奖励可能不足以训练良好策略。这时就可以使用 **`SimpleMultiAgentGroup`** 来 **绑定多个 Agent，使它们共享奖励和终止信号**。

------

## 🧩 代码详解

```c#
// Create a Multi Agent Group in Start() or Initialize()
m_AgentGroup = new SimpleMultiAgentGroup();
```

🔹 在 `Start()` 或 `Initialize()` 中创建一个新的多智能体组实例。
 这个组会管理多个 Agent 的奖励、终止和同步行为。

------

```c#
// Register agents in group at the beginning of an episode(场景)
for (var agent in AgentList)
{
  m_AgentGroup.RegisterAgent(agent);
}
```

🔹 在每个 Episode 开始时，把所有 Agent 注册进组中。

- `AgentList` 是你场景中所有需要联合训练的 Agent 实例。
- 这一步很关键，**注册之后，所有组员的奖励是共享的**。

------

```c#
// if the team scores a goal
m_AgentGroup.AddGroupReward(rewardForGoal);
```

🔹 当组员完成某项团队目标（如进球、推箱子成功等）时，给予整个组奖励。
 **这个奖励会平均分配给所有注册的 Agent。**

------

```c#
// If the goal is reached and the episode is over
m_AgentGroup.EndGroupEpisode();
ResetScene();
```

🔹 当训练目标达成后，**终止整个组的 Episode**（所有 Agent 同时 `EndEpisode()`），
 然后 `ResetScene()` 是你自定义的环境重置逻辑（重置位置、分数、目标等）。

------

```c#
// If time ran out and we need to interrupt the episode
m_AgentGroup.GroupEpisodeInterrupted();
ResetScene();
```

🔹 如果中途需要强制打断（如时间到或外部中断条件），就使用这个方法。
 它**不会影响训练统计数据**，但可以安全地结束当前周期。

------

## 🎯 为什么使用 MultiAgentGroup？

| 场景             | 好处                         |
| :--------------- | ---------------------------- |
| 🧠 多个协作 Agent | 可以共享奖励、同步终止训练   |
| 🥅 团队目标导向   | 训练更加稳定，Agent 学会协作 |
| 🤖 多角色训练     | 简化管理，减少重复代码       |
| 💥 对抗模式       | 可管理敌我双方奖励，精确控制 |



------

## ✅ 总结一句话：

> `SimpleMultiAgentGroup` 是 Unity ML-Agents 提供的多智能体组训练机制，**用于绑定多个 Agent 同步训练、共享奖励与终止控制**，非常适合 FPS 团队对战、组队协作、双人配合等任务。



## note：

- Agents with different behavior names in the same group are not supported.（不允许同组不同名）

- If an agent finished earlier, e.g. completed tasks/be removed/be killed in the game, do not call EndEpisode() on the Agent. （单个代理不觉得组行为）

-  If the episode is completed, you would want to call EndGroupEpisode. But if the episode is not over but it has been running for enough steps, i.e. reaching max step, you would call GroupEpisodeInterrupted.（如果回合已完成，则需要调用 EndGroupEpisode。但如果回合尚未结束但已运行了足够的步数，即达到了最大步数，则需要调用 GroupEpisodeInterrupted。）

- If an agent that was disabled in a scene needs to be re-enabled, it must be re-registered to the MultiAgentGroup.（启用组代理必须注册）

  

