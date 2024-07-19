using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        GameObject.Find("SaveManager").GetComponent<SaveManager>().ResetEntities();
        SceneManager.LoadScene(target.name);
    }
}
