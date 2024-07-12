using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    [Header("SaveManager")] 
    [SerializeField] private SaveManager saveManager;
    
    
    public void NewGame()
    {
        saveManager.StartNewGame();
    }

    public void LoadSave1()
    {
        saveManager.SetSlot(1);
        saveManager.Load();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(300, 10, 250, 50), "Start a new Game"))
        {
            NewGame();
        }
        
        if (GUI.Button(new Rect(300, 60, 250, 50), "Load Save 1"))
        {
            LoadSave1();
        }
    }
}
