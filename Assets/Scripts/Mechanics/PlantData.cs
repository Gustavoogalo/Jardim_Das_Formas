using System;
using UnityEngine;

// 💡 O atributo Serializable é necessário para que o JsonUtility do Unity
// consiga converter esta classe para JSON e vice-versa.
[Serializable]
public class PlantData
{
    public int slotIndex;
    
    // O tempo em que a planta foi plantada, salvo como string no formato ISO
    public string plantedISOTime;       
    
    // O tempo total de crescimento inicial (para referência)
    public float initialTotalGrowthTime; 
    
    // O tempo total de crescimento que foi reduzido por desafios concluídos
    public float timeReduced;            
    
    // Você pode adicionar mais campos aqui, como o tipo de planta (se tiver mais de um)
    // public PlantType type; 
}