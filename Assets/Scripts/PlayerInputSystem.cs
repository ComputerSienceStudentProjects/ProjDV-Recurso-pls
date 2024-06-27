using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerInputSystem : MonoBehaviour
{
    private PlayerController _playerController;
    private AIController _aiController;
    [SerializeField] private bool bMovementPhase = true;
    [SerializeField] private bool bAttackPhase;
    private Camera _mainCamera;

    [SerializeField] private CameraController cameraController;
    [SerializeField] private GameEvent aiStartTurnEvent;

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
        AIController aiTarget = hit.collider.GetComponent<AIController>();
        if (aiTarget != null)
        {
            if (_playerController == null)
            {
                Debug.Log("Clicked on AI TARGET Before selecting a prty chracter");
                return;
            }
            _aiController = hit.collider.GetComponent<AIController>();
            Debug.Log("both offender and target set");
            //ShowAttacks();
            return;
        }
        PlayerController playerController = hit.collider.GetComponent<PlayerController>();
        if (playerController != null) HandlePlayerSelect(playerController);
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
            _aiController = null;
            cameraController.UnlockOnGameObject();
            return;
        }

        playerController.OnSelected();
        _playerController?.OnDeselected();
        _playerController = playerController;
        cameraController.LockOnGameObject(_playerController.gameObject);
    }

    private void OnGUI()
    {
        if (bMovementPhase == false && bAttackPhase == false) return;
        GUI.Label(new Rect(30,200,400,40),"Current phase: " + ((bMovementPhase == true) ? "Movement" : "Attack"));
        if (GUI.Button(new Rect(30,160,200,40),"Finish Phase"))
        {
            if (bMovementPhase)
            {
                bMovementPhase = false;
                bAttackPhase = true;
                _playerController?.OnDeselected();
                _playerController = null; 
            }else if (bAttackPhase)
            {
                bAttackPhase = false;
                _playerController?.OnDeselected();
                _playerController = null;
                _aiController = null;
                aiStartTurnEvent.Raise();
            }
        }
        
        if (_playerController == null || _aiController == null) return;
        GUI.Box(new Rect(10, 10, 400, 400), "Attack character is of type ");
        float attackOdds = CalculateAttackOdds();
        GUI.Label(new Rect(30,40,400,40),"Attack Odds are of " + (attackOdds * 100f) + "%");
        GUI.Label(new Rect(30,60,400,40),"Current phase: " + ((bMovementPhase == true) ? "Movement" : "Attack"));
        if (GUI.Button(new Rect(30,80,200,40),"Confirm Attack"))
        {
            ConfirmAttack(attackOdds);
        }
        
        if (GUI.Button(new Rect(30,120,200,40),"Cancel Attack"))
        {
            _aiController = null;
            _playerController.OnDeselected();
            _playerController = null;
        }
    }
    private void ConfirmAttack(float attackOdds)
    {
        float randomValue = Random.value;
        if (attackOdds > randomValue)
        {
            _playerController.PlayAttackAnim(_aiController.GetPosition(),_aiController);

            Debug.Log("Attack successful with odds of " + (attackOdds * 100f) + "%");
            _playerController.OnDeselected();
            _playerController = null;
            _aiController = null;
        }
        else
        {
            Debug.Log("Attack failed. Random value was " + randomValue + " and attack odds were " + attackOdds);
            _playerController.OnDeselected();
            _playerController = null;
            _aiController = null;
        }
    }
    
    private float CalculateAttackOdds()
    {
        float initialOdds = 0;
        RaycastHit hitInfo;

        float minDistance = float.MaxValue;

        Vector3 playerPos = _playerController.GetPosition();
        Vector3 aiPos = _aiController.GetPosition();

        if (Physics.Linecast(_playerController.GetCastPoint(), aiPos, out hitInfo))
        {
            if (hitInfo.collider.GetComponent<AIController>() == _aiController)
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
        Debug.Log("minDistance = " + minDistance);
        return initialOdds;
    }
}
