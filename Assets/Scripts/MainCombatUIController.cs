using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainCombatUIController : MonoBehaviour
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool SetCursorPos(int X, int Y);
    
    [Header("References")]
    private UIDocument _uiDocument;
    [SerializeField] private TurnCounter turnCounter;
    [SerializeField] private PlayerInputSystem inputSystem;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip buttonAudioClip;
    [SerializeField] private AudioClip escapeAudioClip;
    
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
        _rootVE.Q<Button>("NextPhaseButton").RegisterCallback<ClickEvent>(NextPhaseAction);
        _rootVE.Q<Button>("NextPhaseButton").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        _rootVE.Q<Button>("NextTurnButton").RegisterCallback<ClickEvent>(NextTurnAction);
        _rootVE.Q<Button>("NextTurnButton").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        _rootVE.Q<Button>("ConfirmAttackButton").RegisterCallback<ClickEvent>(ConfirmAttack);
        _rootVE.Q<Button>("ConfirmAttackButton").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        _rootVE.Q<Button>("ResumeBtn").RegisterCallback<ClickEvent>(TogglePauseMenu);
        _rootVE.Q<Button>("ResumeBtn").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        _rootVE.Q<Button>("SaveBtn").RegisterCallback<ClickEvent>(SaveProgress);
        _rootVE.Q<Button>("SaveBtn").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        _rootVE.Q<Button>("MenuBtn").RegisterCallback<ClickEvent>(Menu);
        _rootVE.Q<Button>("MenuBtn").RegisterCallback<ClickEvent>(PlayButtonAudioCLip);
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += FindReferences;
    }

    private void PlayButtonAudioCLip(ClickEvent evt)
    {
        _audioSource.clip = buttonAudioClip;
        _audioSource.loop = false;
        _audioSource.Play();
    }
    
    private void FindReferences(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Finding references");
        _uiDocument = GetComponent<UIDocument>();
        _rootVE = _uiDocument?.rootVisualElement;
        turnCounter = GameObject.Find("TurnTracker")?.GetComponent<TurnCounter>();
        inputSystem = GameObject.Find("PlayerInputSystem")?.GetComponent<PlayerInputSystem>();
    }

    private void SaveProgress(ClickEvent evt)
    {
        FindObjectOfType<SaveManager>().SaveRequest();
        TogglePauseMenu(evt);
    }
    
    private void Menu(ClickEvent evt)
    { 
        TogglePauseMenu(evt);
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
            _audioSource.clip = escapeAudioClip;
            _audioSource.Play();
            TogglePauseMenu(null);
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

    private void TogglePauseMenu(ClickEvent evt)
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
    
    private void ConfirmAttack(ClickEvent evt)
    {
        inputSystem.ConfirmAttack();
        _rootVE.Q<VisualElement>("AttackUI").style.visibility = Visibility.Hidden;
    }
    
    private void NextTurnAction(ClickEvent evt)
    {
        if (Input.mousePosition.x <= evt.position.x
            && Input.mousePosition.y <= evt.position.y)
        {
            if (inputSystem.isPlayerPlaying())
            {
                ChangeToAITurn();
                inputSystem.FinishPlayerTurn();
            }
        }
    }

    private void NextPhaseAction(ClickEvent evt)
    {
        if (Input.mousePosition.x <= evt.position.x 
            && Input.mousePosition.y <= evt.position.y)
        {
            if (inputSystem.isPlayerPlaying())
            {
                _rootVE.Q<Label>("PlayerStatus").text = "ATTACKING";
                inputSystem.FinishPlayerPhase();
            }
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
