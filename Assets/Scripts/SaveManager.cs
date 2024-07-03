using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SaveSlot
{
    AutoSave,
    Slot1,
    Slot2,
    Slot3
}
public class SaveManager : MonoBehaviour
{
    [SerializeField] private Snapshot[] saveSlots;
    [SerializeField] private SaveSlot saveSlot ;

    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("SaveManager").Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += AfterSceneLoading;
    }
   
    private void Load()
    {
        
        GameObject.Find("Reveal").GetComponent<Animator>().SetTrigger("Unreveal");
        Invoke(nameof(DoLoad),1f);
    }

    private void DoLoad()
    {
        saveSlots[(int)saveSlot].LoadSave();
    }
    private void Save()
    {
        saveSlots[(int)saveSlot].Save();
    }

    private void AfterSceneLoading(Scene arg0, LoadSceneMode arg1)
    {
        saveSlots[(int)saveSlot].PostLoad();
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
    }
}
