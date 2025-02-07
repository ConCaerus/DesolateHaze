using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MeatMoundInstance : MonoBehaviour {
    [SerializeField] Transform rotPoint1, rotPoint2, tip;
    [SerializeField] Transform target;

    float l1, l2;

    private void Start() {
        l1 = getFirstLength();
        l2 = getSecondLength();

        target.position = transform.position;
        moveArms();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.U)) strike();
        moveArms();
    }


    /*
    float getRot1() {
        var top = Mathf.Pow(l1, 2f) + Mathf.Pow(getLengthToTarget(), 2f) - Mathf.Pow(l2, 2f);
        var bot = 2f * l1 * getLengthToTarget();
        return Mathf.Acos(top / bot) * Mathf.Rad2Deg;
    }
    float getRot2() {
        var top = Mathf.Pow(l2, 2f) + Mathf.Pow(l1, 2f) - Mathf.Pow(getLengthToTarget(), 2f);
        var bot = 2f * l2 * l1;
        return Mathf.Acos(top / bot) * Mathf.Rad2Deg;
    }*/
    /*
    void calculate_angles() {
        // Calculate distance from shoulder to target point (hypotenuse)
        var offsetFromBase = target.position - rotPoint1.transform.position;
        var distance = Mathf.Sqrt(Mathf.Pow(offsetFromBase.x, 2f) + Mathf.Pow(offsetFromBase.y, 2f));

        if(distance > (l1 + l2)) {
            offsetFromBase.Normalize();
            offsetFromBase *= l1 + l2;
            distance = Mathf.Sqrt(Mathf.Pow(offsetFromBase.x, 2f) + Mathf.Pow(offsetFromBase.y, 2f));
        }

        // Use the Law of Cosines to find the elbow angle (theta2)
        var cos_theta2 = (distance * distance - l1 * l1 - l2 * l2) / (-2 * l1 * l2);

        // Clamp cos_theta2 to avoid numerical errors in acos
        cos_theta2 = Mathf.Max(-1.0f, Mathf.Min(1.0f, cos_theta2));


        var theta2 = Mathf.Acos(cos_theta2); // Elbow angle

        //Use the Law of Cosines to find the shoulder angle (theta1)
        var k1 = l1 + l2 * cos_theta2;
        var k2 = l2 * Mathf.Sin(theta2);


        var theta1 = Mathf.Atan2(target.position.y, target.position.x) - Mathf.Atan2(k2, k1);  // Shoulder angle

        // Convert angles from radians to degrees
        var theta1_deg = theta1 * Mathf.Rad2Deg;
        var theta2_deg = theta2 * Mathf.Rad2Deg;

        rotPoint1.transform.localEulerAngles = Vector3.forward * theta1_deg;
        rotPoint2.transform.localEulerAngles = Vector3.forward * theta2_deg;
    }
    */

    void moveArms() {
        var offsetFromBase = target.position - rotPoint1.transform.position;
        var hyp = Mathf.Sqrt(Mathf.Pow(offsetFromBase.x, 2f) + Mathf.Pow(offsetFromBase.y, 2f));

        if(hyp > (l1 + l2)) {
            offsetFromBase.Normalize();
            offsetFromBase *= (l1 + l2) - .001f;
            hyp = Mathf.Sqrt(Mathf.Pow(offsetFromBase.x, 2f) + Mathf.Pow(offsetFromBase.y, 2f));
        }

        var top = hyp * hyp + l1 * l1 - l2 * l2;
        var bot = 2f * hyp * l1;
        var rot1 = Mathf.Acos(top / Mathf.Max(bot, .01f)) * Mathf.Rad2Deg;
        rotPoint1.transform.localEulerAngles = Vector3.forward * rot1;

        var top2 = l1 * l1 + l2 * l2 - hyp * hyp;
        var bot2 = 2f * l1 * l2;
        var rot2 = Mathf.Acos(top2 / Mathf.Max(bot2, .01f)) * Mathf.Rad2Deg + 180f;
        rotPoint2.transform.localEulerAngles = Vector3.forward * rot2;
    }

    void strike() {
        target.position = transform.position;
        target.DOLocalMoveX(l1 + l2, .25f).OnComplete(() => {
            target.DOLocalMoveX(0f, .5f);
        });
    }

    float getFirstLength() {
        return Vector3.Distance(rotPoint2.position, rotPoint1.position);
    }
    float getSecondLength() {
        return Vector3.Distance(rotPoint2.position, target.position);
    }
    float getLengthToTarget() {
        return Vector3.Distance(rotPoint1.position, target.position);
    }
}
