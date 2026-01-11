using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fase03_Scripts.Basket;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Fase03_Scripts.Fruit
{
    public enum TamanhoModification
    {
        Grande,
        Medio,
        Pequeno
    }
    public enum FormaModification
    {
        Quadrado,
        Circulo,
        Triangulo
    }
    public enum CorModification
    {
        Vermelho,
        Amarelo,
        Verde
    }
    public enum FruitType
    {
        Apple,
        Pear,
        Banana
    }

    [Serializable]
    public class FruitVisualConfig
    {
        public FruitType fruitType;
        public Sprite sprite;
    }

    [Serializable]
    public class SizeConfig
    {
        public TamanhoModification tamanho;
        public float scale;
    }

    [Serializable]
    public class ColorConfig
    {
        public CorModification colorEnum;
        public Color colorValue;
    }

    [Serializable]
    public class FormaConfig
    {
        public FormaModification formaEnum;
        public Sprite maskSprite;
    }
    public class FruitController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Settings")]
        [SerializeField] private FruitType _fruitType;
        [SerializeField] private TamanhoModification _tamanho;
        [SerializeField] private FormaModification _forma;
        [SerializeField] private CorModification _cor;

        [Header("Visual Customization")] [SerializeField]
        private Image _imageComponent;

        [SerializeField] private Image _maskImage;

        [SerializeField] private List<FruitVisualConfig> _typeConfigs;
        [SerializeField] private List<FormaConfig> _formaConfigs;
        [SerializeField] private List<SizeConfig> _sizeConfigs;
        [SerializeField] private List<ColorConfig> _colorConfigs;
        
        
        public bool isRef = false;

        private Vector3 _startPos;
        private CanvasGroup _canvasGroup;
        
        public FruitType Type => _fruitType;
        public TamanhoModification Tamanho => _tamanho;
        public FormaModification Forma => _forma;
        public CorModification Cor => _cor;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if(_imageComponent == null) _imageComponent = GetComponent<Image>();
            UpdateVisuals();
            if (isRef)
            {
                _imageComponent.color = new Color(
                    _imageComponent.color.r, _imageComponent.color.g, _imageComponent.color.b, 0.5f);
            }
        }
        
        public void SetFruitModifications(FruitType type,TamanhoModification tamanho, FormaModification forma, CorModification cor)
        {
            _fruitType = type;
            _tamanho = tamanho;
            _forma = forma;
            _cor = cor;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var typeCfg = _typeConfigs.FirstOrDefault(cfg => cfg.fruitType == _fruitType);
            if(typeCfg != null && _imageComponent != null) _imageComponent.sprite = typeCfg.sprite;

            var formaCfg = _formaConfigs.FirstOrDefault(cfg => cfg.formaEnum == _forma);
            if(formaCfg != null && _maskImage != null) _maskImage.sprite = formaCfg.maskSprite;
            
            var sizeCfg = _sizeConfigs.FirstOrDefault(cfg => cfg.tamanho == _tamanho);
            if(sizeCfg != null && _imageComponent != null) _imageComponent.rectTransform.localScale = new Vector3(sizeCfg.scale, sizeCfg.scale, 1);

            ApplyColorModification();
        }

        private void ApplyColorModification()
        {
            if(_imageComponent == null) return;
           
            var coloCfg = _colorConfigs.FirstOrDefault(cfg => cfg.colorEnum == _cor);
            Color targetColor = coloCfg != null ? coloCfg.colorValue : Color.white;

            float alpha = isRef ? 0.5f : 1f;
            _imageComponent.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isRef) return;
            _startPos = transform.position;
            if(_canvasGroup) _canvasGroup.blocksRaycasts = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isRef) return;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            transform.position = mousePos;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(isRef) return;
            if(_canvasGroup) _canvasGroup.blocksRaycasts = true;
            GameObject hit = eventData.pointerCurrentRaycast.gameObject;
            if (hit != null && hit.TryGetComponent<FruitBasketPatterns>(out var basket))
            {
                basket.VerifyFruit(this);
                Debug.Log("Dropped");
            } else
            {
              transform.position = _startPos;
              Debug.Log("Dropped out of basket");
            }
        }

        public IEnumerator FallAnimation(Vector3 start, float height, float ranged, float duration)
        {
            float elapsed = 0;
            Vector3 targetPos = start + new Vector3(ranged, -500f, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float x = Mathf.Lerp(start.x, targetPos.x, t);
                float y = start.y + (height * 4 * (t - t * t)) - (t * 800f);
                
                transform.position = new Vector3(x, y, 0);
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}