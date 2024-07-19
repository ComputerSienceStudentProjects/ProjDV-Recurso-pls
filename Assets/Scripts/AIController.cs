using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIController : MonoBehaviour
{

    private List<GameObject> aiParty;
    private List<GameObject> playerParty;
    [SerializeField] GameEvent EndAITurn;

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
                playerParty.RemoveAt(index);
        }

        await HandleAITurns();

        Debug.Log("Ai turn over");
        EndAITurn.Raise();
    }

    async Task HandleAITurns()
    {
        foreach (GameObject obj in aiParty)
        {
            await obj.GetComponent<AIControllable>().PerformTurn();
            Debug.Log("AI is done");
        }
    }
}
