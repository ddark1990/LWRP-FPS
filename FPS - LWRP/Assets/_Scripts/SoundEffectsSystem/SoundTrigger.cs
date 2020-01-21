using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Play sound for " + other.gameObject.name);

        //if(other.GetComponent<TerrainCollider>())
        //{
        //    other.GetComponent<TerrainCollider>().
        //}
    }
}
