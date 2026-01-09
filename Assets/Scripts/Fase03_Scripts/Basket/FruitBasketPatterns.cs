using System.Collections;
using Fase03_Scripts.Fruit;
using Helper.EventBusFolder;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using EventBus = Helper.EventBusFolder.EventBus;

namespace Fase03_Scripts.Basket
{
    public class FruitBasketPatterns : MonoBehaviour
    {
        [SerializeField] private FruitController[] _fruitController;
        [SerializeField] private Transform[] _container;

        [Header("Incorrect Settings")] [SerializeField]
        private float jumpHeight = 200f;

        [SerializeField] private float jumpRange = 150f;
        [SerializeField] private int _correctFruitsCount = 0;
        public void Initialize()
        {
            GenerateModifications();
        }

        private void GenerateModifications()
        {
            foreach (var fruit in _fruitController)
            {
                FruitType tipo = (FruitType)Random.Range(0, 3);
                TamanhoModification tamanho = (TamanhoModification)Random.Range(0, 3);
                CorModification cor = (CorModification)Random.Range(0, 3);
                FormaModification forma = (FormaModification)Random.Range(0, 3);
                fruit.SetFruitModifications(tipo, tamanho, forma, cor);
            }
            
            EventBus.Publish(new OnPatternsGeneratedEvent(_fruitController));
        }

        public void VerifyFruit(FruitController droppedFruit)
        {
            bool foundMatch = false;
            foreach (var pattern in _fruitController)
            {
                if (pattern.Type == droppedFruit.Type &&
                    pattern.Tamanho == droppedFruit.Tamanho &&
                    pattern.Cor == droppedFruit.Cor &&
                    pattern.Forma == droppedFruit.Forma)
                {
                    ApplySuccess(pattern, droppedFruit);
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                StartCoroutine(HandleIncorrectSequence(droppedFruit));
            }
        }

        private void ApplySuccess(FruitController pattern, FruitController droppedFruit)
        {
            Image img = pattern.GetComponent<Image>();
            if (img != null)
            {
                if (img.color.a < 1f)
                {
                img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
                _correctFruitsCount++;
                }
            }

            droppedFruit.gameObject.SetActive(false);
            Debug.Log("Success");
            if (_correctFruitsCount >= _fruitController.Length)
            {
                CompleteLevel();
            }
        }

        private void CompleteLevel()
        {
            Debug.Log("Desafio 03 completo! Publicando evento...");
            EventBus.Publish(new OnThirdLevelCompletedEvent());
        }

        private IEnumerator HandleIncorrectSequence(FruitController dropped)
        {
            dropped.gameObject.SetActive(false);

            yield return new WaitForSeconds(1.0f);
            dropped.gameObject.SetActive(true);
            StartCoroutine(dropped.FallAnimation(dropped.transform.position, jumpHeight, jumpRange, 0.7f));
        }
    }
}