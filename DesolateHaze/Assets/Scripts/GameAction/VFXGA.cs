using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGA : GameAction
{
    [SerializeField]
    private VisualEffect vfx;

    public override void Action()
    {
        vfx.Play();
    }
}