using System.Collections;
using Fase03_Scripts.Basket;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
    
    public class FruitController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private FruitType _fruitType;
        [SerializeField] private TamanhoModification _tamanho;
        [SerializeField] private FormaModification _forma;
        [SerializeField] private CorModification _cor;

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
        }
        
        // public bool VerifyFruit(FruitType type,TamanhoModification tamanho, FormaModification forma, CorModification cor)
        // {
        //     return (type == _fruitType && tamanho == _tamanho && forma == _forma && cor == _cor);
        // }
        
        public void SetFruitModifications(FruitType type,TamanhoModification tamanho, FormaModification forma, CorModification cor)
        {
            _fruitType = type;
            _tamanho = tamanho;
            _forma = forma;
            _cor = cor;
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