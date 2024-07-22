using Unity.VisualScripting;
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
    
    private VisualElement _rootVE;
    
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("UIManager").Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        _uiDocument = GetComponent<UIDocument>();
        _rootVE = _uiDocument.rootVisualElement;
        _rootVE.Q<Button>("NextPhaseButton").clicked += NextPhaseAction;
        _rootVE.Q<Button>("NextTurnButton").clicked += NextTurnAction;
        _rootVE.Q<Button>("ConfirmAttackButton").clicked += ConfirmAttack;
        _rootVE.Q<Button>("ResumeBtn").clicked += TogglePauseMenu;
        _rootVE.Q<Button>("SaveBtn").clicked += SaveProgress;
        _rootVE.Q<Button>("MenuBtn").clicked += Menu;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += FindReferences;
    }

    private void FindReferences(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Finding references");
        _uiDocument = GetComponent<UIDocument>();
        _rootVE = _uiDocument?.rootVisualElement;
        turnCounter = GameObject.Find("TurnTracker")?.GetComponent<TurnCounter>();
        inputSystem = GameObject.Find("PlayerInputSystem")?.GetComponent<PlayerInputSystem>();
    }

    private void SaveProgress()
    {
        FindObjectOfType<SaveManager>().SaveRequest();
        TogglePauseMenu();
    }
    
    private void Menu()
    { 
        TogglePauseMenu();
        Destroy(GameObject.Find("SaveManager"));
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Unreveal");
        Invoke(nameof(LoadMenu),1f);
    }

    private void LoadMenu()
    {
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(playerObj);
        }
        SceneManager.LoadScene("MainMenu");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Toggling PauseHUD");
            TogglePauseMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            Toggle2X();
        }
    }

    private void Toggle2X()
    {
        Time.timeScale = Mathf.Approximately(Time.timeScale, 4f) ? 1f : 4f;
    }

    private void TogglePauseMenu()
    {
        Debug.Log("Is Visible? " + _rootVE.Q<VisualElement>("PauseHUD").visible);
        _rootVE.Q<VisualElement>("PauseHUD").visible = !_rootVE.Q<VisualElement>("PauseHUD").visible;
        Time.timeScale = (Time.timeScale == 0.0f)? 1.0f : 0f;
    }

    public void HandleShowAttack()
    {
        float attackOdds = inputSystem.GetOdds();
        int damage = inputSystem.GetDamage();
        _rootVE.Q<Label>("attackOdds").text = (int)(attackOdds * 100) + "%";
        _rootVE.Q<Label>("damageNumber").text = damage + " DMG";
        _rootVE.Q<VisualElement>("AttackUI").style.visibility = Visibility.Visible;
    }
    
    private void ConfirmAttack()
    {
        inputSystem.ConfirmAttack();
        _rootVE.Q<VisualElement>("AttackUI").style.visibility = Visibility.Hidden;
    }
    
    private void NextTurnAction()
    {
        if (inputSystem.isPlayerPlaying())
        {
            inputSystem.FinishPlayerTurn();
            ChangeToAITurn();
        }
    }

    private void NextPhaseAction()
    {
        if (inputSystem.isPlayerPlaying())
        {
            _rootVE.Q<Label>("PlayerStatus").text = "ATTACKING";
            inputSystem.FinishPlayerPhase();
        }
    }

    public void ChangeToPlayerTurn()
    {
        _rootVE.Q<Label>("PlayerStatus").text = "MOVING";
        _rootVE.Q<Label>("AiStatus").text = "WAITING";
        _rootVE.Q<Label>("AiStatus").style.color = Color.gray;
        _rootVE.Q<Label>("PlayerStatus").style.color = Color.black;
    }

    public void UpdateTurnCounter()
    {
        if (_rootVE == null || turnCounter == null || inputSystem == null) 
            FindReferences(SceneManager.GetActiveScene(),LoadSceneMode.Single);
        _rootVE.Q<Label>("TurnCounter").text = "TURN " + turnCounter.GetTurnCount();
    }
    
    public void ChangeToAITurn()
    {
        _rootVE.Q<Label>("PlayerStatus").text = "WAITING";
        _rootVE.Q<Label>("AiStatus").text = "PLAYING";
        _rootVE.Q<Label>("AiStatus").style.color = Color.black;
        _rootVE.Q<Label>("PlayerStatus").style.color = Color.gray;
    }

    public void UpdateHpBars()
    {
        if (_rootVE == null)
            FindReferences(SceneManager.GetActiveScene(),LoadSceneMode.Single);
        int playerIndex = 1;
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in playerObjects)
        {
            VisualElement partyRoot = _rootVE.Q<VisualElement>("CombatPartyHUD");
            VisualElement characterVE = partyRoot.Q<VisualElement>("Character" + playerIndex + "VE");
            PlayerController playerController = playerObject.GetComponent<PlayerController>();
            float sizeMultiplier = playerController.GetHealthPercentage();
            //characterVE.Q<VisualElement>("HPBar").style.width = (int)(74 * sizeMultiplier);
            playerIndex++;
        }
    }

    public void setPlayerOnPhase(PlayerPhaseStatus levelDataPlayerPhaseStatus)
    {
        ChangeToPlayerTurn();
        if (levelDataPlayerPhaseStatus == PlayerPhaseStatus.Attack) 
            _rootVE.Q<Label>("PlayerStatus").text = "ATTACK";
        else _rootVE.Q<Label>("PlayerStatus").text = "MOVING";
    }
}
