using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * <summary>
 *
 * </summary>
 * <version>
 *  <date here>/07/2024
 * </version>
 * <author>
 *  Diogo Capela (<Email here>)
 * </author>
 */
public class AIController : MonoBehaviour
{

    private List<GameObject> aiParty;
    private List<GameObject> playerParty;
    [SerializeField] GameEvent EndAITurn;

    /**
         * <summary>
         *  Waits for AI Event to be triggered and initiates turn logic for ai characters
         * </summary>
         */
    public async void OnAITurnStart()
    {
        Debug.Log("AI TURN STARTED");
        aiParty = new List<GameObject>(GameObject.FindGameObjectsWithTag("AI"));
        playerParty = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        //each ai enemy finds the closest player to designate as a target; probably change later when abilities work;
        foreach (GameObject obj in aiParty)
        {
            int index = obj.GetComponent<AIControllable>().FindClosestTarget(playerParty);
            if (playerParty.Count > 1)
            {
                if (index >= 0 && index < playerParty.Count)
                {
                    Debug.Log(index + "," + playerParty.Count);
                    playerParty.RemoveAt(index);
                }
            }
        }

        await HandleAITurns();

        Debug.Log("Ai turn over");
        EndAITurn.Raise();
    }

    /**
         * <summary>
         *  Awaits for every AI character to finish its turn before starting the player turn event.
         * </summary>
         */
    async Task HandleAITurns()
    {
        foreach (GameObject obj in aiParty)
        {
            await obj.GetComponent<AIControllable>().PerformTurn();
            Debug.Log("AI is done");
        }
    }
}
