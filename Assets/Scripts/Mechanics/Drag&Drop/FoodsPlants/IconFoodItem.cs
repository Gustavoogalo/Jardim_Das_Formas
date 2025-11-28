using Mechanics.Drag_Drop.FoodsPlants;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IconFoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public IconFoodType foodType;
    public CanvasGroup canvasGroup; 
    public RectTransform rectTransform;

    [HideInInspector] public Transform originalParent;
    [HideInInspector] public ItemSlotManager sourceSlotManager;
    private static IconFoodItem draggedItem;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (foodType == IconFoodType.None) Debug.LogError("IconFoodItem não tem um tipo definido!");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        draggedItem = this;
        originalParent = rectTransform.parent;
        sourceSlotManager = originalParent.GetComponent<ItemSlotManager>(); // Adicionado
        // Coloca o item na frente de tudo e desliga o raycast
        rectTransform.SetParent(rectTransform.root); 
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Segue o mouse
        rectTransform.anchoredPosition += eventData.delta / rectTransform.root.localScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggedItem = null;
        canvasGroup.blocksRaycasts = true;
        
        // Se o item não foi solto em um slot válido (FarmSlot), ele retorna
        if (rectTransform.parent == rectTransform.root)
        {
            rectTransform.SetParent(originalParent);
            rectTransform.localPosition = Vector3.zero;
        }
    }
    
    // Método estático para o FarmSlot verificar se há um item sendo arrastado
    public static IconFoodItem GetDraggedItem()
    {
        return draggedItem;
    }
}