using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mechanics.Drag_Drop;
using Mechanics.Drag_Drop.FoodsPlants;

public class FoodPlantController : MonoBehaviour
{
    [Header("Drop Effect")]
    public float dropDuration = 0.5f; // Tempo para cair
    public float dropDistance = 1000f;
    [Header("Growth Settings")]
    public float growthTime = 10f; // Tempo total para crescer (após regar)
    
    [Header("References")]
    public Image plantImage; // O componente visual da planta

    private FarmSlot parentSlot;
    private IconFoodType plantType;
    private PlantState currentState = PlantState.Seed;
    private IconFoodData plantData;
    private Coroutine growthCoroutine;

    public IconFoodType GetPlantType() => plantType;
    public PlantState GetCurrentState() => currentState;
    
    public void Initialize(FarmSlot slot, IconFoodType type, IconFoodData data)
    {
        parentSlot = slot;
        plantType = type;
        plantData = data;
        
        // Estado inicial: Semente (Apenas o sprite de semente)
        if (plantImage != null)
        {
            plantImage.sprite = plantData.seedStageSprite;
            // Configurações de tamanho e posição para o slot (se necessário)
        }
    }
    
    // Chamado pelo REGADOR
    public void WaterPlant()
    {
        if (currentState == PlantState.Seed)
        {
            currentState = PlantState.Growing;
            Debug.Log($"{plantType} recebeu água. Começando a crescer.");
            
            if (growthCoroutine != null) StopCoroutine(growthCoroutine);
            growthCoroutine = StartCoroutine(StartGrowth());
        }
    }
    
    private IEnumerator StartGrowth()
    {
        float timer = 0f;
        
        while (timer < growthTime)
        {
            timer += Time.deltaTime;
            // Opcional: Animação de crescimento gradual (tamanho, cor)
            
            yield return null;
        }
        
        // Crescimento Completo
        currentState = PlantState.Grown;
        if (plantImage != null)
        {
            plantImage.sprite = plantData.grownStageSprite;
        }
        
        Debug.Log($"{plantType} Cresceu completamente!");
        parentSlot.NotifyGrown(); // Notifica o slot, que notifica a linha
    }
    
    public void StartDropEffect()
    {
        // 1. Remove o pai (o FarmSlot) e o Layout Group para que ele possa se mover livremente.
        // O objeto precisa ser colocado na raiz do Canvas ou em um Parent que não seja Layout Group.
        Transform originalRoot = transform.root;
        transform.SetParent(originalRoot);
        
        // 2. Garante que o objeto não seja destruído imediatamente (o Destroy virá no final da Coroutine)
        if (growthCoroutine != null) StopCoroutine(growthCoroutine);
        
        // 3. Inicia o efeito de queda
        StartCoroutine(DoDropEffect());
    }
    
    private IEnumerator DoDropEffect()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + Vector3.down * dropDistance; // Cai para baixo
        float startTime = Time.time;
        
        while (Time.time < startTime + dropDuration)
        {
            float t = (Time.time - startTime) / dropDuration;
            // Efeito de queda: pode usar Ease-in/out se quiser, mas Linear funciona
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Limpeza final: remove o objeto da cena
        Destroy(gameObject);
    }
}