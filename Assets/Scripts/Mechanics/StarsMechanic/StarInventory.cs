using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StarInventory", menuName = "Game/Star Inventory")]
public class StarInventory : ScriptableObject
{
    private const string StarKey = "PlayerStars";

    // Acessível em runtime
    public int CurrentStars { get; private set; }

    // Chamado no Awake do ChallengeSelector ou de um Game Manager para carregar o estado
    public void LoadStars()
    {
        CurrentStars = PlayerPrefs.GetInt(StarKey, 0);
    }

    public void AddStars(int amount)
    {
        if (amount < 0) return;
        CurrentStars += amount;
        SaveStars();
    }

    // Método para limpar o inventário (solicitado)
    public void ClearStars()
    {
        CurrentStars = 0;
        SaveStars();
        Debug.Log("Inventário de Estrelas limpo.");
    }

    public void SaveStars()
    {
        PlayerPrefs.SetInt(StarKey, CurrentStars);
        PlayerPrefs.Save();
    }
}