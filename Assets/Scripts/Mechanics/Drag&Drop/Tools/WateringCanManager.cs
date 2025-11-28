using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class WateringCanManager : MonoBehaviour
{
    [Header("References")]
    public Image wateringCanImage;
    public RectTransform initialWateringCanSlot;
    
    // Ferramenta que se move e interage (deve ter Collider 2D)
    public GameObject draggableWateringCan; 

    public enum ActiveTool { None, WateringCan }
    public ActiveTool CurrentTool { get; private set; } = ActiveTool.None;
    private RectTransform activeToolTransform;

    void Update()
    {
        // Faz o regador ativo seguir o mouse
        if (CurrentTool == ActiveTool.WateringCan && activeToolTransform != null)
        {
            Vector2 mousePos;
        
            // 1. Obter a posição da tela (Screen Position) usando o Novo Input System
            Vector3 screenPosition = Mouse.current.position.ReadValue(); 
        
            // 2. Usar a posição obtida para mapear para o Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)activeToolTransform.root, 
                screenPosition, // Usamos a variável que contém a posição do Input System
                null, 
                out mousePos);
        
            activeToolTransform.localPosition = mousePos;
        }
        
        // Se o regador estiver ativo, o clique com o botão esquerdo o desativa
        // if (CurrentTool == ActiveTool.WateringCan && Input.GetMouseButtonDown(0))
        // {
        //     DeselectTool();
        // }
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
            // 1. Desativa o ícone fixo e ativa o objeto draggable (com o Collider 2D)
            wateringCanImage.gameObject.SetActive(false);
            draggableWateringCan.SetActive(true);
            
            CurrentTool = ActiveTool.WateringCan;
            activeToolTransform = draggableWateringCan.GetComponent<RectTransform>();
            
            // Move para o Canvas Root para sobrepor
            activeToolTransform.SetParent(activeToolTransform.root);
            
            // O componente WateringCanInteraction no draggable vai cuidar da lógica de regar
        }
    }
    
    // --- RESET ---
    public void DeselectTool()
    {
        if (CurrentTool == ActiveTool.WateringCan)
        {
            // 1. Retorna o objeto draggable e desativa-o
            draggableWateringCan.SetActive(false);
            
            // 2. Retorna o ícone fixo para o slot inicial
            wateringCanImage.gameObject.SetActive(true);
            wateringCanImage.transform.SetParent(initialWateringCanSlot);
            wateringCanImage.rectTransform.localPosition = Vector3.zero;
        }
        CurrentTool = ActiveTool.None;
        activeToolTransform = null;
    }
    
    public void HandleClickInput()
    {
        // Só desativa se o regador estiver ativo.
        if (CurrentTool == ActiveTool.WateringCan)
        {
            DeselectTool();
        }
    }
}