using UnityEngine;
using System.Collections.Generic; // Necessário para List<T>

public class FarmManager : MonoBehaviour
{
    [Header("Id Reference")] [SerializeField] private int farmId;
    [SerializeField] private int requiredStars = 9; 

    [Header("Planting Settings")]
    [Tooltip("Todos os GameObjects vazios que representam os locais de plantio.")]
    public Transform[] plantingSlots; 
    
    [Tooltip("O Prefab da planta que será instanciada.")]
    public GameObject plantPrefab;
    
    [Header("Rendering Settings")]
    [Tooltip("O ponto de início da ordem de renderização. Plantas mais baixas terão ordem maior.")]
    public int baseSortingOrder = 100; 

    private Plant_Controller[] plantedPlants; // Armazena as referências das plantas instanciadas

    [SerializeField] private Animator CadeadoAnimator;

    public bool unlocked;
    void Awake()
    {
   
        plantedPlants = new Plant_Controller[plantingSlots.Length];
    }
    
    public int GetFarmId() => farmId;
    public int GetRequiredStars() => requiredStars;
    
    
    
    public void PlaceNewPlant(int slotIndex, int starsGained)
    {
       
        if (slotIndex < 0 || slotIndex >= plantingSlots.Length || plantedPlants[slotIndex] != null)
        {
            return;
        }
        
        Transform slot = plantingSlots[slotIndex];
        
        GameObject newPlantObject = Instantiate(plantPrefab, slot.position, Quaternion.identity, slot);
        Plant_Controller plantController = newPlantObject.GetComponent<Plant_Controller>();

        if (plantController == null)
        {
            Debug.LogError("O Prefab da planta deve ter o componente Plant_Controller.");
            Destroy(newPlantObject);
            return;
        }
        
       
        ApplyDepthSorting(newPlantObject, slotIndex);

        plantedPlants[slotIndex] = plantController;
        plantController.StartGrowing();
        
        // Opcional: Salva o estado imediatamente após o plantio (para o caso de crash)
        //plantController.SavePlantState(slotIndex); 
    }

    
    public void ProcessLoadedPlants(List<PlantData> loadedPlantsData)
    {
        foreach (PlantData data in loadedPlantsData)
        {
            int slotIndex = data.slotIndex;
            
            if (slotIndex < 0 || slotIndex >= plantingSlots.Length)
            {
                Debug.LogError($"Erro ao carregar: Índice do slot ({slotIndex}) inválido.");
                continue;
            }

            // 1. Instanciar
            Transform slot = plantingSlots[slotIndex];
            GameObject newPlantObject = Instantiate(plantPrefab, slot.position, Quaternion.identity, slot);
            Plant_Controller plantController = newPlantObject.GetComponent<Plant_Controller>();
            
            if (plantController == null)
            {
                 Debug.LogError("Erro ao carregar: Planta instanciada não possui Plant_Controller.");
                 Destroy(newPlantObject);
                 continue;
            }

           
            ApplyDepthSorting(newPlantObject, slotIndex);

            // 3. Restaurar o estado (crescimento offline)
           // plantController.RestorePlantState(data);
            
           
            plantedPlants[slotIndex] = plantController;
        }

        Debug.Log($"Carregamento concluído. {loadedPlantsData.Count} plantas restauradas.");
    }
    
  

    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < plantedPlants.Length; i++)
        {
            if (plantedPlants[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
    
    // Método CHAVE para a ilusão 3D
    private void ApplyDepthSorting(GameObject plantObject, int slotIndex)
    {
        int sortingOrder = baseSortingOrder + (plantingSlots.Length - 1 - slotIndex); 
        
        // Se estiver usando SpriteRenderer:
        SpriteRenderer sr = plantObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = sortingOrder;
        }
        // ... (lógica para vários filhos, se necessário)
    }

    public void HandleStarReward(int starsGained)
    {
        if (!unlocked) return;
        Debug.Log($"Evento Game_Events recebido: {starsGained} estrelas. Tentando plantar uma planta para cada estrela.");

        for (int star = 0; star < starsGained; star++)
        {
            int slotIndex = FindFirstEmptySlot();

            if (slotIndex != -1)
            {
                PlaceNewPlant(slotIndex, 1);
                Debug.Log($"Planta #{star + 1} colocada no Slot {slotIndex}.");
            }
            else
            {
                Debug.LogWarning("Campo de plantio cheio! Nem todas as estrelas foram convertidas em plantas.");
                break; 
            }
        }
    }
    
    // --- EVENTOS E TESTE ---
    
    // Subscrição ao evento de estrelas
    void OnEnable()
    {
        Game_Events.OnChallengeCompleted += HandleStarReward;
    }

    void OnDisable()
    {
        Game_Events.OnChallengeCompleted -= HandleStarReward;
    }
    
    // (A classe ChallengeTester pode permanecer separada ou ser integrada, dependendo da sua preferência)
    public void UnlockFarm()
    {
        unlocked = true;
        CadeadoAnimator.Play("Unlocked");
        //TODO:
        //som de Unlock
    }
}