using System;
using Helper.EventBusFolder;
using Mechanics.Selector.Selector;
using Mechanics.StarsMechanic;
using UI.Juyce;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics
{
    public class CanvasManager : MonoBehaviour
    {
        [Header("Level Up")] [SerializeField] private LevelManager levelManager;
        [SerializeField] private JuycenessPanel levelUpPanel;

        [SerializeField] private Button levelUpButton;
        [SerializeField] private FarmManager[] farmManager;
        [SerializeField] private int maxFarms;

        [Header("Challenge Part")] [SerializeField]
        private Button ChallengeButtonType;

        [SerializeField] private Button ChallengeButtonColor;
        [SerializeField] private Button ChallengeButtonSize;
        [SerializeField] private Button ChallengeButton04;
        [SerializeField] private Button ChallengeButton05;
        [SerializeField] private Button ChallengeButton06;

        [SerializeField] private JuycenessPanel _challengePanel;
        [SerializeField] private GameObject _challengeTutorialPanel;
        [SerializeField] private Button _challengeTutorialButton;
        [SerializeField] private Button _challengeTutorialButtonFinal;
        private bool isFirstTime = true;
        [SerializeField] private StarInventory starInventory;

        private ChallengeSelector _challengeSelectorComponent;

        [SerializeField] private GameObject NavigationSetas;

        private void Start()
        {
            maxFarms = farmManager.Length;

            levelManager.OnLevelUp += UpperLevelUpPanel;

            if (GameState.CurrentFarm == null) SetNewCurrentFarm();

            _challengeSelectorComponent = _challengePanel.GetComponentInChildren<ChallengeSelector>(true);

            if (_challengeSelectorComponent != null)
            {
                ChallengeButtonType.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButtonType.GetComponent<FarmManager>(), true, false, false));
                ChallengeButtonColor.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButtonColor.GetComponent<FarmManager>(), false, true, false));
                ChallengeButtonSize.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButtonSize.GetComponent<FarmManager>(), false, false, true));
                ChallengeButton04.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButton04.GetComponent<FarmManager>(), true, false, false));
                ChallengeButton05.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButton05.GetComponent<FarmManager>(), false, true, false));
                ChallengeButton06.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButton06.GetComponent<FarmManager>(), false, false, true));
                _challengeTutorialButton.onClick.AddListener(() => InitializeChallengeAfterTutorial(true, false, false));
                _challengeTutorialButtonFinal.onClick.AddListener(() => InitializeChallengeAfterTutorial(true, false, false));
            }
            else
            {
                Debug.LogError("ChallengeSelector Componente não encontrado no painel de desafio.");
            }

            levelUpPanel.GetCloserButton().onClick.AddListener(CloseLevelUpPanel);
        }

        private void VerifyUnlocked(FarmManager farm, bool isType, bool isColor, bool isSize)
        {
            if (farm.unlocked)
            {
                InitializeChallenge(isType, isColor, isSize);
            }
        }

        private void InitializeChallenge(bool isType, bool isColor, bool isSize)
        {
            if (_challengeSelectorComponent != null)
            {
                if (isFirstTime)
                {
                    starInventory.ClearStars();
                    _challengeTutorialPanel.SetActive(true);
                    NavigationSetas.SetActive(false);
                    isFirstTime = false;
                }
                else
                {
                    _challengeSelectorComponent.SetChallengeCriteriaAndStart(isType, isColor, isSize);
                    _challengePanel.OpenPanel();
                }
            }
        }

        private void InitializeChallengeAfterTutorial(bool isType, bool isColor, bool isSize)
        {
            if (_challengeSelectorComponent != null)
            {
                _challengeTutorialPanel.SetActive(false);
                NavigationSetas.SetActive(true);
                _challengeSelectorComponent.SetChallengeCriteriaAndStart(isType, isColor, isSize);

                _challengePanel.OpenPanel();
            }
        }

        private void UpperLevelUpPanel()
        {
            levelUpPanel.gameObject.SetActive(true);
            levelUpPanel.OpenPanel();
        }

        private void CloseLevelUpPanel()
        {
            SetNewCurrentFarm();
            levelUpPanel.ClosePanel();
            levelUpPanel.gameObject.SetActive(false);
        }

        private void SetNewCurrentFarm()
        {
            foreach (var farm in farmManager)
            {
                if (farm.IsLastFarm)
                {
                    EventBus.Publish(new OnNextLevelEvent(2));
                }

                if (farm.GetFarmId() == levelManager.GetCurrentLevel())
                {
                    EventBus.Publish(new UpdateCurrentFarmEvent(farm));
                    GameState.CurrentFarm = farm;
                    levelManager.SetCurrentRequiredStars(farm.GetRequiredStars());
                    if (!farm.unlocked) farm.UnlockFarm();
                    if (farm.GetFarmId() == maxFarms)
                    {
                        farm.SetIsLastFarm(true);
                    }
                }
            }
        }
    }
}