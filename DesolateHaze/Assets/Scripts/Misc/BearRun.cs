using UnityEngine;

public class BearRun : MonoBehaviour
{
    [SerializeField] Animator BearAnim;
    [SerializeField] float time;
    public bool BStop = false;

    public void RunOff()
    {
        time = 1f;
        BearAnim.SetFloat("Blend", time);
    }

    public void Update()
    {
        if (BStop == true)
        {
            time -= Time.deltaTime;
            BearAnim.SetFloat("Blend", time);
        }
    }
}