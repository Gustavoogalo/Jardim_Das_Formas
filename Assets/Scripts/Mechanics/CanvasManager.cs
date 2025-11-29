using System;
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


        [Header("Challenge Part")] [SerializeField]
        private Button ChallengeButtonType; // NOVO: Botão para desafio de Tipo
        [SerializeField] private Button ChallengeButtonColor; // NOVO: Botão para desafio de Cor
        [SerializeField] private Button ChallengeButtonSize; // NOVO: Botão para desafio de Tamanho

        [SerializeField] private Button ChallengeButton04; // NOVO: Botão para desafio de Tipo
        [SerializeField] private Button ChallengeButton05; // NOVO: Botão para desafio de Cor
        [SerializeField] private Button ChallengeButton06; // NOVO: Botão para desafio de Tamanho
        
        [SerializeField] private JuycenessPanel _challengePanel; // Renomeado para maior clareza

        // 🚨 NOVO: Referência direta ao ChallengeSelector
        private ChallengeSelector _challengeSelectorComponent;

        private void Start()
        {
            levelManager.OnLevelUp += UpperLevelUpPanel;
            if (Game_Events.GetCurrentFarm() == null) SetNewCurrentFarm();

            _challengeSelectorComponent = _challengePanel.GetComponentInChildren<ChallengeSelector>(true);

            if (_challengeSelectorComponent != null)
            {
                ChallengeButtonType.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButtonType.GetComponent<FarmManager>(), true, false, false));
                ChallengeButtonColor.onClick.AddListener(() => VerifyUnlocked(ChallengeButtonColor.GetComponent<FarmManager>(),false, true, false));
                ChallengeButtonSize.onClick.AddListener(() => VerifyUnlocked(ChallengeButtonSize.GetComponent<FarmManager>(),false, false, true));

                ChallengeButton04.onClick.AddListener(() =>
                    VerifyUnlocked(ChallengeButton04.GetComponent<FarmManager>(), true, false, false));
                ChallengeButton05.onClick.AddListener(() => VerifyUnlocked(ChallengeButton05.GetComponent<FarmManager>(),false, true, false));
                ChallengeButton06.onClick.AddListener(() => VerifyUnlocked(ChallengeButton06.GetComponent<FarmManager>(),false, false, true));

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
                // Define os booleanos no ChallengeSelector e inicia o SetupNewChallenge()
                _challengeSelectorComponent.SetChallengeCriteriaAndStart(isType, isColor, isSize);

                // Abre o painel (JuycenessPanel)
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
                if (farm.GetFarmId() == levelManager.GetCurrentLevel())
                {
                    Game_Events.SetCurrentFarm(farm);
                    if (!farm.unlocked) farm.UnlockFarm();
                }
            }
        }
    }
}