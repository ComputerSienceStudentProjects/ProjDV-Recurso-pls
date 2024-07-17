using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInputSystem : MonoBehaviour
{
    private PlayerController _playerController;
    private AIControllable _aiControllable;
    [SerializeField] private bool bMovementPhase = true;
    [SerializeField] private bool bAttackPhase;
    private Camera _mainCamera;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameEvent aiStartTurnEvent;
    [SerializeField] private GameEvent showAttacksEvent;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (bMovementPhase) HandleMovementPhaseInput();
        if (bAttackPhase) HandleAttackPhaseInput();
    }

    private void HandleAttackPhaseInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector3 mouseScreenPosition = Input.mousePosition;
        if (!Physics.Raycast(_mainCamera.ScreenPointToRay(mouseScreenPosition), out var hit)) return;
        AIControllable aiTarget = hit.collider.GetComponent<AIControllable>();
        Debug.Log(hit.collider.gameObject);
        if (aiTarget != null)
        {
            if (_playerController == null) return;
            _aiControllable = hit.collider.GetComponent<AIControllable>();
            showAttacksEvent.Raise();
            return;
        }
        PlayerController playerController = hit.collider.GetComponent<PlayerController>();
        if (playerController != null) HandlePlayerSelect(playerController);
    }

    public void HandlePlayerStartEvent()
    {
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerObj.GetComponent<PlayerController>().ResetMovementFlag();
        }
        bMovementPhase = true;
        bAttackPhase = false;
    }

    private void HandleMovementPhaseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(mouseScreenPosition), out var hit))
            {
                PlayerController playerController = hit.collider.GetComponent<PlayerController>();
                if (playerController != null) HandlePlayerSelect(playerController);
                if (playerController == null) HandleMoveInput(hit);
            }
        }
    }

    private void HandleMoveInput(RaycastHit raycastHit)
    {
        _playerController?.PerformMove(raycastHit.point);
        _playerController?.OnDeselected();
        _playerController = null;
    }

    private void HandlePlayerSelect(PlayerController playerController)
    {

        if (_playerController == playerController)
        {
            playerController.OnDeselected();
            _playerController = null;
            _aiControllable = null;
            cameraController.UnlockOnGameObject();
            return;
        }

        playerController.OnSelected();
        _playerController?.OnDeselected();
        _playerController = playerController;
        cameraController.LockOnGameObject(_playerController.gameObject);
    }


    public float GetOdds()
    {
        if (_playerController == null || _aiControllable == null) return 0;
        return CalculateAttackOdds();
    }

    public void ConfirmAttack()
    {
        ConfirmAttack(CalculateAttackOdds());
    }

    private void ConfirmAttack(float attackOdds)
    {
        float randomValue = Random.value;
        if (attackOdds > randomValue)
        {
            _playerController.PlayAttackAnim(_aiControllable.GetPosition(), _aiControllable, true);
            _playerController.OnDeselected();
            _playerController = null;
            _aiControllable = null;
        }
        else
        {
            _playerController.PlayAttackAnim(_aiControllable.GetPosition(), _aiControllable, false);
            _playerController.OnDeselected();
            _playerController = null;
            _aiControllable = null;
        }
    }

    private float CalculateAttackOdds()
    {
        float initialOdds = 0;
        float minDistance = float.MaxValue;
        Vector3 aiPos = _aiControllable.GetPosition();

        if (Physics.Linecast(_playerController.GetCastPoint(), aiPos, out var hitInfo))
        {
            if (hitInfo.collider.GetComponent<AIController>() == _aiControllable)
            {
                initialOdds = 1f;
                minDistance = hitInfo.distance;
            }
        }

        if (minDistance > _playerController.GetMaxRange())
        {
            initialOdds = 0;
        }
        else
        {
            float maxRange = _playerController.GetMaxRange();
            float distanceFactor = 1 - (minDistance / maxRange);
            initialOdds *= distanceFactor;
        }
        return initialOdds;
    }

    public void SetTurnStatus(TurnState levelDataTurnState, PlayerPhaseStatus levelDataPlayerPhaseStatus)
    {
        switch (levelDataTurnState)
        {
            case TurnState.AI:
                bMovementPhase = false;
                bAttackPhase = false;
                aiStartTurnEvent.Raise();
                break;
            case TurnState.Player:
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

    public void FinishPlayerPhase()
    {
        if (bMovementPhase)
        {
            bAttackPhase = true;
            bMovementPhase = false;
            _playerController = null;
            _aiControllable = null;
        }
        else
        {
            FinishPlayerTurn();
        }
    }


    public void FinishPlayerTurn()
    {
        bAttackPhase = false;
        _playerController?.OnDeselected();
        _playerController = null;
        _aiControllable = null;
        aiStartTurnEvent.Raise();
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerObj.GetComponent<PlayerController>().ResetMovementFlag();
        }
    }

    public TurnState GetTurnOwner()
    {
        if (!bMovementPhase && !bAttackPhase) return TurnState.AI;
        return TurnState.Player;
    }

    public PlayerPhaseStatus GetPlayerPhase()
    {
        if (!bMovementPhase && !bAttackPhase) return PlayerPhaseStatus.None;
        if (bMovementPhase) return PlayerPhaseStatus.Movement;
        if (bAttackPhase) return PlayerPhaseStatus.Attack;
        return PlayerPhaseStatus.None;
    }

    public int GetDamage()
    {
        if (_playerController == null) return -1;
        if (_aiControllable == null) return -1;

        return _playerController.GetBaseDamage();
    }
}
