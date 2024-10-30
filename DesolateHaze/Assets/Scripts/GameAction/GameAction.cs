using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAction : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The delay before the game action executes")]
    public float delay = 0;

    //note: virtual = allow to be overwritten by child
    public virtual void Action()
    {
    }
    public virtual void DeAction()
    {
    }
    public virtual void ResetToDefaults()
    {
    }
}