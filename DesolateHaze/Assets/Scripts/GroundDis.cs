using UnityEngine;

public class GroundDis : MonoBehaviour
{
    [SerializeField] private BoxCollider bc;
    [SerializeField] private bool Repeat;

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
            bc.enabled = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ground")
            bc.enabled = false;
    }
}
