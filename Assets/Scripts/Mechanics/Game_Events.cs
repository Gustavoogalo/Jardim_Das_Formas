using System;
using Mechanics.StarsMechanic;
using UnityEngine;

// Anexe este script a um GameObject vazio (ex: "GameManager")
public class Game_Events : MonoBehaviour
{
    public static event Action<int> OnChallengeCompleted; 
    public static event Action OnChallengeConcluded;

    private static FarmManager currentFarm;
    
    public static void ChallengeCompleted(int starsGained)
    {
        
        OnChallengeCompleted?.Invoke(starsGained);
        
       
        OnChallengeConcluded?.Invoke();
    }

    public static void SetCurrentFarm(FarmManager farmManager)
    {
        currentFarm = farmManager;
    }
    
    public static FarmManager GetCurrentFarm() => currentFarm;

   
}