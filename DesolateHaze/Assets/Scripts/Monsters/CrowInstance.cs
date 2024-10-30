using UnityEngine;

public class CrowInstance : MonoBehaviour {
    [SerializeField] Vector2 exitVel;
    [SerializeField] Rigidbody rb;

    [SerializeField] AudioPoolInfo caw;
    [SerializeField] Animator CrowAnim;
    [SerializeField] float time;
    [SerializeField] bool BOn = false;

    public void flyOff() {
        time = 0f;
        BOn = true;

        rb.linearVelocity = exitVel;
        Destroy(gameObject, 1f);

        AudioManager.I.playSound(caw, transform.position, 1f);
    }

    public void Update()
    {
        if (BOn == true)
        {
            Debug.Log("SCAW");
            time += Time.deltaTime;
            CrowAnim.SetFloat("Blend", time);
        }
    }
}
