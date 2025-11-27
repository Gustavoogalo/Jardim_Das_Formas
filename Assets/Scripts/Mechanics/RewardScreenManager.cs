using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Opcional, se precisar de transição de cena

// Anexado ao GameObject do Painel de Rewards
public class RewardScreenManager : MonoBehaviour
{
    [Header("Visual Elements")]
    [Tooltip("As imagens/GameObjects das 3 estrelas.")]
    public List<GameObject> starVisuals = new List<GameObject>(3);

    [Header("Screen Control")]
    [Tooltip("A tela/painel que estava ativo antes desta tela de Recompensa (Ex: Challenge Selector).")]
    public GameObject previousScreen;

    void Awake()
    {
        // Garante que o painel esteja inicialmente oculto
        gameObject.SetActive(false);
    }
    
    // Método para ser chamado pela tela anterior (Challenge Selector)
    public void ShowRewards(int starsGained, GameObject callerScreen)
    {
        // 1. Guarda a referência da tela que chamou para poder fechá-la
        previousScreen = callerScreen;
        
        // 2. Garante que a tela de Rewards fique na frente
        gameObject.SetActive(true);
        transform.SetAsLastSibling(); // Move o painel para o final da hierarquia (renderiza por cima)

        // 3. Exibe as estrelas
        DisplayStars(starsGained);
        
        // Opcional: Pausa o jogo (se não estiver pausado)
        // if (Time.timeScale != 0f)
        // {
        //     Time.timeScale = 0f;
        // }
    }
    
    private void DisplayStars(int starsGained)
    {
        
        foreach (GameObject star in starVisuals)
        {
            star.SetActive(false);
        }

        
        for (int i = 0; i < starsGained && i < starVisuals.Count; i++)
        {
            starVisuals[i].SetActive(true);
           
        }
        
        
        //Game_Events.ChallengeCompleted(starsGained);
    }
    
    public void OnContinueButtonPressed()
    {
        // 1. Resgata o tempo (Despausa o jogo)
        // Time.timeScale = 1f;

        // 2. Fecha a tela de Rewards
        gameObject.SetActive(false);

        // 3. Fecha a tela anterior (Challenge Selector)
        if (previousScreen != null)
        {
            previousScreen.SetActive(false);
        }
    }
}