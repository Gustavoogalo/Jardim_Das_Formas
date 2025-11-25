using Mechanics.Selector.Selector;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics.Selector.MVVM_Selector
{
    public enum IconType
    {
        quadrado, triangulo, circulo, retangulo
    }
    public enum IconColor
    {
        verde, vermelho, azul, amarelo
    }
    public enum IconSize
    {
        grande, pequeno, medio
    }
    public class FormIconView : MonoBehaviour
    {
        [SerializeField] private Image iconImageComponent;
        
        public IconType Type;
        public IconColor Color;
        public IconSize Size;

        public void Setup(IconData data, Sprite sprite)
        {
            Type = data.Type;
            Color = data.Color;
            Size = data.Size;

            if (iconImageComponent != null)
            {
                iconImageComponent.sprite = sprite;
            }
            else
            {
                Debug.LogError("Componente Image não conectado no FormIconView de " + gameObject.name);
            }
        }
    }
}