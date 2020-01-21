using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Experimental.AI;

public static class AiJobUtil 
{
    [BurstCompile]
    public struct GetDistanceFromTargetJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> pos;
        [ReadOnly] public NativeArray<float3> targetPos;
        public NativeArray<float3> lengthToTarget;
        public NativeArray<float> distanceToTarget;

        public void Execute(int index)
        {
            lengthToTarget[index] = pos[index] - targetPos[index]; //distance vector3 from target
            
            //lengthToTarget[index] = distanceToTarget[index].x * distanceToTarget[index].x + distanceToTarget[index].y * distanceToTarget[index].y + distanceToTarget[index].x * distanceToTarget[index].z; //magnitude
            distanceToTarget[index] = math.sqrt(math.pow(lengthToTarget[index].x, 2f) + math.pow(lengthToTarget[index].y, 2f) + math.pow(lengthToTarget[index].z, 2f)); //distance float from target
        }
    }

    //public struct CalculateNavMeshPathJob : IJob 
    //{
    //    [ReadOnly] public float3 pos;
    //    [ReadOnly] public float3 targetPos;
    //    public NativeArray<float3> paths;
    //    public NavMeshQuery query;

    //    public void Execute()
    //    {
    //        using (var polygonIds = new NativeArray<PolygonId>(100, Allocator.Temp))
    //        {
    //            int maxIterations = 1024;

    //            var from = query.MapLocation(pos, Vector3.one * 10, 0);
    //            var to = query.MapLocation(targetPos, Vector3.one * 10, 0);

    //            query.BeginFindPath(from, to);
    //            query.UpdateFindPath(maxIterations, out int currentIterations);
    //            query.EndFindPath(out int pathLength);

    //            var pathResult = query.GetPathResult(polygonIds);

    //            for (int i = 0; i < pathResult; i++)
    //            {
    //                var polyId = polygonIds[i];
    //                var polyWorldToLocal = query.PolygonWorldToLocalMatrix(polyId);

    //                var location = query.CreateLocation(paths[i], polyId);
    //                paths[i] = location.position;
    //            }
    //        }
    //    }
    //}
}
