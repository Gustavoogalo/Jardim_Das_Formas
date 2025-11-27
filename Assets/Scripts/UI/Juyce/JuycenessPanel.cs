using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Juyce
{
    [RequireComponent(typeof(Animator))]
    public class JuycenessPanel : MonoBehaviour
    {
        private Animator animator;
        [SerializeField] private Button closerButton;
        
        
        private void Awake()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            closerButton.onClick.AddListener(ClosePanel);
        }

        public void OpenPanel()
        {
            if (!enabled)
            {
                enabled = true;
            }
            this.gameObject.SetActive(true);
            if(animator == null)
            {
            animator = GetComponent<Animator>();
            }
            animator.Play("OpenPanel");
        }
        
        public void ClosePanel()
        {
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            animator.Play("ClosePanel");
            this.gameObject.SetActive(false);
            
        }
        
        public Button GetCloserButton() => closerButton;
    }
}