using UnityEngine;
using System.Collections;

public class ShootScript : MonoBehaviour {
	
	public Rigidbody tracer;
	public Transform tracerSpawnPoint;
	[Space(10)]
	public float tracerSpeed;
	
	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
			
			//Set the rotation of the prefab to the same as spawnpoint
			tracer.transform.rotation = tracerSpawnPoint.transform.rotation;
			
			//Spawn the tracer
			Rigidbody instantiatedTracer = Instantiate
				(tracer, tracerSpawnPoint.transform.position, tracerSpawnPoint.transform.rotation)
					as Rigidbody;
			
			//Add velocity to the tracer
			instantiatedTracer.velocity = transform.TransformDirection(Vector3.right * tracerSpeed);
		}
	}
}