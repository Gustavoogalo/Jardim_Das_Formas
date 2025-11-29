using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Selector.MVVM_Selector;
using Mechanics.Selector.Selector;

[CreateAssetMenu(fileName = "IconSpriteMapper", menuName = "Game/Icon Sprite Mapper")]
public class IconSpriteMapper : ScriptableObject
{
    [System.Serializable]
    public class IconMapping
    {
        public IconType Type;
        public IconColor Color;
        public IconSize Size;
        public Sprite IconSprite;
        
        public IconData ToIconData()
        {
            return new IconData(Type, Color, Size);
        }
    }

    [Header("Mapeamentos por Categoria")]
    [Tooltip("Ícones onde a distinção principal é o TIPO.")]
    [SerializeField]
    private List<IconMapping> TypeMappings = new List<IconMapping>();
    
    [Tooltip("Ícones onde a distinção principal é a COR.")]
    [SerializeField]
    private List<IconMapping> ColorMappings = new List<IconMapping>();
    
    [Tooltip("Ícones onde a distinção principal é o TAMANHO.")]
    [SerializeField]
    private List<IconMapping> SizeMappings = new List<IconMapping>();

    public Sprite GetSprite(IconType type, IconColor color, IconSize size)
    {
        // Esta busca deve idealmente pesquisar em todas as listas,
        // pois um ícone é definido unicamente por TIPO, COR e TAMANHO,
        // independentemente de qual lista ele foi categorizado no editor.
        
        var allMappings = TypeMappings
            .Concat(ColorMappings)
            .Concat(SizeMappings)
            .Distinct();

        var mapping = allMappings.FirstOrDefault(m => 
            m.Type == type && 
            m.Color == color && 
            m.Size == size
        );

        if (mapping != null)
        {
            return mapping.IconSprite;
        }
        
        Debug.LogError($"Sprite não encontrado para: {type}, {color}, {size}");
        return null;
    }

    public List<IconData> GetAllValidIconData(bool isType, bool isColor, bool isSize)
    {
        List<IconMapping> combinedMappings = new List<IconMapping>();

        if (isType)
        {
            combinedMappings.AddRange(TypeMappings);
        }
        
        if (isColor)
        {
            combinedMappings.AddRange(ColorMappings);
        }
        
        if (isSize)
        {
            combinedMappings.AddRange(SizeMappings);
        }
        
        // Se nenhum booleano estiver ativo, usamos todos os mapeamentos para evitar um erro.
        if (combinedMappings.Count == 0)
        {
            combinedMappings.AddRange(TypeMappings);
            combinedMappings.AddRange(ColorMappings);
            combinedMappings.AddRange(SizeMappings);
            Debug.LogWarning("Nenhum critério (isType/isColor/isSize) ativo no ChallengeSelector. Usando todos os ícones disponíveis.");
        }

        // Remove duplicatas e retorna os dados do ícone.
        return combinedMappings
            .Distinct()
            .Select(m => m.ToIconData())
            .ToList();
    }
}