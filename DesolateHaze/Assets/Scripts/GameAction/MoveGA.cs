using UnityEngine;
using UnityEngine.VFX;

public class MoveGA : GameAction
{
    [SerializeField]
    private Transform VFX;
    [SerializeField]
    private Transform LMine;

    public override void Action()
    {
        VFX.position = LMine.position;
    }
}