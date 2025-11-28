using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{
    [Header("References")]
    public Image hoeImage;
    public RectTransform initialHoeSlot; 
    public Image wateringCanImage;
    public RectTransform initialWateringCanSlot;
    
    public enum ActiveTool { None, Hoe, WateringCan }
    public ActiveTool CurrentTool { get; private set; } = ActiveTool.None;
    private RectTransform activeToolTransform;

    void Update()
    {
        // Faz a ferramenta ativa seguir o mouse
        if (CurrentTool != ActiveTool.None && activeToolTransform != null)
        {
            Vector2 mousePos;
            // Usa o RectTransform do Canvas para mapear a posição do mouse
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)activeToolTransform.root, 
                Input.mousePosition, 
                null, 
                out mousePos);
            
            activeToolTransform.localPosition = mousePos;
        }
    }

    // --- ENXADA ---
    public void SelectHoe()
    {
        if (CurrentTool == ActiveTool.Hoe)
        {
            DeselectTool();
        }
        else
        {
            DeselectTool(); 
            CurrentTool = ActiveTool.Hoe;
            activeToolTransform = hoeImage.GetComponent<RectTransform>();
            hoeImage.transform.SetParent(hoeImage.canvas.transform);
            hoeImage.raycastTarget = false;
        }
    }

    // Chamado pelo FarmSlot
    public void UseHoe(FarmSlot slot)
    {
        if (CurrentTool != ActiveTool.Hoe) return;
        
        slot.RemovePlant();
        DeselectTool(); 
    }

    // --- REGADOR ---
    public void SelectWateringCan()
    {
        if (CurrentTool == ActiveTool.WateringCan)
        {
            DeselectTool();
        }
        else
        {
            DeselectTool();
            CurrentTool = ActiveTool.WateringCan;
            activeToolTransform = wateringCanImage.GetComponent<RectTransform>();
            wateringCanImage.transform.SetParent(wateringCanImage.canvas.transform);
            wateringCanImage.raycastTarget = false;
        }
    }
    
    // Chamado pelo Regador ao passar por cima do FarmSlot
    public void WaterSlot(FarmSlot slot)
    {
        if (CurrentTool == ActiveTool.WateringCan && slot.currentPlant != null)
        {
            // O regador só afeta sementes
            slot.currentPlant.WaterPlant();
        }
    }

    // --- RESET ---
    public void DeselectTool()
    {
        if (CurrentTool == ActiveTool.Hoe)
        {
            hoeImage.transform.SetParent(initialHoeSlot);
            hoeImage.rectTransform.localPosition = Vector3.zero;
            hoeImage.raycastTarget = true;
        }
        else if (CurrentTool == ActiveTool.WateringCan)
        {
            wateringCanImage.transform.SetParent(initialWateringCanSlot);
            wateringCanImage.rectTransform.localPosition = Vector3.zero;
            wateringCanImage.raycastTarget = true;
        }
        CurrentTool = ActiveTool.None;
        activeToolTransform = null;
    }
}