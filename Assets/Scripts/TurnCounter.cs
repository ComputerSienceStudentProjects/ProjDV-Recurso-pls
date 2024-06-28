using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCounter : MonoBehaviour
{
    private int _turn;

    public void IncreaseTurn()
    {
        _turn++;
    }

    public void SetTurn(int turn)
    {
        _turn = turn;
    }
}
