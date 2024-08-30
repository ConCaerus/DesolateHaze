using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : Singleton<PlayerInteraction> {
    [HideInInspector] public List<InteractableInstance> interactables = new List<InteractableInstance>();
    [SerializeField] float interactDist;

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Interact.performed += ctx => checkInteractions();
    }
    private void OnDisable() {
        interactables.Clear();
    }

    void checkInteractions() {
        var curInteractable = getCurInteractable();
        if(curInteractable == null || Vector2.Distance(transform.position, curInteractable.transform.position) > interactDist) return;

        //  changes movement if needs to change movement
        switch(curInteractable.gameObject.tag) {
            case "Ladder": PlayerMovement.I.climbLadder(); break;
            case "Rope": PlayerMovement.I.climbRope(curInteractable.GetComponentInParent<RopeInstance>()); break;
        }

        //  triggers a sequence if there is one
        if(curInteractable.TryGetComponent<InteractableInstance>(out var ii))
            ii.trigger();
    }

    public Transform getCurInteractable() {
        InteractableInstance temp = null;
        float dist = -1f;
        foreach(var i in interactables) {
            var newDist = Vector3.Distance(transform.position, i.transform.position);
            if(dist == -1f || newDist < dist) {
                dist = newDist;
                temp = i;
            }
        }
        return temp == null ? null : temp.transform;
    }
}
