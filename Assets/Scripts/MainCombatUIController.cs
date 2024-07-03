using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainCombatUIController : MonoBehaviour
{
    [Header("References")]
    private UIDocument _uiDocument;
    [SerializeField] private TurnCounter turnCounter;
    [SerializeField] private PlayerInputSystem inputSystem;
    
    private VisualElement rootVE;
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("UIManager").Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        _uiDocument = GetComponent<UIDocument>();
        rootVE = _uiDocument.rootVisualElement;
        rootVE.Q<Button>("NextPhaseButton").clicked += NextPhaseAction;
        rootVE.Q<Button>("NextTurnButton").clicked += NextTurnAction;
        
        //TODO IMPLEMENT
        rootVE.Q<Button>("ConfirmAttackButton").clicked += ConfirmAttack;
        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += FindReferences;

    }

    private void FindReferences(Scene arg0, LoadSceneMode arg1)
    {
        turnCounter = GameObject.Find("TurnTracker").GetComponent<TurnCounter>();
        inputSystem = GameObject.Find("PlayerInputSystem").GetComponent<PlayerInputSystem>();
    }

    public void HandleShowAttack()
    {
        float attackOdds = inputSystem.GetOdds();
        rootVE.Q<Label>("attackOdds").text = (int)(attackOdds * 100) + "%";
        rootVE.Q<VisualElement>("AttackUI").style.visibility = Visibility.Visible;
    }
    
    private void ConfirmAttack()
    {
        inputSystem.ConfirmAttack();
        rootVE.Q<VisualElement>("AttackUI").style.visibility = Visibility.Hidden;
    }
    
    private void NextTurnAction()
    {
        inputSystem.FinishPlayerTurn();
        ChangeToAITurn();
    }

    private void NextPhaseAction()
    {
        rootVE.Q<Label>("PlayerStatus").text = "ATTACKING";
        inputSystem.FinishPlayerPhase();
    }

    public void ChangeToPlayerTurn()
    {
        rootVE.Q<Label>("PlayerStatus").text = "MOVING";
        rootVE.Q<Label>("AiStatus").text = "WAITING";
        rootVE.Q<Label>("AiStatus").style.color = Color.gray;
        rootVE.Q<Label>("PlayerStatus").style.color = Color.black;
    }

    public void UpdateTurnCounter()
    {
        Debug.Log("TURN " + turnCounter.GetTurnCount());
        rootVE.Q<Label>("TurnCounter").text = "TURN " + turnCounter.GetTurnCount();
    }
    
    public void ChangeToAITurn()
    {
        rootVE.Q<Label>("PlayerStatus").text = "WAITING";
        rootVE.Q<Label>("AiStatus").text = "PLAYING";
        rootVE.Q<Label>("AiStatus").style.color = Color.black;
        rootVE.Q<Label>("PlayerStatus").style.color = Color.gray;
    }

    public void setPlayerOnPhase(PlayerPhaseStatus levelDataPlayerPhaseStatus)
    {
        ChangeToPlayerTurn();
        if (levelDataPlayerPhaseStatus == PlayerPhaseStatus.Attack) 
            rootVE.Q<Label>("PlayerStatus").text = "ATTACK";
        else rootVE.Q<Label>("PlayerStatus").text = "MOVING";
    }
}