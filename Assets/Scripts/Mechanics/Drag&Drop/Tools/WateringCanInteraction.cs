// Script: WateringCanInteraction.cs (No Regador Ativo)
using UnityEngine;

public class WateringCanInteraction : MonoBehaviour
{
    private ToolManager toolManager;

    void Awake()
    {
        toolManager = FindObjectOfType<ToolManager>();
    }
    
    // Assumindo que o FarmSlot tem um BoxCollider 2D (is Trigger)
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu é um FarmSlot
        FarmSlot slot = other.GetComponent<FarmSlot>();
        if (slot != null && slot.currentPlant != null)
        {
            Debug.Log("Regando semente...");
            // Chama o método de regar da planta no slot
            slot.currentPlant.WaterPlant();
        }
    }
}