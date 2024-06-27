using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class CharacterData
{
    public GameObject modelReference;
    public Vector3 position;
    public Quaternion rotation;
    public float health;
}


[CreateAssetMenu(menuName = "Scriptables/Snapshot",fileName = "New Snapshot")]
public class Snapshot : ScriptableObject
{
    [Header("Player data")] 
    
    [SerializeField] private CharacterData[] playerObjects;
    [SerializeField] private CharacterData[] aiObjects;
    [SerializeField] private int levelIndex;
    
    private const string playerPrefabPath = "Assets/Prefabs/Player/";
    private const string aiPrefabPath = "Assets/Prefabs/AI/";
    
    public void LoadSave()
    { 
       SceneManager.LoadScene(levelIndex);
    }

    public void PostLoad()
    {
        foreach (CharacterData playerCharacterData in playerObjects)
        {
            Instantiate(playerCharacterData.modelReference, playerCharacterData.position, playerCharacterData.rotation, GameObject.Find("Player Characters").transform);
        }
    }

    public void Save()
    {
        
    }

}
