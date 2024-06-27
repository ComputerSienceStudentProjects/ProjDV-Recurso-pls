using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private Snapshot saveSnapshot;

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
        saveSnapshot.LoadSave();
    }
    private void Save()
    {
        saveSnapshot.Save();
    }

    private void AfterSceneLoading(Scene arg0, LoadSceneMode arg1)
    {
        saveSnapshot.PostLoad();
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
