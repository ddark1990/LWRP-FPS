using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform RagdollRoot;
    [SerializeField] private Transform RagdollHead;
    public Transform mainModel;
    public Transform ragdoll;
    public float testfloat;
    public AudioSource audioSource;

    public void SetRadDollState(AiVitals vitals, Animator animator, NavMeshAgent agent, Rigidbody rb, bool active)
    {
        agent.enabled = !active;
        rb.isKinematic = true; 
        animator.enabled = !active;
        vitals.GetComponent<AiStateController>().enabled = !active;
        vitals.GetComponent<ThirdPersonCharacter>().enabled = !active;

        mainModel.gameObject.SetActive(!active);

        ragdoll.gameObject.SetActive(active);
        ApplyBulletForceToCollider();

        EffectsManager.Instance.PlayDeathSound(audioSource); //eh jok
    } //set the active state of the ragdoll, trasfer animator velocity to the ragdoll based on rootmotion

    public static void CopyTransformData(Transform sourceTransform, Transform destTransform, Vector3 velocity)
    {
        if (sourceTransform.childCount != destTransform.childCount)
        {
            Debug.LogWarning("Invalid transform copy! Do not match transform heirarchies");
            return;
        }

        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            var rb = destination.GetComponent<Rigidbody>();
            if (rb != null)
                rb.velocity = velocity;

            CopyTransformData(source, destination, velocity);
        }
    }

    public void ApplyBulletForceToCollider()
    {
        for (int i = 0; i < 1; i++)
        {
            var child = RagdollRoot.transform.GetChild(i);

            RagdollHead.GetComponent<Rigidbody>().AddForce(WeaponManager.Instance.BulletVelocity * testfloat, ForceMode.Impulse);
            //child.GetComponent<Rigidbody>().AddForce(Vector3.forward * 100, ForceMode.Impulse);
            //Debug.Log(child.name);

        }
    }
}
