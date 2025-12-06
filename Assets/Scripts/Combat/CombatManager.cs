using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Combat
{
    /// <summary>
    /// Manages turn-based combat encounters using the Action Point (AP) system
    /// Each character starts with 5 AP and can bank up to 2 AP between turns
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Combat Settings")]
        public int maxAPPerTurn = 5;
        public int maxBankedAP = 2;

        [Header("Current Combat")]
        public List<CombatantData> combatants = new List<CombatantData>();
        public int currentTurn = 0;
        public int currentCombatantIndex = 0;

        private bool combatActive = false;
        private BattleStatistics currentBattleStats;

        /// <summary>
        /// Set the battle statistics tracker
        /// </summary>
        public void SetBattleStatistics(BattleStatistics stats)
        {
            currentBattleStats = stats;
        }

        /// <summary>
        /// Initialize combat with a list of characters
        /// </summary>
        public void StartCombat(List<CharacterData> team1, List<CharacterData> team2)
        {
            combatants.Clear();
            currentTurn = 0;
            currentCombatantIndex = 0;

            // Create combatant data for each character
            foreach (var character in team1)
            {
                combatants.Add(new CombatantData(character, 0)); // Team 0
            }

            foreach (var character in team2)
            {
                combatants.Add(new CombatantData(character, 1)); // Team 1
            }

            // Roll initiative for all combatants
            RollInitiative();

            // Sort by initiative (highest first)
            combatants = combatants.OrderByDescending(c => c.initiativeRoll).ToList();

            combatActive = true;

            Debug.Log($"Combat started! {combatants.Count} combatants");
            Debug.Log("Initiative order:");
            for (int i = 0; i < combatants.Count; i++)
            {
                Debug.Log($"{i + 1}. {combatants[i].character.characterName} - Initiative: {combatants[i].initiativeRoll}");
            }

            // Start first turn
            StartNextTurn();
        }

        /// <summary>
        /// Roll initiative for all combatants
        /// </summary>
        private void RollInitiative()
        {
            foreach (var combatant in combatants)
            {
                combatant.character.CalculateInitiative();
                int roll = Random.Range(1, 21); // 1d20
                combatant.initiativeRoll = roll + combatant.character.initiative;
            }
        }

        /// <summary>
        /// Start the next character's turn
        /// </summary>
        public void StartNextTurn()
        {
            if (!combatActive)
                return;

            // Skip defeated combatants
            while (currentCombatantIndex < combatants.Count &&
                   !combatants[currentCombatantIndex].character.IsAlive())
            {
                currentCombatantIndex++;
            }

            // Check if round is complete
            if (currentCombatantIndex >= combatants.Count)
            {
                currentCombatantIndex = 0;
                currentTurn++;
                Debug.Log($"=== Round {currentTurn} ===");
            }

            // Check win conditions
            if (CheckCombatEnd())
            {
                EndCombat();
                return;
            }

            var currentCombatant = combatants[currentCombatantIndex];
            currentCombatant.character.StartTurn();

            Debug.Log($"{currentCombatant.character.characterName}'s turn (AP: {currentCombatant.character.currentAP})");
        }

        /// <summary>
        /// End the current character's turn
        /// </summary>
        public void EndCurrentTurn()
        {
            if (!combatActive)
                return;

            var currentCombatant = combatants[currentCombatantIndex];
            currentCombatant.character.EndTurn();

            currentCombatantIndex++;
            StartNextTurn();
        }

        /// <summary>
        /// Use an ability on a target
        /// </summary>
        public bool UseAbility(AbilityData ability, CharacterData user, CharacterData target)
        {
            if (!ability.CanUse(user, user.currentAP))
            {
                Debug.LogWarning($"{user.characterName} cannot use {ability.abilityName}");
                return false;
            }

            // Spend AP
            if (!user.SpendAP(ability.actionPointCost))
            {
                Debug.LogWarning($"Not enough AP to use {ability.abilityName}");
                return false;
            }

            Debug.Log($"{user.characterName} uses {ability.abilityName} on {target.characterName}");

            // Record ability used in statistics
            if (currentBattleStats != null)
            {
                currentBattleStats.RecordAbilityUsed(user);
            }

            // Handle saving throw if required
            bool savedAgainst = false;
            if (ability.requiresSave)
            {
                int saveRoll = RollSave(target, ability.saveType);
                int saveDC = 10 + user.attributes.GetModifier(ability.damageAttributeType);
                savedAgainst = saveRoll >= saveDC;

                Debug.Log($"{target.characterName} save roll: {saveRoll} vs DC {saveDC} - {(savedAgainst ? "Success!" : "Failed!")}");
            }

            // Calculate and apply damage/healing
            if (ability.damageRoll.numberOfDice > 0)
            {
                int amount = ability.CalculateDamage(user);

                // Half damage on successful save
                if (savedAgainst)
                    amount /= 2;

                if (ability.damageType == DamageType.Healing)
                {
                    target.Heal(amount);
                    Debug.Log($"{target.characterName} healed for {amount} HP");
                }
                else
                {
                    target.TakeDamage(amount);
                    Debug.Log($"{target.characterName} takes {amount} {ability.damageType} damage!");

                    // Record damage in statistics
                    if (currentBattleStats != null)
                    {
                        currentBattleStats.RecordDamage(user, target, amount);
                    }

                    if (!target.IsAlive())
                    {
                        Debug.Log($"{target.characterName} has been defeated!");
                    }
                }
            }

            // Apply status effects
            if (!savedAgainst)
            {
                foreach (var statusEffect in ability.inflictsStatus)
                {
                    target.AddStatusEffect(statusEffect.condition, statusEffect.duration);
                    Debug.Log($"{target.characterName} is now {statusEffect.condition}!");
                }
            }

            return true;
        }

        /// <summary>
        /// Perform a basic attack
        /// </summary>
        public bool PerformBasicAttack(CharacterData attacker, CharacterData target)
        {
            int apCost = 2; // Basic attack costs 2 AP

            if (!attacker.SpendAP(apCost))
            {
                Debug.LogWarning("Not enough AP for basic attack");
                return false;
            }

            // Record basic attack in statistics
            if (currentBattleStats != null)
            {
                currentBattleStats.RecordBasicAttack(attacker);
            }

            // Roll to hit
            int attackRoll = Random.Range(1, 21);
            int attackBonus = attacker.attributes.GetModifier(AttributeType.Strength);
            int totalAttack = attackRoll + attackBonus;

            Debug.Log($"{attacker.characterName} attacks {target.characterName}: {totalAttack} vs DEF {target.defense}");

            if (totalAttack >= target.defense)
            {
                // Hit! Calculate damage
                int damage = Random.Range(1, 7) + attackBonus; // 1d6 + STR

                // Critical hit on natural 20
                if (attackRoll == 20)
                {
                    damage *= 2;
                    Debug.Log("CRITICAL HIT!");
                }

                target.TakeDamage(damage);
                Debug.Log($"Hit! {target.characterName} takes {damage} damage");

                // Record damage in statistics
                if (currentBattleStats != null)
                {
                    currentBattleStats.RecordDamage(attacker, target, damage);
                }

                if (!target.IsAlive())
                {
                    Debug.Log($"{target.characterName} has been defeated!");
                }

                return true;
            }
            else
            {
                Debug.Log("Miss!");
                return false;
            }
        }

        /// <summary>
        /// Roll a saving throw
        /// </summary>
        private int RollSave(CharacterData character, AttributeType saveType)
        {
            int roll = Random.Range(1, 21); // 1d20
            int modifier = character.attributes.GetModifier(saveType);
            return roll + modifier;
        }

        /// <summary>
        /// Check if combat should end
        /// </summary>
        private bool CheckCombatEnd()
        {
            // Check if all members of a team are defeated
            bool team0Alive = combatants.Any(c => c.teamId == 0 && c.character.IsAlive());
            bool team1Alive = combatants.Any(c => c.teamId == 1 && c.character.IsAlive());

            return !team0Alive || !team1Alive;
        }

        /// <summary>
        /// End combat and determine winner
        /// </summary>
        private void EndCombat()
        {
            combatActive = false;

            bool team0Alive = combatants.Any(c => c.teamId == 0 && c.character.IsAlive());
            bool team1Alive = combatants.Any(c => c.teamId == 1 && c.character.IsAlive());

            if (team0Alive && !team1Alive)
            {
                Debug.Log("Team 0 wins!");
            }
            else if (team1Alive && !team0Alive)
            {
                Debug.Log("Team 1 wins!");
            }
            else
            {
                Debug.Log("Combat ended in a draw!");
            }

            // Calculate and award XP, loot, etc.
            AwardCombatRewards();
        }

        private void AwardCombatRewards()
        {
            // Award XP to winners
            bool team0Won = combatants.Any(c => c.teamId == 0 && c.character.IsAlive());

            if (team0Won)
            {
                int xpPerCharacter = 100; // Base XP
                foreach (var combatant in combatants.Where(c => c.teamId == 0 && c.character.IsAlive()))
                {
                    combatant.character.GainExperience(xpPerCharacter);
                }
            }
        }

        /// <summary>
        /// Get the current active combatant
        /// </summary>
        public CombatantData GetCurrentCombatant()
        {
            if (currentCombatantIndex < combatants.Count)
                return combatants[currentCombatantIndex];
            return null;
        }

        public bool IsCombatActive()
        {
            return combatActive;
        }
    }

    /// <summary>
    /// Wrapper class for characters in combat
    /// </summary>
    [System.Serializable]
    public class CombatantData
    {
        public CharacterData character;
        public int teamId;
        public int initiativeRoll;

        public CombatantData(CharacterData character, int teamId)
        {
            this.character = character;
            this.teamId = teamId;
        }
    }
}
