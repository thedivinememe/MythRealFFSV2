using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Combat;
using System.Collections.Generic;
using System.Linq;

namespace MythRealFFSV2.UI
{
    /// <summary>
    /// Displays match results and battle statistics
    /// </summary>
    public class MatchResultsUI : MonoBehaviour
    {
        [Header("Display Settings")]
        public int maxMatchesToShow = 10;

        [Header("UI Elements")]
        public TextMeshProUGUI headerText;
        public Transform matchListContainer;
        public GameObject matchResultPrefab;

        [Header("Detail Panel")]
        public GameObject detailPanel;
        public TextMeshProUGUI detailMatchupText;
        public TextMeshProUGUI detailScoreText;
        public TextMeshProUGUI detailStatsText;
        public Button closeDetailButton;

        [Header("Navigation")]
        public Button backButton;

        private LeagueManager leagueManager;
        private Match selectedMatch;

        void Start()
        {
            // Get managers
            leagueManager = FindObjectOfType<LeagueManager>();

            // Setup buttons
            if (backButton != null)
                backButton.onClick.AddListener(OnBack);

            if (closeDetailButton != null)
                closeDetailButton.onClick.AddListener(OnCloseDetail);

            // Hide detail panel initially
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        void OnEnable()
        {
            RefreshDisplay();
        }

        /// <summary>
        /// Refresh the match results display
        /// </summary>
        public void RefreshDisplay()
        {
            if (leagueManager == null)
                return;

            UpdateHeader();
            UpdateMatchList();
        }

        /// <summary>
        /// Update header information
        /// </summary>
        void UpdateHeader()
        {
            if (headerText != null)
            {
                headerText.text = $"Match Results - Season {leagueManager.currentSeasonYear}";
            }
        }

        /// <summary>
        /// Update the list of recent matches
        /// </summary>
        void UpdateMatchList()
        {
            if (matchListContainer == null)
                return;

            // Clear existing items
            foreach (Transform child in matchListContainer)
            {
                Destroy(child.gameObject);
            }

            // Get all matches
            var allMatches = leagueManager.GetAllMatches();

            // Get only played matches, sorted by most recent
            var playedMatches = allMatches
                .Where(m => m.hasBeenPlayed)
                .OrderByDescending(m => m.week)
                .Take(maxMatchesToShow)
                .ToList();

            if (playedMatches.Count == 0)
            {
                CreateNoMatchesMessage();
                return;
            }

            // Create match result row for each match
            foreach (var match in playedMatches)
            {
                CreateMatchResultRow(match);
            }
        }

        /// <summary>
        /// Create a message when no matches have been played
        /// </summary>
        void CreateNoMatchesMessage()
        {
            if (matchResultPrefab == null)
                return;

            GameObject messageObj = Instantiate(matchResultPrefab, matchListContainer);
            TextMeshProUGUI messageText = messageObj.GetComponentInChildren<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = "No matches have been played yet.";
            }
        }

        /// <summary>
        /// Create a match result row
        /// </summary>
        void CreateMatchResultRow(Match match)
        {
            if (matchResultPrefab == null)
            {
                Debug.LogWarning("Match result prefab not assigned!");
                return;
            }

            GameObject row = Instantiate(matchResultPrefab, matchListContainer);

            // Find text elements (adjust based on your prefab structure)
            TextMeshProUGUI weekText = row.transform.Find("WeekText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI matchupText = row.transform.Find("MatchupText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI scoreText = row.transform.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI resultText = row.transform.Find("ResultText")?.GetComponent<TextMeshProUGUI>();

            // Set text values
            if (weekText != null)
                weekText.text = $"Week {match.week + 1}";

            if (matchupText != null)
                matchupText.text = $"{match.homeTeam.teamName} vs {match.awayTeam.teamName}";

            if (scoreText != null)
                scoreText.text = $"{match.homeScore} - {match.awayScore}";

            if (resultText != null)
            {
                if (match.winner != null)
                {
                    resultText.text = $"W: {match.winner.teamName}";
                }
                else
                {
                    resultText.text = "DRAW";
                }
            }

            // Add click handler to show details
            Button rowButton = row.GetComponent<Button>();
            if (rowButton != null)
            {
                rowButton.onClick.AddListener(() => OnShowMatchDetail(match));
            }
        }

        /// <summary>
        /// Show detailed match information
        /// </summary>
        void OnShowMatchDetail(Match match)
        {
            selectedMatch = match;

            if (detailPanel == null)
                return;

            detailPanel.SetActive(true);

            // Set matchup info
            if (detailMatchupText != null)
            {
                detailMatchupText.text = $"Week {match.week + 1}: {match.homeTeam.teamName} vs {match.awayTeam.teamName}";
            }

            // Set score info
            if (detailScoreText != null)
            {
                string winnerText = match.winner != null ? $"\nWinner: {match.winner.teamName}" : "\nResult: DRAW";
                detailScoreText.text = $"Final Score: {match.homeScore} - {match.awayScore}{winnerText}";
            }

            // Set battle statistics
            if (detailStatsText != null && match.battleResult != null)
            {
                var stats = match.battleResult.statistics;
                string statsText = "Battle Statistics:\n\n";
                statsText += $"{match.homeTeam.teamName}:\n";
                statsText += $"  Total Damage: {stats.team1TotalDamage}\n";
                statsText += $"  Characters KO'd: {stats.team2CharactersKOd}\n";
                statsText += $"  Final HP: {stats.team1FinalHP}\n\n";
                statsText += $"{match.awayTeam.teamName}:\n";
                statsText += $"  Total Damage: {stats.team2TotalDamage}\n";
                statsText += $"  Characters KO'd: {stats.team1CharactersKOd}\n";
                statsText += $"  Final HP: {stats.team2FinalHP}\n\n";
                statsText += $"Total Turns: {stats.totalTurns}\n";
                statsText += $"Battle Duration: {stats.battleDuration:F2}s";

                detailStatsText.text = statsText;
            }
        }

        /// <summary>
        /// Close the detail panel
        /// </summary>
        void OnCloseDetail()
        {
            if (detailPanel != null)
                detailPanel.SetActive(false);
        }

        /// <summary>
        /// Go back to league dashboard
        /// </summary>
        void OnBack()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.GoBack();
            }
        }
    }
}
