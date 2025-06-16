using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject player1Ghost;
    public GameObject player2Ghost;
    
    [Tooltip("Controls the visual UI elements for player turns and scoring")]
    [SerializeField] private PlayerVisual playerVisualScript;

    [Header("Game Logic")]
    public GameObject Player1;
    public GameObject Player2;
    GameObject fallingPeice;

    bool playerOneTurn = true;

    public GameObject[] spawnLocation;

    public int rows = 6;
    public int columns = 7;
    int[ , ] boardState; // 0 is empty, 1 is player 1, 2 is player 2

    public GameObject secretButton1;
    public GameObject secretButton2;

    [Header("Score Keeping")]
    [SerializeField] private int player1Score = 0;
    [SerializeField] private int player2Score = 0;

    [SerializeField] private TextMeshProUGUI p1PointsText;
    [SerializeField] private TextMeshProUGUI p2PointsText;

    bool p1PlayerStarts = true;


    
    [Space]
    [Header("Game End")]
    public bool gameWon = false;

    public GameObject winCircle;
    public GameObject winScreen;
    public TextMeshProUGUI winText;





    public GameObject towerDestroy;

    [Space]
    [Header("Pause Menu")]
    public GameObject pauseMenu;
    public TextMeshProUGUI inputMaxTimerValue;
    public int timerValue = 2;
    
    [Space]
    [Header("Timer")]
    public CountdownSlider countdownSlider;
    public GameObject timerUI;
    
    [Space]
    [Header("Settings")]
    public GameObject settingsUI;
    
    [Space]
    [Header("Player Name")]
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    [SerializeField] private string player1Name;
    [SerializeField] private string player2Name;
    
    [Space]
    [Header("Too Slow Menu")]
    public GameObject tooSlowUI;
    public TextMeshProUGUI tooSlowText;
    public TextMeshProUGUI tooSlowResumeText;

    [Space]
    [Header("Audio")]
    AudioPlayer audioPlayer;

    [Space]
    [Header("Fun Stuff")]
    public GameObject Player1Spray;
    public GameObject Player2Spray;
    public GameObject Player1SpecialSpray; // New special spray piece
    public GameObject Player2SpecialSpray; // New special spray piece
    [Range(1, 50)] // Limit the range in the Inspector
    public int specialSprayAmount = 30; // Amount for special spray
    public int regularSprayAmount = 10; // Amount for regular spray

    public Transform[] coinSpawnLocations; // Array of spawn points for coins
    public Transform[] coinSpecialSpawnLocations; // Array of spawn points for coins

    public GameObject sun;
    public float sunTurn = 0.1f;
    private bool isXRotating = false;

    bool explotionBool = false;



    // 0 0 0 0 0 0 0
    // 0 0 0 0 0 0 0
    // 0 0 0 0 0 0 0
    // 0 0 0 0 0 0 0
    // 0 0 0 0 0 0 0
    // 0 0 0 0 0 0 0

    void Awake()
    {
        audioPlayer = FindFirstObjectByType<AudioPlayer>();
    }

    public void Start()
    {
        gameWon = false;
        boardState = new int[rows, columns];

        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        playerVisualScript.Player2TurnTimer();
        timerUI.SetActive(true);
        settingsUI.SetActive(true);

        pauseMenu.SetActive(true);
        winScreen.SetActive(false);
        tooSlowUI.SetActive(false);
        secretButton1.SetActive(false);
        secretButton2.SetActive(false);

        // Update player names from MainMenu3
        player1Name = MainMenu3.player1Name;
        player2Name = MainMenu3.player2Name;

        // Update with actual player names if they exist
        if (!string.IsNullOrEmpty(player1Name))
            player1NameText.text = player1Name;
        if (!string.IsNullOrEmpty(player2Name))
            player2NameText.text = player2Name;
        
        Menu_ShowPauseMenu();
    }

    void Update()
    {
        SunMove();
    }

    private bool IsGamePaused()
    {
        return pauseMenu != null && pauseMenu.activeSelf;
    }

    private bool IsTooSlow()
    {
        return tooSlowUI != null && tooSlowUI.activeSelf;
    }

    #region GAME BRAIN
    /// <summary>
    /// Game Brain
    /// </summary>

    bool UpdateBoardState(int column)
    {
        for (int row = 0; row < rows; row++)
        {
            if (boardState[row, column] == 0) // found empty spot 
            {
                if (playerOneTurn == true)
                {
                    boardState[row, column] = 1;
                } else
                {
                    boardState[row, column] = 2;
                }

                Debug.Log($"Peice is being spawned at {column}, {row}");
                return true;
            }
        }
        Debug.LogWarning($"{column}, is full!");
        return false;
    }

    public void HoverColumn(int column)
    {
        if (gameWon || IsGamePaused() || IsTooSlow())
        {
            return;
        }
        
        if (column < 0 || column >= columns) // Prevents out-of-bounds errors
        {
            return;
        }

        if (boardState[rows-1, column] == 0 && (fallingPeice == null || fallingPeice.GetComponent<Rigidbody>().linearVelocity == Vector3.zero)) //&& fallingPeice.GetComponent<Rigidbody>().linearVelocity == 0
        {
            if (playerOneTurn == true)
            {
                player1Ghost.SetActive(true);
                player1Ghost.transform.position = spawnLocation[column].transform.position;
            } else
            {
                player2Ghost.SetActive(true);
                player2Ghost.transform.position = spawnLocation[column].transform.position;
            }
        }        
    }

    public void SelectColumn(int column)
    {
        if (gameWon || IsGamePaused() || IsTooSlow())
        {
            return;
        }

        if (fallingPeice == null || fallingPeice.GetComponent<Rigidbody>().linearVelocity == Vector3.zero)
        {
            TakeTurn(column);
        }
    }

    //public bool IsPlayerOneTurn() => playerOneTurn;
    public bool IsPlayerOneTurn()
    {
        return playerOneTurn;
    }
    //public void SetPlayerOneTurn(bool value) => playerOneTurn = value;
    public void SetPlayerOneTurn(bool value)
    {
        playerOneTurn = value;
    }

    public void TakeTurn(int column)
    {
        if (!UpdateBoardState(column)) return;

        player1Ghost.SetActive(false);
        player2Ghost.SetActive(false);
        HandlePlayerTurn(column);
    }

    private void HandlePlayerTurn(int column)
    {
        if (playerOneTurn)
        {
            HandlePlayer1Turn(column);
        }
        else
        {
            HandlePlayer2Turn(column);
        }

        CheckForDraw();
    }

    private void HandlePlayer1Turn(int column)
    {
        SpawnPlayerPiece(Player1, column);
        countdownSlider.slider.value = countdownSlider.slider.maxValue;
        audioPlayer.PlayerTurnSound();

        if (DidWin(1))
        {
            HandleWin(1);
            return;
        }
        playerOneTurn = false;
        playerVisualScript.Player1TurnTimer();
    }

    private void HandlePlayer2Turn(int column)
    {
        SpawnPlayerPiece(Player2, column);
        countdownSlider.slider.value = countdownSlider.slider.maxValue;
        audioPlayer.PlayerTurnSound();

        if (DidWin(2))
        {
            HandleWin(2);
            return;
        }
        playerOneTurn = true;
        playerVisualScript.Player2TurnTimer();
    }

    private void SpawnPlayerPiece(GameObject playerPiece, int column)
    {
        fallingPeice = Instantiate(playerPiece, spawnLocation[column].transform.position, Quaternion.identity);
        fallingPeice.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0.01f, 0);
    }

    public void SecretMenu()
    {
        secretButton1.SetActive(true);
        secretButton2.SetActive(true);
    }
    #endregion

    #region GAME END
    /// <summary>
    /// Game End
    /// </summary>

    public void HandleWin(int playerNumber)
    {
        audioPlayer.WinGameSound();
        Debug.LogWarning($"Player {playerNumber} Won");
        
        // Disable menus
        pauseMenu.SetActive(false);
        tooSlowUI.SetActive(false);
       StartCoroutine(HandleWinShowMenu());
        
        
        if (playerNumber == 1)
        {
            winText.text = $"{player1Name} Won!";
            winText.color = Color.green;
        }
        else
        {
            winText.text = $"{player2Name} Won!";
            winText.color = Color.red;
        }

        if (playerOneTurn == true)
        {
            player1Score++;
            p1PointsText.text = player1Score.ToString();
            StartCoroutine(HandleWinSpray(Player1Spray, Player1SpecialSpray));
        } else
        {
            player2Score++;
            p2PointsText.text = player2Score.ToString();
            StartCoroutine(HandleWinSpray(Player2Spray, Player2SpecialSpray));
        }

        // Check if tower should be destroyed
        TowerDestroy();
        CreateExplosionEffect();

        gameWon = true;
        countdownSlider.timerPaused = true;
        p1PlayerStarts = !p1PlayerStarts;
        TriggerXRotation();
    }

    private IEnumerator HandleWinShowMenu()
    {
        yield return new WaitForSeconds(3f);
        winScreen.SetActive(true);
    }

    private void CheckForDraw()
    {
        if (!DidDraw()) return;
        
        audioPlayer.DrawGameSound();
        Debug.LogWarning("⏸️ Draw");
        
        // Disable menus
        pauseMenu.SetActive(false);
        tooSlowUI.SetActive(false);
        
        winScreen.SetActive(true);
        winText.text = "Game Drawn!\n No Winners";
        winText.color = Color.grey;
        gameWon = true;
        countdownSlider.timerPaused = true;
    }

    bool DidDraw()
    {
        // Check all columns
        for (int x = 0; x < columns; x++)
        {
            // Check the top row of each column
            if (boardState[rows-1, x] == 0)
            {
                return false;  // Found an empty space, not a draw
            }
        }
        // No empty spaces found in the top row of any column
        Debug.Log("Draw detected!");
        return true;  // It's a draw
    }

    bool DidWin(int playerNum)
    {
        //Debug.Log("🔥 DidWin() checking...");

        // Horizontal win check
        for (int x = 0; x < columns - 3; x++) // Corrected from rows - 3
        {
            for (int y = 0; y < rows; y++)
            {
                if (boardState[y, x] == playerNum && 
                    boardState[y, x + 1] == playerNum && 
                    boardState[y, x + 2] == playerNum && 
                    boardState[y, x + 3] == playerNum)
                {
                    Instantiate(winCircle, new Vector3(x, y, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y +1, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y +2, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y +3, -28), Quaternion.identity);
                    return true;
                }
            }
        }

        // Vertical win check
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows - 3; y++) // Corrected from columns - 3
            {
                if (boardState[y, x] == playerNum && 
                    boardState[y + 1, x] == playerNum && 
                    boardState[y + 2, x] == playerNum && 
                    boardState[y + 3, x] == playerNum)
                {
                    Instantiate(winCircle, new Vector3(x, y, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y + 1, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y + 2, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x, y + 3, -28), Quaternion.identity);                    
                    return true;
                }
            }
        }

        // Diagonal win (bottom-left to top-right)
        for (int x = 0; x < columns - 3; x++)
        {
            for (int y = 0; y < rows - 3; y++)
            {
                if (boardState[y, x] == playerNum && 
                    boardState[y + 1, x + 1] == playerNum && 
                    boardState[y + 2, x + 2] == playerNum && 
                    boardState[y + 3, x + 3] == playerNum)
                {
                    Instantiate(winCircle, new Vector3(x, y, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x + 1, y +1, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x +2, y +2, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x +3, y +3, -28), Quaternion.identity);   
                    return true;
                }
            }
        }

        // Diagonal win (top-left to bottom-right)
        for (int x = 0; x < columns - 3; x++)
        {
            for (int y = 3; y < rows; y++) // y starts at 3 to avoid negative index
            {
                if (boardState[y, x] == playerNum && 
                    boardState[y - 1, x + 1] == playerNum && 
                    boardState[y - 2, x + 2] == playerNum && 
                    boardState[y - 3, x + 3] == playerNum)
                {
                    Instantiate(winCircle, new Vector3(x, y, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x - 1, y +1, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x - 2, y +2, -28), Quaternion.identity);
                    Instantiate(winCircle, new Vector3(x - 3, y +3, -28), Quaternion.identity);   
                    return true;
                }
            }
        }

        return false;
    }

    public void ViewBoard()
    {
        winScreen.SetActive(false);
        secretButton1.SetActive(false);
        secretButton2.SetActive(false);
        pauseMenu.SetActive(false);
        StartCoroutine(ViewBoredReset());
    }

    private IEnumerator ViewBoredReset()
    {
        yield return new WaitForSecondsRealtime(7f);
        winScreen.SetActive(true);
    }

    public void WinMenuTemp(int playerNumber) { HandleWin(playerNumber); }

    public void DrawMenuTemp()
    {
        // Disable any ongoing game processes
        gameWon = true;
        
        // Stop any currently playing sounds and play draw sound
        // audioPlayer.GetComponent<AudioSource>().Stop();
        audioPlayer.DrawGameSound();
        
        // Disable other menus
        pauseMenu.SetActive(false);
        tooSlowUI.SetActive(false);
        
        // Show UI
        Debug.LogWarning("⏸️ Draw");
        winScreen.SetActive(true);
        winText.text = "Game Drawn\nNo Winners";
        countdownSlider.timerPaused = true;
        //CheckForDraw();
    }

    public void NextGame()
    {
        // Clear the board state array
        boardState = new int[rows, columns];
        
        // Find and destroy all player pieces in the scene (not the prefabs)
        GameObject[] pieces = GameObject.FindGameObjectsWithTag("Player1BoardPeices");
        foreach (GameObject piece in pieces)
        {
            // Only destroy if it's not the prefab and not a spray piece
            if (piece != Player1 && piece.layer != LayerMask.NameToLayer("SprayPieces"))
            {
                Destroy(piece);
            }
        }
        
        pieces = GameObject.FindGameObjectsWithTag("Player2BoardPeices");
        foreach (GameObject piece in pieces)
        {
            // Only destroy if it's not the prefab and not a spray piece
            if (piece != Player2 && piece.layer != LayerMask.NameToLayer("SprayPieces"))
            {
                Destroy(piece);
            }
        }

        // Reset game state
        pauseMenu.SetActive(true);
        tooSlowUI.SetActive(false);
        winScreen.SetActive(false);
        gameWon = false;
        playerOneTurn = true;
        countdownSlider.slider.value = countdownSlider.slider.maxValue;
        
        // // Reset any win circles
        // GameObject[] winCircles = GameObject.FindGameObjectsWithTag("WinCircle");
        // foreach (GameObject circle in winCircles)
        // {
        //     Destroy(circle);
        // }
    }


    #endregion

    #region PAUSE MENU
    /// <summary>
    /// Pause Menu
    /// </summary>

    public void Menu_ShowPauseMenu()
    {

        //Debug.Log("Pause Menu");
        audioPlayer.PlayButtonClickSound();
        pauseMenu.SetActive(true);
        //timerUI.SetActive(false);
        settingsUI.SetActive(false);
        tooSlowUI.SetActive(false);
        countdownSlider.timerPaused = true;
    }

    public void Button_PauseMenuStart()
    {
        audioPlayer.PlayButtonClickSound();

        string cleanText = new string(inputMaxTimerValue.text.Where(c => char.IsDigit(c)).ToArray());

        if (int.TryParse(cleanText, out timerValue))
        {
            //Debug.Log($"Parsed value: {timerValue}");
            countdownSlider.slider.maxValue = timerValue;  // Set max first
            countdownSlider.slider.value = timerValue;     // Then set current value
            //Debug.Log($"New max value: {countdownSlider.slider.maxValue}");
        }

        pauseMenu.SetActive(false);
        timerUI.SetActive(true);
        settingsUI.SetActive(true);
        tooSlowUI.SetActive(false);
        countdownSlider.timerPaused = false;

        if (p1PlayerStarts)
        {
            SetPlayerOneTurn(true);
            playerVisualScript.Player2TurnTimer(); // This sets P2 transparent and P1 opaque
        }
        else
        {
            SetPlayerOneTurn(false);
            playerVisualScript.Player1TurnTimer(); // This sets P1 transparent and P2 opaque
        }
    }

    public void Button_RestartGame()
    {
        audioPlayer.PlayButtonClickSound();
        SceneManager.LoadScene(1);
        countdownSlider.timerPaused = false;
    }

    public void Button_PauseMenuMainMenu()
    {
        audioPlayer.PlayButtonClickSound();
        SceneManager.LoadScene(0);  
    }
    #endregion

    #region TOO SLOW
    /// <summary>
    /// Too Slow Menu
    /// </summary>

    public void Menu_TooSlow()
    {
        Debug.Log("Menu_TooSlow called");
        // Disable other UIs first
        settingsUI.SetActive(false);
        timerUI.SetActive(false);
        
        // Show TooSlow UI
        tooSlowUI.SetActive(true);
        
        // Update text - the next player will take the turn
        tooSlowText.text = playerOneTurn ? $"{player1Name} Too Slow!" : $"{player2Name} Too Slow!";
        tooSlowResumeText.text = playerOneTurn ? $"{player2Name} Take Turn" : $"{player1Name} Take Turn";
        
        // Pause timer
        countdownSlider.timerPaused = true;
    }

    public void Button_TooSlowUIContinue()
    {
        Debug.Log("Button_TooSlowUIContinue called!");

        // Switch to next player first (before UI changes)
        if (playerOneTurn)
        {
            SetPlayerOneTurn(false);
            playerVisualScript.Player1TurnTimer(); // This sets P1 transparent and P2 opaque
        }
        else
        {
            SetPlayerOneTurn(true);
            playerVisualScript.Player2TurnTimer(); // This sets P2 transparent and P1 opaque
        }

        // Reset UI states
        tooSlowUI.SetActive(false);
        settingsUI.SetActive(true);
        timerUI.SetActive(true);

        // Reset and unpause timer
        countdownSlider.slider.value = countdownSlider.slider.maxValue;
        countdownSlider.timerPaused = false;

        Debug.Log($"Player One Turn: {playerOneTurn}");
        Debug.Log($"TooSlowUI Active: {tooSlowUI.activeSelf}");
    }   
    #endregion


    #region FUN STUFF
    /// <summary>
    /// Fun Stuff
    /// </summary>
    
    //Sun Move
    
    public void SunMove()
    {
        // Regular Y rotation
        Vector3 currentRotation = sun.transform.eulerAngles;
        currentRotation.y += sunTurn * Time.deltaTime;
        sun.transform.eulerAngles = currentRotation;
    }

    public void TriggerXRotation()
    {
        if (!isXRotating)
        {
            isXRotating = true;
            StartCoroutine(RotateXOver10Seconds());
        }
    }

    private IEnumerator RotateXOver10Seconds()
    {
        float elapsedTime = 0;
        Quaternion startRotation = sun.transform.rotation;
        
        while (elapsedTime < 10f)
        {
            elapsedTime += Time.deltaTime;
            float xRotation = (elapsedTime / 10f) * 360f;
            sun.transform.rotation = startRotation * Quaternion.Euler(xRotation, 0, 0);
            yield return null;
        }
        
        isXRotating = false;
    }

    //Win Spray

    private void SetupSprayPiece(GameObject sprayPiece)
    {
        // Set the layer of the spray piece when it's instantiated
        sprayPiece.layer = LayerMask.NameToLayer("SprayPieces");
    }

    private IEnumerator HandleWinSpray(GameObject regularSpray, GameObject specialSpray)
    {
        // First do the regular spray
        yield return StartCoroutine(RegularSpray(regularSpray));
        // Then do the special spray
        yield return StartCoroutine(SpecialSpray(specialSpray));
    }

    private IEnumerator RegularSpray(GameObject playerPrefab)
    {
        if (coinSpawnLocations == null || coinSpawnLocations.Length == 0)
        {
            Debug.LogError("No regular coin spawn locations set!");
            yield break;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Regular spray prefab is null!");
            yield break;
        }
        
        for (int i = 0; i < regularSprayAmount; i++)
        {
            foreach (Transform spawnPoint in coinSpawnLocations)
            {
                if (playerPrefab == null)
                {
                    Debug.LogError("Regular spray prefab became null during execution!");
                    yield break;
                }
                
                GameObject coin = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
                SetupSprayPiece(coin);
                Rigidbody rb = coin.GetComponent<Rigidbody>();
                
                if (rb == null)
                {
                    continue;
                }
                
                float randomX = UnityEngine.Random.Range(-12f, 12f);
                float randomY = UnityEngine.Random.Range(-15f, -20f);
                float randomZ = UnityEngine.Random.Range(-3f, 20f);
                
                rb.AddForce(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator SpecialSpray(GameObject playerPrefab)
    {
        if (coinSpecialSpawnLocations == null || coinSpecialSpawnLocations.Length == 0)
        {
            Debug.LogError("No special coin spawn locations set!");
            yield break;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Special spray prefab is null!");
            yield break;
        }
        
        Debug.Log($"Starting special spray with amount: {specialSprayAmount}");
        
        // Only use the first spawn location
        Transform spawnPoint = coinSpecialSpawnLocations[0];
        
        for (int i = 0; i < specialSprayAmount; i++)
        {
            if (playerPrefab == null)
            {
                Debug.LogError("Special spray prefab became null during execution!");
                yield break;
            }
            
            GameObject coin = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
            SetupSprayPiece(coin);
            Rigidbody rb = coin.GetComponent<Rigidbody>();
            
            if (rb == null)
            {
                continue;
            }
            
            // Reduced force values for special spray
            float randomX = UnityEngine.Random.Range(-4f, 0f);
            float randomY = UnityEngine.Random.Range(-10f, -15f);
            float randomZ = UnityEngine.Random.Range(-2f, 10f);
            
            rb.AddForce(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);
            
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.Log("Special spray completed");
    }

    //Tower Destroy

    public void TowerDestroy()
    {
        if (player1Score >= 15 || player2Score >= 15)
        {
            Destroy(towerDestroy);

        }
    }

    private void CreateExplosionEffect()
    {
        if (player1Score == 3 || player2Score == 3)
        {
            if (explotionBool == true)
            {
                return;
            }
            explotionBool = true;
            // Find all pieces in the scene with Rigidbody
            GameObject[] allPieces = GameObject.FindGameObjectsWithTag("SprayPieces");
            GameObject[] player2Pieces = GameObject.FindGameObjectsWithTag("SprayPieces");
            
            // Combine the arrays
            GameObject[] allGamePieces = new GameObject[allPieces.Length + player2Pieces.Length];
            allPieces.CopyTo(allGamePieces, 0);
            player2Pieces.CopyTo(allGamePieces, allPieces.Length);

            foreach (GameObject piece in allGamePieces)
            {
                if (piece != null)
                {
                    // Make sure the piece can be affected by physics
                    Rigidbody rb = piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // Ensure the Rigidbody is not kinematic and uses gravity
                        rb.isKinematic = false;
                        rb.useGravity = true;
                        
                        // Create explosion at piece position
                        Vector3 explosionPosition = piece.transform.position;
                        float explosionForce = 2000f; // Increased force
                        float explosionRadius = 10f;  // Increased radius
                        float upwardsModifier = 5f;   // Increased upward force
                        
                        // Add random torque for spinning
                        rb.AddTorque(UnityEngine.Random.Range(-500f, 500f), 
                                UnityEngine.Random.Range(-500f, 500f), 
                                UnityEngine.Random.Range(-500f, 500f));
                        
                        // Add explosion force
                        rb.AddExplosionForce(explosionForce, explosionPosition - Vector3.up, 
                                        explosionRadius, upwardsModifier, ForceMode.Impulse);
                        
                        // Add random force for more chaos
                        rb.AddForce(new Vector3(UnityEngine.Random.Range(-500f, 500f), 
                                            UnityEngine.Random.Range(200f, 500f), 
                                            UnityEngine.Random.Range(-500f, 500f)));
                    }
                }
            }
        }
    }
    #endregion
}

