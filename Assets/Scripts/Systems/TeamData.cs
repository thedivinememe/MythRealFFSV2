using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Represents a team with its roster and statistics
    /// Used for league/season management
    /// </summary>
    [Serializable]
    public class TeamData
    {
        [Header("Team Identity")]
        public string teamName;
        public int teamId;
        public Color teamColor = Color.white;
        public string managerName;

        [Header("Roster")]
        public List<CharacterData> roster = new List<CharacterData>();
        public int maxRosterSize = 10;
        public int activeRosterSize = 3; // How many characters can be active in a match

        [Header("Season Statistics")]
        public int wins;
        public int losses;
        public int draws;
        public int pointsScored; // Total damage dealt in all matches
        public int pointsAgainst; // Total damage taken in all matches
        public int matchesPlayed;

        [Header("All-Time Statistics")]
        public int totalWins;
        public int totalLosses;
        public int totalDraws;
        public int championships;

        /// <summary>
        /// Win percentage for current season
        /// </summary>
        public float WinPercentage
        {
            get
            {
                if (matchesPlayed == 0) return 0f;
                return (float)wins / matchesPlayed;
            }
        }

        /// <summary>
        /// Point differential for current season
        /// </summary>
        public int PointDifferential
        {
            get { return pointsScored - pointsAgainst; }
        }

        /// <summary>
        /// Add a character to the roster
        /// </summary>
        public bool AddCharacter(CharacterData character)
        {
            if (roster.Count >= maxRosterSize)
            {
                Debug.LogWarning($"Team {teamName} roster is full! Cannot add {character.characterName}");
                return false;
            }

            if (roster.Contains(character))
            {
                Debug.LogWarning($"{character.characterName} is already on team {teamName}");
                return false;
            }

            roster.Add(character);
            Debug.Log($"{character.characterName} added to {teamName}");
            return true;
        }

        /// <summary>
        /// Remove a character from the roster
        /// </summary>
        public bool RemoveCharacter(CharacterData character)
        {
            if (roster.Remove(character))
            {
                Debug.Log($"{character.characterName} removed from {teamName}");
                return true;
            }

            Debug.LogWarning($"{character.characterName} not found on team {teamName}");
            return false;
        }

        /// <summary>
        /// Get the active roster for a match (top characters by level/HP)
        /// </summary>
        public List<CharacterData> GetActiveRoster()
        {
            // Return top characters, prioritizing highest level and HP
            return roster
                .Where(c => c.IsAlive())
                .OrderByDescending(c => c.level)
                .ThenByDescending(c => c.maxHP)
                .Take(activeRosterSize)
                .ToList();
        }

        /// <summary>
        /// Record a win
        /// </summary>
        public void RecordWin(int pointsFor, int pointsAgainst)
        {
            wins++;
            totalWins++;
            matchesPlayed++;
            pointsScored += pointsFor;
            this.pointsAgainst += pointsAgainst;
        }

        /// <summary>
        /// Record a loss
        /// </summary>
        public void RecordLoss(int pointsFor, int pointsAgainst)
        {
            losses++;
            totalLosses++;
            matchesPlayed++;
            pointsScored += pointsFor;
            this.pointsAgainst += pointsAgainst;
        }

        /// <summary>
        /// Record a draw
        /// </summary>
        public void RecordDraw(int pointsFor, int pointsAgainst)
        {
            draws++;
            totalDraws++;
            matchesPlayed++;
            pointsScored += pointsFor;
            this.pointsAgainst += pointsAgainst;
        }

        /// <summary>
        /// Reset season statistics (for new season)
        /// </summary>
        public void ResetSeasonStats()
        {
            wins = 0;
            losses = 0;
            draws = 0;
            pointsScored = 0;
            pointsAgainst = 0;
            matchesPlayed = 0;
        }

        /// <summary>
        /// Heal all characters on the roster (after match)
        /// </summary>
        public void HealRoster()
        {
            foreach (var character in roster)
            {
                character.currentHP = character.maxHP;
                character.activeStatusEffects.Clear();
            }
        }

        /// <summary>
        /// Get team strength rating (average level * roster size)
        /// </summary>
        public float GetTeamStrength()
        {
            if (roster.Count == 0) return 0f;

            float avgLevel = (float)roster.Average(c => c.level);
            float avgHP = (float)roster.Average(c => c.maxHP);
            float avgDefense = (float)roster.Average(c => c.defense);

            return (avgLevel * 10) + avgHP + avgDefense;
        }

        public override string ToString()
        {
            return $"{teamName} ({wins}-{losses}-{draws}) | Roster: {roster.Count}/{maxRosterSize}";
        }

        /// <summary>
        /// Get detailed team statistics
        /// </summary>
        public string GetDetailedStats()
        {
            string stats = $"=== {teamName} ===\n";
            stats += $"Manager: {managerName}\n";
            stats += $"Record: {wins}-{losses}-{draws} ({WinPercentage:P1})\n";
            stats += $"Points: {pointsScored} - {pointsAgainst} (Diff: {PointDifferential:+#;-#;0})\n";
            stats += $"Roster Size: {roster.Count}/{maxRosterSize}\n";
            stats += $"Team Strength: {GetTeamStrength():F0}\n\n";

            stats += "ROSTER:\n";
            foreach (var character in roster.OrderByDescending(c => c.level))
            {
                stats += $"  {character.characterName} - Lvl {character.level} {character.ancestry?.ancestryName} {character.background?.backgroundName}\n";
                stats += $"    HP: {character.currentHP}/{character.maxHP} | DEF: {character.defense}\n";
            }

            return stats;
        }
    }
}
