using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Se estiver usando UI.Image, use esta linha

public class Plant_Controller : MonoBehaviour
{
    // --- Configurações da Planta ---
    [Header("Growth Settings")]
    [Tooltip("Tempo total em segundos para a planta atingir a fase final.")]
    public float totalGrowthTime = 600f; // Ex: 10 minutos
    
    [Tooltip("Valor em segundos que cada desafio concluído reduz no tempo de crescimento.")]
    public float timeReductionPerChallenge = 60f; // Ex: 1 minuto

    [Header("Visuals")]
    [Tooltip("Lista de Sprites ou GameObjects para as fases de crescimento.")]
    public Sprite[] growthStages; 
    
    private Image plantImage; 

   
    private float currentGrowthProgress = 0f; 
    private int currentGrowthStage = 0;
    
    
    private float remainingGrowthTime; 
    
    private Coroutine activeCoroutine;
    
    void Awake()
    {
        
        SubscribeToEvents();
        
        
        remainingGrowthTime = totalGrowthTime;
        
        // Pega o componente visual
        plantImage = GetComponent<Image>(); // Ou GetComponent<SpriteRenderer>();
        UpdateVisualStage();
    }

    void OnDestroy()
    {
        // É CRUCIAL se desinscrever do evento ao destruir o objeto
        UnsubscribeFromEvents();
    }
    
    // --- Lógica de Eventos C# ---

    private void SubscribeToEvents()
    {
        // Subscrição: Adiciona o método 'OnStarGained' à lista de chamadas do evento
        Game_Events.OnChallengeCompleted += OnStarGained;
        
        // Subscrição para redução de tempo
        Game_Events.OnChallengeConcluded += ReduceGrowthTime; 
    }

    private void UnsubscribeFromEvents()
    {
        // Desinscrição: Remove o método da lista de chamadas
        Game_Events.OnChallengeCompleted -= OnStarGained;
        Game_Events.OnChallengeConcluded -= ReduceGrowthTime; 
    }

    // --- Métodos Chamados pelo Evento ---

    private void OnStarGained(int stars)
    {
        // Lógica para adicionar uma nova planta se o campo estiver vazio ou
        // qualquer lógica que dependa do número de estrelas
        Debug.Log($"Planta notificada: O jogador ganhou {stars} estrelas.");
    }
    
    private void ReduceGrowthTime()
    {
        if (currentGrowthProgress < 1f)
        {
            remainingGrowthTime -= timeReductionPerChallenge;
            remainingGrowthTime = Mathf.Max(0f, remainingGrowthTime); // Garante que não vá abaixo de zero
            Debug.Log($"Tempo de crescimento restante reduzido. Novo tempo: {remainingGrowthTime}s");
        }
    }

    // --- Lógica de Crescimento ---
    
    public void StartGrowing()
    {
        // Inicia a coroutine para crescimento gradual (se não estiver crescendo)
        if (activeCoroutine == null)
        {
             activeCoroutine = StartCoroutine(GrowPlantCoroutine());
        }
    }

    private IEnumerator GrowPlantCoroutine()
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        // Calcula a duração real do crescimento após reduções de tempo
        float effectiveDuration = remainingGrowthTime; 
        
        // Crescimento de 0% (início) até 100% (final)
        while (currentGrowthProgress < 1f)
        {
            // Calcula o progresso baseado no tempo decorrido
            elapsedTime = Time.time - startTime;
            currentGrowthProgress = elapsedTime / effectiveDuration;
            
            // Garante que o progresso não ultrapasse 1.0
            currentGrowthProgress = Mathf.Clamp01(currentGrowthProgress); 

            // Atualiza a fase visual
            UpdateVisualStage();

            // Suspende a execução e continua no próximo frame (crescimento gradual)
            yield return null; 
        }

        // Garante a fase final
        currentGrowthProgress = 1f;
        UpdateVisualStage();
        Debug.Log("Crescimento da planta concluído!");
        activeCoroutine = null;
    }

    private void UpdateVisualStage()
    {
        if (growthStages.Length == 0) return;
        
        // Calcula a fase atual (de 0 a N-1)
        int maxStages = growthStages.Length - 1;
        
        // Mapeia o progresso (0.0 a 1.0) para a fase (0 a maxStages)
        int targetStage = Mathf.FloorToInt(currentGrowthProgress * maxStages);
        
        // Se a fase mudou, atualiza o sprite/GameObject
        if (targetStage != currentGrowthStage)
        {
            currentGrowthStage = targetStage;
            if (plantImage != null)
            {
                plantImage.sprite = growthStages[currentGrowthStage];
            }
            // Se estiver usando GameObjects, você ativaria o GameObject do estágio atual
        }
    }
}