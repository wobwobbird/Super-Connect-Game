using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;

public class MainMenu3 : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject nameInputMenu;
    
    AudioPlayer audioPlayer;

    [Header("Name Input")]
    public GameObject nameInputPanel;
    public TMP_InputField Playert1nameInputField;
    public TMP_InputField Playert2nameInputField;
    public TextMeshProUGUI EnterNameHere;
    public Button startButton;

    public static string player1Name;
    public static string player2Name;
    

    void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }

    void Start()
    {
        mainMenu.SetActive(true);
        nameInputMenu.SetActive(false);
    }

    void Update()
    {
        // Store the name
        player1Name = Playert1nameInputField.text;
        player2Name = Playert2nameInputField.text;
    }

    /// <summary>
    /// Maun Menu Screen
    /// </summary>

    public void Button_OnStartClick()
    {
        audioPlayer.PlayButtonClickSound();
        mainMenu.SetActive(false);
        nameInputPanel.SetActive(true);
    }

    public void Button_OnExitCLick()
    {
        audioPlayer.PlayButtonClickSound();
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    /// <summary>
    /// For the buttons in Game Scene
    /// </summary>

    public void Button_GameBackToMainMenu()
    {
        audioPlayer.PlayButtonClickSound();
        SceneManager.LoadScene(0);
    }

    public void Button_NameChosenStartClick()
    {
        if (string.IsNullOrEmpty(player1Name) || string.IsNullOrEmpty(player2Name))
        {
            Debug.LogWarning("Please enter names for both players!");
            EnterNameHere.color = Color.red;
            return;
        }

        audioPlayer.PlayButtonClickSound();
        Debug.Log("Game started");
        SceneManager.LoadScene(1);
    }
} 