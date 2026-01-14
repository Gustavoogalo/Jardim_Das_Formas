using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Configurations
{
    // A classe precisa herdar de MonoBehaviour para ser anexada a um GameObject
    public class Configurations_Panel : MonoBehaviour
    {
        // Referência pública para o painel de configurações (o GameObject pai)
        // Arraste o seu painel de UI para este campo no Inspector.
        [SerializeField] private GameObject pausePanel;

        [SerializeField] private Button[] pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button toMenuButton;

        [Header("Configuracoes Settings")] [SerializeField]
        private Button openConfig;

        [SerializeField] private Button closeConfig;
        [SerializeField] private Button toggleAudio;

        [SerializeField] private GameObject configPanel;

        [Header("Audio Settings")] [SerializeField]
        private Sprite audioOn;

        [SerializeField] private Sprite audioOff;

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
            }

            foreach (var button in pauseButton)
            {
                button.onClick.AddListener(TogglePause);
            }

            resumeButton.onClick.AddListener(TogglePause);
            toMenuButton.onClick.AddListener(BackToMenu);

            openConfig.onClick.AddListener(OpenConfig);
            closeConfig.onClick.AddListener(CloseConfig);
        }

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

        private void OpenConfig()
        {
            configPanel.SetActive(true);
        }

        private void CloseConfig()
        {
            configPanel.SetActive(false);
        }

        private void ToggleAudio()
        {
        }

        private void UpdateAudioIcon()
        {
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene(0);
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