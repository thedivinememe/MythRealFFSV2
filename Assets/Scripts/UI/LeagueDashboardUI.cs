using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MythRealFFSV2.Systems;
using System.Collections.Generic;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Controls the League Dashboard UI - main hub for season management
    /// </summary>
    public class LeagueDashboardUI : MonoBehaviour
    {
        [Header("Season Info")]
        public TextMeshProUGUI leagueNameText;
        public TextMeshProUGUI seasonInfoText;
        public TextMeshProUGUI scheduleInfoText;

        [Header("Playoff Info")]
        public GameObject playoffPanel;
        public TextMeshProUGUI playoffStatusText;
        public TextMeshProUGUI playoffMatchupsText;

        [Header("Standings Display")]
        public Transform standingsContainer;
        public GameObject standingsRowPrefab;

        [Header("Control Buttons")]
        public Button playNextWeekButton;
        public Button simulateSeasonButton;
        public Button startNewSeasonButton;
        public Button playPlayoffsButton;

        [Header("Navigation Buttons")]
        public Button viewMatchResultsButton;
        public Button viewTeamRostersButton;
        public Button saveGameButton;
        public Button mainMenuButton;

        [Header("Managers")]
        public LeagueManager leagueManager;
        public TeamManager teamManager;
        public SaveLoadManager saveLoadManager;

        void Start()
        {
            // Get managers if not assigned
            if (leagueManager == null)
                leagueManager = FindFirstObjectByType<LeagueManager>();
            if (teamManager == null)
                teamManager = FindFirstObjectByType<TeamManager>();
            if (saveLoadManager == null)
                saveLoadManager = FindFirstObjectByType<SaveLoadManager>();

            // Setup control buttons
            if (playNextWeekButton != null)
                playNextWeekButton.onClick.AddListener(OnPlayNextWeek);

            if (simulateSeasonButton != null)
                simulateSeasonButton.onClick.AddListener(OnSimulateSeason);

            if (startNewSeasonButton != null)
                startNewSeasonButton.onClick.AddListener(OnStartNewSeason);

            if (playPlayoffsButton != null)
                playPlayoffsButton.onClick.AddListener(OnPlayPlayoffs);

            // Setup navigation buttons
            if (viewMatchResultsButton != null)
                viewMatchResultsButton.onClick.AddListener(OnViewMatchResults);

            if (viewTeamRostersButton != null)
                viewTeamRostersButton.onClick.AddListener(OnViewTeamRosters);

            if (saveGameButton != null)
                saveGameButton.onClick.AddListener(OnSaveGame);

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(OnMainMenu);

            // Subscribe to league events
            if (leagueManager != null)
            {
                leagueManager.onMatchComplete.AddListener(OnMatchComplete);
                leagueManager.onSeasonComplete.AddListener(OnSeasonComplete);
            }
        }

        void OnEnable()
        {
            // Refresh display when screen becomes active
            RefreshDisplay();
        }

        /// <summary>
        /// Refresh all UI elements with current game state
        /// </summary>
        public void RefreshDisplay()
        {
            if (leagueManager == null || teamManager == null)
                return;

            UpdateSeasonInfo();
            UpdateStandings();
            UpdateButtons();
            UpdatePlayoffDisplay();
        }

        /// <summary>
        /// Update season information display
        /// </summary>
        void UpdateSeasonInfo()
        {
            if (leagueNameText != null)
            {
                leagueNameText.text = leagueManager.leagueName;
            }

            if (seasonInfoText != null)
            {
                string status = leagueManager.seasonInProgress ? "In Progress" :
                               leagueManager.playoffsInProgress ? "Playoffs" : "Offseason";
                seasonInfoText.text = $"Season {leagueManager.currentSeasonYear} - {status}";
            }

            if (scheduleInfoText != null)
            {
                scheduleInfoText.text = leagueManager.GetScheduleSummary();
            }
        }

        /// <summary>
        /// Update standings table
        /// </summary>
        void UpdateStandings()
        {
            if (standingsContainer == null)
                return;

            // Clear existing rows
            foreach (Transform child in standingsContainer)
            {
                Destroy(child.gameObject);
            }

            // Get current standings
            var standings = teamManager.GetStandings();

            // Create row for each team
            for (int i = 0; i < standings.Count; i++)
            {
                CreateStandingsRow(i + 1, standings[i]);
            }
        }

        /// <summary>
        /// Create a standings row for a team
        /// </summary>
        void CreateStandingsRow(int rank, TeamData team)
        {
            if (standingsRowPrefab == null)
            {
                Debug.LogWarning("Standings row prefab not assigned!");
                return;
            }

            GameObject row = Instantiate(standingsRowPrefab, standingsContainer);

            // Get the Image component for background color
            UnityEngine.UI.Image rowBackground = row.GetComponent<UnityEngine.UI.Image>();

            // Find text elements in the row (assumes specific naming convention)
            // You'll need to adjust these based on your actual prefab structure
            TextMeshProUGUI rankText = row.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI teamNameText = row.transform.Find("TeamNameText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI recordText = row.transform.Find("RecordText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI winPctText = row.transform.Find("WinPctText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI diffText = row.transform.Find("DiffText")?.GetComponent<TextMeshProUGUI>();

            // Set text values
            if (rankText != null)
                rankText.text = rank.ToString();

            if (teamNameText != null)
            {
                teamNameText.text = team.teamName;
                // Use team color for team name
                teamNameText.color = team.teamColor;
            }

            if (recordText != null)
                recordText.text = $"{team.wins}-{team.losses}-{team.draws}";

            if (winPctText != null)
                winPctText.text = team.WinPercentage.ToString("P1");

            if (diffText != null)
            {
                int diff = team.PointDifferential;
                diffText.text = diff >= 0 ? $"+{diff}" : diff.ToString();
                // Color code the differential
                diffText.color = diff > 0 ? Color.green : (diff < 0 ? Color.red : Color.gray);
            }

            // Style the row background
            if (rowBackground != null)
            {
                // Playoff teams get highlighted (top 4)
                if (rank <= leagueManager.playoffTeamCount && leagueManager.enablePlayoffs)
                {
                    // Gold tint for playoff teams
                    rowBackground.color = new Color(1f, 0.9f, 0.6f, 0.3f);
                }
                else
                {
                    // Alternating row colors for readability
                    rowBackground.color = (rank % 2 == 0) ?
                        new Color(0.9f, 0.9f, 0.9f, 0.5f) :
                        new Color(1f, 1f, 1f, 0.3f);
                }
            }

            // Optional: Add click handler to view team roster
            Button rowButton = row.GetComponent<Button>();
            if (rowButton != null)
            {
                rowButton.onClick.AddListener(() => OnViewTeamRoster(team));
            }
        }

        /// <summary>
        /// Update button states based on game state
        /// </summary>
        void UpdateButtons()
        {
            bool seasonActive = leagueManager.seasonInProgress;
            bool playoffsActive = leagueManager.playoffsInProgress;
            bool canStartSeason = !seasonActive && !playoffsActive && teamManager.teams.Count >= 2;

            if (playNextWeekButton != null)
                playNextWeekButton.interactable = seasonActive;

            if (simulateSeasonButton != null)
                simulateSeasonButton.interactable = seasonActive;

            if (startNewSeasonButton != null)
                startNewSeasonButton.interactable = canStartSeason;

            if (playPlayoffsButton != null)
                playPlayoffsButton.interactable = playoffsActive;
        }

        /// <summary>
        /// Update playoff display panel
        /// </summary>
        void UpdatePlayoffDisplay()
        {
            bool showPlayoffs = leagueManager.playoffsInProgress;

            // Show/hide playoff panel
            if (playoffPanel != null)
                playoffPanel.SetActive(showPlayoffs);

            if (!showPlayoffs || leagueManager.currentPlayoffBracket == null)
                return;

            var bracket = leagueManager.currentPlayoffBracket;

            // Update playoff status
            if (playoffStatusText != null)
            {
                string roundName = "Playoffs";
                var currentRoundMatches = bracket.GetMatchesForRound(bracket.currentRound);
                if (currentRoundMatches.Count > 0)
                {
                    roundName = currentRoundMatches[0].roundName;
                }

                playoffStatusText.text = $"{roundName} - {leagueManager.currentSeasonYear}";

                if (bracket.champion != null)
                {
                    playoffStatusText.text += $"\n🏆 CHAMPION: {bracket.champion.teamName}! 🏆";
                }
            }

            // Update playoff matchups
            if (playoffMatchupsText != null)
            {
                var currentRoundMatches = bracket.GetMatchesForRound(bracket.currentRound);
                string matchups = "Current Matchups:\n\n";

                if (currentRoundMatches.Count == 0)
                {
                    matchups = "Playoffs Complete!";
                }
                else
                {
                    foreach (var match in currentRoundMatches)
                    {
                        if (match.team1 != null && match.team2 != null)
                        {
                            if (match.winner != null)
                            {
                                matchups += $"#{match.seed1} {match.team1.teamName} vs #{match.seed2} {match.team2.teamName}\n";
                                matchups += $"   ✓ Winner: {match.winner.teamName}\n\n";
                            }
                            else
                            {
                                matchups += $"#{match.seed1} {match.team1.teamName} vs #{match.seed2} {match.team2.teamName}\n\n";
                            }
                        }
                    }
                }

                playoffMatchupsText.text = matchups;
            }
        }

        #region Button Handlers

        void OnPlayNextWeek()
        {
            if (leagueManager != null)
            {
                leagueManager.PlayNextWeek();
                // UI will update via event handlers
            }
        }

        void OnSimulateSeason()
        {
            if (leagueManager != null)
            {
                leagueManager.PlayEntireSeason();
                // UI will update via event handlers
            }
        }

        void OnStartNewSeason()
        {
            if (leagueManager != null)
            {
                leagueManager.StartNewSeason();
                RefreshDisplay();
            }
        }

        void OnPlayPlayoffs()
        {
            if (leagueManager != null)
            {
                leagueManager.PlayAllPlayoffRounds();
                // UI will update via event handlers
            }
        }

        void OnViewMatchResults()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMatchResults();
            }
        }

        void OnViewTeamRosters()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTeamRoster();
            }
        }

        void OnViewTeamRoster(TeamData team)
        {
            // Store selected team reference (you'll need to add this to UIManager or TeamRosterUI)
            // For now, just navigate to roster screen
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowTeamRoster();
            }
        }

        void OnSaveGame()
        {
            if (saveLoadManager != null && teamManager != null && leagueManager != null)
            {
                string saveName = "QuickSave";
                var saveData = saveLoadManager.CreateSaveData(saveName, teamManager, leagueManager);
                bool success = saveLoadManager.SaveGame(saveData, saveName);

                if (success)
                {
                    Debug.Log("Game saved successfully!");
                    // Optionally show a save confirmation popup
                }
            }
        }

        void OnMainMenu()
        {
            if (UIManager.Instance != null)
            {
                // Show confirmation dialog before returning to main menu
                // For now, just go back
                UIManager.Instance.ShowMainMenu();
            }
        }

        #endregion

        #region Event Handlers

        void OnMatchComplete(Match match)
        {
            // Refresh entire display after each match
            RefreshDisplay();
        }

        void OnSeasonComplete(Schedule schedule, List<TeamData> finalStandings)
        {
            // Refresh entire display when season ends
            RefreshDisplay();

            // Optionally show a season complete message
            Debug.Log("Season Complete!");
        }

        #endregion

        void OnDestroy()
        {
            // Cleanup event listeners
            if (leagueManager != null)
            {
                leagueManager.onMatchComplete.RemoveListener(OnMatchComplete);
                leagueManager.onSeasonComplete.RemoveListener(OnSeasonComplete);
            }
        }
    }
}
