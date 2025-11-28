using Mechanics.Drag_Drop.FoodsPlants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Implementa IDropHandler para receber a semente e IPointerClickHandler para interagir com a Enxada
public class FarmSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("Slot Settings")]
    public int rowIndex;    // Coluna Horizontal
    public int columnIndex; // Posição dentro da coluna
    
    [Header("References")]
    public Transform plantContainer; // Ponto de instância da planta
    
    [HideInInspector] public FoodPlantController currentPlant;

    private SequenceManager sequenceManager;
    private ToolManager toolManager;

    void Awake()
    {
        sequenceManager = FindObjectOfType<SequenceManager>();
        if (plantContainer == null) plantContainer = transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        IconFoodItem droppedItem = IconFoodItem.GetDraggedItem();
        
        // Verifica se há um item válido sendo arrastado e se o slot está vazio
        if (droppedItem != null && currentPlant == null)
        {
            // 1. Instancia a planta
            IconFoodType typeToPlant = droppedItem.foodType;
            GameObject newPlant = sequenceManager.InstantiatePlant(typeToPlant, plantContainer);
            
            FoodPlantController plantController = newPlant.GetComponent<FoodPlantController>();
            if (plantController != null)
            {
                currentPlant = plantController;
                currentPlant.Initialize(this, typeToPlant, sequenceManager.GetFoodData(typeToPlant));
                
                // 2. Dispara a reposição no slot de origem
                if (droppedItem.sourceSlotManager != null)
                {
                    droppedItem.sourceSlotManager.SpawnNewItem();
                }
                
                // 3. DESTRÓI o item de semente que foi usado!
                Destroy(droppedItem.gameObject); 

                Debug.Log($"Planta {typeToPlant} plantada e item de origem reposto.");
            }
            else
            {
                Destroy(newPlant);
            }
        }
    }
    
    // Método de limpeza simples (uso interno do slot, sem efeito)
    public void CleanSlot()
    {
        if (currentPlant != null)
        {
            Destroy(currentPlant.gameObject);
            currentPlant = null;
            sequenceManager.NotifyPlantRemoved(); 
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // Lógica de Retirada da Planta (Enxada)
        if (currentPlant != null && toolManager != null && toolManager.CurrentTool == ToolManager.ActiveTool.Hoe)
        {
            toolManager.UseHoe(this); // Remove a planta e reseta a enxada
        }
    }
    
    public void RemovePlant()
    {
        if (currentPlant != null)
        {
            Destroy(currentPlant.gameObject);
            currentPlant = null;
            // Notifica o gerenciador central que uma mudança ocorreu (pode ser útil)
            sequenceManager.NotifyPlantRemoved(); 
        }
    }
    
    public void RemovePlantWithEffect()
    {
        if (currentPlant != null)
        {
            // O Slot perde a referência da planta IMEDIATAMENTE.
            // A planta instanciada (currentPlant) agora é um objeto solto que irá cair.
            FoodPlantController plantToDrop = currentPlant;
            currentPlant = null;
            
            // Inicia o efeito na planta e remove a referência do slot
            plantToDrop.StartDropEffect(); 

            sequenceManager.NotifyPlantRemoved();
        }
    }
    
    public void NotifyGrown()
    {
        // Notifica o container superior (FarmRowContainer) quando esta planta terminar de crescer
        if (transform.parent != null)
        {
            FarmRowContainer rowContainer = transform.parent.GetComponent<FarmRowContainer>();
            rowContainer?.CheckSequenceStatus();
        }
    }
}