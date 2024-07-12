using UnityEngine;

/**
 * <summary>
 *  This class is responsible for keeping track of the turn counter
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 * João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public class TurnCounter : MonoBehaviour
{
    /**
     * <summary>
     *  Integer that represents the current run
     * </summary>
     */
    [SerializeField] private int _turn;

    /**
     * <summary>
     *  Method for increasing the turn count
     * </summary>
     */
    public void IncreaseTurn()
    {
        _turn++;
        GameObject.Find("UI").GetComponent<MainCombatUIController>().UpdateTurnCounter();
    }

    /**
     * <summary>
     *  Method for setting the turn to a fixed value, can be used to set turn count loaded from save
     * </summary>
     */
    public void SetTurn(int turn)
    {
        _turn = turn;
        GameObject.Find("UI").GetComponent<MainCombatUIController>().UpdateTurnCounter();
    }

    /**
     * <summary>
     *  Getter for the turn count
     * </summary>
     */
    public int GetTurnCount()
    {
        return _turn;
    }
}
