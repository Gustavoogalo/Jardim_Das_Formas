using System;
using System.Collections;
using Helper.EventBusFolder;
using UnityEngine;
using UnityEngine.UI;

namespace Fase02_Scripts
{
    public class Phase02Manager : MonoBehaviour
    {
        [SerializeField] private int level = 2;
        
        [Header("Containers")]
        [SerializeField] private GameObject _TutorialFase02;

        [SerializeField] private Button _JumpTutorial;
        [SerializeField] private Button _FinalButtonTutorial;
        [SerializeField] private GameObject _NavigationSetas;


        [Header("Unlock Settings")] [SerializeField]
        private GameObject _lockedPanel;
        private bool isUnlocked;
        private void OnEnable()
        {
            EventBus.Subscribe<OnNextLevelEvent>(InitializateLevel);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnNextLevelEvent>(InitializateLevel);
        }

        private void InitializateLevel(OnNextLevelEvent levelEvent)
        {
            if (levelEvent.Level == level && !isUnlocked)
            {
                isUnlocked = true;
            }
        }

        private void InitializePhase02()
        {
            if (!isUnlocked) return;
            
            _lockedPanel.SetActive(false);
            _TutorialFase02.SetActive(true);
            _NavigationSetas.SetActive(false);
            _FinalButtonTutorial.onClick.AddListener(OnFinalizeTutorialFase02);
            _JumpTutorial.onClick.AddListener(OnFinalizeTutorialFase02);
            EventBus.Publish(new OnSecondLevelInitiateEvent());
        }

        public void OnFinalizeTutorialFase02()
        {
            _TutorialFase02.SetActive(false);
            _NavigationSetas.SetActive(true);
        }

        public void InitializeLevel02WithAnim()
        {
            StartCoroutine(WaitOneSecond());
        }
        private IEnumerator WaitOneSecond()
        {
            yield return new WaitForSeconds(1.01f);
            InitializePhase02();
        }

        public void ResetLevel02()
        {
           EventBus.Publish(new OnResetLevel02Event());
        }
        #region Challenge Part
        private void InitializeRandomSequence()
        {
            
        }

        public void VerifyCorrectLines()
        {
            
        }
        
        #endregion
    }
}