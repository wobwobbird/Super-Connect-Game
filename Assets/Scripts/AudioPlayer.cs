using UnityEngine;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip[] songs;
    [SerializeField] private Slider volumeSlider;
    private AudioSource audioSource;

    [Header("UI Sounds")]
    [SerializeField] AudioClip buttonClickSound;
    [SerializeField] [Range(0f, 1f)] float buttonClickVolume = 0.5f;

    [Header("Game Sounds")]
    [SerializeField] AudioClip playerTurnSound;
    [SerializeField] [Range(0f, 1f)] float playerTurnVolume = 0.5f;

    [Header("Win States")]
    [SerializeField] AudioClip winGameSound;
    [SerializeField] [Range(0f, 1f)] float winGameVolume = 0.5f;

    [SerializeField] AudioClip drawGameSound;
    [SerializeField] [Range(0f, 1f)] float drawGameVolume = 0.5f;

    private int currentSongIndex = 0;
    private bool isPaused = false;

    void Awake()
    {
        // Initial audio setup
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Setup volume slider if it exists in this scene
        if (volumeSlider != null)
        {
            audioSource.volume = volumeSlider.value / 5f;
            volumeSlider.onValueChanged.AddListener(HandleSliderValueChanged);
        }
        else
        {
            audioSource.volume = 1f;
        }
    }

    void Start()
    {
        // Randomly select a starting song
        int randomIndex = Random.Range(0, songs.Length);
        Music_Play(randomIndex);
        currentSongIndex = randomIndex; // Make sure to update the current index
    }

    void Update()
    {
        // Only check for song end if we're not paused
        if (!isPaused && audioSource.clip != null && !audioSource.isPlaying)
        {
            Music_NextSong();
        }
    }

    public void Music_Play(int index)
    {
        if (index < 0 || index >= songs.Length) return;
        audioSource.clip = songs[index];
        audioSource.Play();
        isPaused = false;
    }

    public void Music_UnPause(int index)
    {
        audioSource.UnPause();
        isPaused = false;
    }

    public void Music_Pause()
    {
        audioSource.Pause();
        isPaused = true;
    }

    public void Music_NextSong()
    {
        // Increment song index and wrap around if we reach the end
        currentSongIndex = (currentSongIndex + 1) % songs.Length;
        
        // Play the next song
        Music_Play(currentSongIndex);
    }

    void HandleSliderValueChanged(float value)
    {
        if (audioSource != null)  // Add safety check
        {
            // Convert the 0-5 range to 0-1 range
            float normalizedVolume = value / 5f;
            audioSource.volume = normalizedVolume;
            
            Debug.Log($"Slider value: {value}, Volume: {normalizedVolume}");
        }
    }

    public void PlayButtonClickSound()
    {
        if(buttonClickSound != null)
        {
            AudioSource.PlayClipAtPoint(buttonClickSound, Camera.main.transform.position, buttonClickVolume);
        }
    }

    public void PlayerTurnSound()
    {
        if(playerTurnSound != null)
        {
            AudioSource.PlayClipAtPoint(playerTurnSound, Camera.main.transform.position, playerTurnVolume);
        }
    }

    public void WinGameSound()
    {
        if(winGameSound != null)
        {
            AudioSource.PlayClipAtPoint(winGameSound, Camera.main.transform.position, winGameVolume);
        }
    }

    public void DrawGameSound()
    {
        if(drawGameSound != null)
        {
            AudioSource.PlayClipAtPoint(drawGameSound, Camera.main.transform.position, drawGameVolume);
        }
    }
}

