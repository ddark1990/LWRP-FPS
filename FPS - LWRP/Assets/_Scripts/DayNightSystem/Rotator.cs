using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotateSpeed = 2;
    public Transform transformToRotate;

    private Vector3 v3;

    private void Start()
    {
        v3 = transformToRotate.eulerAngles;
    }

    void Update()
    {
        if (transformToRotate.eulerAngles.x == 360)
            transformToRotate.eulerAngles = new Vector3(0, 0, 0);

        v3.x += rotateSpeed * Time.deltaTime;
        transformToRotate.eulerAngles = v3;
    }
}
