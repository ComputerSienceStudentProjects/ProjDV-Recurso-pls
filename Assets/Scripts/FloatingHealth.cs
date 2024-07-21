using UnityEngine;
using UnityEngine.UI;


/**
 * <summary>
 *
 * </summary>
 * <version>
 *  <date here>/07/2024
 * </version>
 * <author>
 *  Diogo Capela (<Email here>)
 * </author>
 */
public class FloatingHealth : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    // Update is called once per frame

    public void UpdateHealthBar(float health, float maxHealth)
    {
        slider.value = health / maxHealth;
    }

    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        transform.position = target.position + offset;
    }
}
