using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * <summary>
 *  Class CharacterData is a representation of the state of a character in-game, allowing the save, and load mid-game.
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
[Serializable]
public class CharacterData
{
    /**
     * <summary>
     *  Reference model / prefab for the character
     * </summary>
     */
    [SerializeField] private GameObject modelReference;
    /**
     * <summary>
     *  A Vector3 class representing the position of the character
     * </summary>
     * <remarks>
     *  If the position Vector3 is exactly the same as Vector3.Zero(0,0,0), it indicates the character must be spawned in the level spawnpoints
     * </remarks>
     */
    [SerializeField] private Vector3 position;
    /**
     * <summary>
     *  Quaternion representing the rotation in 3d Space of the character Object
     * </summary>
     */
    [SerializeField] private Quaternion rotation;
    /**
     * <summary>
     *  Float representing the current health of the character
     * </summary>
     */
    [SerializeField] private float health;
    /**
     * <summary>
     *  Int representing the base damage for the character
     * </summary>
     */
    [SerializeField] private int baseDMG;
    /**
     * <summary>
     *  Boolean indicating the current state of the hasAttacked flag of the character
     * </summary>
     */
    [SerializeField] private bool hasAttacked;
    /**
     * <summary>
     *  Boolean indicating the current state of the hasMoved flag of the character
     * </summary>
     */
    [SerializeField] private bool hasMoved;

    /**
     * <summary>
     *  Base constructor for CharacterData
     * </summary>
     */
    public CharacterData(GameObject modelReference, Vector3 position, Quaternion rotation, float health, bool hasAttacked, bool hasMoved, int baseDMG)
    {
        this.modelReference = modelReference;
        this.position = position;
        this.rotation = rotation;
        this.health = health;
        this.hasAttacked = hasAttacked;
        this.hasMoved = hasMoved;
        this.baseDMG = baseDMG;
    }

    /**
     * <summary>
     *  Public get/set for ModelReference
     * </summary>
     */
    public GameObject ModelReference
    {
        get => modelReference;
        set => modelReference = value;
    }

    /**
     * <summary>
     *  Public get/set for BaseDamage
     * </summary>
     */
    public int BaseDamage
    {
        get => baseDMG;
        set => baseDMG = value;
    }

    /**
     * <summary>
     *  Public get/set for Position
     * </summary>
     */
    public Vector3 Position
    {
        get => position;
        set => position = value;
    }

    /**
     * <summary>
     *  Public get/set for Rotation
     * </summary>
     */
    public Quaternion Rotation
    {
        get => rotation;
        set => rotation = value;

    }

    /**
     * <summary>
     *  Public get/set for Health
     * </summary>
     */
    public float Health
    {
        get => health;
        set => health = value;
    }

    /**
     * <summary>
     *  Public get/set for HasAttacked
     * </summary>
     */
    public bool HasAttacked
    {
        get => hasAttacked;
        set => hasAttacked = value;
    }

    /**
     * <summary>
     *  Public get/set for HasMoved
     * </summary>
     */
    public bool HasMoved
    {
        get => hasMoved;
        set => hasMoved = value;
    }

}

/**
 * <summary>
 *  Enum representing the current turn state/owner
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public enum TurnState
{
    /**
     * <summary>
     *  Player is the current turn owner
     * </summary>
     */
    Player,
    /**
     * <summary>
     *  AI is the current turn owner
     * </summary>
     */
    AI
}

/**
 * <summary>
 *  Enum representing the current player phase
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public enum PlayerPhaseStatus
{
    /**
     * <summary>
     *  Player is not owner of the turn therefore it's on a empty phase
     * </summary>
     */
    None,
    /**
     * <summary>
     *  Player is currently on Movement Phase
     * </summary>
     */
    Movement,
    /**
     * <summary>
     *  Player is currently on Attack Phase
     * </summary>
     */
    Attack
}

/**
 * <summary>
 *  Class representing the level data of the save
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
[Serializable]
public class LevelData
{

    /**
     * <summary>
     *  Current save levelIndex starting from 1, since 0 is the Menu
     * </summary>
     */
    [SerializeField] private int levelIndex;
    /**
     * <summary>
     *  Stores the turnCount of the current level
     * </summary>
     */
    [SerializeField] private int turnCount;
    /**
     * <summary>
     *  Stores the turnState for the current level
     * </summary>
     */
    [SerializeField] private TurnState turnState;
    /**
     * <summary>
     *  Stores the playerPhaseStatus for the current level
     * </summary>
     */
    [SerializeField] private PlayerPhaseStatus playerPhaseStatus;


    /**
     * <summary>
     * Public Getter/Setter forLevelIndex
     * </summary>
     */
    public int LevelIndex
    {
        get => levelIndex;
        set => levelIndex = value;
    }

    /**
     * <summary>
     * Public Getter/Setter TurnCount
     * </summary>
     */
    public int TurnCount
    {
        get => turnCount;
        set => turnCount = value;
    }

    /**
     * <summary>
     * Public Getter/Setter TurnState
     * </summary>
     */
    public TurnState TurnState
    {
        get => turnState;
        set => turnState = value;
    }

    /**
     * <summary>
     * Public Getter/Setter PlayerPhaseStatus
     * </summary>
     */
    public PlayerPhaseStatus PlayerPhaseStatus
    {
        get => playerPhaseStatus;
        set => playerPhaseStatus = value;
    }
}

/**
 * <summary>
 *  Enum representing the type of save
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public enum SaveType
{
    /**
     * <summary>
     *  Manual save
     * </summary>
     */
    Manual,
    /**
     * <summary>
     *  Automatic save
     * </summary>
     */
    Auto
}


/**
 * <summary>
 *  ScriptableObject representing a snapshot/save state
 * </summary>
 * <remarks>
 *  The save system follows the following life cycle
 *  Load -> PostLoad
 *  Save
 * </remarks>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
[CreateAssetMenu(menuName = "Scriptables/Snapshot", fileName = "New Snapshot")]
public class Snapshot : ScriptableObject
{
    /**
     * <summary>
     *  Field containing the save name
     * </summary>
     */
    [SerializeField] private String saveName;

    /**
     * <summary>
     *  Field containing the save type
     * </summary>
     */
    [SerializeField] private SaveType saveType;

    /**
     * <summary>
     *  List of all player characterData saved in the snapshot / savestate
     * </summary>
     */
    [SerializeField] private List<CharacterData> playerObjects;

    /**
     * <summary>
     *  List of all AI characterData saved in the snapshot / savestate
     * </summary>
     */
    [SerializeField] private List<CharacterData> aiObjects;

    /**
     * <summary>
     *  Field containing the level data 
     * </summary>
     */
    [SerializeField] private LevelData levelData;

    /**
     * <summary>
     *  Field containing the playtime of the save
     * </summary>
     */
    [SerializeField] private float playTime;

    /**
     * <summary>
     *  Field containing the last time the save was played
     * </summary>
     */
    [SerializeField] private float lastPlayed;

    /**
     * <summary>
     *  Field containing the last time the save was saved
     * </summary>
     */
    [SerializeField] private float lasSaved;

    /**
     * Event for updating HpBars of the ui,to be called during post Load
     */
    [SerializeField] private GameEvent updateHpBarsEvent;

    /**
     * <summary>
     *  prefab for the player characters
     * </summary>
     */
    [SerializeField] private GameObject playerPrefab;

    /**
     * <summary>
     *  prefab for the AI characters
     * </summary>
     */
    [SerializeField] private GameObject aiPrefab;

    /**
     * <summary>
     *  First Step of loading the save, is to load into the scene the player is at
     * </summary>
     */
    public void LoadSave()
    {
        SceneManager.LoadScene(levelData.LevelIndex);
    }

    /**
     * <summary>
     *  Method post load is responsible for spawning all entities on the loaded level
     * </summary>
     */
    public void PostLoad()
    {
        //Go through each player character Data for spawning
        foreach (CharacterData playerCharacterData in playerObjects)
        {
            // Instantiates a player GameObject,either using all data from the save
            // or if the character position if Vector3.Zero, it calls SpawnNewEntity
            // which will return the created GameObject on the spawn points
            GameObject target = (playerCharacterData.Position == Vector3.zero) ? SpawnNewEntity(playerCharacterData, false) : Instantiate(playerCharacterData.ModelReference, playerCharacterData.Position, playerCharacterData.Rotation, GameObject.Find("Player Characters").transform);
            //After GameObject Creation we obtain the component PlayerController
            PlayerController targetController = target.GetComponent<PlayerController>();
            // We set all data for the PlayerController
            targetController.SetMoved(playerCharacterData.HasMoved);
            targetController.SetAttacked(playerCharacterData.HasAttacked);
            targetController.SetHealth(playerCharacterData.Health);
            targetController.SetBaseDMG(playerCharacterData.BaseDamage);
        }

        //Go through each player character Data for spawning
        foreach (CharacterData aiCharacterData in aiObjects)
        {
            // Instantiates a AI GameObject,either using all data from the save
            // or if the character position if Vector3.Zero, it calls SpawnNewEntity
            // which will return the created GameObject on the spawn points
            GameObject target = (aiCharacterData.Position == Vector3.zero) ? SpawnNewEntity(aiCharacterData, true) : Instantiate(aiCharacterData.ModelReference, aiCharacterData.Position, aiCharacterData.Rotation, GameObject.Find("AICharacters").transform);
            //After GameObject Creation we obtain the component AIController
            AIControllable targetController = target.GetComponent<AIControllable>();
            // We set all data for the AIController
            targetController.SetMoved(aiCharacterData.HasMoved);
            targetController.SetAttacked(aiCharacterData.HasAttacked);
            targetController.SetHealth(aiCharacterData.Health);
            targetController.setBaseDmg(aiCharacterData.BaseDamage);
        }
        // After all objects are spawned, we update global trackers such as the turn tracker
        // the status for the PlayerInputSystem
        GameObject.Find("TurnTracker").GetComponent<TurnCounter>()?.SetTurn(levelData.TurnCount);
        GameObject.Find("PlayerInputSystem").GetComponent<PlayerInputSystem>().SetTurnStatus(levelData.TurnState, levelData.PlayerPhaseStatus);
        //Trigger the reveal animation since we are done loading
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Reveal");
        //Raise the UpdateHpBars Event for the UI to update the hp bars
        updateHpBarsEvent?.Raise();

        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("AISpawnPoints"))
        {
          if (spawnPoint.transform.childCount == 0) continue;
          spawnPoint.transform.GetChild(0).transform.parent = GameObject.Find("AICharacters").transform;
        }
        
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag("PlayerSpawnPoints"))
        {
         if (spawnPoint.transform.childCount == 0) continue;
         spawnPoint.transform.GetChild(0).transform.parent = GameObject.Find("Player Characters").transform;
        }
    }

    /**
     * <summary>
     * Method responsible for spawning an entity on an available Spawn point
     * </summary>
     */
    private GameObject SpawnNewEntity(CharacterData data, bool isAI)
    {
        // Get the spawn points tagged with SpawnPointAI if isAI is true, or SpawnPointPlayer if isAI is false
        foreach (GameObject spawnPoint in GameObject.FindGameObjectsWithTag(isAI ? "AISpawnPoints" : "PlayerSpawnPoints"))
        {
            //Finds the first spawnPoint With no children to spawn under
            if (spawnPoint.transform.childCount == 0)
            {
                //returns the spawned GameObject with the character Data
                if (isAI)
                {
                    GameObject ai = Instantiate(data.ModelReference, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
                    AIControllable aIControllable = ai.GetComponent<AIControllable>();
                    aIControllable.ResetHealth();
                    data.Health = aIControllable.GetHealth();
                    return ai;
                }
                return Instantiate(data.ModelReference, spawnPoint.transform.position, Quaternion.identity, spawnPoint.transform);
            }
        }
        //if for some reason we cant find a place to spawn the entity, return null as not spawned
        return null;
    }

    /**
     * <summary>
     * Method for saving the data on the snapshot / save state
     * </summary>
     */
    public void Save()
    {
        //Gets all GameObjects tagged with Player, and AI
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] aiGameObjects = GameObject.FindGameObjectsWithTag("AI");

        //clears the current state of the playerObjects list
        playerObjects.Clear();
        //Loops trough each player GameObjects found
        foreach (GameObject playerObject in playerGameObjects)
        {
            //Obtaining the PlayerController
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            //Creating a new CharacterData for the character
            CharacterData playerObjectData = new CharacterData(playerPrefab, playerObject.transform.position,
             playerObject.transform.rotation, playerController.GetHealth(), playerController.HasAttacked(),
             playerController.HasMoved(), playerController.GetBaseDamage());
            //Add the new character data to the playerObjects list
            playerObjects.Add(playerObjectData);
        }

        //clears the current state of the aiObjects list
        aiObjects.Clear();
        //Loops trough each AI GameObjects found
        foreach (GameObject aiObject in aiGameObjects)
        {
            //Obtaining the AIControllable
            AIControllable aiController = aiObject.GetComponent<AIControllable>();
            //Creating a new CharacterData for the character
            CharacterData aiObjectData = new CharacterData(aiPrefab, aiController.transform.position,
                aiController.transform.rotation, aiController.GetHealth(), aiController.HasAttacked(),
                aiController.HasMoved(), aiController.getBaseDMG());
            //Add the new character data to the aiObjects list
            aiObjects.Add(aiObjectData);
        }

        //Saving the current Level data
        TurnCounter turnCounter = GameObject.Find("TurnTracker").GetComponent<TurnCounter>();
        PlayerInputSystem playerInputSystem = GameObject.Find("PlayerInputSystem").GetComponent<PlayerInputSystem>();
        levelData.LevelIndex = SceneManager.GetActiveScene().buildIndex;
        levelData.TurnState = playerInputSystem.GetTurnOwner();
        levelData.PlayerPhaseStatus = playerInputSystem.GetPlayerPhase();
        levelData.TurnCount = turnCounter.GetTurnCount();
    }

    /**
     * <summary>
     *  Method responsible for resetting the save state
     * </summary>
     */
    public void ClearSnapshot()
    {
        levelData.LevelIndex = 1;
        levelData.TurnState = TurnState.Player;
        levelData.PlayerPhaseStatus = PlayerPhaseStatus.Movement;
        ResetEntities();
    }

    /**
     * <summary>
     * Method responsible for resetting the character data either for new save or moving to a new level
     * </summary>
     */
    public void ResetEntities()
    {
        foreach (CharacterData characterData in playerObjects)
        {
            characterData.HasAttacked = false;
            characterData.HasMoved = false;
            characterData.Health = 10;
            characterData.Position = Vector3.zero;
            characterData.Rotation = Quaternion.identity;
        }

        foreach (CharacterData aiCharacterData in aiObjects)
        {
            aiCharacterData.HasAttacked = false;
            aiCharacterData.HasMoved = false;
            aiCharacterData.Health = 10;
            aiCharacterData.Position = Vector3.zero;
            aiCharacterData.Rotation = Quaternion.identity;
        }
    }
}
