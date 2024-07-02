using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        _uiDocument = GetComponent<UIDocument>();
        rootVE = _uiDocument.rootVisualElement;
        rootVE.Q<Button>("NextPhaseButton").clicked += NextPhaseAction;
        rootVE.Q<Button>("NextTurnButton").clicked += NextTurnAction;
        
        //TODO IMPLEMENT
        rootVE.Q<Button>("ConfirmAttack").clicked += NextTurnAction;
    }

    public void HandleShowAttack()
    {
        float attackOdds = inputSystem.GetOdds();
        Debug.Log("Odds -> " + attackOdds);
    }
    
    private void ConfirmAttack()
    {
        inputSystem.ConfirmAttack();
    }
    
    private void NextTurnAction()
    {
        inputSystem.FinishPlayerTurn();
    }

    private void NextPhaseAction()
    {
        inputSystem.FinishPlayerPhase();
    }
}
