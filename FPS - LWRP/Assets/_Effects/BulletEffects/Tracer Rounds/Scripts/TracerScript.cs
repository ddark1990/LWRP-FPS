using UnityEngine;
using System.Collections;

public class TracerScript : MonoBehaviour {
	
	public float despawnTime;
	
	void Start () {
		StartCoroutine (Despawn());
	}
	
	IEnumerator Despawn() {
		yield return new WaitForSeconds (despawnTime);
		Destroy (gameObject);
	}
}