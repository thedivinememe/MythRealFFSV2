using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Combat
{
    /// <summary>
    /// Serializable UnityEvent for BattleResult
    /// </summary>
    [System.Serializable]
    public class BattleResultEvent : UnityEvent<BattleResult> { }

    /// <summary>
    /// Simulates complete battles between two teams with AI
    /// Tracks statistics and provides detailed battle results
    /// </summary>
    public class BattleSimulator : MonoBehaviour
    {
        [Header("Simulation Settings")]
        [Tooltip("Maximum number of turns before declaring a draw")]
        public int maxTurns = 50;

        [Tooltip("Delay between AI actions (for visualization)")]
        public float actionDelay = 0.5f;

        [Tooltip("Enable instant simulation (no delays)")]
        public bool instantSimulation = false;

        [Header("AI Configuration")]
        public AIPersonality team1Personality = AIPersonality.Balanced;
        public AIPersonality team2Personality = AIPersonality.Balanced;

        [Header("Events")]
        public BattleResultEvent onBattleComplete = new BattleResultEvent();

        private CombatManager combatManager;
        private BattleAI team1AI;
        private BattleAI team2AI;
        private bool isSimulating = false;

        private BattleStatistics currentBattleStats;

        void Awake()
        {
            combatManager = GetComponent<CombatManager>();
            if (combatManager == null)
                combatManager = gameObject.AddComponent<CombatManager>();

            // Create AI instances
            team1AI = gameObject.AddComponent<BattleAI>();
            team2AI = gameObject.AddComponent<BattleAI>();

            // Configure AI personalities
            AIPersonalityPresets.ConfigureAI(team1AI, team1Personality);
            AIPersonalityPresets.ConfigureAI(team2AI, team2Personality);
        }

        /// <summary>
        /// Simulate a battle between two teams
        /// </summary>
        public void SimulateBattle(List<CharacterData> team1, List<CharacterData> team2)
        {
            if (isSimulating)
            {
                Debug.LogWarning("Already simulating a battle!");
                return;
            }

            StartCoroutine(RunBattleSimulation(team1, team2));
        }

        /// <summary>
        /// Run the battle simulation
        /// </summary>
        IEnumerator RunBattleSimulation(List<CharacterData> team1, List<CharacterData> team2)
        {
            isSimulating = true;
            currentBattleStats = new BattleStatistics();
            currentBattleStats.StartBattle(team1, team2);

            // Start combat and pass statistics tracker
            combatManager.SetBattleStatistics(currentBattleStats);
            combatManager.StartCombat(team1, team2);

            Debug.Log("=== BATTLE SIMULATION STARTED ===");
            Debug.Log($"Team 1: {string.Join(", ", team1.Select(c => c.characterName))}");
            Debug.Log($"Team 2: {string.Join(", ", team2.Select(c => c.characterName))}");

            int turnCount = 0;

            // Main battle loop
            while (combatManager.IsCombatActive() && turnCount < maxTurns)
            {
                var currentCombatant = combatManager.GetCurrentCombatant();
                if (currentCombatant == null)
                    break;

                // Determine which AI to use
                BattleAI activeAI = currentCombatant.teamId == 0 ? team1AI : team2AI;

                // Get allies and enemies for this combatant
                var allies = GetTeamMembers(currentCombatant.teamId);
                var enemies = GetTeamMembers(currentCombatant.teamId == 0 ? 1 : 0);

                // Make AI decision and take actions
                while (currentCombatant.character.currentAP > 0)
                {
                    bool tookAction = activeAI.MakeDecision(
                        currentCombatant.character,
                        allies,
                        enemies,
                        combatManager
                    );

                    if (!tookAction)
                    {
                        // AI chose not to act or can't act
                        break;
                    }

                    // Track action in statistics
                    currentBattleStats.RecordAction(currentCombatant.character);

                    // Delay for visualization if needed
                    if (!instantSimulation && actionDelay > 0)
                    {
                        yield return new WaitForSeconds(actionDelay);
                    }
                }

                // End turn
                combatManager.EndCurrentTurn();
                turnCount++;

                // Update statistics turn count
                if (currentBattleStats != null)
                {
                    currentBattleStats.totalTurns = turnCount;
                }

                // Small delay between turns if not instant
                if (!instantSimulation)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }

            // Battle ended
            BattleResult result = FinalizeBattle(turnCount >= maxTurns);
            Debug.Log("\n=== BATTLE SIMULATION COMPLETE ===");
            Debug.Log(result.ToString());

            onBattleComplete?.Invoke(result);

            isSimulating = false;
        }

        /// <summary>
        /// Get all team members for a team ID
        /// </summary>
        List<CharacterData> GetTeamMembers(int teamId)
        {
            return combatManager.combatants
                .Where(c => c.teamId == teamId && c.character.IsAlive())
                .Select(c => c.character)
                .ToList();
        }

        /// <summary>
        /// Finalize battle and create result
        /// </summary>
        BattleResult FinalizeBattle(bool wasDraw)
        {
            BattleResult result = new BattleResult();

            // Determine winner
            bool team1Alive = combatManager.combatants.Any(c => c.teamId == 0 && c.character.IsAlive());
            bool team2Alive = combatManager.combatants.Any(c => c.teamId == 1 && c.character.IsAlive());

            if (wasDraw)
            {
                result.outcome = BattleOutcome.Draw;
                result.winningTeam = -1;
            }
            else if (team1Alive && !team2Alive)
            {
                result.outcome = BattleOutcome.Team1Victory;
                result.winningTeam = 0;
            }
            else if (team2Alive && !team1Alive)
            {
                result.outcome = BattleOutcome.Team2Victory;
                result.winningTeam = 1;
            }
            else
            {
                result.outcome = BattleOutcome.Draw;
                result.winningTeam = -1;
            }

            // Compile statistics
            currentBattleStats.EndBattle();
            result.statistics = currentBattleStats;

            // Copy final character states
            result.team1FinalState = combatManager.combatants
                .Where(c => c.teamId == 0)
                .Select(c => c.character)
                .ToList();

            result.team2FinalState = combatManager.combatants
                .Where(c => c.teamId == 1)
                .Select(c => c.character)
                .ToList();

            return result;
        }
    }

    #region Battle Results and Statistics
    /// <summary>
    /// Outcome of a battle
    /// </summary>
    public enum BattleOutcome
    {
        Team1Victory,
        Team2Victory,
        Draw
    }

    /// <summary>
    /// Complete battle result with statistics
    /// </summary>
    [System.Serializable]
    public class BattleResult
    {
        public BattleOutcome outcome;
        public int winningTeam; // 0, 1, or -1 for draw
        public BattleStatistics statistics;
        public List<CharacterData> team1FinalState;
        public List<CharacterData> team2FinalState;

        public override string ToString()
        {
            string result = $"=== BATTLE RESULT ===\n";
            result += $"Outcome: {outcome}\n";

            if (statistics != null)
            {
                result += statistics.ToString();
            }

            return result;
        }
    }

    /// <summary>
    /// Detailed battle statistics
    /// </summary>
    [System.Serializable]
    public class BattleStatistics
    {
        public int totalTurns;
        public float battleDuration; // in seconds
        private float startTime;

        // Team statistics
        public int team1TotalDamage;
        public int team2TotalDamage;
        public int team1TotalHealing;
        public int team2TotalHealing;
        public int team1AbilitiesUsed;
        public int team2AbilitiesUsed;
        public int team1BasicAttacks;
        public int team2BasicAttacks;

        // Character statistics
        public Dictionary<string, CharacterStatistics> characterStats = new Dictionary<string, CharacterStatistics>();

        // Team rosters
        private List<CharacterData> team1Roster;
        private List<CharacterData> team2Roster;

        public void StartBattle(List<CharacterData> team1, List<CharacterData> team2)
        {
            startTime = Time.time;
            totalTurns = 0;

            team1Roster = new List<CharacterData>(team1);
            team2Roster = new List<CharacterData>(team2);

            // Initialize character statistics
            foreach (var character in team1)
            {
                characterStats[character.characterName] = new CharacterStatistics
                {
                    characterName = character.characterName,
                    teamId = 0,
                    startingHP = character.currentHP
                };
            }

            foreach (var character in team2)
            {
                characterStats[character.characterName] = new CharacterStatistics
                {
                    characterName = character.characterName,
                    teamId = 1,
                    startingHP = character.currentHP
                };
            }
        }

        public void EndBattle()
        {
            battleDuration = Time.time - startTime;

            // Calculate final stats
            foreach (var kvp in characterStats)
            {
                var stats = kvp.Value;
                var character = GetCharacterByName(kvp.Key);

                if (character != null)
                {
                    stats.endingHP = character.currentHP;
                    stats.survived = character.IsAlive();
                    stats.damageTaken = stats.startingHP - stats.endingHP;
                }
            }
        }

        public void RecordAction(CharacterData character)
        {
            if (!characterStats.ContainsKey(character.characterName))
                return;

            var stats = characterStats[character.characterName];
            stats.actionsTaken++;
        }

        public void RecordDamage(CharacterData attacker, CharacterData target, int damage)
        {
            if (characterStats.ContainsKey(attacker.characterName))
            {
                characterStats[attacker.characterName].damageDealt += damage;

                // Add to team totals
                if (characterStats[attacker.characterName].teamId == 0)
                    team1TotalDamage += damage;
                else
                    team2TotalDamage += damage;
            }
        }

        public void RecordAbilityUsed(CharacterData user)
        {
            if (characterStats.ContainsKey(user.characterName))
            {
                characterStats[user.characterName].abilitiesUsed++;

                if (characterStats[user.characterName].teamId == 0)
                    team1AbilitiesUsed++;
                else
                    team2AbilitiesUsed++;
            }
        }

        public void RecordBasicAttack(CharacterData attacker)
        {
            if (characterStats.ContainsKey(attacker.characterName))
            {
                characterStats[attacker.characterName].basicAttacks++;

                if (characterStats[attacker.characterName].teamId == 0)
                    team1BasicAttacks++;
                else
                    team2BasicAttacks++;
            }
        }

        private CharacterData GetCharacterByName(string name)
        {
            var character = team1Roster?.FirstOrDefault(c => c.characterName == name);
            if (character == null)
                character = team2Roster?.FirstOrDefault(c => c.characterName == name);
            return character;
        }

        public override string ToString()
        {
            string result = "\n=== BATTLE STATISTICS ===\n";
            result += $"Duration: {battleDuration:F2} seconds\n";
            result += $"Total Turns: {totalTurns}\n\n";

            result += "TEAM 1:\n";
            result += $"  Total Damage: {team1TotalDamage}\n";
            result += $"  Total Healing: {team1TotalHealing}\n";
            result += $"  Abilities Used: {team1AbilitiesUsed}\n";
            result += $"  Basic Attacks: {team1BasicAttacks}\n\n";

            result += "TEAM 2:\n";
            result += $"  Total Damage: {team2TotalDamage}\n";
            result += $"  Total Healing: {team2TotalHealing}\n";
            result += $"  Abilities Used: {team2AbilitiesUsed}\n";
            result += $"  Basic Attacks: {team2BasicAttacks}\n\n";

            result += "TOP PERFORMERS:\n";
            var topDamage = characterStats.Values.OrderByDescending(s => s.damageDealt).Take(3);
            foreach (var stat in topDamage)
            {
                result += $"  {stat.characterName}: {stat.damageDealt} damage\n";
            }

            return result;
        }
    }

    /// <summary>
    /// Statistics for individual characters
    /// </summary>
    [System.Serializable]
    public class CharacterStatistics
    {
        public string characterName;
        public int teamId;
        public int startingHP;
        public int endingHP;
        public int damageDealt;
        public int damageTaken;
        public int healingDone;
        public int abilitiesUsed;
        public int basicAttacks;
        public int actionsTaken;
        public bool survived;

        public override string ToString()
        {
            return $"{characterName}: {damageDealt} dmg, {damageTaken} taken, {abilitiesUsed} abilities, " +
                   $"{(survived ? "SURVIVED" : "DEFEATED")}";
        }
    }
    #endregion
}
