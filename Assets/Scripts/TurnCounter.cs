using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    [SerializeField] private int _turn;

    public void IncreaseTurn()
    {
        _turn++;
        GameObject.Find("UI").GetComponent<MainCombatUIController>().UpdateTurnCounter();
    }

    public void SetTurn(int turn)
    {
        _turn = turn;
        GameObject.Find("UI").GetComponent<MainCombatUIController>().UpdateTurnCounter();
    }

    public int GetTurnCount()
    {
        return _turn;
    }
}
