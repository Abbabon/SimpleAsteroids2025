using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private RectTransform livesParentElement;
    [SerializeField] private GameObject livesPrefab;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI hiScoreText;
    [SerializeField] private TextMeshProUGUI statusText;
    
    private List<GameObject> _livesDisplayObjects = new();
    
    private void Start()
    {
        // Hook up button
        startButton.onClick.AddListener(OnStartButtonClicked);
        
        // Subscribe to game events
        GameManager.Instance.OnScoreChanged += OnScoreChanged;
        GameManager.Instance.OnLivesChanged += OnLivesChanged;
        GameManager.Instance.OnHighScoreChanged += OnHighScoreChanged;
        GameManager.Instance.OnGameOver += OnGameOver;
        GameManager.Instance.OnPlayerDied += OnPlayerDied;
        GameManager.Instance.OnPlayerRespawned += OnPlayerRespawned;
        GameManager.Instance.OnGameStarted += OnGameStarted;
        GameManager.Instance.OnGameWon += OnGameWon;
        
        // Initialize UI
        UpdateScoreDisplay(GameManager.Instance.Score);
        UpdateHighScoreDisplay(GameManager.Instance.HighScore);
        UpdateLivesDisplay(GameManager.Instance.Lives);
        UpdateStatusText("ASTEROIDS");
        UpdateStartButton();
    }
    
    private void OnDestroy()
    {
        if (startButton != null)
            startButton.onClick.RemoveListener(OnStartButtonClicked);
            
        GameManager.Instance.OnScoreChanged -= OnScoreChanged;
        GameManager.Instance.OnLivesChanged -= OnLivesChanged;
        GameManager.Instance.OnHighScoreChanged -= OnHighScoreChanged;
        GameManager.Instance.OnGameOver -= OnGameOver;
        GameManager.Instance.OnPlayerDied -= OnPlayerDied;
        GameManager.Instance.OnPlayerRespawned -= OnPlayerRespawned;
        GameManager.Instance.OnGameStarted -= OnGameStarted;
        GameManager.Instance.OnGameWon -= OnGameWon;
    }
    
    private void OnStartButtonClicked()
    {
        if (GameManager.Instance.GameOver || GameManager.Instance.GameWon)
        {
            GameManager.Instance.RestartGame();
        }
        else if (!GameManager.Instance.GameStarted)
        {
            GameManager.Instance.StartGame();
        }
    }
    
    private void OnScoreChanged(int newScore)
    {
        UpdateScoreDisplay(newScore);
    }
    
    private void OnLivesChanged(int newLives)
    {
        UpdateLivesDisplay(newLives);
    }
    
    private void OnHighScoreChanged(int newHighScore)
    {
        UpdateHighScoreDisplay(newHighScore);
    }
    
    private void OnGameOver()
    {
        UpdateStatusText("GAME OVER!");
        UpdateStartButton();
    }
    
    private void OnPlayerDied()
    {
        // Let animations handle the death visual feedback
    }
    
    private void OnPlayerRespawned()
    {
        UpdateStatusText("");
    }
    
    private void OnGameStarted()
    {
        UpdateStatusText("");
        UpdateStartButton();
    }
    
    private void OnGameWon()
    {
        UpdateStatusText("YOU WIN!");
        UpdateStartButton();
    }
    
    private void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
            scoreText.text = $"SCORE: {score}";
    }
    
    private void UpdateHighScoreDisplay(int highScore)
    {
        if (hiScoreText != null)
            hiScoreText.text = $"HIGH SCORE: {highScore}";
    }
    
    private void UpdateLivesDisplay(int lives)
    {
        // Clear existing lives display
        foreach (var lifeObject in _livesDisplayObjects)
        {
            if (lifeObject != null)
                Destroy(lifeObject);
        }
        _livesDisplayObjects.Clear();
        
        // Create new lives display
        for (int i = 0; i < lives; i++)
        {
            if (livesPrefab != null && livesParentElement != null)
            {
                var lifeObject = Instantiate(livesPrefab, livesParentElement);
                _livesDisplayObjects.Add(lifeObject);
            }
        }
    }
    
    private void UpdateStatusText(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }
    
    private void UpdateStartButton()
    {
        if (startButton == null) return;
        
        var buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        
        if (!GameManager.Instance.GameStarted)
        {
            // Game hasn't started yet
            startButton.gameObject.SetActive(true);
            if (buttonText != null)
                buttonText.text = "START";
        }
        else if (GameManager.Instance.GameOver)
        {
            // Game is over, show retry
            startButton.gameObject.SetActive(true);
            if (buttonText != null)
                buttonText.text = "RETRY";
        }
        else if (GameManager.Instance.GameWon)
        {
            // Player won, show play again
            startButton.gameObject.SetActive(true);
            if (buttonText != null)
                buttonText.text = "PLAY AGAIN";
        }
        else
        {
            // Game is active, hide button
            startButton.gameObject.SetActive(false);
        }
    }
}