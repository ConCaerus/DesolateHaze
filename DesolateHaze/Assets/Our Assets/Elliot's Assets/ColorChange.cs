using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] Material Red;
    [SerializeField] Material Green;
    [SerializeField] GameObject self;
    bool off;

    private void Start()
    {
        self.GetComponent<MeshRenderer>().material = Red;
        off = true;
    }

    public void toggleColor()
    {
        if (off)
        {
            self.GetComponent<MeshRenderer>().material = Green;
            off = false;
        }
        else
        {
            self.GetComponent<MeshRenderer>().material = Red;
            off = true;
        }
    }
}
