using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * <summary>
 *  Enum representing the save states
 * </summary>
 */
public enum SaveSlot
{
    AutoSave,
    Slot1,
    Slot2,
    Slot3
}

/**
 * <summary>
 *  Class responsible for managing the saves
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public class SaveManager : MonoBehaviour
{
    /**
     * <summary>
     *  Array containing all 4 possible saves
     * </summary>
     */
    [SerializeField] private Snapshot[] saveSlots;
    /**
     * <summary>
     *  Currently selected save
     * </summary>
     */
    [SerializeField] private SaveSlot saveSlot ;

    private void Start()
    {
        //Make sure we only have one SaveManager Tagged Object
        if (GameObject.FindGameObjectsWithTag("SaveManager").Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //Add callback for when a scene is done loading
        SceneManager.sceneLoaded += AfterSceneLoading;
    }
   
    public void Load()
    {
        //Triggers the Unreveal animation
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Unreveal");
        //Invokes DoLoad after 1s
        Invoke(nameof(DoLoad),1f);
    }
    
    /**
     * <summary>
     *  Method responsible for calling LoadSave on the currently selected SaveSlot
     * </summary>
     */
    private void DoLoad()
    {
        saveSlots[(int)saveSlot].LoadSave();
    }
    /**
     * Method responsible for calling Save on the currently selected SaveSlot
     */
    private void Save()
    {
        saveSlots[(int)saveSlot].Save();
    }

    /**
     * <summary>
     *  Callback after scene is done loading, to call PostLoad on the saveslot
     * </summary>
     */
    private void AfterSceneLoading(Scene arg0, LoadSceneMode arg1)
    {
        saveSlots[(int)saveSlot].PostLoad();
    }
    
    /**
     * <summary>
     *  Method responsible for requesting a save
     * </summary>
     */
    public void SaveRequest()
    {
        Save();
    }
    
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 50), "Load Snapshot"))
        {
            Load();
        }
        if (GUI.Button(new Rect(10, 60, 200, 50), "save Snapshot"))
        {
            Save();
        }
        if (GUI.Button(new Rect(10, 110, 200, 50), "reset Snapshot"))
        {
            StartNewGame();
        }
    }
        
    /**
     * <summary>
     *  Method responsible for starting a new game
     * </summary>
     */
    public void StartNewGame()
    {
        saveSlots[(int)saveSlot].ClearSnapshot();
        Load();
    }

    /**
     * <summary>
     *  Method responsible for changing the current SaveSlot
     * </summary>
     */
    public void SetSlot(int slot)
    {
        if (slot < 0 || slot >= Enum.GetValues(typeof(SaveSlot)).Length)
        {
            Debug.LogError("Invalid save slot number");
            return;
        }

        saveSlot = (SaveSlot)slot;
        Debug.Log("Save slot set to: " + saveSlot);
    }
}
