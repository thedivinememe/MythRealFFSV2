using UnityEngine;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Data;
using MythRealFFSV2.Character;
using System.Collections.Generic;

namespace MythRealFFSV2.Examples
{
    /// <summary>
    /// Example demonstrating season simulation with teams and leagues
    /// Shows how to create teams, run a season, and track standings
    /// </summary>
    public class SeasonSimulationExample : MonoBehaviour
    {
        [Header("Data Assets")]
        [Tooltip("Assign your created ScriptableObject assets")]
        public List<AncestryData> ancestries;
        public List<BackgroundData> backgrounds;
        public List<AbilityData> abilities;

        [Header("League Settings")]
        public int numberOfTeams = 4;
        public int charactersPerTeam = 5;
        public bool useDoubleRoundRobin = false;

        private TeamManager teamManager;
        private LeagueManager leagueManager;

        void Start()
        {
            // Get or create components
            teamManager = GetComponent<TeamManager>();
            if (teamManager == null)
                teamManager = gameObject.AddComponent<TeamManager>();

            leagueManager = GetComponent<LeagueManager>();
            if (leagueManager == null)
                leagueManager = gameObject.AddComponent<LeagueManager>();

            leagueManager.doubleRoundRobin = useDoubleRoundRobin;

            // Subscribe to events
            leagueManager.onMatchComplete.AddListener(OnMatchComplete);
            leagueManager.onSeasonComplete.AddListener(OnSeasonComplete);
        }

        [ContextMenu("Create League")]
        void CreateLeague()
        {
            Debug.Log("=== CREATING LEAGUE ===\n");

            if (ancestries.Count == 0 || backgrounds.Count == 0)
            {
                Debug.LogError("Please assign ancestries and backgrounds first!");
                return;
            }

            // Clear existing teams
            teamManager.teams.Clear();

            // Create teams with random rosters
            for (int i = 0; i < numberOfTeams; i++)
            {
                var team = teamManager.CreateRandomTeamWithRoster(
                    ancestries,
                    backgrounds,
                    charactersPerTeam
                );

                // Assign some abilities to team members
                if (abilities != null && abilities.Count > 0)
                {
                    AssignAbilitiesToTeam(team);
                }

                Debug.Log($"Created: {team.teamName} with {team.roster.Count} characters");
            }

            Debug.Log($"\nLeague created with {teamManager.teams.Count} teams!");
            Debug.Log("Ready to start season. Use 'Start Season' to begin.\n");
        }

        [ContextMenu("Start Season")]
        void StartSeason()
        {
            if (teamManager.teams.Count < 2)
            {
                Debug.LogError("Create a league first! (Right-click → Create League)");
                return;
            }

            leagueManager.StartNewSeason();
        }

        [ContextMenu("Play Next Week")]
        void PlayNextWeek()
        {
            leagueManager.PlayNextWeek();
        }

        [ContextMenu("Play Entire Season")]
        void PlayEntireSeason()
        {
            leagueManager.PlayEntireSeason();
        }

        [ContextMenu("Show Current Standings")]
        void ShowStandings()
        {
            if (!leagueManager.seasonInProgress && leagueManager.currentSchedule == null)
            {
                Debug.Log("No season has been started yet!");
                return;
            }

            Debug.Log("\n=== CURRENT STANDINGS ===");
            var standings = leagueManager.GetCurrentStandings();

            for (int i = 0; i < standings.Count; i++)
            {
                var team = standings[i];
                Debug.Log($"{i + 1}. {team}");
            }

            Debug.Log($"\n{leagueManager.GetScheduleSummary()}\n");
        }

        [ContextMenu("Show Team Details")]
        void ShowTeamDetails()
        {
            Debug.Log("\n=== TEAM DETAILS ===\n");

            foreach (var team in teamManager.teams)
            {
                Debug.Log(team.GetDetailedStats());
            }
        }

        [ContextMenu("Run Multiple Seasons")]
        void RunMultipleSeasons()
        {
            int seasonsToRun = 3;
            Debug.Log($"=== RUNNING {seasonsToRun} SEASONS ===\n");

            // Create league if needed
            if (teamManager.teams.Count < 2)
            {
                CreateLeague();
            }

            for (int i = 0; i < seasonsToRun; i++)
            {
                Debug.Log($"\n========== SEASON {leagueManager.currentSeasonYear} ==========");
                leagueManager.StartNewSeason();
                leagueManager.PlayEntireSeason();

                // Note: Since PlayEntireSeason is a coroutine, this won't work as expected
                // In practice, you'd need to wait for the season to complete
                // This is just for demonstration
            }
        }

        void OnMatchComplete(Match match)
        {
            // Custom logic when a match completes
            // For example, you could update UI, play sound effects, etc.
        }

        void OnSeasonComplete(Schedule schedule, List<TeamData> finalStandings)
        {
            // Custom logic when a season completes
            Debug.Log("\n=== SEASON SUMMARY ===");
            Debug.Log($"Total matches played: {schedule.matches.Count}");
            Debug.Log($"Champion: {finalStandings[0].teamName}");
            Debug.Log($"Runner-up: {finalStandings[1].teamName}");
        }

        void AssignAbilitiesToTeam(TeamData team)
        {
            foreach (var character in team.roster)
            {
                // Give each character 1-2 random abilities
                int numAbilities = Random.Range(1, 3);

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

        // Example: Create a custom team manually
        [ContextMenu("Create Custom Team")]
        void CreateCustomTeam()
        {
            if (ancestries.Count == 0 || backgrounds.Count == 0)
            {
                Debug.LogError("Please assign ancestries and backgrounds first!");
                return;
            }

            // Create a new team
            var customTeam = teamManager.CreateTeam("My Custom Team", "Player");

            // Add specific characters
            var characterGen = GetComponent<CharacterGenerator>();
            if (characterGen == null)
                characterGen = gameObject.AddComponent<CharacterGenerator>();

            // Create a warrior
            var warrior = characterGen.CreateCharacter(
                "Thorin Ironhammer",
                ancestries[0], // First ancestry
                backgrounds[1], // Second background
                AttributeType.Strength,
                useStandardArray: true
            );
            customTeam.AddCharacter(warrior);

            // Create a mage
            var mage = characterGen.CreateCharacter(
                "Mystara Starweaver",
                ancestries[1],
                backgrounds[2],
                AttributeType.Intelligence,
                useStandardArray: true
            );
            customTeam.AddCharacter(mage);

            Debug.Log($"Created custom team: {customTeam.GetDetailedStats()}");
        }
    }
}
