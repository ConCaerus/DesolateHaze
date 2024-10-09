using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : Singleton<PlayerInteraction> {
    [SerializeField] float interactDist;

    InteractableInstance curInteractable;

    InputMaster controls;

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Player.Interact.performed += ctx => { if(curInteractable != null) curInteractable.trigger(); };
    }

    public void setCurInteractable(InteractableInstance ii) {
        curInteractable = ii;
    }
}
