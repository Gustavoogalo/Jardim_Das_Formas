using System;
using Helper.EventBusFolder;
using UnityEngine;

namespace Mechanics.StarsMechanic
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private int level = 1;

        [SerializeField] private int necessaryStars;
        [SerializeField] private StarInventory starInventory;
        
        public event Action OnLevelUp;

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnChallengeCompleted>(HandleChallengeCompleted);
        }

        private void Start()
        {
            starInventory.ClearStars();
            EventBus.Subscribe<OnChallengeCompleted>(HandleChallengeCompleted);
        }

        private void HandleChallengeCompleted(OnChallengeCompleted challengeCompletedEvent)
        {
            VerifyToUpLevel();
        }
        private void VerifyToUpLevel()
        {
            if (starInventory.CurrentStars >= necessaryStars)
            {
                level++;
                OnLevelUp?.Invoke();
            }
        }
        
        public int GetCurrentLevel() => level;

        public void SetCurrentRequiredStars(int requiredStars)
        {
            necessaryStars = requiredStars;
            Debug.Log($"valor de estrelas requeridas updated para {necessaryStars}.");
        }
    }
}