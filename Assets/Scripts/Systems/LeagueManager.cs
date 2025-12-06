using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MythRealFFSV2.Combat;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// UnityEvent for match completion
    /// </summary>
    [System.Serializable]
    public class MatchCompleteEvent : UnityEvent<Match> { }

    /// <summary>
    /// UnityEvent for season completion
    /// </summary>
    [System.Serializable]
    public class SeasonCompleteEvent : UnityEvent<Schedule, List<TeamData>> { }

    /// <summary>
    /// Manages league operations, season progression, and match simulation
    /// </summary>
    public class LeagueManager : MonoBehaviour
    {
        [Header("League Settings")]
        public string leagueName = "MythReal Fantasy League";
        public int currentSeasonYear = 2025;
        public bool doubleRoundRobin = false;

        [Header("Simulation Settings")]
        public bool autoPlayMatches = true;
        public float matchDelay = 1f; // Delay between matches in a week

        [Header("Current Season")]
        public Schedule currentSchedule;
        public bool seasonInProgress;

        [Header("Events")]
        public MatchCompleteEvent onMatchComplete = new MatchCompleteEvent();
        public SeasonCompleteEvent onSeasonComplete = new SeasonCompleteEvent();

        [Header("Components")]
        private TeamManager teamManager;
        private BattleSimulator battleSimulator;

        void Awake()
        {
            teamManager = GetComponent<TeamManager>();
            if (teamManager == null)
                teamManager = gameObject.AddComponent<TeamManager>();

            battleSimulator = GetComponent<BattleSimulator>();
            if (battleSimulator == null)
                battleSimulator = gameObject.AddComponent<BattleSimulator>();

            // Configure battle simulator for quick matches
            battleSimulator.instantSimulation = true;
        }

        /// <summary>
        /// Start a new season with the current teams
        /// </summary>
        public void StartNewSeason()
        {
            if (teamManager.teams.Count < 2)
            {
                Debug.LogError("Need at least 2 teams to start a season!");
                return;
            }

            if (seasonInProgress)
            {
                Debug.LogWarning("Season already in progress! Finish current season first.");
                return;
            }

            // Reset all team stats
            teamManager.ResetAllSeasonStats();

            // Generate schedule
            if (doubleRoundRobin)
            {
                currentSchedule = ScheduleGenerator.GenerateDoubleRoundRobinSchedule(
                    teamManager.GetAllTeams(), currentSeasonYear);
            }
            else
            {
                currentSchedule = ScheduleGenerator.GenerateRoundRobinSchedule(
                    teamManager.GetAllTeams(), currentSeasonYear);
            }

            seasonInProgress = true;

            Debug.Log($"=== {leagueName} Season {currentSeasonYear} Started ===");
            Debug.Log($"Teams: {teamManager.teams.Count}");
            Debug.Log($"Total Matches: {currentSchedule.matches.Count}");
            Debug.Log($"Weeks: {currentSchedule.GetTotalWeeks()}");
        }

        /// <summary>
        /// Play the next week of matches
        /// </summary>
        public void PlayNextWeek()
        {
            if (!seasonInProgress)
            {
                Debug.LogWarning("No season in progress!");
                return;
            }

            if (!currentSchedule.HasMoreMatches())
            {
                Debug.LogWarning("Season is complete!");
                EndSeason();
                return;
            }

            StartCoroutine(PlayWeekMatches());
        }

        /// <summary>
        /// Play all remaining matches in the season
        /// </summary>
        public void PlayEntireSeason()
        {
            if (!seasonInProgress)
            {
                Debug.LogWarning("No season in progress!");
                return;
            }

            StartCoroutine(PlayAllWeeks());
        }

        /// <summary>
        /// Coroutine to play all matches in a week
        /// </summary>
        IEnumerator PlayWeekMatches()
        {
            List<Match> weekMatches = currentSchedule.GetNextMatches();

            Debug.Log($"\n=== WEEK {currentSchedule.currentWeek + 1} ===");
            Debug.Log($"Matches: {weekMatches.Count}");

            foreach (var match in weekMatches)
            {
                yield return PlayMatch(match);

                if (matchDelay > 0)
                {
                    yield return new WaitForSeconds(matchDelay);
                }
            }

            // Heal all rosters after the week
            teamManager.HealAllRosters();

            // Advance to next week
            currentSchedule.AdvanceWeek();

            // Print standings
            Debug.Log("\n=== STANDINGS ===");
            teamManager.PrintStandings();

            // Check if season is complete
            if (!currentSchedule.HasMoreMatches())
            {
                EndSeason();
            }
        }

        /// <summary>
        /// Coroutine to play entire season
        /// </summary>
        IEnumerator PlayAllWeeks()
        {
            while (currentSchedule.HasMoreMatches())
            {
                yield return PlayWeekMatches();
            }
        }

        /// <summary>
        /// Play a single match
        /// </summary>
        IEnumerator PlayMatch(Match match)
        {
            Debug.Log($"\n{match.homeTeam.teamName} vs {match.awayTeam.teamName}");

            // Get active rosters
            var homeRoster = match.homeTeam.GetActiveRoster();
            var awayRoster = match.awayTeam.GetActiveRoster();

            if (homeRoster.Count == 0 || awayRoster.Count == 0)
            {
                Debug.LogError($"Cannot play match - one or both teams have no active characters!");
                yield break;
            }

            // Simulate battle
            bool battleComplete = false;
            BattleResult result = null;

            battleSimulator.onBattleComplete.AddListener((BattleResult r) =>
            {
                result = r;
                battleComplete = true;
            });

            battleSimulator.SimulateBattle(homeRoster, awayRoster);

            // Wait for battle to complete
            while (!battleComplete)
            {
                yield return null;
            }

            // Calculate scores based on damage dealt
            int homeScore = result.statistics.team1TotalDamage;
            int awayScore = result.statistics.team2TotalDamage;

            // Record result
            match.RecordResult(result, homeScore, awayScore);

            Debug.Log($"FINAL: {match.homeTeam.teamName} {homeScore} - {awayScore} {match.awayTeam.teamName}");
            if (match.winner != null)
            {
                Debug.Log($"Winner: {match.winner.teamName}");
            }
            else
            {
                Debug.Log("Result: DRAW");
            }

            // Fire match complete event
            onMatchComplete?.Invoke(match);
        }

        /// <summary>
        /// End the current season
        /// </summary>
        void EndSeason()
        {
            seasonInProgress = false;

            Debug.Log($"\n=== SEASON {currentSeasonYear} COMPLETE ===");

            // Get final standings
            var finalStandings = teamManager.GetStandings();

            Debug.Log("\n=== FINAL STANDINGS ===");
            for (int i = 0; i < finalStandings.Count; i++)
            {
                var team = finalStandings[i];
                string trophy = i == 0 ? " 🏆" : "";
                Debug.Log($"{i + 1}. {team.teamName}{trophy}: {team.wins}-{team.losses}-{team.draws} " +
                         $"({team.WinPercentage:P1}) | +/- {team.PointDifferential:+#;-#;0}");
            }

            // Award championship
            if (finalStandings.Count > 0)
            {
                finalStandings[0].championships++;
                Debug.Log($"\n🏆 {finalStandings[0].teamName} are the champions! 🏆");
            }

            // Fire season complete event
            onSeasonComplete?.Invoke(currentSchedule, finalStandings);

            // Increment season year
            currentSeasonYear++;
        }

        /// <summary>
        /// Get current standings
        /// </summary>
        public List<TeamData> GetCurrentStandings()
        {
            return teamManager.GetStandings();
        }

        /// <summary>
        /// Get remaining matches
        /// </summary>
        public int GetRemainingMatches()
        {
            if (currentSchedule == null) return 0;

            int remaining = 0;
            foreach (var match in currentSchedule.matches)
            {
                if (!match.hasBeenPlayed)
                    remaining++;
            }
            return remaining;
        }

        /// <summary>
        /// Get all matches for the current season
        /// </summary>
        public List<Match> GetAllMatches()
        {
            if (currentSchedule == null)
                return new List<Match>();
            return currentSchedule.matches;
        }

        /// <summary>
        /// Get schedule summary
        /// </summary>
        public string GetScheduleSummary()
        {
            if (currentSchedule == null)
                return "No season in progress";

            int totalMatches = currentSchedule.matches.Count;
            int playedMatches = currentSchedule.matches.Count(m => m.hasBeenPlayed);
            int remainingMatches = totalMatches - playedMatches;

            return $"Season {currentSeasonYear}: Week {currentSchedule.currentWeek + 1}/{currentSchedule.GetTotalWeeks()} | " +
                   $"Matches: {playedMatches}/{totalMatches} ({remainingMatches} remaining)";
        }
    }
}
