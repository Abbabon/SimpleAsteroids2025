using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    private readonly HashSet<AsteroidSpawner> _registeredSpawners = new();
    private readonly HashSet<Asteroid> _registeredAsteroids = new();
    
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
    
    public void RegisterSpawner(AsteroidSpawner spawner)
    {
        _registeredSpawners.Add(spawner);
    }
    
    public void UnregisterSpawner(AsteroidSpawner spawner)
    {
        _registeredSpawners.Remove(spawner);
    }
    
    public void RegisterAsteroid(Asteroid asteroid)
    {
        _registeredAsteroids.Add(asteroid);
    }
    
    public void UnregisterAsteroid(Asteroid asteroid)
    {
        _registeredAsteroids.Remove(asteroid);
    }
    
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
        
        // Spawn initial asteroids
        SpawnInitialAsteroids();
        
        RespawnPlayer();
    }
    
    public void OnAsteroidDestroyed(bool awardPoints = true)
    {
        if (_gameOver || _gameWon || !_gameStarted) return;
        
        if (awardPoints)
        {
            _score += pointsPerAsteroid;
            OnScoreChanged?.Invoke(_score);
            
            if (_score > _highScore)
            {
                _highScore = _score;
                OnHighScoreChanged?.Invoke(_highScore);
                
                PlayerPrefs.SetInt("HighScore", _highScore);
                PlayerPrefs.Save();
            }
        }
        
        // Check if all asteroids are destroyed
        CheckWinCondition();
    }
    
    private void CheckWinCondition()
    {
        foreach (var asteroid in _registeredAsteroids)
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
        // Clear warp manager transforms
        WarpManager.Instance.ClearTransforms();
        
        // Destroy all active asteroids
        var asteroidsToDestroy = new List<Asteroid>(_registeredAsteroids);
        foreach (var asteroid in asteroidsToDestroy)
        {
            Destroy(asteroid.gameObject);
        }
        
        // Return all active bullets to the pool
        var activeBullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None);
        foreach (var bullet in activeBullets)
        {
            if (bullet.gameObject.activeInHierarchy)
            {
                PlayerController.Instance.ReturnBullet(bullet);
            }
        }
        
        // Cancel any pending respawn
        CancelInvoke(nameof(RespawnPlayer));
        
        // Reset game state and immediately start the game
        _gameStarted = true;
        _gameOver = false;
        _gameWon = false;
        _playerAlive = true;
        _score = 0;
        _lives = startingLives;
        
        // Update UI
        OnScoreChanged?.Invoke(_score);
        OnLivesChanged?.Invoke(_lives);
        OnGameStarted?.Invoke();
        
        // Reset and respawn player ship
        PlayerController.Instance.RespawnShip();
        
        // Spawn asteroids manually since we can't rely on Start()
        SpawnInitialAsteroids();
    }
    
    private void SpawnInitialAsteroids()
    {
        foreach (var spawner in _registeredSpawners)
        {
            spawner.SpawnAsteroid();
        }
    }
}