using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Combat;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Represents a single match between two teams
    /// </summary>
    [Serializable]
    public class Match
    {
        public int matchId;
        public int week;
        public TeamData homeTeam;
        public TeamData awayTeam;

        // Match result
        public bool hasBeenPlayed;
        public TeamData winner; // null if draw
        public int homeScore;
        public int awayScore;
        public BattleResult battleResult;

        public Match(int matchId, int week, TeamData homeTeam, TeamData awayTeam)
        {
            this.matchId = matchId;
            this.week = week;
            this.homeTeam = homeTeam;
            this.awayTeam = awayTeam;
            this.hasBeenPlayed = false;
        }

        /// <summary>
        /// Record the result of this match
        /// </summary>
        public void RecordResult(BattleResult result, int homeScore, int awayScore)
        {
            this.battleResult = result;
            this.homeScore = homeScore;
            this.awayScore = awayScore;
            this.hasBeenPlayed = true;

            // Determine winner
            if (homeScore > awayScore)
            {
                winner = homeTeam;
                homeTeam.RecordWin(homeScore, awayScore);
                awayTeam.RecordLoss(awayScore, homeScore);
            }
            else if (awayScore > homeScore)
            {
                winner = awayTeam;
                awayTeam.RecordWin(awayScore, homeScore);
                homeTeam.RecordLoss(homeScore, awayScore);
            }
            else
            {
                winner = null; // Draw
                homeTeam.RecordDraw(homeScore, awayScore);
                awayTeam.RecordDraw(awayScore, homeScore);
            }
        }

        public override string ToString()
        {
            if (!hasBeenPlayed)
            {
                return $"Week {week}: {homeTeam.teamName} vs {awayTeam.teamName}";
            }
            else
            {
                string winnerName = winner != null ? winner.teamName : "DRAW";
                return $"Week {week}: {homeTeam.teamName} {homeScore} - {awayScore} {awayTeam.teamName} (Winner: {winnerName})";
            }
        }
    }

    /// <summary>
    /// Represents a complete season schedule
    /// </summary>
    [Serializable]
    public class Schedule
    {
        public int seasonYear;
        public List<Match> matches = new List<Match>();
        public int currentWeek = 0;
        public bool isComplete;

        private int nextMatchId = 0;

        public Schedule(int seasonYear)
        {
            this.seasonYear = seasonYear;
        }

        /// <summary>
        /// Add a match to the schedule
        /// </summary>
        public void AddMatch(int week, TeamData homeTeam, TeamData awayTeam)
        {
            matches.Add(new Match(nextMatchId++, week, homeTeam, awayTeam));
        }

        /// <summary>
        /// Get all matches for a specific week
        /// </summary>
        public List<Match> GetMatchesForWeek(int week)
        {
            List<Match> weekMatches = new List<Match>();
            foreach (var match in matches)
            {
                if (match.week == week)
                {
                    weekMatches.Add(match);
                }
            }
            return weekMatches;
        }

        /// <summary>
        /// Get the next unplayed matches
        /// </summary>
        public List<Match> GetNextMatches()
        {
            return GetMatchesForWeek(currentWeek);
        }

        /// <summary>
        /// Advance to the next week
        /// </summary>
        public void AdvanceWeek()
        {
            currentWeek++;

            // Check if season is complete
            bool allMatchesPlayed = true;
            foreach (var match in matches)
            {
                if (!match.hasBeenPlayed)
                {
                    allMatchesPlayed = false;
                    break;
                }
            }
            isComplete = allMatchesPlayed;
        }

        /// <summary>
        /// Get total number of weeks in the schedule
        /// </summary>
        public int GetTotalWeeks()
        {
            int maxWeek = 0;
            foreach (var match in matches)
            {
                if (match.week > maxWeek)
                    maxWeek = match.week;
            }
            return maxWeek + 1; // +1 because weeks are 0-indexed
        }

        /// <summary>
        /// Check if there are more matches to play
        /// </summary>
        public bool HasMoreMatches()
        {
            return !isComplete && currentWeek < GetTotalWeeks();
        }
    }

    /// <summary>
    /// Generates schedules for leagues
    /// </summary>
    public class ScheduleGenerator
    {
        /// <summary>
        /// Generate a round-robin schedule where each team plays each other team once
        /// </summary>
        public static Schedule GenerateRoundRobinSchedule(List<TeamData> teams, int seasonYear)
        {
            Schedule schedule = new Schedule(seasonYear);

            if (teams.Count < 2)
            {
                Debug.LogWarning("Need at least 2 teams to generate a schedule");
                return schedule;
            }

            List<TeamData> teamList = new List<TeamData>(teams);

            // If odd number of teams, add a "bye" team (null)
            bool hasbye = (teamList.Count % 2 != 0);
            if (hasbye)
            {
                teamList.Add(null);
            }

            int numTeams = teamList.Count;
            int numRounds = numTeams - 1;
            int matchesPerRound = numTeams / 2;

            for (int round = 0; round < numRounds; round++)
            {
                for (int match = 0; match < matchesPerRound; match++)
                {
                    int home = (round + match) % (numTeams - 1);
                    int away = (numTeams - 1 - match + round) % (numTeams - 1);

                    // Last team stays in place
                    if (match == 0)
                    {
                        away = numTeams - 1;
                    }

                    TeamData homeTeam = teamList[home];
                    TeamData awayTeam = teamList[away];

                    // Skip if either team is null (bye week)
                    if (homeTeam != null && awayTeam != null)
                    {
                        schedule.AddMatch(round, homeTeam, awayTeam);
                    }
                }
            }

            Debug.Log($"Generated schedule: {teams.Count} teams, {schedule.matches.Count} matches, {schedule.GetTotalWeeks()} weeks");
            return schedule;
        }

        /// <summary>
        /// Generate a double round-robin schedule (home and away)
        /// </summary>
        public static Schedule GenerateDoubleRoundRobinSchedule(List<TeamData> teams, int seasonYear)
        {
            Schedule firstHalf = GenerateRoundRobinSchedule(teams, seasonYear);
            Schedule fullSchedule = new Schedule(seasonYear);

            // Copy first half
            foreach (var match in firstHalf.matches)
            {
                fullSchedule.AddMatch(match.week, match.homeTeam, match.awayTeam);
            }

            // Create second half with reversed home/away
            int weekOffset = firstHalf.GetTotalWeeks();
            foreach (var match in firstHalf.matches)
            {
                fullSchedule.AddMatch(match.week + weekOffset, match.awayTeam, match.homeTeam);
            }

            Debug.Log($"Generated double round-robin schedule: {fullSchedule.matches.Count} matches, {fullSchedule.GetTotalWeeks()} weeks");
            return fullSchedule;
        }
    }

    /// <summary>
    /// Represents a single playoff match (elimination)
    /// </summary>
    [Serializable]
    public class PlayoffMatch
    {
        public int matchId;
        public int round; // 0 = first round, 1 = semifinals, 2 = finals
        public string roundName; // "Quarterfinals", "Semifinals", "Championship"
        public TeamData team1;
        public TeamData team2;
        public int seed1; // Seeding position
        public int seed2;

        // Match result
        public bool hasBeenPlayed;
        public TeamData winner;
        public TeamData loser;
        public int team1Score;
        public int team2Score;
        public BattleResult battleResult;

        public PlayoffMatch(int matchId, int round, string roundName, TeamData team1, TeamData team2, int seed1, int seed2)
        {
            this.matchId = matchId;
            this.round = round;
            this.roundName = roundName;
            this.team1 = team1;
            this.team2 = team2;
            this.seed1 = seed1;
            this.seed2 = seed2;
            this.hasBeenPlayed = false;
        }

        /// <summary>
        /// Record the result of this playoff match
        /// </summary>
        public void RecordResult(BattleResult result, int team1Score, int team2Score)
        {
            this.battleResult = result;
            this.team1Score = team1Score;
            this.team2Score = team2Score;
            this.hasBeenPlayed = true;

            // Determine winner
            if (team1Score > team2Score)
            {
                winner = team1;
                loser = team2;
            }
            else if (team2Score > team1Score)
            {
                winner = team2;
                loser = team1;
            }
            else
            {
                // In playoffs, we need a winner - use sudden death (whoever has more HP remaining)
                int team1HP = result.team1FinalState.Sum(c => c.currentHP);
                int team2HP = result.team2FinalState.Sum(c => c.currentHP);

                if (team1HP > team2HP)
                {
                    winner = team1;
                    loser = team2;
                    Debug.Log($"Sudden death! {team1.teamName} wins with {team1HP} HP remaining vs {team2HP}");
                }
                else
                {
                    winner = team2;
                    loser = team1;
                    Debug.Log($"Sudden death! {team2.teamName} wins with {team2HP} HP remaining vs {team1HP}");
                }
            }
        }

        public override string ToString()
        {
            if (!hasBeenPlayed)
            {
                return $"{roundName}: #{seed1} {team1.teamName} vs #{seed2} {team2.teamName}";
            }
            else
            {
                return $"{roundName}: {team1.teamName} {team1Score} - {team2Score} {team2.teamName} (Winner: {winner.teamName})";
            }
        }
    }

    /// <summary>
    /// Represents a playoff bracket/tournament
    /// </summary>
    [Serializable]
    public class PlayoffBracket
    {
        public int seasonYear;
        public List<PlayoffMatch> allMatches = new List<PlayoffMatch>();
        public int currentRound = 0;
        public bool isComplete;
        public TeamData champion;

        private int nextMatchId = 0;

        public PlayoffBracket(int seasonYear)
        {
            this.seasonYear = seasonYear;
        }

        /// <summary>
        /// Add a playoff match
        /// </summary>
        public void AddMatch(int round, string roundName, TeamData team1, TeamData team2, int seed1, int seed2)
        {
            allMatches.Add(new PlayoffMatch(nextMatchId++, round, roundName, team1, team2, seed1, seed2));
        }

        /// <summary>
        /// Get all matches for a specific round
        /// </summary>
        public List<PlayoffMatch> GetMatchesForRound(int round)
        {
            List<PlayoffMatch> roundMatches = new List<PlayoffMatch>();
            foreach (var match in allMatches)
            {
                if (match.round == round)
                {
                    roundMatches.Add(match);
                }
            }
            return roundMatches;
        }

        /// <summary>
        /// Get the next unplayed matches
        /// </summary>
        public List<PlayoffMatch> GetNextMatches()
        {
            return GetMatchesForRound(currentRound);
        }

        /// <summary>
        /// Advance to the next round
        /// </summary>
        public void AdvanceRound()
        {
            currentRound++;

            // Check if tournament is complete
            var finalRoundMatches = GetMatchesForRound(GetTotalRounds() - 1);
            if (finalRoundMatches.Count > 0 && finalRoundMatches[0].hasBeenPlayed)
            {
                isComplete = true;
                champion = finalRoundMatches[0].winner;
            }
        }

        /// <summary>
        /// Get total number of rounds
        /// </summary>
        public int GetTotalRounds()
        {
            int maxRound = 0;
            foreach (var match in allMatches)
            {
                if (match.round > maxRound)
                    maxRound = match.round;
            }
            return maxRound + 1;
        }

        /// <summary>
        /// Check if there are more rounds to play
        /// </summary>
        public bool HasMoreRounds()
        {
            return !isComplete && currentRound < GetTotalRounds();
        }

        /// <summary>
        /// Get winners from a specific round
        /// </summary>
        public List<TeamData> GetRoundWinners(int round)
        {
            List<TeamData> winners = new List<TeamData>();
            foreach (var match in GetMatchesForRound(round))
            {
                if (match.hasBeenPlayed && match.winner != null)
                {
                    winners.Add(match.winner);
                }
            }
            return winners;
        }
    }

    /// <summary>
    /// Generates playoff brackets
    /// </summary>
    public class PlayoffGenerator
    {
        /// <summary>
        /// Generate a single-elimination playoff bracket
        /// Supports 2, 4, or 8 team playoffs
        /// </summary>
        public static PlayoffBracket GeneratePlayoffBracket(List<TeamData> seededTeams, int seasonYear)
        {
            PlayoffBracket bracket = new PlayoffBracket(seasonYear);

            int numTeams = seededTeams.Count;

            // Validate team count (must be power of 2)
            if (numTeams != 2 && numTeams != 4 && numTeams != 8)
            {
                Debug.LogError($"Playoff bracket requires 2, 4, or 8 teams. Got {numTeams} teams.");
                return bracket;
            }

            if (numTeams == 2)
            {
                // Just a championship match
                bracket.AddMatch(0, "Championship", seededTeams[0], seededTeams[1], 1, 2);
            }
            else if (numTeams == 4)
            {
                // Semifinals (round 0) -> Championship (round 1)
                bracket.AddMatch(0, "Semifinals", seededTeams[0], seededTeams[3], 1, 4); // 1 vs 4
                bracket.AddMatch(0, "Semifinals", seededTeams[1], seededTeams[2], 2, 3); // 2 vs 3

                // Championship will be added after semifinals complete
                bracket.AddMatch(1, "Championship", null, null, 0, 0); // Placeholder
            }
            else if (numTeams == 8)
            {
                // Quarterfinals (round 0) -> Semifinals (round 1) -> Championship (round 2)
                bracket.AddMatch(0, "Quarterfinals", seededTeams[0], seededTeams[7], 1, 8); // 1 vs 8
                bracket.AddMatch(0, "Quarterfinals", seededTeams[3], seededTeams[4], 4, 5); // 4 vs 5
                bracket.AddMatch(0, "Quarterfinals", seededTeams[1], seededTeams[6], 2, 7); // 2 vs 7
                bracket.AddMatch(0, "Quarterfinals", seededTeams[2], seededTeams[5], 3, 6); // 3 vs 6

                // Semifinals and Championship will be added after previous rounds complete
                bracket.AddMatch(1, "Semifinals", null, null, 0, 0); // Placeholder
                bracket.AddMatch(1, "Semifinals", null, null, 0, 0); // Placeholder
                bracket.AddMatch(2, "Championship", null, null, 0, 0); // Placeholder
            }

            Debug.Log($"Generated playoff bracket: {numTeams} teams, {bracket.GetTotalRounds()} rounds");
            return bracket;
        }

        /// <summary>
        /// Update bracket with winners from completed round to set up next round
        /// </summary>
        public static void UpdateBracketForNextRound(PlayoffBracket bracket, int completedRound)
        {
            var winners = bracket.GetRoundWinners(completedRound);

            if (winners.Count == 0)
            {
                Debug.LogWarning("No winners found for completed round");
                return;
            }

            int nextRound = completedRound + 1;
            var nextRoundMatches = bracket.GetMatchesForRound(nextRound);

            if (nextRoundMatches.Count == 0)
            {
                Debug.Log("No more rounds to update");
                return;
            }

            // Update next round matches with winners
            int winnerIndex = 0;
            foreach (var match in nextRoundMatches)
            {
                if (winnerIndex < winners.Count)
                {
                    match.team1 = winners[winnerIndex];
                    match.seed1 = GetOriginalSeed(bracket, winners[winnerIndex]);
                    winnerIndex++;
                }

                if (winnerIndex < winners.Count)
                {
                    match.team2 = winners[winnerIndex];
                    match.seed2 = GetOriginalSeed(bracket, winners[winnerIndex]);
                    winnerIndex++;
                }
            }

            Debug.Log($"Updated round {nextRound} matches with {winners.Count} winners from round {completedRound}");
        }

        /// <summary>
        /// Get the original seed of a team from the first round
        /// </summary>
        private static int GetOriginalSeed(PlayoffBracket bracket, TeamData team)
        {
            var firstRound = bracket.GetMatchesForRound(0);
            foreach (var match in firstRound)
            {
                if (match.team1 == team)
                    return match.seed1;
                if (match.team2 == team)
                    return match.seed2;
            }
            return 0;
        }
    }
}
