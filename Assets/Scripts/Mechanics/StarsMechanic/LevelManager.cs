using System;
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
        
        private void Start()
        {
            VerifyToUpLevel();
            Game_Events.OnChallengeConcluded += VerifyToUpLevel;
        }

        private void VerifyToUpLevel()
        {
            if (starInventory.CurrentStars >= necessaryStars)
            {
                level++;
                necessaryStars *= 3;
                OnLevelUp?.Invoke();
            }
        }
        
        public int GetCurrentLevel() => level;
    }
}