using UnityEngine;
using UnityEngine.UI;

namespace UI.Configurations
{
    // A classe precisa herdar de MonoBehaviour para ser anexada a um GameObject
    public class Configurations_Panel : MonoBehaviour
    {
        // Referência pública para o painel de configurações (o GameObject pai)
        // Arraste o seu painel de UI para este campo no Inspector.
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private Button pauseButton;

        private bool isPaused = false;

        void Start()
        {
            // Garante que o painel esteja desativado no início do jogo
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
            // Garante que o jogo esteja rodando
            Time.timeScale = 1f;

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(TogglePause);
            }
        }

        // Método público que será chamado pelo botão
        public void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                // Pausa o jogo (Time.timeScale = 0f)
                Time.timeScale = 0f;
                
                // Mostra o painel de UI
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                }
            }
            else
            {
                // Despausa o jogo (Time.timeScale = 1f)
                Time.timeScale = 1f;
                
                // Esconde o painel de UI
                if (pausePanel != null)
                {
                    pausePanel.SetActive(false);
                }
            }
        }

        // Opcional: Adiciona a funcionalidade de pausar/despausar com a tecla ESC
        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     TogglePause();
            // }
        }
    }
}