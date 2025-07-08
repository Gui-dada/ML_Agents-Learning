using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class RollerAgent : Agent
{
    [SerializeField]Rigidbody rBody;
    // Start is called before the first frame update
    public float forceMultiplier = 10;

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }
    ///<summary>
    ///OnEpisodeBegin()
    ///CollectObservations(VectorSensor sensor)
    ///OnActionReceived(ActionBuffers actionBuffers)
    ///</summary>

    public Transform Target;
    public override void OnEpisodeBegin()
    {
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y < 0)
        {
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.localPosition = new Vector3(0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.value * 8 - 4,0.5f,Random.value * 8 - 4);
    }

    //添加观察的数据向量
    //从而影响Agents的判断
    public override void CollectObservations(VectorSensor sensor)
    {
        //总共八个数据向量

        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);//3
        sensor.AddObservation(this.transform.localPosition);//3

        // Agent velocity
        sensor.AddObservation(rBody.velocity.x);//1
        sensor.AddObservation(rBody.velocity.z);//1
    }
    
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        //根据距离设计奖励
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);

        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Fell off platform
        else if (this.transform.localPosition.y < 0)
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
