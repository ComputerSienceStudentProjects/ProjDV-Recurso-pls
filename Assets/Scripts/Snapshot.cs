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

    public CharacterData(GameObject modelReference, Vector3 position, Quaternion rotation, float health, bool hasAttacked, bool hasMoved)
    {
        this.modelReference = modelReference;
        this.position = position;
        this.rotation = rotation;
        this.health = health;
        this.hasAttacked = hasAttacked;
        this.hasMoved = hasMoved;
    }

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
    [SerializeField] private List<CharacterData> playerObjects;
    [SerializeField] private List<CharacterData> aiObjects;
    [SerializeField] private LevelData levelData;
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject aiPrefab;
    
    
    public void LoadSave()
    { 
        SceneManager.LoadScene(levelData.LevelIndex);
    }


    public void PostLoad()
    {
        foreach (CharacterData playerCharacterData in playerObjects)
        {
            GameObject target = Instantiate(playerCharacterData.ModelReference, playerCharacterData.Position, playerCharacterData.Rotation, GameObject.Find("Player Characters").transform);
            PlayerController targetController = target.GetComponent<PlayerController>();
            targetController.SetMoved(playerCharacterData.HasMoved);
            targetController.SetAttacked(playerCharacterData.HasAttacked);
            targetController.SetHealth(playerCharacterData.Health);
            GameObject.Find("TurnTracker").GetComponent<TurnCounter>()?.SetTurn(levelData.TurnCount);
        }
        
        foreach (CharacterData aiCharacterData in aiObjects)
        {
            GameObject target = Instantiate(aiCharacterData.ModelReference, aiCharacterData.Position, aiCharacterData.Rotation, GameObject.Find("AICharacters").transform);
            AIController targetController = target.GetComponent<AIController>();
            targetController.SetMoved(aiCharacterData.HasMoved);
            targetController.SetAttacked(aiCharacterData.HasAttacked);
            targetController.SetHealth(aiCharacterData.Health);
        }

        GameObject.Find("PlayerInputSystem").GetComponent<PlayerInputSystem>().SetTurnStatus(levelData.TurnState,levelData.PlayerPhaseStatus);
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Reveal");
    }

    public void Save()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] aiGameObjects = GameObject.FindGameObjectsWithTag("AI");

        playerObjects.Clear();
        foreach (GameObject playerObject in playerGameObjects)
        {
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            CharacterData playerObjectData = new CharacterData(playerPrefab,playerObject.transform.position,playerObject.transform.rotation,playerController.GetHealth(),playerController.HasAttacked(),playerController.HasMoved());
            playerObjects.Add(playerObjectData);
        }
        
        aiObjects.Clear();
        foreach (GameObject aiObject in aiGameObjects)
        {
            AIController aiController = aiObject.GetComponent<AIController>();
            if (aiController != null)
            {
                CharacterData aiObjectData = new CharacterData(playerPrefab,aiController.transform.position,aiController.transform.rotation,aiController.GetHealth(),aiController.HasAttacked(),aiController.HasMoved());
                aiObjects.Add(aiObjectData);
            }
        }
    }

}
