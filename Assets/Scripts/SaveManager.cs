using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Snapshot[] saveSlots;

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
        saveSlots[0].LoadSave();
    }
    private void Save()
    {
        saveSlots[0].Save();
    }

    private void AfterSceneLoading(Scene arg0, LoadSceneMode arg1)
    {
        saveSlots[0].PostLoad();
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
