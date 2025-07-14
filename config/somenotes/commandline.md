### 启动tensorboard

~~~bash
tensorboard --logdir results --port 6006
~~~

~~~bash
tensorboard --logdir=config/result --port 6006
~~~



### 自定义训练模型的文件读取和结果反馈

~~~bash
mlagents-learn config/ppo/SphereAgent.yaml --run-id=run01 --results-dir=config/result/
~~~

### 初始化已有模型继续训练：

#### 使用 --initialize-from：

~~~bash
bash mlagents-learn config/ppo/YourEnv.yaml --run-id=newRun --initialize-from=run01 --results-dir=config/result/
~~~

