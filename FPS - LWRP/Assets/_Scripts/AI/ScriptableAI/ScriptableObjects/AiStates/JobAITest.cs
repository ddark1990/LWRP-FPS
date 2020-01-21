using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental;
using UnityEngine.Experimental.AI;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Jobs;

public class JobAITest : MonoBehaviour
{
    public Vector3[] paths;

    private void Start()
    {

    }

    private void Update()
    {


        for (int i = 0; i < paths.Length; i++)
        {
            Debug.DrawLine(transform.position, paths[i], Color.green);
        }

        if (Input.GetKeyDown(KeyCode.O))
            TryFindPath(transform.position, transform.position + (UnityEngine.Random.insideUnitSphere * 10));
    }

    public void TryFindPath(Vector3 start, Vector3 end)
    {
        using (var polygonIds = new NativeArray<PolygonId>(100, Allocator.Persistent))
        using (var query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Persistent, 100))
        {
            int maxIterations = 1024;
            var from = query.MapLocation(start, Vector3.one * 10, 0);
            var to = query.MapLocation(end, Vector3.one * 10, 0);

            var status = query.BeginFindPath(from, to);

            status = query.UpdateFindPath(maxIterations, out int currentIterations);

            var finalStatus = query.EndFindPath(out int pathLength);
            var pathResult = query.GetPathResult(polygonIds);

            var straightPath = new NativeArray<NavMeshLocation>(pathLength, Allocator.Temp);
            paths = new Vector3[pathResult];

            for (int i = 0; i < pathResult; i++)
            {
                var polyId = polygonIds[i];
                var polyWorldToLocal = query.PolygonWorldToLocalMatrix(polyId);

                var b = query.CreateLocation(paths[i], polyId);
                paths[i] = b.position;
                //Debug.Log(b.position);
            }

            //Debug.Log(pathResult);
            Debug.DrawLine(from.position, to.position);
        }
    }
}