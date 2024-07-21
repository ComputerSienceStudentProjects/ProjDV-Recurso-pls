using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    [Header("SaveManager")] 
    [SerializeField] private SaveManager saveManager;

    [SerializeField] private UIDocument _uiDocument;
    private VisualElement _rootVE;
    
    
    private void Start()
    {
        _rootVE = _uiDocument.rootVisualElement;
        _rootVE.Q<Button>("NewGameBtn").clicked += NewGame;
        _rootVE.Q<Button>("QuitGameBtn").clicked += ExitGame;
        _rootVE.Q<Button>("LoadGameBtn").clicked += LoadGame;
        _rootVE.Q<Button>("backButton").clicked += Back;
        _rootVE.Q<Button>("LoadSave1").clicked += LoadSave1;
        _rootVE.Q<Button>("LoadSave2").clicked += LoadSave2;
        _rootVE.Q<Button>("LoadSave3").clicked += LoadSave3;
    }

    private void LoadGame()
    {
        _rootVE.Q<VisualElement>("LoadingSaves").visible = true;
        _rootVE.Q<VisualElement>("NewGame").visible = false;
        _rootVE.Q<VisualElement>("QuitGame").visible = false;
        _rootVE.Q<Button>("NewGameBtn").visible = false;
        _rootVE.Q<Button>("QuitGameBtn").visible = false;
    }

    private void Back()
    {
        _rootVE.Q<VisualElement>("LoadingSaves").visible = false;
        _rootVE.Q<VisualElement>("NewGame").visible = true;
        _rootVE.Q<VisualElement>("QuitGame").visible = true;
        _rootVE.Q<Button>("NewGameBtn").visible = true;
        _rootVE.Q<Button>("QuitGameBtn").visible = true;
    }
    
    private void NewGame()
    {
        saveManager.SetSlot(1);
        saveManager.StartNewGame();
    }

    private void DoExit()
    {
        Application.Quit(0);
    }
    
    private void ExitGame()
    {
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Unreveal");
        Invoke(nameof(DoExit),1f);
    }

    private void LoadSave1()
    {
        saveManager.SetSlot(1);
        saveManager.Load();
    }
    
    private void LoadSave2()
    {
        saveManager.SetSlot(2);
        saveManager.Load();
    }
    
    private void LoadSave3()
    {
        saveManager.SetSlot(3);
        saveManager.Load();
    }

}
