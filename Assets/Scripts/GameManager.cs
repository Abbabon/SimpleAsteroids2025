using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int pointsPerAsteroid = 100;
    [SerializeField] private float respawnDelay = 2f;
    
    private int _score;
    private int _lives;
    private int _highScore;
    private bool _gameOver;
    private bool _playerAlive = true;
    private bool _gameStarted;
    private bool _gameWon;
    
    public int Score => _score;
    public int Lives => _lives;
    public int HighScore => _highScore;
    public bool GameOver => _gameOver;
    public bool PlayerAlive => _playerAlive;
    public bool GameStarted => _gameStarted;
    public bool GameWon => _gameWon;
    
    public event Action<int> OnScoreChanged;
    public event Action<int> OnLivesChanged;
    public event Action<int> OnHighScoreChanged;
    public event Action OnGameOver;
    public event Action OnPlayerDied;
    public event Action OnPlayerRespawned;
    public event Action OnGameStarted;
    public event Action OnGameWon;
    
    private void Start()
    {
        _gameStarted = false;
        _gameOver = false;
        _playerAlive = false;
        _score = 0;
        _lives = startingLives;
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        OnScoreChanged?.Invoke(_score);
        OnLivesChanged?.Invoke(_lives);
        OnHighScoreChanged?.Invoke(_highScore);
    }

    public void StartGame()
    {
        _gameStarted = true;
        _gameOver = false;
        _gameWon = false;
        _playerAlive = true;
        _score = 0;
        _lives = startingLives;
        
        OnScoreChanged?.Invoke(_score);
        OnLivesChanged?.Invoke(_lives);
        OnGameStarted?.Invoke();
        
        RespawnPlayer();
    }
    
    public void OnAsteroidDestroyed()
    {
        if (_gameOver || _gameWon || !_gameStarted) return;
        
        _score += pointsPerAsteroid;
        OnScoreChanged?.Invoke(_score);
        
        if (_score > _highScore)
        {
            _highScore = _score;
            OnHighScoreChanged?.Invoke(_highScore);
            
            PlayerPrefs.SetInt("HighScore", _highScore);
            PlayerPrefs.Save();
        }
        
        // Check if all asteroids are destroyed
        CheckWinCondition();
    }
    
    private void CheckWinCondition()
    {
        var remainingAsteroids = FindObjectsByType<Asteroid>(FindObjectsSortMode.None);

        foreach (var asteroid in remainingAsteroids)
        {
            if (!asteroid.IsDestroyed)
            {
                return;
            }
        }
        
        _gameWon = true;
        _playerAlive = false;
        OnGameWon?.Invoke();
    }
    
    public void OnShipHit()
    {
        if (_gameOver || _gameWon || !_playerAlive || !_gameStarted) return;
        
        _playerAlive = false;
        _lives--;
        
        OnPlayerDied?.Invoke();
        OnLivesChanged?.Invoke(_lives);
        
        PlayerController.Instance.DestroyShip();
        
        if (_lives <= 0)
        {
            _gameOver = true;
            OnGameOver?.Invoke();
        }
        else
        {
            Invoke(nameof(RespawnPlayer), respawnDelay);
        }
    }
    
    private void RespawnPlayer()
    {
        if (_gameOver || !_gameStarted) return;
        
        _playerAlive = true;
        PlayerController.Instance.RespawnShip();
        OnPlayerRespawned?.Invoke();
    }
    
    public void RestartGame()
    {
        WarpManager.Instance.ClearTransforms();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}