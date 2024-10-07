using UnityEngine;
using System.Collections.Generic;

public class ViewRangeCol : MonoBehaviour {
    [SerializeField] float viewRange;
    List<GameObject> objs = new List<GameObject>();

    private void Awake() {
        foreach(var i in GameObject.FindGameObjectsWithTag("Environment")) {
            i.SetActive(false);
            objs.Add(i);
        }
    }

    private void LateUpdate() {
        manageEnvironment();
    }

    void manageEnvironment() {
        foreach(var i in objs) {
            bool close = Mathf.Abs(i.transform.position.x - transform.position.x) < viewRange;
            if(close && !i.activeInHierarchy) i.SetActive(true);
            else if(!close && i.activeInHierarchy) i.SetActive(false);
        }
    }
}
