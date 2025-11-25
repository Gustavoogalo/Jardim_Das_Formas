using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Mechanics.Selector.MVVM_Selector;
using Mechanics.Selector.Selector; // Para acessar IconData

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

    [SerializeField]
    private List<IconMapping> Mappings = new List<IconMapping>();

    public Sprite GetSprite(IconType type, IconColor color, IconSize size)
    {
        var mapping = Mappings.FirstOrDefault(m => 
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

    // MÉTODO REQUERIDO PELO ChallengeSelector
    public List<IconData> GetAllValidIconData()
    {
        // Retorna a lista de todas as combinações IconData que possuem um Sprite configurado.
        return Mappings.Select(m => m.ToIconData()).ToList();
    }
}