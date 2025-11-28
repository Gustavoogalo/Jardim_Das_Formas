using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    void Start()
    {
        playButton.onClick.AddListener(ToGamePlay);
    }

    private void ToGamePlay()
    {
        SceneManager.LoadScene(1);
    }
}
