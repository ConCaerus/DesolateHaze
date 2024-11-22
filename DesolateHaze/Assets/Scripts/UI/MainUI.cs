using UnityEngine;

public class MainUI : Singleton<MainUI> {
    [SerializeField] float growSize;

    public void grow(Transform t) {
        t.localScale = Vector3.one * growSize;
    }
    public void shrink(Transform t) {
        t.localScale = Vector3.one;
    }
}
