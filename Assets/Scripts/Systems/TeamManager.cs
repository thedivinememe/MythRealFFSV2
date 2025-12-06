using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Manages teams, team creation, and roster management
    /// </summary>
    public class TeamManager : MonoBehaviour
    {
        [Header("Team Templates")]
        public List<string> defaultTeamNames = new List<string>
        {
            "Thunder Dragons", "Shadow Wolves", "Iron Eagles", "Crimson Phoenix",
            "Storm Raiders", "Jade Serpents", "Silver Lions", "Obsidian Knights"
        };

        public List<Color> teamColors = new List<Color>
        {
            Color.red, Color.blue, Color.green, Color.yellow,
            new Color(1f, 0.5f, 0f), // Orange
            new Color(0.5f, 0f, 1f), // Purple
            Color.cyan, Color.magenta
        };

        [Header("Current Teams")]
        public List<TeamData> teams = new List<TeamData>();

        private CharacterGenerator characterGenerator;
        private int nextTeamId = 0;

        void Awake()
        {
            characterGenerator = GetComponent<CharacterGenerator>();
            if (characterGenerator == null)
                characterGenerator = gameObject.AddComponent<CharacterGenerator>();
        }

        /// <summary>
        /// Create a new team
        /// </summary>
        public TeamData CreateTeam(string teamName, string managerName = "AI Manager")
        {
            TeamData team = new TeamData
            {
                teamName = teamName,
                teamId = nextTeamId++,
                managerName = managerName,
                teamColor = teamColors[teams.Count % teamColors.Count]
            };

            teams.Add(team);
            Debug.Log($"Created team: {teamName} (ID: {team.teamId})");
            return team;
        }

        /// <summary>
        /// Create a team with random name
        /// </summary>
        public TeamData CreateRandomTeam()
        {
            string teamName = defaultTeamNames[Random.Range(0, defaultTeamNames.Count)] + " " + Random.Range(1, 100);
            return CreateTeam(teamName);
        }

        /// <summary>
        /// Create a team and populate it with random characters
        /// </summary>
        public TeamData CreateRandomTeamWithRoster(
            List<AncestryData> availableAncestries,
            List<BackgroundData> availableBackgrounds,
            int rosterSize = 5)
        {
            TeamData team = CreateRandomTeam();

            for (int i = 0; i < rosterSize; i++)
            {
                var character = characterGenerator.CreateRandomCharacter(availableAncestries, availableBackgrounds);
                if (character != null)
                {
                    team.AddCharacter(character);
                }
            }

            Debug.Log($"{team.teamName} created with {team.roster.Count} characters");
            return team;
        }

        /// <summary>
        /// Get a team by ID
        /// </summary>
        public TeamData GetTeam(int teamId)
        {
            return teams.FirstOrDefault(t => t.teamId == teamId);
        }

        /// <summary>
        /// Get a team by name
        /// </summary>
        public TeamData GetTeamByName(string teamName)
        {
            return teams.FirstOrDefault(t => t.teamName == teamName);
        }

        /// <summary>
        /// Remove a team
        /// </summary>
        public bool RemoveTeam(TeamData team)
        {
            if (teams.Remove(team))
            {
                Debug.Log($"Removed team: {team.teamName}");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get current standings sorted by win percentage
        /// </summary>
        public List<TeamData> GetStandings()
        {
            return teams
                .OrderByDescending(t => t.WinPercentage)
                .ThenByDescending(t => t.PointDifferential)
                .ToList();
        }

        /// <summary>
        /// Print current standings
        /// </summary>
        public void PrintStandings()
        {
            Debug.Log("=== CURRENT STANDINGS ===");
            var standings = GetStandings();

            for (int i = 0; i < standings.Count; i++)
            {
                var team = standings[i];
                Debug.Log($"{i + 1}. {team.teamName}: {team.wins}-{team.losses}-{team.draws} " +
                         $"({team.WinPercentage:P1}) | Pts: {team.pointsScored}-{team.pointsAgainst} " +
                         $"(Diff: {team.PointDifferential:+#;-#;0})");
            }
        }

        /// <summary>
        /// Reset all team season stats (for new season)
        /// </summary>
        public void ResetAllSeasonStats()
        {
            foreach (var team in teams)
            {
                team.ResetSeasonStats();
            }
            Debug.Log("All team season stats reset");
        }

        /// <summary>
        /// Heal all rosters (typically after each match day)
        /// </summary>
        public void HealAllRosters()
        {
            foreach (var team in teams)
            {
                team.HealRoster();
            }
        }

        /// <summary>
        /// Get average team strength across the league
        /// </summary>
        public float GetAverageTeamStrength()
        {
            if (teams.Count == 0) return 0f;
            return teams.Average(t => t.GetTeamStrength());
        }

        /// <summary>
        /// Balance teams by redistributing characters (draft system)
        /// </summary>
        public void BalanceTeams()
        {
            // Collect all characters
            List<CharacterData> allCharacters = new List<CharacterData>();
            foreach (var team in teams)
            {
                allCharacters.AddRange(team.roster);
                team.roster.Clear();
            }

            // Sort characters by level/strength
            allCharacters = allCharacters
                .OrderByDescending(c => c.level)
                .ThenByDescending(c => c.maxHP)
                .ToList();

            // Snake draft: 1,2,3,4,4,3,2,1,1,2,3,4...
            int teamIndex = 0;
            bool forward = true;

            foreach (var character in allCharacters)
            {
                teams[teamIndex].AddCharacter(character);

                if (forward)
                {
                    teamIndex++;
                    if (teamIndex >= teams.Count)
                    {
                        teamIndex = teams.Count - 1;
                        forward = false;
                    }
                }
                else
                {
                    teamIndex--;
                    if (teamIndex < 0)
                    {
                        teamIndex = 0;
                        forward = true;
                    }
                }
            }

            Debug.Log("Teams balanced using snake draft");
        }

        /// <summary>
        /// Get all teams as a simple list
        /// </summary>
        public List<TeamData> GetAllTeams()
        {
            return new List<TeamData>(teams);
        }
    }
}
