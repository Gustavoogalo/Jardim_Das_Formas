using UnityEngine;
using UnityEngine.UI;
using Mechanics.Drag_Drop.FoodsPlants;

public class ItemSlotManager : MonoBehaviour
{
    public GameObject iconFoodItemPrefab; 
    public RectTransform spawnPoint;      
    public Image backgroundImage; 
    
    private SequenceManager sequenceManager;

    void Awake()
    {
        sequenceManager = FindObjectOfType<SequenceManager>();
        
        if (sequenceManager != null)
        {
            // Garante que a primeira semente é spawnada ao iniciar,
            // usando o tipo fornecido pela fila do SequenceManager.
            SpawnNewItem(); 
        }
        else
        {
            Debug.LogError("SequenceManager não encontrado na cena. O ItemSlotManager não pode funcionar.");
        }
    }
    
    public void SpawnNewItem()
    {
        // Pede o próximo tipo de semente ao SequenceManager (que gerencia a fila)
        IconFoodData nextSeedData = sequenceManager.GetNextSeedType();

        if (nextSeedData == null)
        {
            Debug.LogWarning($"Não foi possível spawnar semente. Fila de sementes vazia ou dados inválidos.");
            return;
        }
        
        // 1. Instancia o novo item
        GameObject newItemObject = Instantiate(iconFoodItemPrefab, spawnPoint);
        
        newItemObject.transform.localPosition = Vector3.zero;
        
        IconFoodItem newItemController = newItemObject.GetComponent<IconFoodItem>();
        if (newItemController != null)
        {
            // 2. Configura a semente com o tipo recebido
            newItemController.foodType = nextSeedData.type;
            newItemController.sourceSlotManager = this; 
            
            // Configura o Sprite da Semente
            Image itemImage = newItemController.GetComponent<Image>();
            if (itemImage != null && nextSeedData.seedPacketSprite != null)
            {
                itemImage.sprite = nextSeedData.seedPacketSprite;
            }
        }
        else
        {
            Debug.LogError("O Prefab da Semente não tem o componente IconFoodItem.");
        }
    }
}