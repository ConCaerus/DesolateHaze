using UnityEngine;

public class ControlSchemeManager : Singleton<ControlSchemeManager> {
    InputMaster controls;

    bool initted = false;

    bool kb = false;
    public bool usingKeyboard {
        get { return kb; }
        private set {
            if(kb == value && initted) return;
            if(!initted) initted = true;
            kb = value;
            runOnChange(kb);
        }
    }

    public static System.Action<bool> runOnChange = (keyb) => { };

    private void Start() {
        controls = new InputMaster();
        controls.Enable();
        controls.Scheme.Keyboard.performed += ctx => { usingKeyboard = true; };
        controls.Scheme.Gamepad.performed += ctx => { usingKeyboard = false; };
    }

    private void OnDisable() {
        controls.Disable();
        runOnChange = (b) => { };
    }
}
