using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : Singleton<PlayerInteraction> {
    [SerializeField] float interactDist;
    bool interacting = false;

    InteractableInstance curInteractable;

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Interact.performed += ctx => {
            if(curInteractable != null) {
                if(!interacting)
                    curInteractable.trigger();
                else
                    curInteractable.detrigger();
                interacting = !interacting;
            }
        };
    }

    public void setCurInteractable(InteractableInstance ii) {
        curInteractable = ii;
    }
    public InteractableInstance getCurInteractable() {
        return curInteractable;
    }
}
