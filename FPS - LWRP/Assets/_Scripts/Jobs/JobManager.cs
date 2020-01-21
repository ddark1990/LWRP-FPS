using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using static AiJobUtil;
using UnityEngine.Experimental.AI;
using System.Collections.Concurrent;

public class JobManager : MonoBehaviour
{
    public List<AiStateController> globalAiList;

    //distance to target job variables
    //private NativeList<JobHandle> jobHandleList;
    private NativeArray<float3> _posArray;
    private NativeArray<float3> _targetPosArray;
    private NativeArray<float3> _distanceToTargetResult;
    private NativeArray<float> _lengthToTarget;

    private void Update()
    {
        GetDistanceToTargetJob(globalAiList); //runs the distance comparer between ai's & its focus (maybe other things too like magnitude, etc)
    }

    public void InitializeAi(AiStateController aiController) //when ai is spawned into the world, it initializes itself by adding self to global ai list
    {
        globalAiList.Add(aiController);
    } 
    //public void CalculateNavMeshPaths(List<AiStateController> listOfAi)
    //{
    //    pathArray = new NativeArray<float3>(listOfAi.Count, Allocator.TempJob);

    //    for (int i = 0; i < listOfAi.Count; i++)
    //    {
    //        var controller = listOfAi[i];

    //        var job = ScheduleCalculateNavMeshPathsJob(listOfAi, controller.transform, controller.transform.position + (UnityEngine.Random.insideUnitSphere * 10), i);
    //        jobQueue.Enqueue(job);
    //    }
    //    pathArray.Dispose();
    //}
    //private JobHandle ScheduleCalculateNavMeshPathsJob(List<AiStateController> listOfAi, Transform selfTransform, Vector3 targetPosition, int index)
    //{
    //    pathArray[index] = selfTransform.GetComponent<AiStateController>().Paths[index];

    //    CalculateNavMeshPathJob job = new CalculateNavMeshPathJob
    //    {
    //        pos = selfTransform.position,
    //        targetPos = targetPosition,
    //        paths = pathArray,
    //        query = query
    //    };

    //    var jobHandle = job.Schedule();
    //    jobHandle.Complete();

    //    Debug.Log("GETTINGAIPATHS");

    //    return jobHandle;
    //}

    private void GetDistanceToTargetJob(List<AiStateController> listOfAi)
    {
        //jobHandleList = new NativeList<JobHandle>(Allocator.Temp); //first create native collections to store data
        _posArray = new NativeArray<float3>(listOfAi.Count, Allocator.TempJob);
        _targetPosArray = new NativeArray<float3>(listOfAi.Count, Allocator.TempJob);
        _distanceToTargetResult = new NativeArray<float3>(listOfAi.Count, Allocator.TempJob);
        _lengthToTarget = new NativeArray<float>(listOfAi.Count, Allocator.TempJob);

        for (var i = 0; i < listOfAi.Count; i++) //loop through list of affected objects
        {
            var controller = listOfAi[i];
            var aiFov = controller.GetComponent<FieldOfView>();

            if (!controller.target) continue;
            
            ScheduleDistanceToTargetJob(listOfAi, controller.transform, controller.target, i); //run job with wtver parameters you need
            //jobHandleList.Add(job); //add the job to ur native list 

            //JobHandle.CompleteAll(jobHandleList); //run ur entire list of scheduled up jobs

            controller.distanceFromTarget = _lengthToTarget[i]; //write data output anywhere you need
        }

        //jobHandleList.Dispose(); //dispose of all native collections after ur done with ur job's process
        _posArray.Dispose();
        _targetPosArray.Dispose();
        _distanceToTargetResult.Dispose();
        _lengthToTarget.Dispose();
    }
    
    private JobHandle ScheduleDistanceToTargetJob(ICollection listOfAi, Transform posTransform, Transform targetPosTransform, int index)
    {
        _posArray[index] = posTransform.position; //put all data into ur collections
        _targetPosArray[index] = targetPosTransform.position;
        _lengthToTarget[index] = posTransform.GetComponent<AiStateController>().distanceFromTarget;

        GetDistanceFromTargetJob job = new GetDistanceFromTargetJob //create job and pass any data it requires
        {
            pos = _posArray,
            targetPos = _targetPosArray,
            lengthToTarget = _distanceToTargetResult,
            distanceToTarget = _lengthToTarget
        };

        var jobHandle = job.Schedule(listOfAi.Count, listOfAi.Count / 10); //schedule the job and cache it
        jobHandle.Complete();

        posTransform.position = _posArray[index]; //apply data from native collections back to the affected object
        targetPosTransform.position = _targetPosArray[index];

        Debug.DrawLine(_posArray[index], _targetPosArray[index]);

        return jobHandle;
    }


}
