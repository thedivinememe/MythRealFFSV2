using UnityEngine;
using MythRealFFSV2.Systems;
using MythRealFFSV2.Data;
using MythRealFFSV2.Character;
using System.Collections.Generic;
using System.Linq;

namespace MythRealFFSV2.Examples
{
    /// <summary>
    /// Example demonstrating the draft system
    /// Shows how to run a character draft for multiple teams
    /// </summary>
    public class DraftExample : MonoBehaviour
    {
        [Header("Data Assets")]
        [Tooltip("Assign your created ScriptableObject assets")]
        public List<AncestryData> ancestries;
        public List<BackgroundData> backgrounds;
        public List<AbilityData> abilities;

        [Header("Draft Settings")]
        public int numberOfTeams = 4;
        public int draftRounds = 5; // Each team will draft 5 characters
        public int characterPoolSize = 30; // Pool of characters to draft from

        private TeamManager teamManager;
        private DraftManager draftManager;

        void Start()
        {
            // Get or create components
            teamManager = GetComponent<TeamManager>();
            if (teamManager == null)
                teamManager = gameObject.AddComponent<TeamManager>();

            draftManager = GetComponent<DraftManager>();
            if (draftManager == null)
                draftManager = gameObject.AddComponent<DraftManager>();
        }

        [ContextMenu("Create Teams for Draft")]
        void CreateTeamsForDraft()
        {
            Debug.Log("=== CREATING TEAMS FOR DRAFT ===\n");

            // Clear existing teams
            teamManager.teams.Clear();

            // Create empty teams (no characters yet - they'll be drafted)
            for (int i = 0; i < numberOfTeams; i++)
            {
                var team = teamManager.CreateRandomTeam();
                Debug.Log($"Created: {team.teamName}");
            }

            Debug.Log($"\n{numberOfTeams} teams created and ready to draft!");
            Debug.Log("Use 'Start Draft' to begin drafting characters.\n");
        }

        [ContextMenu("Start Draft")]
        void StartDraft()
        {
            if (teamManager.teams.Count == 0)
            {
                Debug.LogError("Create teams first! (Right-click → Create Teams for Draft)");
                return;
            }

            if (ancestries.Count == 0 || backgrounds.Count == 0)
            {
                Debug.LogError("Please assign ancestries and backgrounds first!");
                return;
            }

            draftManager.StartDraft(
                teamManager.GetAllTeams(),
                draftRounds,
                characterPoolSize,
                ancestries,
                backgrounds
            );

            Debug.Log("\nDraft is ready! Use:");
            Debug.Log("- 'Show Top Prospects' to see best available");
            Debug.Log("- 'Make Next Pick' to draft one character");
            Debug.Log("- 'Run Full Auto Draft' to simulate entire draft\n");
        }

        [ContextMenu("Show Top Prospects")]
        void ShowTopProspects()
        {
            draftManager.ShowAvailableCharacters(15);
        }

        [ContextMenu("Show Draft Status")]
        void ShowDraftStatus()
        {
            Debug.Log(draftManager.GetDraftStatus());
        }

        [ContextMenu("Make Next Pick")]
        void MakeNextPick()
        {
            draftManager.MakeNextPick();
        }

        [ContextMenu("Run Full Auto Draft")]
        void RunFullAutoDraft()
        {
            draftManager.RunAutoDraft();
        }

        [ContextMenu("Show Team Rosters")]
        void ShowTeamRosters()
        {
            Debug.Log("\n=== TEAM ROSTERS ===\n");

            foreach (var team in teamManager.teams)
            {
                Debug.Log($"{team.teamName} ({team.roster.Count} players):");

                // Sort by draft rating
                var sortedRoster = team.roster
                    .OrderByDescending(c => Draft.CalculateCharacterRating(c))
                    .ToList();

                for (int i = 0; i < sortedRoster.Count; i++)
                {
                    var character = sortedRoster[i];
                    float rating = Draft.CalculateCharacterRating(character);
                    Debug.Log($"  {i + 1}. {character.characterName} - {character.ancestry?.ancestryName} " +
                             $"(Rating: {rating:F0}, HP: {character.maxHP}, DEF: {character.defense})");
                }
                Debug.Log("");
            }
        }

        [ContextMenu("Assign Abilities to Drafted Players")]
        void AssignAbilities()
        {
            if (abilities == null || abilities.Count == 0)
            {
                Debug.LogWarning("No abilities available to assign");
                return;
            }

            Debug.Log("=== ASSIGNING ABILITIES ===\n");

            foreach (var team in teamManager.teams)
            {
                foreach (var character in team.roster)
                {
                    // Give each character 1-3 random abilities
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

            Debug.Log("Abilities assigned to all drafted players!");
        }

        [ContextMenu("Complete Draft Setup")]
        void CompleteDraftSetup()
        {
            Debug.Log("=== RUNNING COMPLETE DRAFT SETUP ===\n");

            // Step 1: Create teams
            CreateTeamsForDraft();

            // Step 2: Start draft
            StartDraft();

            // Step 3: Run auto draft
            RunFullAutoDraft();

            // Step 4: Assign abilities
            AssignAbilities();

            // Step 5: Show final rosters
            ShowTeamRosters();

            Debug.Log("\n=== DRAFT COMPLETE! ===");
            Debug.Log("Teams are ready to compete!");
            Debug.Log("You can now use the LeagueManager to start a season with these teams.\n");
        }
    }
}
