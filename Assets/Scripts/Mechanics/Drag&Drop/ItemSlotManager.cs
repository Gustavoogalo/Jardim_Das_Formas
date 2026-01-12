using System;
using Helper.EventBusFolder;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mechanics.Drag_Drop.FoodsPlants;

public class ItemSlotManager : MonoBehaviour
{
    public GameObject iconFoodItemPrefab;
    public RectTransform spawnPoint;
    public Image backgroundImage;

    private SequenceManager sequenceManager;

    void InitializeSlotManager(OnSequenceInitialized eventData)
    {
        sequenceManager = eventData.sequenceManager;

        if (sequenceManager != null)
        {
            foreach (Transform child in spawnPoint)
            {
                Destroy(child.gameObject);
            }
            
            var uniqueFoods = sequenceManager.GetUniqueAvailableFoods();
            foreach (var foodData in uniqueFoods)
            {
                SpawnSpecificItem(foodData);
            }
        }
        else
        {
            Debug.LogError("SequenceManager não encontrado na cena. O ItemSlotManager não pode funcionar.");
        }
    }

    private void SpawnSpecificItem(IconFoodData data)
    {
        GameObject newItemObject = Instantiate(iconFoodItemPrefab, spawnPoint);
        newItemObject.transform.localPosition = Vector3.zero;
        IconFoodItem newItemController = newItemObject.GetComponent<IconFoodItem>();
        if (newItemController == null) return;
        
        newItemController.foodType = data.type;
        newItemController.sourceSlotManager = this;
        Image itemImage = newItemController.GetComponent<Image>();
        if (itemImage != null && data.seedPacketSprite != null)
        {
            itemImage.sprite = data.seedPacketSprite;
        }
    }

    public void RepositionUsedItem(IconFoodType type)
    {
        IconFoodData data = sequenceManager.GetFoodDataFromType(type);
        if (data != null)
        {
            SpawnSpecificItem(data);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<OnSequenceInitialized>(InitializeSlotManager);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<OnSequenceInitialized>(InitializeSlotManager);
    }
}