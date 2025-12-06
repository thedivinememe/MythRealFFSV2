using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Systems
{
    /// <summary>
    /// Represents a single draft pick
    /// </summary>
    [Serializable]
    public class DraftPick
    {
        public int pickNumber;
        public int round;
        public int pickInRound;
        public TeamData draftingTeam;
        public CharacterData selectedCharacter;
        public bool isPicked;

        public DraftPick(int pickNumber, int round, int pickInRound, TeamData team)
        {
            this.pickNumber = pickNumber;
            this.round = round;
            this.pickInRound = pickInRound;
            this.draftingTeam = team;
            this.isPicked = false;
        }

        public override string ToString()
        {
            if (isPicked)
            {
                return $"Pick #{pickNumber} (Round {round}, Pick {pickInRound}): {draftingTeam.teamName} selects {selectedCharacter.characterName}";
            }
            else
            {
                return $"Pick #{pickNumber} (Round {round}, Pick {pickInRound}): {draftingTeam.teamName} - ON THE CLOCK";
            }
        }
    }

    /// <summary>
    /// Represents the complete draft
    /// </summary>
    [Serializable]
    public class Draft
    {
        public int draftYear;
        public List<TeamData> draftingTeams = new List<TeamData>();
        public List<CharacterData> availableCharacters = new List<CharacterData>();
        public List<DraftPick> allPicks = new List<DraftPick>();
        public int currentPickIndex = 0;
        public int totalRounds;
        public bool isComplete;

        public Draft(int draftYear, List<TeamData> teams, int rounds)
        {
            this.draftYear = draftYear;
            this.draftingTeams = new List<TeamData>(teams);
            this.totalRounds = rounds;
            GenerateDraftOrder();
        }

        /// <summary>
        /// Generate snake draft order
        /// Example for 4 teams, 3 rounds:
        /// Round 1: Team1, Team2, Team3, Team4
        /// Round 2: Team4, Team3, Team2, Team1 (reversed)
        /// Round 3: Team1, Team2, Team3, Team4
        /// </summary>
        void GenerateDraftOrder()
        {
            int pickNumber = 1;

            for (int round = 1; round <= totalRounds; round++)
            {
                bool isEvenRound = (round % 2 == 0);
                int numTeams = draftingTeams.Count;

                for (int i = 0; i < numTeams; i++)
                {
                    // Snake draft: reverse order on even rounds
                    int teamIndex = isEvenRound ? (numTeams - 1 - i) : i;
                    TeamData team = draftingTeams[teamIndex];

                    DraftPick pick = new DraftPick(pickNumber, round, i + 1, team);
                    allPicks.Add(pick);
                    pickNumber++;
                }
            }

            Debug.Log($"Draft order generated: {allPicks.Count} total picks ({totalRounds} rounds, {draftingTeams.Count} teams)");
        }

        /// <summary>
        /// Get the current pick
        /// </summary>
        public DraftPick GetCurrentPick()
        {
            if (currentPickIndex < allPicks.Count)
            {
                return allPicks[currentPickIndex];
            }
            return null;
        }

        /// <summary>
        /// Make a pick
        /// </summary>
        public bool MakePick(CharacterData character)
        {
            var currentPick = GetCurrentPick();
            if (currentPick == null)
            {
                Debug.LogWarning("No more picks available!");
                return false;
            }

            if (!availableCharacters.Contains(character))
            {
                Debug.LogWarning($"{character.characterName} is not available!");
                return false;
            }

            // Make the pick
            currentPick.selectedCharacter = character;
            currentPick.isPicked = true;

            // Add to team
            currentPick.draftingTeam.AddCharacter(character);

            // Remove from available pool
            availableCharacters.Remove(character);

            Debug.Log($"[PICK #{currentPick.pickNumber}] {currentPick.draftingTeam.teamName} selects {character.characterName}");

            // Advance to next pick
            currentPickIndex++;

            // Check if draft is complete
            if (currentPickIndex >= allPicks.Count)
            {
                isComplete = true;
                Debug.Log("\n=== DRAFT COMPLETE ===");
            }

            return true;
        }

        /// <summary>
        /// Get picks for a specific round
        /// </summary>
        public List<DraftPick> GetPicksForRound(int round)
        {
            return allPicks.Where(p => p.round == round).ToList();
        }

        /// <summary>
        /// Get all picks made by a team
        /// </summary>
        public List<DraftPick> GetTeamPicks(TeamData team)
        {
            return allPicks.Where(p => p.draftingTeam == team && p.isPicked).ToList();
        }

        /// <summary>
        /// Get the best available character based on rating
        /// </summary>
        public CharacterData GetBestAvailable()
        {
            if (availableCharacters.Count == 0)
                return null;

            return availableCharacters
                .OrderByDescending(c => CalculateCharacterRating(c))
                .First();
        }

        /// <summary>
        /// Get top N available characters
        /// </summary>
        public List<CharacterData> GetTopAvailable(int count)
        {
            return availableCharacters
                .OrderByDescending(c => CalculateCharacterRating(c))
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Calculate a character's draft rating
        /// Higher is better
        /// </summary>
        public static float CalculateCharacterRating(CharacterData character)
        {
            float rating = 0;

            // Base stats
            rating += character.maxHP * 0.5f;
            rating += character.defense * 2f;
            rating += character.level * 10f;

            // Attributes (avg modifier)
            if (character.attributes != null)
            {
                float totalModifiers = 0;
                foreach (AttributeType attrType in Enum.GetValues(typeof(AttributeType)))
                {
                    totalModifiers += character.attributes.GetModifier(attrType);
                }
                rating += totalModifiers * 5f;
            }

            // Abilities
            rating += character.knownAbilities.Count * 8f;

            // Talents
            rating += character.talents.Count * 6f;

            return rating;
        }

        /// <summary>
        /// Get draft summary
        /// </summary>
        public string GetDraftSummary()
        {
            string summary = $"=== DRAFT {draftYear} SUMMARY ===\n";
            summary += $"Total Picks: {allPicks.Count(p => p.isPicked)}/{allPicks.Count}\n";
            summary += $"Characters Remaining: {availableCharacters.Count}\n\n";

            foreach (var team in draftingTeams)
            {
                var teamPicks = GetTeamPicks(team);
                summary += $"{team.teamName}: {teamPicks.Count} picks\n";
                foreach (var pick in teamPicks)
                {
                    summary += $"  [{pick.round}-{pick.pickInRound}] {pick.selectedCharacter.characterName}\n";
                }
                summary += "\n";
            }

            return summary;
        }
    }

    /// <summary>
    /// Manages draft operations
    /// </summary>
    public class DraftManager : MonoBehaviour
    {
        [Header("Current Draft")]
        public Draft currentDraft;
        public bool draftInProgress;

        [Header("Components")]
        private TeamManager teamManager;
        private CharacterGenerator characterGenerator;

        void Awake()
        {
            teamManager = GetComponent<TeamManager>();
            if (teamManager == null)
                teamManager = gameObject.AddComponent<TeamManager>();

            characterGenerator = GetComponent<CharacterGenerator>();
            if (characterGenerator == null)
                characterGenerator = gameObject.AddComponent<CharacterGenerator>();
        }

        /// <summary>
        /// Start a new draft
        /// </summary>
        public void StartDraft(List<TeamData> teams, int rounds, int characterPoolSize,
                              List<AncestryData> ancestries, List<BackgroundData> backgrounds)
        {
            if (draftInProgress)
            {
                Debug.LogWarning("Draft already in progress!");
                return;
            }

            Debug.Log($"=== STARTING DRAFT ===");
            Debug.Log($"Teams: {teams.Count}");
            Debug.Log($"Rounds: {rounds}");
            Debug.Log($"Character Pool Size: {characterPoolSize}\n");

            // Create draft
            currentDraft = new Draft(DateTime.Now.Year, teams, rounds);

            // Generate character pool
            GenerateCharacterPool(characterPoolSize, ancestries, backgrounds);

            draftInProgress = true;

            Debug.Log($"Draft created with {currentDraft.allPicks.Count} total picks");
            Debug.Log("Use MakeNextPick() to draft characters or RunAutoDraft() to simulate entire draft");
        }

        /// <summary>
        /// Generate a pool of characters for drafting
        /// </summary>
        void GenerateCharacterPool(int poolSize, List<AncestryData> ancestries, List<BackgroundData> backgrounds)
        {
            currentDraft.availableCharacters.Clear();

            for (int i = 0; i < poolSize; i++)
            {
                var character = characterGenerator.CreateRandomCharacter(ancestries, backgrounds);
                if (character != null)
                {
                    currentDraft.availableCharacters.Add(character);
                }
            }

            // Sort by rating
            currentDraft.availableCharacters = currentDraft.availableCharacters
                .OrderByDescending(c => Draft.CalculateCharacterRating(c))
                .ToList();

            Debug.Log($"Generated character pool: {currentDraft.availableCharacters.Count} characters");
        }

        /// <summary>
        /// Make the next pick (AI or manual)
        /// </summary>
        public bool MakeNextPick(CharacterData character = null)
        {
            if (!draftInProgress)
            {
                Debug.LogWarning("No draft in progress!");
                return false;
            }

            var currentPick = currentDraft.GetCurrentPick();
            if (currentPick == null)
            {
                Debug.Log("Draft is complete!");
                EndDraft();
                return false;
            }

            Debug.Log($"\n[Round {currentPick.round}, Pick {currentPick.pickInRound}]");
            Debug.Log($"{currentPick.draftingTeam.teamName} is on the clock...");

            // If no character specified, use AI to pick
            if (character == null)
            {
                character = MakeAIPick(currentPick.draftingTeam);
            }

            return currentDraft.MakePick(character);
        }

        /// <summary>
        /// AI logic for picking a character
        /// </summary>
        CharacterData MakeAIPick(TeamData team)
        {
            // Simple AI: Pick best available character
            // Can be enhanced with team needs analysis
            return currentDraft.GetBestAvailable();
        }

        /// <summary>
        /// Run the entire draft automatically
        /// </summary>
        public void RunAutoDraft()
        {
            if (!draftInProgress)
            {
                Debug.LogWarning("No draft in progress!");
                return;
            }

            Debug.Log("\n=== RUNNING AUTO DRAFT ===\n");

            while (!currentDraft.isComplete)
            {
                MakeNextPick();
            }
        }

        /// <summary>
        /// End the current draft
        /// </summary>
        void EndDraft()
        {
            draftInProgress = false;
            Debug.Log(currentDraft.GetDraftSummary());
        }

        /// <summary>
        /// Show available characters
        /// </summary>
        public void ShowAvailableCharacters(int count = 10)
        {
            if (currentDraft == null)
            {
                Debug.Log("No draft in progress");
                return;
            }

            Debug.Log($"\n=== TOP {count} AVAILABLE CHARACTERS ===");
            var topChars = currentDraft.GetTopAvailable(count);

            for (int i = 0; i < topChars.Count; i++)
            {
                var character = topChars[i];
                float rating = Draft.CalculateCharacterRating(character);
                Debug.Log($"{i + 1}. {character.characterName} - {character.ancestry?.ancestryName} {character.background?.backgroundName} " +
                         $"(Rating: {rating:F0}, HP: {character.maxHP}, DEF: {character.defense})");
            }
        }

        /// <summary>
        /// Get the current draft state
        /// </summary>
        public string GetDraftStatus()
        {
            if (currentDraft == null || !draftInProgress)
                return "No draft in progress";

            var currentPick = currentDraft.GetCurrentPick();
            if (currentPick == null)
                return "Draft complete";

            return $"Round {currentPick.round} - Pick {currentPick.pickNumber}/{currentDraft.allPicks.Count} - {currentPick.draftingTeam.teamName} on the clock";
        }
    }
}
