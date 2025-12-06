using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Data;
using System.Collections.Generic;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Controls the main menu UI
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button newGameButton;
        public Button loadGameButton;
        public Button quitButton;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI versionText;

        [Header("New Game Settings")]
        public int defaultNumberOfTeams = 4;
        public int defaultDraftRounds = 5;
        public int defaultCharacterPoolSize = 30;

        [Header("Data Assets")]
        public List<AncestryData> ancestries;
        public List<BackgroundData> backgrounds;
        public List<AbilityData> abilities;

        private TeamManager teamManager;
        private DraftManager draftManager;
        private LeagueManager leagueManager;
        private SaveLoadManager saveLoadManager;

        void Start()
        {
            // Get managers
            teamManager = FindObjectOfType<TeamManager>();
            draftManager = FindObjectOfType<DraftManager>();
            leagueManager = FindObjectOfType<LeagueManager>();
            saveLoadManager = FindObjectOfType<SaveLoadManager>();

            // Setup buttons
            if (newGameButton != null)
                newGameButton.onClick.AddListener(OnNewGame);

            if (loadGameButton != null)
                loadGameButton.onClick.AddListener(OnLoadGame);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuit);

            // Set version text
            if (versionText != null)
                versionText.text = "v1.0";
        }

        void OnNewGame()
        {
            Debug.Log("Starting new game...");

            // Clear any existing data
            if (teamManager != null)
                teamManager.teams.Clear();

            // Start draft process
            StartDraftProcess();
        }

        void StartDraftProcess()
        {
            Debug.Log("=== STARTING NEW LEAGUE ===\n");

            // Create teams
            if (teamManager == null)
            {
                Debug.LogError("TeamManager not found!");
                return;
            }

            for (int i = 0; i < defaultNumberOfTeams; i++)
            {
                teamManager.CreateRandomTeam();
            }

            Debug.Log($"Created {defaultNumberOfTeams} teams");

            // Start draft
            if (draftManager != null && ancestries.Count > 0 && backgrounds.Count > 0)
            {
                draftManager.StartDraft(
                    teamManager.GetAllTeams(),
                    defaultDraftRounds,
                    defaultCharacterPoolSize,
                    ancestries,
                    backgrounds
                );

                // Run auto draft
                draftManager.RunAutoDraft();

                // Assign abilities
                AssignAbilities();

                Debug.Log("Draft complete!");
            }

            // Show league dashboard
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowLeagueDashboard();
            }
        }

        void AssignAbilities()
        {
            if (abilities == null || abilities.Count == 0 || teamManager == null)
                return;

            foreach (var team in teamManager.teams)
            {
                foreach (var character in team.roster)
                {
                    int numAbilities = Random.Range(1, 4);
                    for (int i = 0; i < numAbilities; i++)
                    {
                        var ability = abilities[Random.Range(0, abilities.Count)];
                        if (character.CanLearnAbility(ability) && !character.HasAbility(ability))
                        {
                            character.LearnAbility(ability);
                        }
                    }
                }
            }
        }

        void OnLoadGame()
        {
            Debug.Log("Loading game...");

            if (saveLoadManager == null)
            {
                Debug.LogError("SaveLoadManager not found!");
                return;
            }

            // Try to load quick save
            var saveData = saveLoadManager.LoadGame("QuickSave");

            if (saveData != null && teamManager != null && leagueManager != null)
            {
                saveLoadManager.LoadIntoManagers(saveData, teamManager, leagueManager);
                Debug.Log("Game loaded successfully!");

                // Show league dashboard
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowLeagueDashboard();
                }
            }
            else
            {
                Debug.LogWarning("No save file found! Starting new game instead.");
                OnNewGame();
            }
        }

        void OnQuit()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.QuitGame();
            }
        }
    }
}
