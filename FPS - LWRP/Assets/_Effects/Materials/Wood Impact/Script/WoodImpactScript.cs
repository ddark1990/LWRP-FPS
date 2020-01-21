using UnityEngine;
using System.Collections;

public class WoodImpactScript : MonoBehaviour {
	
	public float despawnTime;
	
	void Start () {
		StartCoroutine(DespawnTimer());
	}
	
	IEnumerator DespawnTimer () {
		yield return new WaitForSeconds(despawnTime);
		Destroy(gameObject);
	}
}