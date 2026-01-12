using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class Menu : MonoBehaviour
{
    [Header("Pre Start Settings")]
    [SerializeField] private GameObject preStartPanel;
    [SerializeField] private CanvasGroup preStartCG; // Para o FadeOut
    private bool inPreStart = true;

    [Header("Main Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private Button playButton, configButton, creditsButton, endButton;

    [Header("Config Panel")]
    [SerializeField] private GameObject configPanel;
    [SerializeField] private Button closeConfig;
    [SerializeField] private Slider musicVolumeSlider, sfxVolumeSlider;

    [Header("Credits Panel")]
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button closeCredits;

    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button closeEnd, quitButton;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TextMeshProUGUI loadingText;

    void Start()
    {
        SoundManager.Instance.PlayMusic("MenuMusic", true);
        
        SetupButtons();
        SetupSliders();
        
        // Estado inicial
        preStartPanel.SetActive(true);
        startPanel.SetActive(true);
        configPanel.SetActive(false);
        creditsPanel.SetActive(false);
        endPanel.SetActive(false);
        loadingPanel.SetActive(false);
        inPreStart = true;
    }

    void Update()
    {
        if (inPreStart)
        {
            // Detecta qualquer tecla ou clique do mouse
            if (Mouse.current.press.wasPressedThisFrame || Keyboard.current.anyKey.wasPressedThisFrame)
            {
                StartCoroutine(FadeOutPreStart());
            }
        }
    }

    private void SetupButtons()
    {
        // Positive SFX (Abre telas)
        playButton.onClick.AddListener(() => { PlayPositive(); StartCoroutine(LoadLevelCoroutine()); });
        configButton.onClick.AddListener(() => { PlayPositive(); configPanel.SetActive(true); });
        creditsButton.onClick.AddListener(() => { PlayPositive(); creditsPanel.SetActive(true); });
        endButton.onClick.AddListener(() => { PlayPositive(); endPanel.SetActive(true); });

        // Negative SFX (Fecha telas/Volta)
        closeConfig.onClick.AddListener(() => { PlayNegative(); configPanel.SetActive(false); });
        closeCredits.onClick.AddListener(() => { PlayNegative(); creditsPanel.SetActive(false); });
        closeEnd.onClick.AddListener(() => { PlayNegative(); endPanel.SetActive(false); });
        
        quitButton.onClick.AddListener(QuitGame);
    }

    private void SetupSliders()
    {
        musicVolumeSlider.value = SoundManager.Instance.GetMusicVolume();
        sfxVolumeSlider.value = SoundManager.Instance.GetSFXVolume();

        musicVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.ChangeMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SoundManager.Instance.ChangeSFXVolume);
    }

    private void PlayPositive() => SoundManager.Instance.PlaySFX("Positive");
    private void PlayNegative() => SoundManager.Instance.PlaySFX("Negative");

    #region Functionalities

    IEnumerator FadeOutPreStart()
    {
        inPreStart = false;
        float duration = 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            preStartCG.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }

        preStartPanel.SetActive(false);
        startPanel.SetActive(true);
        PlayPositive();
    }

    IEnumerator LoadLevelCoroutine()
    {
        loadingPanel.SetActive(true);
        loadingText.text = "Carregando...";
        
        yield return new WaitForSeconds(2.0f);
        
        // Carrega a próxima cena pelo index
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void QuitGame()
    {
        Debug.Log("Saindo do Jogo...");
        Application.Quit();
    }
    #endregion
}