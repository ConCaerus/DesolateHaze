using UnityEngine;
using System.Collections.Generic;

public class ViewRangeCol : MonoBehaviour {
    [SerializeField] float viewRange;
    List<MeshRenderer> envRenderers = new List<MeshRenderer>();

    private void Awake() {
        foreach(var i in GameObject.FindGameObjectsWithTag("Environment")) {
            if(i.TryGetComponent<MeshRenderer>(out var m)) {
                m.enabled = false;
                envRenderers.Add(m);
            }
        }
    }

    private void LateUpdate() {
        manageEnvironment();
    }

    void manageEnvironment() {
        foreach(var i in envRenderers) {
            bool close = Mathf.Abs(i.transform.position.x - transform.position.x) < viewRange;
            if(close && !i.enabled) i.enabled = true;
            else if(!close && i.enabled) i.enabled = false;
        }
    }
}
