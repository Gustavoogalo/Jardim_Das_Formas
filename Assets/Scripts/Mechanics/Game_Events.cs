using System;
using UnityEngine;

// Anexe este script a um GameObject vazio (ex: "GameManager")
public class Game_Events : MonoBehaviour
{
    // O evento que o Game_Events irá "transmitir"
    // Os métodos subscritos devem aceitar um int (o número de estrelas ganhas)
    public static event Action<int> OnChallengeCompleted; 
    
    // O evento para redução do tempo de crescimento (poderia ser separado)
    public static event Action OnChallengeConcluded; 
    
    // Método que a sua lógica de "Desafio Concluído" deve chamar
    public static void ChallengeCompleted(int starsGained)
    {
        
        OnChallengeCompleted?.Invoke(starsGained);
        
       
        OnChallengeConcluded?.Invoke();
    }
}