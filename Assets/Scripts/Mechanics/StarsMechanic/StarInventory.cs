using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StarInventory", menuName = "Game/Star Inventory")]
public class StarInventory : ScriptableObject
{
    private const string StarKey = "PlayerStars";
    public int CurrentStars { get; private set; }

    public void AddStars(int amount)
    {
        if (amount < 0) return;
        CurrentStars += amount;
    }

    // Método para limpar o inventário
    [ContextMenu( "Clear Stars" )]
    public void ClearStars()
    {
        CurrentStars = 0;
        Debug.Log("Inventário de Estrelas limpo.");
    }
}