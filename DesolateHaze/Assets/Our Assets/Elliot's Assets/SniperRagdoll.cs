using UnityEngine;

public class SniperRagdoll : MonoBehaviour
{
    public GameObject sniperRig;
    Collider[] ragdollColliders;
    Rigidbody[] limbsRigidbodies;

    private void Start()
    {
        RagdollMode(true);
    }

    public void RagdollMode(bool state)
    {
        ragdollColliders = sniperRig.GetComponentsInChildren<Collider>();
        limbsRigidbodies = sniperRig.GetComponentsInChildren<Rigidbody>();

        foreach (Collider col in ragdollColliders)
        {
            col.enabled = state;
        }
        foreach (Rigidbody rigid in limbsRigidbodies)
        {
            rigid.isKinematic = !state;
        }
    }
}
