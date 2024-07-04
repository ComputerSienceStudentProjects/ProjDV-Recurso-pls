using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private SceneAsset target;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        SceneManager.sceneLoaded += UpdateSave;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _animator.SetTrigger("Unreveal");
            Invoke(nameof(loadNextLevel),1f);
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    void UpdateSave(Scene arg0, LoadSceneMode arg1)
    {
        GameObject.Find("SaveManager").GetComponent<SaveManager>().SaveRequest();
        if (this == null) return;
        Destroy(gameObject);
    }
    
    private void loadNextLevel()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene(target.name);
    }
}
