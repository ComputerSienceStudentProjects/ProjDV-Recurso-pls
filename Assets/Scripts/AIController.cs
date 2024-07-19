using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    private List<GameObject> aiParty;
    private List<GameObject> playerParty;
    [SerializeField] GameEvent EndAITurn;
    [SerializeField] GameEvent PlayerTurnStart;

    public void OnAITurnStart()
    {
        Debug.Log("AI TURN STARTED");
    }
}
