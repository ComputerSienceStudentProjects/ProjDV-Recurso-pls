using UnityEngine;

public class HealthBoostPowerUp : MonoBehaviour
{
    [SerializeField] private int aditionalHealth;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private float resetTimer = 60;
    private bool bIsEnabled = true;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other is { gameObject: { tag: "Player" } } && bIsEnabled)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.AddToMaxHP(aditionalHealth);
            bIsEnabled = false;
            _meshRenderer.material = offMaterial;
            Invoke(nameof(ResetHealing),resetTimer);
        }
    }

    private void ResetHealing()
    {
        bIsEnabled = true;
        _meshRenderer.material = onMaterial;
    }
}
