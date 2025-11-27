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
        [Header("Level Up")]
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private JuycenessPanel levelUpPanel;

        [SerializeField] private Button levelUpButton;
        [SerializeField] private FarmManager[] farmManager;
        //evento de som
        
        
        
        
        [Header("Challenge01 Part")]
        [SerializeField] private Button ChallengeButton;

        [SerializeField] private JuycenessPanel _challengeSelector;
        
        private void Start()
        {
            levelManager.OnLevelUp += UpperLevelUpPanel;
            if(Game_Events.GetCurrentFarm() == null) SetNewCurrentFarm();
            ChallengeButton.onClick.AddListener(InitializeChallengeSelector);
            levelUpPanel.GetCloserButton().onClick.AddListener(CloseLevelUpPanel);
        }

        private void InitializeChallengeSelector()
        {
            //_challengeSelector.gameObject.SetActive(true);
            _challengeSelector.OpenPanel();
                //_challengeSelector.SetupNewChallenge();
        }

        private void UpperLevelUpPanel()
        {
            levelUpPanel.gameObject.SetActive(true);
            //evento do som
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
                    if(!farm.unlocked) farm.UnlockFarm();
                }
            }
        }
    }
}