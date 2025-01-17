using UnityEngine;
using UnityEngine.Events;

public class CustomButtonInformation : MonoBehaviour {
    public float normalSize = 1f, hoverSize = 1.5f;
    public bool playSound = true;

    public UnityEvent hoverEvents, dehoverEvents, clickEvents;
}
