using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreRangePowerUp : MonoBehaviour
{
    
    [SerializeField] private float resetTimer = 60;
    [SerializeField] private float healingValue;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    
    
    private bool bIsEnabled = true;
    private void OnTriggerEnter(Collider other)
    {
        if (other is { gameObject: { tag: "Player" } } && bIsEnabled)
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.SetMovementRadius(2f);
            bIsEnabled = false;
            _meshRenderer.material = offMaterial;
            Invoke(nameof(ResetPowerUp),resetTimer);
        }
    }

    private void ResetPowerUp()
    {
        bIsEnabled = true;
        _meshRenderer.material = onMaterial;
    }
}
