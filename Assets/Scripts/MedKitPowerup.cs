using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedKitPowerup : MonoBehaviour
{
    [SerializeField] private float healingValue;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material offMaterial;
    [SerializeField] private Material onMaterial;
    [SerializeField] private float resetTimer = 60;
    
    private float availableHealing;

    private void Start()
    {
        availableHealing = healingValue;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Here V777");
        if (other is { gameObject: { tag: "Player" } })
        {
            PlayerController controller = other.GetComponent<PlayerController>();
            controller.AddHp(availableHealing);
            availableHealing = 0;
            _meshRenderer.material = offMaterial;
            Invoke(nameof(ResetHealing),resetTimer);
        }
    }

    private void ResetHealing()
    {
        availableHealing = healingValue;
        _meshRenderer.material = onMaterial;
    }
}
