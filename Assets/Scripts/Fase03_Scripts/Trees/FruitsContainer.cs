using System;
using Fase03_Scripts.Fruit;
using Helper.EventBusFolder;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fase03_Scripts.Trees
{
    public class FruitsContainer : MonoBehaviour
    {
        [SerializeField] private FruitController[] _fruits;

        [Header("Sintony Settings")] [SerializeField]
        private float _myTargetValue;
        [SerializeField] private float _tuningRange = 0.2f;

        private FruitType targetType;
        private TamanhoModification targetTamanho;
        private CorModification targetCor;
        private FormaModification targetForma;
        
        public void Initialize(FruitController patternReference, float targetValue)
        {
            targetType = patternReference.Type;
            targetTamanho = patternReference.Tamanho;
            targetCor = patternReference.Cor;
            targetForma = patternReference.Forma;
            _myTargetValue = targetValue;
            Debug.Log($"Target Value da {gameObject} é: {targetValue}");
            EventBus.Subscribe<OnSliderChangeEvent>(ChangeFruit);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnSliderChangeEvent>(ChangeFruit);
        }

        private void ChangeFruit(OnSliderChangeEvent eventData)
        {
            float currentSlider = eventData.value;
            
            float distance = Mathf.Abs(currentSlider - _myTargetValue);
            if (distance <= _tuningRange)
            {
                SetFruits(targetType,targetTamanho,targetForma,targetCor);
            }
            else
            {

                GenerateNoise(currentSlider);

            }
        }

        private void SetFruits(FruitType type, TamanhoModification tamanho, FormaModification forma,
            CorModification cor)
        {
            foreach (var fruit in _fruits)
            {
                fruit.SetFruitModifications(type, tamanho, forma, cor);
            }
        }

        private void GenerateNoise(float seed)
        {
            int s = Mathf.FloorToInt(seed * 100);
            Random.InitState(s + gameObject.GetInstanceID());
            
            FruitType t = (FruitType)Random.Range(0, 3);
            TamanhoModification tam = (TamanhoModification)Random.Range(0, 3);
            CorModification cor = (CorModification)Random.Range(0, 3);
            FormaModification forma = (FormaModification)Random.Range(0, 3);
            
            SetFruits(t,tam,forma,cor);
        }
    }
}