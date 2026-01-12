using System;
using System.Collections.Generic;
using Fase03_Scripts.Basket;
using Fase03_Scripts.Fruit;
using Fase03_Scripts.Sun;
using Fase03_Scripts.Trees;
using Helper.EventBusFolder;
using Unity.VisualScripting;
using UnityEngine;
using EventBus = Helper.EventBusFolder.EventBus;
using Random = UnityEngine.Random;

namespace Fase03_Scripts
{
    public class Challenge03_Manager : MonoBehaviour
    {
        // [SerializeField] private Transform _basketContainer;
        // [SerializeField] private Transform _treeContainer;
        
        [SerializeField] private FruitBasketPatterns _fruitBasketPatterns;
        [SerializeField] private Challenge03_Controller _challengeController;
        [SerializeField] private FruitsContainer[] _fruitContainers;

      [SerializeField] private GameObject _lockedPanel;
        private bool isUnlocked;
        [SerializeField] private int level = 3;

        private void OnEnable()
        {
            EventBus.Subscribe<OnThirdLevelInitiateEvent>(InitializeChallenge);
            EventBus.Subscribe<OnPatternsGeneratedEvent>(OnPatternsReady);
            EventBus.Subscribe<OnNextLevelEvent>(UnlockLevel);
        }


        private void OnDisable()
        {
            EventBus.Unsubscribe<OnThirdLevelInitiateEvent>(InitializeChallenge);
            EventBus.Unsubscribe<OnPatternsGeneratedEvent>(OnPatternsReady);
            EventBus.Unsubscribe<OnNextLevelEvent>(UnlockLevel);
        }

        private void UnlockLevel(OnNextLevelEvent levelEvent)
        {
            if (levelEvent.Level == level && !isUnlocked)
            {
                isUnlocked = true;
            }
        }
        private void InitializeChallenge(OnThirdLevelInitiateEvent eventData)
        {
            if (!isUnlocked) return;
            if(_lockedPanel.activeSelf) _lockedPanel.SetActive(false);
            
         _fruitBasketPatterns.Initialize();
         _challengeController.Initialize();
        }

        private void OnPatternsReady(OnPatternsGeneratedEvent eventData)
        {
            List<FruitController> availablePatterns = new List<FruitController>(eventData.patterns);
            List<float> usedSliderValues = new();

            for (int i = 0; i < _fruitContainers.Length; i++)
            {
                if (availablePatterns.Count == 0) break;
                
                int randomIndex = Random.Range(0, availablePatterns.Count);
                FruitController selectedPattern = availablePatterns[randomIndex];
                availablePatterns.RemoveAt(randomIndex);

                float targetSlider = GenerateUniqueSliderValue(usedSliderValues);
                usedSliderValues.Add(targetSlider);
                
                _fruitContainers[i].Initialize(selectedPattern, targetSlider);
           Debug.Log("containers de fruta inicializados");
            }
        }

        private float GenerateUniqueSliderValue(List<float> usedSliderValues)
        {
            float val;
            int safetyNet = 0;
            do
            {
                val = Random.Range(0.1f, 0.9f);
                safetyNet++;
            } while(IsValueTooClose(val, usedSliderValues) && safetyNet < 100);
            return val;
        }

        private bool IsValueTooClose(float val, List<float> used)
        {
            foreach (var u in used)
            {
                if (Mathf.Abs(val - u) < 0.15f) return true; // distancia minima entre valores
            }

            return false;
        }

        [ContextMenu( "Initialize Fase03 Teste" )]
        public void InitializeTest()
        {
            _fruitBasketPatterns.Initialize();
            _challengeController.Initialize();
        }
    }
}
