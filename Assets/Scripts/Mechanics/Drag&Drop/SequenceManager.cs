using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Mechanics.Drag_Drop.FoodsPlants;
using Random = UnityEngine.Random;

public class SequenceManager : MonoBehaviour
{
    [Header("References")]
    public FarmRowContainer[] allRows;
    public IconFoodData[] availableFoodData; 
    public GameObject plantPrefab; 
    public GameObject winCanvas;   
    public GameObject warningCanvas; 
    
    [Header("Sequence")]
    public IconFoodType[] referenceSequence; 
    private IconFoodType[] availableTypes;

    private Queue<IconFoodType> seedQueue; 

    private List<FarmRowContainer> correctRows = new List<FarmRowContainer>();

    void Awake()
    {
        availableTypes = availableFoodData.Select(d => d.type).Where(t => t != IconFoodType.None).ToArray();
        
        GenerateReferenceSequence();
        
        // NOVO: Inicializa a fila de sementes (em vez de InitializeSeedSpawnSlots)
        InitializeSeedQueue();
    }

    private void GenerateReferenceSequence()
    {
        if (allRows == null || allRows.Length == 0 || allRows[0].slots == null)
        {
            Debug.LogError("FarmRows não configuradas. Não é possível gerar a sequência.");
            return;
        }
        int sequenceLength = allRows[0].slots.Length;
        referenceSequence = new IconFoodType[sequenceLength];

        for (int i = 0; i < sequenceLength; i++)
        {
            if (availableTypes.Length > 0)
            {
                int randomIndex = Random.Range(0, availableTypes.Length);
                referenceSequence[i] = availableTypes[randomIndex];
            } else {
                referenceSequence[i] = IconFoodType.None;
            }
        }
        
        Debug.Log("Sequência de Referência Gerada: " + string.Join(", ", referenceSequence.Select(t => t.ToString())));
    }
    
    private void InitializeSeedQueue()
    {
        IconFoodType[] requiredTypes = referenceSequence.Distinct().ToArray();
        
        // Garante que o jogador tem 3 de cada tipo de semente para começar a jogar
        seedQueue = new Queue<IconFoodType>(requiredTypes.SelectMany(type => Enumerable.Repeat(type, 3)));
    }

    public IconFoodData GetNextSeedType()
    {
        if (seedQueue == null || seedQueue.Count == 0)
        {
            Debug.LogWarning("A fila de sementes está vazia. Não há mais sementes disponíveis!");
            return null;
        }
        
        IconFoodType nextType = seedQueue.Dequeue();
        
        // Coloca o tipo de volta no final para que ele possa ser reposto
        seedQueue.Enqueue(nextType); 

        return GetFoodData(nextType);
    }
    
    public IconFoodData GetFoodData(IconFoodType type)
    {
        return availableFoodData.FirstOrDefault(d => d.type == type);
    }
    
    public GameObject InstantiatePlant(IconFoodType type, Transform parent)
    {
        return Instantiate(plantPrefab, parent.position, Quaternion.identity, parent);
    }

    public bool CheckRowSequence(IconFoodType[] planted)
    {
        if (planted.Length != referenceSequence.Length) return false;
        
        for (int i = 0; i < planted.Length; i++)
        {
            if (planted[i] != referenceSequence[i])
            {
                return false;
            }
        }
        return true;
    }
    
    public void NotifyPlantRemoved()
    {
        correctRows.Clear();
        foreach (var row in allRows)
        {
            row.CheckSequenceStatus();
        }
    }
    
    public void NotifyRowCorrect(FarmRowContainer row)
    {
        if (!correctRows.Contains(row))
        {
            correctRows.Add(row);
        }
        CheckWinCondition();
    }
    
    public void NotifyRowIncorrect(FarmRowContainer row)
    {
        correctRows.Remove(row); 
        CheckFinalState(); 
    }

    private void CheckWinCondition()
    {
        if (correctRows.Count == allRows.Length)
        {
            winCanvas.SetActive(true);
            Debug.Log("Nível Concluído: Todas as sequências estão corretas!");
        }
        else
        {
            CheckFinalState();
        }
    }
    
    private void CheckFinalState()
    {
        bool allSlotsFull = allRows.All(r => r.slots.All(s => s.currentPlant != null));
        
        bool allPlantsGrown = allRows.All(r => r.slots.All(s => s.currentPlant != null && s.currentPlant.GetCurrentState() == PlantState.Grown));

        if (allSlotsFull && allPlantsGrown)
        {
            if (correctRows.Count < allRows.Length)
            {
                warningCanvas.SetActive(true);
                Debug.Log("Aviso: Todas as plantas cresceram, mas há sequências incorretas. Dropping!");
                
                foreach (var row in allRows)
                {
                    IconFoodType[] plantedSequence = row.slots.Select(s => s.currentPlant.GetPlantType()).ToArray();
                    if (!CheckRowSequence(plantedSequence))
                    {
                        row.DropAllPlants();
                    }
                }
            }
        }
        else
        {
            warningCanvas.SetActive(false);
        }
    }
}