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

    /**
     * <summary>
     *  Updates fill percentage of health bar to reflect target health.
     * </summary>
     * <param name="health">
     *    Float value indicating current target health. Shows as red part of health bar.
     * </param>
     * <param name="maxHealth">
     *    float value indicating target maximum health. Used to calculate fill percentage of health bar.
     * </param>
     */
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
