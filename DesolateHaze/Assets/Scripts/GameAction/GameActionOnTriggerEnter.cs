using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameActionOnTriggerEnter : MonoBehaviour
{
    [Tooltip("Is the game action enabled?"), SerializeField]
    private bool isEnabled = true;
    [Tooltip("List of game actions to run when triggered"), SerializeField]
    private List<GameAction> gActions;

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameAction ga in gActions)
        {
            ga.Action();
        }
    }
    public void PlaySequence()
    {
        StartCoroutine(nameof(TriggerSequence));
    }
    IEnumerator TriggerSequence()
    {
        foreach (GameAction ga in gActions)
        {
            yield return new WaitForSeconds(ga.delay);
            ga.Action();
        }
    }
}