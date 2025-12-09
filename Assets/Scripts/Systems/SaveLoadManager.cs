using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Complete save data for the game
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public string saveName;
        public string saveDate;
        public int saveVersion = 1;

        // League data
        public string leagueName;
        public int currentSeasonYear;
        public bool seasonInProgress;
        public bool playoffsInProgress;

        // Teams
        public List<TeamSaveData> teams = new List<TeamSaveData>();

        // Current season/schedule (if in progress)
        public ScheduleSaveData currentSchedule;
        public PlayoffSaveData currentPlayoffs;

        public GameSaveData()
        {
            saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    /// <summary>
    /// Serializable team data
    /// </summary>
    [Serializable]
    public class TeamSaveData
    {
        public string teamName;
        public int teamId;
        public string managerName;
        public SerializableColor teamColor;

        // Season stats
        public int wins;
        public int losses;
        public int draws;
        public int pointsScored;
        public int pointsAgainst;
        public int matchesPlayed;

        // All-time stats
        public int totalWins;
        public int totalLosses;
        public int totalDraws;
        public int championships;

        // Roster
        public List<CharacterSaveData> roster = new List<CharacterSaveData>();

        public static TeamSaveData FromTeamData(TeamData team)
        {
            var saveData = new TeamSaveData
            {
                teamName = team.teamName,
                teamId = team.teamId,
                managerName = team.managerName,
                teamColor = new SerializableColor(team.teamColor),
                wins = team.wins,
                losses = team.losses,
                draws = team.draws,
                pointsScored = team.pointsScored,
                pointsAgainst = team.pointsAgainst,
                matchesPlayed = team.matchesPlayed,
                totalWins = team.totalWins,
                totalLosses = team.totalLosses,
                totalDraws = team.totalDraws,
                championships = team.championships
            };

            foreach (var character in team.roster)
            {
                saveData.roster.Add(CharacterSaveData.FromCharacterData(character));
            }

            return saveData;
        }

        public TeamData ToTeamData()
        {
            var team = new TeamData
            {
                teamName = teamName,
                teamId = teamId,
                managerName = managerName,
                teamColor = teamColor.ToColor(),
                wins = wins,
                losses = losses,
                draws = draws,
                pointsScored = pointsScored,
                pointsAgainst = pointsAgainst,
                matchesPlayed = matchesPlayed,
                totalWins = totalWins,
                totalLosses = totalLosses,
                totalDraws = totalDraws,
                championships = championships
            };

            foreach (var charData in roster)
            {
                team.roster.Add(charData.ToCharacterData());
            }

            return team;
        }
    }

    /// <summary>
    /// Serializable character data (simplified)
    /// </summary>
    [Serializable]
    public class CharacterSaveData
    {
        public string characterName;
        public int level;
        public int currentHP;
        public int maxHP;
        public int defense;
        public int experiencePoints;

        // We'll save key stats but not everything
        // ScriptableObject references (ancestry/background) will need to be re-linked on load
        public string ancestryName;
        public string backgroundName;

        public static CharacterSaveData FromCharacterData(CharacterData character)
        {
            return new CharacterSaveData
            {
                characterName = character.characterName,
                level = character.level,
                currentHP = character.currentHP,
                maxHP = character.maxHP,
                defense = character.defense,
                experiencePoints = character.experiencePoints,
                ancestryName = character.ancestry?.ancestryName ?? "",
                backgroundName = character.background?.backgroundName ?? ""
            };
        }

        public CharacterData ToCharacterData()
        {
            // Note: This creates a basic character
            // Full reconstruction would need access to ScriptableObjects
            var character = new CharacterData
            {
                characterName = characterName,
                level = level,
                currentHP = currentHP,
                maxHP = maxHP,
                defense = defense,
                experiencePoints = experiencePoints
            };

            // Initialize basic systems
            character.attributes = new CharacterAttributes();

            return character;
        }
    }

    /// <summary>
    /// Serializable schedule data
    /// </summary>
    [Serializable]
    public class ScheduleSaveData
    {
        public int seasonYear;
        public int currentWeek;
        public bool isComplete;
        public List<MatchSaveData> matches = new List<MatchSaveData>();
    }

    /// <summary>
    /// Serializable match data
    /// </summary>
    [Serializable]
    public class MatchSaveData
    {
        public int week;
        public string homeTeamName;
        public string awayTeamName;
        public bool hasBeenPlayed;
        public int homeScore;
        public int awayScore;
        public string winnerTeamName;
    }

    /// <summary>
    /// Serializable playoff data
    /// </summary>
    [Serializable]
    public class PlayoffSaveData
    {
        public int seasonYear;
        public int currentRound;
        public bool isComplete;
        public string championTeamName;
    }

    /// <summary>
    /// Serializable Color (Unity Color is not serializable to JSON)
    /// </summary>
    [Serializable]
    public class SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public SerializableColor() { }

        public SerializableColor(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }

    /// <summary>
    /// Manages saving and loading game data
    /// </summary>
    public class SaveLoadManager : MonoBehaviour
    {
        [Header("Save Settings")]
        public string saveDirectory = "Saves";
        public string defaultSaveName = "GameSave";
        public bool autoSaveEnabled = true;

        private string SavePath => Path.Combine(Application.persistentDataPath, saveDirectory);

        void Awake()
        {
            // Ensure save directory exists
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
                Debug.Log($"Created save directory: {SavePath}");
            }
        }

        /// <summary>
        /// Save the complete game state
        /// </summary>
        public bool SaveGame(GameSaveData saveData, string fileName = null)
        {
            if (fileName == null)
                fileName = defaultSaveName;

            try
            {
                string filePath = Path.Combine(SavePath, fileName + ".json");
                string json = JsonUtility.ToJson(saveData, true);
                File.WriteAllText(filePath, json);

                Debug.Log($"Game saved successfully to: {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load game state from file
        /// </summary>
        public GameSaveData LoadGame(string fileName = null)
        {
            if (fileName == null)
                fileName = defaultSaveName;

            try
            {
                string filePath = Path.Combine(SavePath, fileName + ".json");

                if (!File.Exists(filePath))
                {
                    Debug.LogWarning($"Save file not found: {filePath}");
                    return null;
                }

                string json = File.ReadAllText(filePath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                Debug.Log($"Game loaded successfully from: {filePath}");
                Debug.Log($"Save: {saveData.saveName} - {saveData.saveDate}");
                return saveData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Delete a save file
        /// </summary>
        public bool DeleteSave(string fileName)
        {
            try
            {
                string filePath = Path.Combine(SavePath, fileName + ".json");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"Save file deleted: {filePath}");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Save file not found: {filePath}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete save: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get list of all save files
        /// </summary>
        public List<string> GetSaveFiles()
        {
            List<string> saveFiles = new List<string>();

            try
            {
                string[] files = Directory.GetFiles(SavePath, "*.json");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    saveFiles.Add(fileName);
                }

                Debug.Log($"Found {saveFiles.Count} save file(s)");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to get save files: {e.Message}");
            }

            return saveFiles;
        }

        /// <summary>
        /// Check if a save file exists
        /// </summary>
        public bool SaveExists(string fileName)
        {
            string filePath = Path.Combine(SavePath, fileName + ".json");
            return File.Exists(filePath);
        }

        /// <summary>
        /// Get save file info without loading the entire file
        /// </summary>
        public string GetSaveInfo(string fileName)
        {
            try
            {
                string filePath = Path.Combine(SavePath, fileName + ".json");

                if (!File.Exists(filePath))
                    return "Save file not found";

                FileInfo fileInfo = new FileInfo(filePath);
                return $"File: {fileName}\nSize: {fileInfo.Length / 1024}KB\nModified: {fileInfo.LastWriteTime}";
            }
            catch (Exception e)
            {
                return $"Error: {e.Message}";
            }
        }

        /// <summary>
        /// Create a GameSaveData from current game state
        /// </summary>
        public GameSaveData CreateSaveData(string saveName, TeamManager teamManager, LeagueManager leagueManager)
        {
            var saveData = new GameSaveData
            {
                saveName = saveName,
                leagueName = leagueManager.leagueName,
                currentSeasonYear = leagueManager.currentSeasonYear,
                seasonInProgress = leagueManager.seasonInProgress,
                playoffsInProgress = leagueManager.playoffsInProgress
            };

            // Save teams
            foreach (var team in teamManager.teams)
            {
                saveData.teams.Add(TeamSaveData.FromTeamData(team));
            }

            // Save current schedule (simplified - we'll save basic state)
            if (leagueManager.currentSchedule != null)
            {
                saveData.currentSchedule = new ScheduleSaveData
                {
                    seasonYear = leagueManager.currentSchedule.seasonYear,
                    currentWeek = leagueManager.currentSchedule.currentWeek,
                    isComplete = leagueManager.currentSchedule.isComplete
                };
            }

            // Save playoff state
            if (leagueManager.currentPlayoffBracket != null)
            {
                saveData.currentPlayoffs = new PlayoffSaveData
                {
                    seasonYear = leagueManager.currentPlayoffBracket.seasonYear,
                    currentRound = leagueManager.currentPlayoffBracket.currentRound,
                    isComplete = leagueManager.currentPlayoffBracket.isComplete,
                    championTeamName = leagueManager.currentPlayoffBracket.champion?.teamName ?? ""
                };
            }

            return saveData;
        }

        /// <summary>
        /// Load game state into managers
        /// </summary>
        public void LoadIntoManagers(GameSaveData saveData, TeamManager teamManager, LeagueManager leagueManager)
        {
            if (saveData == null)
            {
                Debug.LogError("Cannot load null save data");
                return;
            }

            // Clear existing data
            teamManager.teams.Clear();

            // Load teams
            foreach (var teamSave in saveData.teams)
            {
                var team = teamSave.ToTeamData();
                teamManager.teams.Add(team);
            }

            // Restore league state
            leagueManager.leagueName = saveData.leagueName;
            leagueManager.currentSeasonYear = saveData.currentSeasonYear;
            leagueManager.seasonInProgress = saveData.seasonInProgress;
            leagueManager.playoffsInProgress = saveData.playoffsInProgress;

            Debug.Log($"Loaded game: {saveData.saveName}");
            Debug.Log($"Teams: {teamManager.teams.Count}");
            Debug.Log($"Season: {saveData.currentSeasonYear}");
        }
    }
}
