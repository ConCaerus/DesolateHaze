using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Wall_PressurePlate : MonoBehaviour
{
    [SerializeField] Collider baseCol, plateCol;
    [SerializeField] List<Collider> groundCols = new List<Collider>();
    [SerializeField] Rigidbody plateRb;
    [SerializeField] float speed;

    [SerializeField] UnityEvent runOnDepressed;
    [SerializeField] UnityEvent runOnReset;

    float plateOffset;

    bool b = false;
    bool depressed
    {
        get { return b; }
        set
        {
            if (b == value) return;
            b = value;
            if (b) runOnDepressed.Invoke();
            else runOnReset.Invoke();

            if (!b)
            {
                plateRb.linearVelocity = Vector3.zero;
                plateRb.transform.position = baseCol.transform.position + Vector3.up * plateOffset;
            }
        }
    }

    public bool heldDown = false;

    private void Start()
    {
        Physics.IgnoreCollision(baseCol, plateCol, true);
        foreach (var i in groundCols)
            Physics.IgnoreCollision(i, plateCol, true);
        plateOffset = plateCol.transform.position.y - baseCol.transform.position.y;
    }

    private void LateUpdate()
    {
        manage();
    }

    void manage()
    {
        var curOffset = plateCol.transform.position.y - baseCol.transform.position.y;
        //  manages events
        depressed = curOffset < (plateOffset - .001f);

        //  manages velocity
        if (depressed && !heldDown)
        {
            var diff = plateOffset - curOffset;
            if (diff > 0f)
                plateRb.linearVelocity = Vector3.MoveTowards(plateRb.linearVelocity, Vector3.up * diff * speed, 50f * Time.deltaTime);
        }
    }
}
