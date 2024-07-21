using UnityEngine;

public class MedKitPowerup : MonoBehaviour
{
    [SerializeField] private float healingValue;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private float resetTimer = 60;
    
    private float _availableHealing;

    private void Start()
    {
        _availableHealing = healingValue;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other is { gameObject: { tag: "Player" } })
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.AddHp(_availableHealing);
            _availableHealing = 0;
            _meshRenderer.material = offMaterial;
            Invoke(nameof(ResetHealing),resetTimer);
        }
    }

    private void ResetHealing()
    {
        _availableHealing = healingValue;
        _meshRenderer.material = onMaterial;
    }
}
