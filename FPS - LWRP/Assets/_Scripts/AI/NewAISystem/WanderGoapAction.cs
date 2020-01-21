using UnityEngine;

namespace NewAISystem
{
    public class WanderGoapAction : GoapAction
    {
        public float waitMinTime = 1f;
        public float waitMaxTime = 1f;
        public float wanderRadius = 5f;

        private float _tempWaitTime;
        private float _elapsedTime;
        private bool _completed;

    public WanderGoapAction()
    {
        addPrecondition("hasTarget", false);
    }

    public override void reset()
    {
        _tempWaitTime = Random.Range(waitMinTime, waitMaxTime);
        _elapsedTime = 0f;
        _completed = false;
    }

    public override bool isDone()
    {
        return _completed;
    }

    public override bool requiresInRange()
    {
        return false;
    }

    public override bool checkProceduralPrecondition(AiStateController controller)
    {
        return true;
    }

    public override bool perform(AiStateController controller)
    {
        //if (controller.target || controller.aiVitals.isDead) return false;

        if (_elapsedTime == 0 && !controller.navAgent.hasPath)
        {
            //Debug.Log("Starting to wander.");
            
            //Debug.Log("Path Calculated for " + controller.name);
            
            //Debug.Log("Path Set for " + agent.name);
        }
        if (controller.distanceFromTarget <= controller.navAgent.stoppingDistance)
        {
            //Debug.Log("Started waiting.");
            _elapsedTime += Time.deltaTime;
        
            if(_elapsedTime >= _tempWaitTime)
            {
                //Debug.Log("Finished waiting.");
                _completed = true;
            }
        }

        return true;
    }
    }
}