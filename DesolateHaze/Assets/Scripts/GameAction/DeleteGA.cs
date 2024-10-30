using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteGA : GameAction
{
    [SerializeField]
    private GameObject delete;

    public override void Action()
    {
        delete.SetActive(false);
    }
}

