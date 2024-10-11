using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[ExecuteInEditMode]

public class RandomTranslate : MonoBehaviour
{
    private int timer;
    [SerializeField]
    private List<Transform> Groupign;
    [SerializeField]
    private bool IsParent;
    [SerializeField]
    private bool LockPosX, LockPosY, LockPosZ;
    [SerializeField]
    private bool LockRotX, LockRotY, LockRotZ;
    [SerializeField]
    private int MinRange, MaxRange;

    private float Min, Max;

    public void Starto()
    {
        Min = (float)MinRange;
        Max = (float)MaxRange;

        timer = Random.Range(4, 20);

        if (IsParent)
            PRando();
        else
            RANDO();
    }

    void PRando()
    {
        foreach (Transform t in Groupign)
        {
            Vector3 pos = new Vector3();

            for (int i = 0; i < timer; i++)
            {
                pos = Vector3.zero;

                if (!LockPosX)
                    pos.x = Random.Range(Min, Max);
                if (!LockPosY)
                    pos.y = Random.Range(Min, Max);
                if (!LockPosZ)
                    pos.z = Random.Range(Min, Max);

                Undo.RecordObject(t,"Undo Translation");
                t.position += pos;
            }
        }
    }

    void RANDO()
    {
        Vector3 pos = new Vector3();

        for (int i = 0; i < timer; i++)
        {
            pos = Vector3.zero;

            if(!LockPosX)
                pos.x = Random.Range(Min, Max);
            if (!LockPosY)
                pos.y = Random.Range(Min, Max);
            if (!LockPosZ)
                pos.z = Random.Range(Min, Max);

            Undo.RecordObject(transform, "Undo Translation");
            transform.position += pos;
        }
    }

    public void Rotato()
    {
        Min = (float)MinRange;
        Max = (float)MaxRange;

        timer = Random.Range(4, 20);

        if (IsParent)
            PRoto();
        else
            ROTO();
    }

    void PRoto()
    {
        foreach (Transform t in Groupign)
        {
            Vector3 Rototo = new Vector3();

            for (int i = 0; i < timer; i++)
            {
                Rototo = t.rotation.eulerAngles;

                if (!LockRotX)
                    Rototo.x = Random.Range(Min, Max);
                if (!LockRotY)
                    Rototo.y = Random.Range(Min, Max);
                if (!LockRotZ)
                    Rototo.z = Random.Range(Min, Max);

                Quaternion QU;
                QU = Quaternion.Euler(Rototo);

                Undo.RecordObject(t, "Undo Rotation");
                t.rotation = QU;
            }
        }
    }

    void ROTO()
    {
        Vector3 Rototo = new Vector3();

        for (int i = 0; i < timer; i++)
        {
            Rototo = transform.rotation.eulerAngles;

            if(!LockRotX)
                Rototo.x = Random.Range(Min, Max);
            if (!LockRotY)
                Rototo.y = Random.Range(Min, Max);
            if (!LockRotZ)
                Rototo.z = Random.Range(Min, Max);
            
            Quaternion QU;
            QU = Quaternion.Euler(Rototo);

            Undo.RecordObject(transform, "Undo Rotation");
            transform.rotation = QU;
        }
    }
}

[CustomEditor(typeof(RandomTranslate))]
public class RandTransCI : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        RandomTranslate Button = (RandomTranslate)target;

        if (GUILayout.Button("Random"))
            Button.Starto();

        if (GUILayout.Button("Rotate"))
            Button.Rotato();

    }
}
