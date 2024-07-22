using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/**
 * <summary>
 *  Class representing a scene
 * </summary>
 *  * <version>
 *  21/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
[Serializable]
public class SceneReference
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";

    public string SceneName => sceneName;

    public static implicit operator string(SceneReference sceneReference) => sceneReference.SceneName;
}

/**
 * <summary>
 *  Class for controlling the portals between levels
 * </summary>
 * <version>
 *  12/07/2024
 * </version>
 * <author>
 *  João Gouveia (joao.c.gouveia10@gmail.com)
 * </author>
 */
public class Portal : MonoBehaviour
{
    [SerializeField] private SceneReference targetScene;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip portalSound;

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
            _audioSource.clip = portalSound;
            _audioSource.Play();
        }
    }

    void UpdateSave(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex is > 0 and < 3)
        {
            GameObject.Find("SaveManager").GetComponent<SaveManager>().SaveRequest();
            if (this == null) return;
            Destroy(gameObject);
        }
    }
    
    private void loadNextLevel()
    {
        DontDestroyOnLoad(gameObject);
        GameObject.Find("SaveManager").GetComponent<SaveManager>().ResetEntities();
        SceneManager.LoadScene(targetScene.SceneName);
    }
}
