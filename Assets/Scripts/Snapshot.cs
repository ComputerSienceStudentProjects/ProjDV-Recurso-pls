using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class CharacterData
{
    
    [SerializeField] private GameObject modelReference;
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;
    [SerializeField] private float health;
    [SerializeField] private bool hasAttacked;
    [SerializeField] private bool hasMoved;
    
    public GameObject ModelReference
    {
        get => modelReference;
        set => modelReference = value;
    }

    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    public Quaternion Rotation
    {
        get => rotation;
        set => rotation = value;
    }

    public float Health
    {
        get => health;
        set => health = value;
    }

    public bool HasAttacked
    {
        get => hasAttacked;
        set => hasAttacked = value;
    }

    public bool HasMoved
    {
        get => hasMoved;
        set => hasMoved = value;
    }

}

public enum TurnState
{
    Player,
    AI
}

public enum PlayerPhaseStatus
{
    None,
    Movement,
    Attack
}

[Serializable]
public class LevelData
{

    [SerializeField] private int levelIndex;
    [SerializeField] private int turnCount;
    [SerializeField] private TurnState turnState;
    [SerializeField] private PlayerPhaseStatus playerPhaseStatus;
    
    public int LevelIndex
    {
        get => levelIndex;
        set => levelIndex = value;
    }

    public int TurnCount
    {
        get => turnCount;
        set => turnCount = value;
    }

    public TurnState TurnState
    {
        get => turnState;
        set => turnState = value;
    }

    public PlayerPhaseStatus PlayerPhaseStatus
    {
        get => playerPhaseStatus;
        set => playerPhaseStatus = value;
    }
}

public enum SaveType
{
    Manual,
    Auto
}

[CreateAssetMenu(menuName = "Scriptables/Snapshot",fileName = "New Snapshot")]
public class Snapshot : ScriptableObject
{
    [SerializeField] private String saveName;
    [SerializeField] private SaveType saveType;
    [SerializeField] private CharacterData[] playerObjects;
    [SerializeField] private CharacterData[] aiObjects;
    [SerializeField] private LevelData levelData;
    
    public void LoadSave()
    { 
       SceneManager.LoadScene(levelData.LevelIndex);
    }

    public void PostLoad()
    {
        foreach (CharacterData playerCharacterData in playerObjects)
        {
            Instantiate(playerCharacterData.ModelReference, playerCharacterData.Position, playerCharacterData.Rotation, GameObject.Find("Player Characters").transform);
            GameObject.Find("TurnTracker").GetComponent<TurnCounter>()?.SetTurn(levelData.TurnCount);
        }
    }

    public void Save()
    {
        
    }

}
