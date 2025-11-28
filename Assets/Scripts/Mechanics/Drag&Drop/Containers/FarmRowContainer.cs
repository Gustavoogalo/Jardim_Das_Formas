using UnityEngine;
using System.Linq;
using Mechanics.Drag_Drop.FoodsPlants;

public class FarmRowContainer : MonoBehaviour
{
    [Header("References")]
    public FarmSlot[] slots; 
    public RectTransform containerBorder; // Opcional: para feedback visual
    
    private SequenceManager sequenceManager;

    void Awake()
    {
        sequenceManager = FindObjectOfType<SequenceManager>();
    }
    
    // Método principal chamado sempre que uma planta cresce ou é removida
    public void CheckSequenceStatus()
    {
        // 1. Verifica se TODAS as plantas estão no estado Crescida
        // Só verifica a sequência se todas as plantas estiverem no estado final!
        if (slots.Any(s => s.currentPlant == null || s.currentPlant.GetCurrentState() != PlantState.Grown))
        {
            return;
        }

        // 2. Obtém a sequência de tipos plantados
        IconFoodType[] plantedSequence = slots.Select(s => s.currentPlant.GetPlantType()).ToArray();
        
        // 3. Compara com a sequência de referência global
        bool isCorrect = sequenceManager.CheckRowSequence(plantedSequence);

        if (isCorrect)
        {
            // Opcional: Feedback visual verde
            sequenceManager.NotifyRowCorrect(this);
        }
        else
        {
            // Opcional: Feedback visual vermelho
            sequenceManager.NotifyRowIncorrect(this);
        }
    }
    
    public void DropAllPlants()
    {
        foreach (FarmSlot slot in slots)
        {
            if (slot.currentPlant != null)
            {
                // A FarmSlot.RemovePlant() foi modificada para iniciar a queda
                slot.RemovePlantWithEffect(); 
            }
        }
        // Opcional: Feedback visual da linha, como o fundo do container piscando em vermelho.
    }
}