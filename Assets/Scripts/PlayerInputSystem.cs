using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;


/**
 * <summary>
 *  Class responsible for managing Player Input
 * </summary>
 * <version>
 *  21/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public class PlayerInputSystem : MonoBehaviour
{
    [Header("Player Input System flags")]
    [SerializeField] private bool bMovementPhase = true;
    [SerializeField] private bool bAttackPhase;

    [Header("Camera controller + game events used")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameEvent aiStartTurnEvent;
    [SerializeField] private GameEvent showAttacksEvent;
    [SerializeField] private GameEvent logEvent;

    #region private fields
    private PlayerController _playerController;
    private AIControllable _aiControllable;
    private Camera _mainCamera;
    #endregion

    #region Public Methods

    /**
     * <summary>
     *  Callback for when Player turn start even is raised
     * </summary>
     */
    public void HandlePlayerStartEvent()
    {
        // Loop through each of the player party Objects
        // and reset movement flags, allowing player to move again
        // TODO: Test if the attack flag is also being properly reset
        // TODO: Test if this is actually needed, since its
        // TODO: actually reset when the player finishes the turn
        GameObject[] playerParty = GameObject.FindGameObjectsWithTag("Player");
        if (playerParty.Length < 1) SceneManager.LoadScene("GameOver");
        foreach (GameObject playerObj in playerParty)
        {
            playerObj.GetComponent<PlayerController>().ResetMovementFlag();
            playerObj.GetComponent<PlayerController>().ResetAttackFlag();
        }
        // Set the current phase for movement, since the movement phase
        // is always the first phase of each turn
        bMovementPhase = true;
        bAttackPhase = false;
    }

    /**
     * <summary>
     *  Public getter for the attack odds, to be used by the UI Controller
     * </summary>
     * <returns>
     *  Returns a float with a range of [0.0f,1.0f]
     * </returns>
     */
    public float GetOdds()
    {
        // If we somehow are trying to get odds of an unselected player
        // character or unselected target we return 0 as the hit odds
        if (_playerController == null || _aiControllable == null) return 0;
        // If everything is properly selected we calculate the odds and return them
        return CalculateAttackOdds();
    }

    /**
     * <summary>
     *  Callback responsible for allowing the user to confirm attack by clicking
     *  on the confirm attack button
     * </summary>
     */
    public void ConfirmAttack()
    {
        // Calls the private method Confirm Attack with calculated odds
        ConfirmAttack(CalculateAttackOdds());
    }

    /**
     * <summary>
     *  Defines the current turn status, including phase status, and turn ownership
     * </summary>
     * <param name="levelDataTurnState">
     *    Data structure containing the turnState, eg:
     *      Player,
     *      AI
     * </param>
     * <param name="levelDataPlayerPhaseStatus">
     *    Data structure containing the current phase, eg:
     *      None,
     *      Movement
     *      Attack
     * </param>
     */
    public void SetTurnStatus(TurnState levelDataTurnState, PlayerPhaseStatus levelDataPlayerPhaseStatus)
    {
        // Depending on the current turn owner set on levelDataTurnState,
        // we set the correct flags for the input System
        switch (levelDataTurnState)
        {
            case TurnState.AI:
                bMovementPhase = false;
                bAttackPhase = false;
                aiStartTurnEvent.Raise();
                break;
            case TurnState.Player:
                // This could be done using events, by creating an event that would take a 
                // PlayerPhaseStatus parameter that the ui controller,could have subscribed to
                GameObject.Find("UI").GetComponent<MainCombatUIController>()
                    .setPlayerOnPhase(levelDataPlayerPhaseStatus);
                switch (levelDataPlayerPhaseStatus)
                {
                    case PlayerPhaseStatus.Attack:
                        bMovementPhase = false;
                        bAttackPhase = true;
                        break;
                    case PlayerPhaseStatus.None:
                    case PlayerPhaseStatus.Movement:
                        bMovementPhase = true;
                        bAttackPhase = false;
                        break;
                }
                break;
        }
    }

    /**
     * <summary>
     *  Callback for when player clicks on the Next Phase button
     * </summary>
     * <remarks>
     *  if the player is on the attack phase we should finish the player turn,
     *  hence why we call FinishPlayerTurn if we are on the attack phase
     * </remarks>
     */
    public void FinishPlayerPhase()
    {
        switch (bMovementPhase)
        {
            case false when bAttackPhase == false:
                return;
            case true:
                bAttackPhase = true;
                bMovementPhase = false;
                _playerController = null;
                _aiControllable = null;
                break;
            default:
                FinishPlayerTurn();
                break;
        }
    }

    /**
     * <summary>
     *  Callback for when the player clicks on Finish turn button
     * </summary>
     * <remarks>
     *  This method is also called in method FinishPlayerPhase, if the player
     *  attempts to finish attack phase, making it finish the turn instead
     * </remarks>
     */
    public void FinishPlayerTurn()
    {
        bAttackPhase = false;
        bMovementPhase = false;
        _playerController?.OnDeselected();
        _playerController = null;
        _aiControllable = null;
        aiStartTurnEvent.Raise();
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerObj.GetComponent<PlayerController>().ResetMovementFlag();
            playerObj.GetComponent<PlayerController>().ResetAttackFlag();
        }
    }

    /**
     * <summary>
     *  Gets the current turn owner
     * </summary>
     * <returns>
     *  TurnState representing the turn owner
     * </returns>
     */
    public TurnState GetTurnOwner()
    {
        if (!bMovementPhase && !bAttackPhase) return TurnState.AI;
        return TurnState.Player;
    }

    /**
     * <summary>
     *  Gets the current player phase
     * </summary>
     * <returns>
     *  PlayerPhaseStatus containing the current player phase
     * </returns>
     */
    public PlayerPhaseStatus GetPlayerPhase()
    {
        if (!bMovementPhase && !bAttackPhase) return PlayerPhaseStatus.None;
        if (bMovementPhase) return PlayerPhaseStatus.Movement;
        if (bAttackPhase) return PlayerPhaseStatus.Attack;
        return PlayerPhaseStatus.None;
    }

    /**
     * <summary>
     *  Gets the base damage for the currently selected attack character
     * </summary>
     * <remarks>
     *  If either the selected player character or AI target are not set this will return -1
     * </remarks>
     * <returns>
     *  returns the character base damage
     * </returns>
     */
    public int GetDamage()
    {
        if (_playerController == null) return -1;
        if (_aiControllable == null) return -1;

        return _playerController.GetBaseDamage();
    }
    #endregion

    #region Private methods

    #region UnityDefault
    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {   
        if (bMovementPhase) HandleMovementPhaseInput();
        else if (bAttackPhase) HandleAttackPhaseInput();
    }
    #endregion

    /**
     * <summary>
     *  Private method responsible for handling player input during the attack Phase
     * </summary>
     */
    private void HandleAttackPhaseInput()
    {
        // If we didn't get a mouse down input, we returns meaning we didn't 
        // get a player input this method is handling
        if (!Input.GetMouseButtonDown(0)) return;
        // We get the vector3 representing the mouse location
        Vector3 mouseScreenPosition = Input.mousePosition;
        // If the player didn't click on a valid target returns to stop execution
        // If the player clicked on a valid target, we expose a new var called hit
        // containing the raycast resulting hit information
        if (!Physics.Raycast(_mainCamera.ScreenPointToRay(mouseScreenPosition), out var hit)) return;
        // We try to obtain a AIControllable Component on the clicked gameObject
        AIControllable aiTarget = hit.collider.GetComponent<AIControllable>();
        // If we got an AIControllable we check if the player already
        // selected a character to perform the attack with
        if (aiTarget != null)
        {
            // If we didnt select a character to attack with returns
            if (_playerController == null) return;
            // Otherwise, we set the target to the selected AIControllable
            _aiControllable = hit.collider.GetComponent<AIControllable>();
            // We then raise an event telling the UI to show the attackList
            showAttacksEvent.Raise();
            return;
        }
        // If we didn't click on an AIControllable, we obtain a possible
        // PlayerController character to use as selected attacking character
        PlayerController playerController = hit.collider.GetComponent<PlayerController>();
        // Should only handle player Select if we got a PlayerController
        if (playerController != null) HandlePlayerSelect(playerController);
        cameraController.UnlockOnGameObject();
    }

    /**
     * <summary>
     *  Private method responsible for handling movement Input
     * </summary>
     */
    private void HandleMovementPhaseInput()
    {
        // If we didn't get a mouse down input, we returns meaning we didn't 
        // get a player input this method is handling
        if (Input.GetMouseButtonDown(0))
        {
            // We get the mouse screen position
            Vector3 mouseScreenPosition = Input.mousePosition;
            // We test if the player clicked on a gameObject, and expose a new var
            // called hit containing the resulting HitInfo
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(mouseScreenPosition), out var hit))
            {
                // We Attempt to get The playerController component, to select
                PlayerController playerController = hit.collider.GetComponent<PlayerController>();
                // If we got a PlayerController component we handle the playerSelect
                if (playerController != null) HandlePlayerSelect(playerController);
                // If not we handle MoveInput, since we know we clicked on a valid GameObject
                if (playerController == null) HandleMoveInput(hit);
            }
            cameraController.UnlockOnGameObject();
        }
    }

    /**
     * <summary>
     *  Handler for the movement Input
     * </summary>
     * <param name="raycastHit">
     *  RaycastHit containing the hit to attempt to move towards the point
     * </param>
     */
    private void HandleMoveInput(RaycastHit raycastHit)
    {
        // We call PlayerController Perform Move, providing the
        // resulting hit point on the raycastHit
        _playerController?.PerformMove(raycastHit.point);
        // After that we deselect the character
        _playerController?.OnDeselected();
        // We also null the PlayerController reference
        _playerController = null;
    }

    /**
     * <summary>
     *  Handles player select intent
     * </summary>
     * <param name="playerController">
     *  PlayerController attempted to handle select input at
     * </param>
     */
    private void HandlePlayerSelect(PlayerController playerController)
    {
        // If we are handling select on the currently selected
        // character we should deselect instead of selecting
        if (_playerController == playerController)
        {
            playerController.OnDeselected();
            _playerController = null;
            _aiControllable = null;
            // We unlock the camera so the player can move it around
            cameraController.UnlockOnGameObject();
            return;
        }
        // Since we are not trying to select the currently
        // selected PlayerCharacter we select it, and
        // lock the camera to the GameObject
        playerController.OnSelected();
        _playerController?.OnDeselected();
        _playerController = playerController;
        cameraController.LockOnGameObject(_playerController.gameObject);
    }

    /**
     * <summary>
     *  Private Method for confirming an attack
     * </summary>
     * <param name="attackOdds">
     *  Attack odds to calculate againt 
     * </param>
     */
    private void ConfirmAttack(float attackOdds)
    {
        // We get a random value from [0.0f,1.0f]
        float randomValue = Random.value;
        // If the attackOdds are greater than the random value
        // confirm the attack was a success
        // TODO: Raise the attackLog event
        string log;
        if (_playerController.HasAttacked())
        {
            log = string.Format("{0} has already attacked, ignoring", _playerController.gameObject.name);
            logEvent.SetArgument("text", typeof(string), string.Format("{0}", log));
            logEvent.Raise();
            return;
        }
        _playerController.PlayAttackAnim(_aiControllable.GetPosition(), _aiControllable, attackOdds > randomValue);
        log = string.Format("Player used {0} to attack {1}, {2}", _playerController.gameObject.name, _aiControllable.gameObject.name, attackOdds > randomValue ? "Sucess" : "Failed");
        logEvent.SetArgument("text", typeof(string), string.Format("{0}, dealing {1} damage", log, (attackOdds > randomValue) ? _playerController.GetBaseDamage().ToString() : "0"));
        logEvent.Raise();
        _playerController.OnDeselected();
        _playerController = null;
        _aiControllable = null;
        cameraController.UnlockOnGameObject();
    }

    /**
     * <summary>
     *  Private method for calculating the attack Odds
     * </summary>
     * <returns>
     *  Float representing the attackOdds with the range of [0.0f,1.0f]
     * </returns>
     */
    private float CalculateAttackOdds()
    {
        // We set the initialOdds to 0, at the start of calculation
        float initialOdds = 0;
        float minDistance = float.MaxValue;
        Vector3 aiPos = _aiControllable.GetPosition();

        // We get a direct LineCast from the player Character
        // to the AI target GameObjct
        if (Physics.Linecast(_playerController.GetCastPoint(), aiPos, out var hitInfo))
        {
            // If we did hit an AI GameObject, set the odds to 1f
            // we also save the hit distance
            if (hitInfo.collider.GetComponent<AIControllable>() == _aiControllable)
            {
                initialOdds = 1f;
                minDistance = hitInfo.distance;
            }
        }

        // we check if the distance is > than the max character Range
        // If so we set the odds to 0f
        if (minDistance > _playerController.GetMaxRange())
        {
            initialOdds = 0;
        }
        else
        {
            // If it's within the range, we calculate a distanceFactor
            // to be used to calculate attack odds based on distance
            float maxRange = _playerController.GetMaxRange();
            float distanceFactor = 1 - (minDistance / maxRange);
            initialOdds *= distanceFactor;
        }
        return initialOdds;
    }
    #endregion

    public bool isPlayerPlaying()
    {
        return bMovementPhase || bAttackPhase;
    }

    public void SetPlayerControllerReference(PlayerController reference)
    {
        this._playerController = reference;
    }
}
