using System;
using Helper.EventBusFolder;
using TMPro;
using UnityEngine;
using EventBus = Helper.EventBusFolder.EventBus;

namespace Mechanics.StarsMechanic
{
    public class PointsManager : MonoBehaviour
    {
        [SerializeField] private StarInventory starInventory;
        [SerializeField] private TMP_Text TextMeshPro;
        private int points;

        private void Start()
        {
            starInventory.ClearStars();
            UpdateUI();
            EventBus.Subscribe<OnChallengeCompleted>(HandleChallengeCompleted);
            
        }

        private void HandleChallengeCompleted(OnChallengeCompleted challengeCompletedEvent)
        {
            UpdateUI();
        }
        private void UpdateUI()
        {
            TextMeshPro.text = starInventory.CurrentStars.ToString(@"0000");
            
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<OnChallengeCompleted>(HandleChallengeCompleted);
        }
    }
}