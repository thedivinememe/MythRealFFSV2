using UnityEngine;
using MythRealFFSV2.Systems;

namespace MythRealFFSV2.Examples
{
    /// <summary>
    /// Example demonstrating save/load functionality
    /// Shows how to save and load game progress
    /// </summary>
    public class SaveLoadExample : MonoBehaviour
    {
        [Header("Save Settings")]
        public string currentSaveName = "MyLeague";

        private SaveLoadManager saveLoadManager;
        private TeamManager teamManager;
        private LeagueManager leagueManager;

        void Start()
        {
            // Get or create components
            saveLoadManager = GetComponent<SaveLoadManager>();
            if (saveLoadManager == null)
                saveLoadManager = gameObject.AddComponent<SaveLoadManager>();

            teamManager = GetComponent<TeamManager>();
            if (teamManager == null)
                teamManager = gameObject.AddComponent<TeamManager>();

            leagueManager = GetComponent<LeagueManager>();
            if (leagueManager == null)
                leagueManager = gameObject.AddComponent<LeagueManager>();
        }

        [ContextMenu("Save Game")]
        void SaveGame()
        {
            if (teamManager.teams.Count == 0)
            {
                Debug.LogWarning("No teams to save! Create a league first.");
                return;
            }

            Debug.Log("=== SAVING GAME ===\n");

            var saveData = saveLoadManager.CreateSaveData(currentSaveName, teamManager, leagueManager);
            bool success = saveLoadManager.SaveGame(saveData, currentSaveName);

            if (success)
            {
                Debug.Log($"✓ Game saved as '{currentSaveName}'");
                Debug.Log($"  Teams: {saveData.teams.Count}");
                Debug.Log($"  Season: {saveData.currentSeasonYear}");
                Debug.Log($"  In Progress: {saveData.seasonInProgress}\n");
            }
        }

        [ContextMenu("Load Game")]
        void LoadGame()
        {
            Debug.Log("=== LOADING GAME ===\n");

            var saveData = saveLoadManager.LoadGame(currentSaveName);

            if (saveData != null)
            {
                saveLoadManager.LoadIntoManagers(saveData, teamManager, leagueManager);

                Debug.Log($"✓ Game loaded: {saveData.saveName}");
                Debug.Log($"  Saved on: {saveData.saveDate}");
                Debug.Log($"  League: {saveData.leagueName}");
                Debug.Log($"  Teams: {saveData.teams.Count}");
                Debug.Log($"  Season: {saveData.currentSeasonYear}\n");

                // Show team details
                ShowLoadedTeams();
            }
            else
            {
                Debug.LogError($"Failed to load save '{currentSaveName}'");
            }
        }

        [ContextMenu("Quick Save")]
        void QuickSave()
        {
            currentSaveName = "QuickSave";
            SaveGame();
        }

        [ContextMenu("Quick Load")]
        void QuickLoad()
        {
            currentSaveName = "QuickSave";
            LoadGame();
        }

        [ContextMenu("List All Saves")]
        void ListAllSaves()
        {
            Debug.Log("=== SAVE FILES ===\n");

            var saves = saveLoadManager.GetSaveFiles();

            if (saves.Count == 0)
            {
                Debug.Log("No save files found");
                return;
            }

            foreach (var saveName in saves)
            {
                Debug.Log($"📁 {saveName}");
                Debug.Log($"   {saveLoadManager.GetSaveInfo(saveName)}\n");
            }
        }

        [ContextMenu("Delete Current Save")]
        void DeleteCurrentSave()
        {
            if (saveLoadManager.SaveExists(currentSaveName))
            {
                Debug.Log($"Deleting save: {currentSaveName}");
                saveLoadManager.DeleteSave(currentSaveName);
            }
            else
            {
                Debug.Log($"Save '{currentSaveName}' does not exist");
            }
        }

        [ContextMenu("Show Save Location")]
        void ShowSaveLocation()
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, saveLoadManager.saveDirectory);
            Debug.Log($"Save files location:\n{path}");

            // Open the folder (platform specific)
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path.Replace("/", "\\"));
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#else
            Debug.Log("Open folder manually: " + path);
#endif
        }

        void ShowLoadedTeams()
        {
            Debug.Log("=== LOADED TEAMS ===\n");

            foreach (var team in teamManager.teams)
            {
                Debug.Log($"{team.teamName}:");
                Debug.Log($"  Record: {team.wins}-{team.losses}-{team.draws}");
                Debug.Log($"  Roster: {team.roster.Count} players");
                Debug.Log($"  Championships: {team.championships}\n");
            }
        }

        #region Complete Workflow Examples

        [ContextMenu("Test: Save After Draft")]
        void TestSaveAfterDraft()
        {
            Debug.Log("=== TEST: Save After Draft ===\n");
            Debug.Log("1. Use DraftExample to create and draft teams");
            Debug.Log("2. Then use 'Save Game' to save the drafted rosters");
            Debug.Log("3. Use 'Load Game' to restore them later\n");
        }

        [ContextMenu("Test: Save Mid-Season")]
        void TestSaveMidSeason()
        {
            Debug.Log("=== TEST: Save Mid-Season ===\n");
            Debug.Log("1. Use SeasonSimulationExample to start a season");
            Debug.Log("2. Play a few weeks");
            Debug.Log("3. Use 'Save Game' to save progress");
            Debug.Log("4. Use 'Load Game' to continue later\n");
        }

        #endregion
    }
}
