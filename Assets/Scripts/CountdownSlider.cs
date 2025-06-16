using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownSlider : MonoBehaviour
{
    public Slider slider;
   // public float countDownTime = 10f;

    public GameManager gameManager;

    public PlayerVisual playerVisualScript;
    public bool timerPaused;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    void Update()
    {
        if (slider != null && slider.value > 0 && timerPaused == false)
        {
            slider.value -= Time.deltaTime;
        }

        if (slider.value <= 0)
        {
            //Debug.Log("Slider ended");
            gameManager.Menu_TooSlow();
        }
    }

    public void NextPlayerTimerAction()
    {
        if (gameManager.IsPlayerOneTurn())
        {
            gameManager.SetPlayerOneTurn(false);
            playerVisualScript.Player2TurnTimer();
            slider.value = gameManager.timerValue; 
            // transparency of text
            player1NameText.alpha = 1f;
            player2NameText.alpha = 0.5f;
            player1ScoreText.alpha = 1f;
            player2ScoreText.alpha = 0.5f;
        }
        else
        {
            gameManager.SetPlayerOneTurn(true);
            playerVisualScript.Player1TurnTimer();
            slider.value = gameManager.timerValue; 
            // transparency of text
            player1NameText.alpha = 0.5f;
            player2NameText.alpha = 1f;
            player1ScoreText.alpha = 0.5f;
            player2ScoreText.alpha = 1f;
        }
    }
}
