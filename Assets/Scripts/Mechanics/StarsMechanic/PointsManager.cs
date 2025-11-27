using System;
using TMPro;
using UnityEngine;

namespace Mechanics.StarsMechanic
{
    public class PointsManager : MonoBehaviour
    {
        [SerializeField] private StarInventory starInventory;
        [SerializeField] private TMP_Text TextMeshPro;
        private int points;

        private void Start()
        {
            UpdateUI();
            Game_Events.OnChallengeConcluded += UpdateUI;
        }

        private void UpdateUI()
        {
            TextMeshPro.text = starInventory.CurrentStars.ToString(@"0000");
            
        }
        
    }
}