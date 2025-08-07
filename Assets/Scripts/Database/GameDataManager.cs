using UnityEngine;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System;

namespace Databases
{
    /// <summary>
    /// Game Data Manager for handling SQLite database operations
    /// </summary>
    public class GameDataManager : MonoBehaviour
    {
        [Header("Database Configuration")]
        [SerializeField] private string databaseName = "GameData.db";

        private SQLiteConnection _database;
        private string _databasePath;

        // Singleton pattern for easy access
        public static GameDataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initialize database path, connection, and table creation
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                // Set up database path
                _databasePath = Path.Combine(Application.persistentDataPath, databaseName);

                // Create SQLite connection
                _database = new SQLiteConnection(_databasePath);

                // Create tables for game data
                _database.CreateTable<HighScore>();

                Debug.Log($"Database initialized at: {_databasePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize database: {ex.Message}");
            }
        }

        #region High Score Operations

        /// <summary>
        /// Add a new high score to the database
        /// </summary>
        public void AddHighScore(string playerName, int score, string levelName = "Default")
        {
            try
            {
                HighScore newScore = new HighScore(playerName, score, levelName);
                _database.Insert(newScore);
                Debug.Log($"High score added: {playerName} - {score} points");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add high score: {ex.Message}");
            }
        }

        /// <summary>
        /// Get the top N high scores across all levels
        /// </summary>
        public List<HighScore> GetTopHighScores(int limit = 10)
        {
            try
            {
                return _database.Table<HighScore>()
                                .OrderByDescending(h => h.Score)
                                .Take(limit)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get high scores: {ex.Message}");
                return new List<HighScore>();
            }
        }

        /// <summary>
        /// Get the top N high scores for a specific level
        /// </summary>
        public List<HighScore> GetHighScoresForLevel(string levelName, int limit = 10)
        {
            try
            {
                return _database.Table<HighScore>()
                                .Where(h => h.LevelName == levelName)
                                .OrderByDescending(h => h.Score)
                                .Take(limit)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get level high scores: {ex.Message}");
                return new List<HighScore>();
            }
        }

        #endregion

        #region Database Utility Methods

        /// <summary>
        /// Get the total number of high scores recorded
        /// </summary>
        public int GetHighScoreCount()
        {
            try
            {
                return _database.Table<HighScore>().Count();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get high score count: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Delete all high scores from the database
        /// </summary>
        public void ClearAllHighScores()
        {
            try
            {
                _database.DeleteAll<HighScore>();
                Debug.Log("All high scores cleared");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clear high scores: {ex.Message}");
            }
        }

        /// <summary>
        /// Close the database connection when the application quits
        /// </summary>
        private void OnApplicationQuit()
        {
            _database?.Close();
        }

        #endregion
    }
}

