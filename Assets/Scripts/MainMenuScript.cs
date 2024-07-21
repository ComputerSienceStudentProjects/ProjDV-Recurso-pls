using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    [Header("SaveManager")] 
    [SerializeField] private SaveManager saveManager;

    [SerializeField] private UIDocument _uiDocument;
    private VisualElement rootVE;
    
    
    private void Start()
    {
        rootVE = _uiDocument.rootVisualElement;
        rootVE.Q<Button>("NewGameBtn").clicked += NewGame;
        rootVE.Q<Button>("QuitGameBtn").clicked += ExitGame;
        rootVE.Q<Button>("LoadGameBtn").clicked += LoadGame;
        rootVE.Q<Button>("backButton").clicked += Back;
        rootVE.Q<Button>("LoadSave1").clicked += LoadSave1;
        rootVE.Q<Button>("LoadSave2").clicked += LoadSave2;
        rootVE.Q<Button>("LoadSave3").clicked += LoadSave3;
    }

    private void LoadGame()
    {
        rootVE.Q<VisualElement>("LoadingSaves").visible = true;
        rootVE.Q<VisualElement>("NewGame").visible = false;
        rootVE.Q<VisualElement>("QuitGame").visible = false;
        rootVE.Q<Button>("NewGameBtn").visible = false;
        rootVE.Q<Button>("QuitGameBtn").visible = false;
    }

    private void Back()
    {
        rootVE.Q<VisualElement>("LoadingSaves").visible = false;
        rootVE.Q<VisualElement>("NewGame").visible = true;
        rootVE.Q<VisualElement>("QuitGame").visible = true;
        rootVE.Q<Button>("NewGameBtn").visible = true;
        rootVE.Q<Button>("QuitGameBtn").visible = true;
    }
    
    private void NewGame()
    {
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
