using System;
using System.Collections.Generic;
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
}
