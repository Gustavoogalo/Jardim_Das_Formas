using System;
using Fase02_Scripts;
using Helper.EventBusFolder;
using UnityEngine;
using UnityEngine.UI;

namespace ManagerLevels
{
    public class SelectLevelManager: MonoBehaviour
    {
        [SerializeField] private Button _ForwardButton;
        [SerializeField] private Button _BackwardButton;

        [SerializeField] private Animator _animator;
        
        [SerializeField] private Phase02Manager _phase02Manager;

        public int Level = 1;

        private void Awake()
        {
            _ForwardButton.onClick.AddListener(ForwardScreen);
            _BackwardButton.onClick.AddListener(BackwardScreen);
        }

        private void ForwardScreen()
        {
            if (Level == 1)
            {
                Level = 2;
                _animator.Play("Forward01");
             _phase02Manager.InitializeLevel02WithAnim();
             _phase02Manager.ResetLevel02();
            }
            else if (Level == 2)
            {
                Level = 3;
                _animator.Play("Forward02");
                EventBus.Publish(new OnThirdLevelInitiateEvent());
            }
        }

        private void BackwardScreen()
        {
            if (Level == 2)
            {
                _animator.Play("ReturnToLevel1");
                Level = 1;
            }
            else if (Level == 3)
            {
                _animator.Play("ReturnToLevel2");
                Level = 2;
                _phase02Manager.InitializeLevel02WithAnim();
                _phase02Manager.ResetLevel02();
            }
        }
    }
}