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
                // TODO: fix AI Behavior
                /*
                    ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
                    Parameter name: index
                    System.Collections.Generic.List`1[T].RemoveAt (System.Int32 index) (at <51fded79cd284d4d911c5949aff4cb21>:0)
                    AIController.OnAITurnStart () (at Assets/Scripts/AIController.cs:35)
                    System.Runtime.CompilerServices.AsyncMethodBuilderCore+<>c.<ThrowAsync>b__7_0 (System.Object state) (at <51fded79cd284d4d911c5949aff4cb21>:0)
                    UnityEngine.UnitySynchronizationContext+WorkRequest.Invoke () (at <f1e29a71158e46cdb750140a667aea01>:0)
                    UnityEngine.UnitySynchronizationContext.Exec () (at <f1e29a71158e46cdb750140a667aea01>:0)
                    UnityEngine.UnitySynchronizationContext.ExecuteTasks () (at <f1e29a71158e46cdb750140a667aea01>:0)
                 */
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
