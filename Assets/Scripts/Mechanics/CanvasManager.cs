using System;
using Mechanics.Selector.Selector;
using UnityEngine;
using UnityEngine.UI;

namespace Mechanics
{
    public class CanvasManager : MonoBehaviour
    {
        [Header("Level")]
        
        
        
        [Header("Challenge01 Part")]
        [SerializeField] private Button ChallengeButton;

        [SerializeField] private ChallengeSelector _challengeSelector;
        
        private void Start()
        {
            ChallengeButton.onClick.AddListener(InitializeChallengeSelector);
        }

        private void InitializeChallengeSelector()
        {
            _challengeSelector.gameObject.SetActive(true);
            _challengeSelector.SetupNewChallenge();
        }
    }
}