using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;

namespace MythRealFFSV2.Combat
{
    /// <summary>
    /// AI system for making intelligent combat decisions
    /// Handles target selection, ability usage, and tactical decisions
    /// </summary>
    public class BattleAI : MonoBehaviour
    {
        [Header("AI Settings")]
        [Range(0f, 1f)]
        [Tooltip("How aggressively the AI attacks (0 = defensive, 1 = all-out attack)")]
        public float aggression = 0.7f;

        [Range(0f, 1f)]
        [Tooltip("How likely the AI is to use abilities vs basic attacks")]
        public float abilityUsageRate = 0.6f;

        [Range(0f, 1f)]
        [Tooltip("How smart the AI is at making optimal decisions")]
        public float intelligence = 0.7f;

        [Tooltip("Minimum HP percentage before AI becomes defensive")]
        public float defensiveThreshold = 0.3f;

        /// <summary>
        /// Make a decision for an AI-controlled character
        /// Returns true if the character took an action
        /// </summary>
        public bool MakeDecision(CharacterData character, List<CharacterData> allies, List<CharacterData> enemies, CombatManager combat)
        {
            if (character.currentAP <= 0)
                return false;

            // Check if we need to move into range
            if (combat.battlefield != null && ShouldMove(character, enemies, combat, out HexCoordinate targetPos))
            {
                if (combat.MoveCharacter(character, targetPos))
                    return true;
            }

            // Check if we should be defensive
            bool isLowHealth = (float)character.currentHP / character.maxHP < defensiveThreshold;

            // Decide on action based on situation
            if (isLowHealth && ShouldUseDefensiveAbility(character))
            {
                return UseDefensiveAbility(character, combat);
            }

            // Filter enemies by range for offensive abilities
            List<CharacterData> enemiesInRange = combat.battlefield != null
                ? GetEnemiesInRange(character, enemies, combat)
                : enemies;

            // Try to use an offensive ability on enemies in range
            if (enemiesInRange.Count > 0 && Random.value < abilityUsageRate && ShouldUseOffensiveAbility(character, enemiesInRange))
            {
                return UseOffensiveAbility(character, enemiesInRange, combat);
            }

            // Fall back to basic attack on enemies in melee range
            List<CharacterData> enemiesInMelee = combat.battlefield != null
                ? GetEnemiesInMeleeRange(character, enemies, combat)
                : enemies;

            // If no enemies in melee range, try to move closer
            if (enemiesInMelee.Count == 0 && combat.battlefield != null && character.currentAP >= 1)
            {
                if (ShouldMove(character, enemies, combat, out HexCoordinate movePos))
                {
                    if (combat.MoveCharacter(character, movePos))
                        return true;
                }
            }

            // Try basic attack if we have targets in melee
            if (enemiesInMelee.Count > 0)
            {
                CharacterData target = SelectTarget(character, enemiesInMelee);
                if (target != null && character.currentAP >= 2)
                {
                    return combat.PerformBasicAttack(character, target);
                }
            }

            return false;
        }

        #region Target Selection
        /// <summary>
        /// Select the best target based on AI strategy
        /// </summary>
        CharacterData SelectTarget(CharacterData attacker, List<CharacterData> enemies)
        {
            if (enemies == null || enemies.Count == 0)
                return null;

            // Filter to only alive enemies
            var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();
            if (aliveEnemies.Count == 0)
                return null;

            // Different targeting strategies based on intelligence
            if (Random.value < intelligence)
            {
                // Smart targeting
                return SelectOptimalTarget(attacker, aliveEnemies);
            }
            else
            {
                // Random targeting
                return aliveEnemies[Random.Range(0, aliveEnemies.Count)];
            }
        }

        /// <summary>
        /// Select the optimal target using tactical analysis
        /// </summary>
        CharacterData SelectOptimalTarget(CharacterData attacker, List<CharacterData> enemies)
        {
            CharacterData bestTarget = null;
            float highestThreat = float.MinValue;

            foreach (var enemy in enemies)
            {
                float threat = CalculateThreatLevel(enemy);

                // Bonus threat for low HP enemies (finish them off)
                float hpPercent = (float)enemy.currentHP / enemy.maxHP;
                if (hpPercent < 0.3f)
                {
                    threat += 50; // High priority to finish off weak enemies
                }

                // Bonus threat for high-damage enemies
                int avgDamage = EstimateAverageDamage(enemy);
                threat += avgDamage * 2;

                // Bonus threat for support/healer characters
                if (HasHealingAbilities(enemy))
                {
                    threat += 40; // Kill healers first
                }

                if (threat > highestThreat)
                {
                    highestThreat = threat;
                    bestTarget = enemy;
                }
            }

            return bestTarget ?? enemies[0];
        }

        /// <summary>
        /// Calculate how threatening an enemy is
        /// </summary>
        float CalculateThreatLevel(CharacterData enemy)
        {
            float threat = 0;

            // Base threat from current HP
            threat += enemy.currentHP;

            // Threat from attributes
            threat += enemy.attributes.GetValue(AttributeType.Strength) * 2;
            threat += enemy.attributes.GetValue(AttributeType.Intelligence) * 1.5f;

            // Threat from level
            threat += enemy.level * 10;

            return threat;
        }

        /// <summary>
        /// Estimate average damage output of a character
        /// </summary>
        int EstimateAverageDamage(CharacterData character)
        {
            // Base damage from strength
            int baseDamage = character.attributes.GetModifier(AttributeType.Strength) + 4; // Assuming ~1d6+STR

            // Add bonus for known damaging abilities
            if (character.knownAbilities.Count > 0)
            {
                baseDamage += 3; // Abilities generally do more damage
            }

            return baseDamage;
        }

        /// <summary>
        /// Check if character has healing abilities
        /// </summary>
        bool HasHealingAbilities(CharacterData character)
        {
            return character.knownAbilities.Any(a => a.damageType == DamageType.Healing);
        }
        #endregion

        #region Ability Usage
        /// <summary>
        /// Check if we should use a defensive ability
        /// </summary>
        bool ShouldUseDefensiveAbility(CharacterData character)
        {
            // Look for defensive abilities
            var defensiveAbilities = character.knownAbilities
                .Where(a => IsDefensiveAbility(a) && a.CanUse(character, character.currentAP))
                .ToList();

            return defensiveAbilities.Count > 0;
        }

        /// <summary>
        /// Use a defensive ability (heal, buff, etc.)
        /// </summary>
        bool UseDefensiveAbility(CharacterData character, CombatManager combat)
        {
            var defensiveAbilities = character.knownAbilities
                .Where(a => IsDefensiveAbility(a) && a.CanUse(character, character.currentAP))
                .OrderByDescending(a => a.actionPointCost) // Use more powerful abilities first
                .ToList();

            if (defensiveAbilities.Count > 0)
            {
                var ability = defensiveAbilities[0];
                // Use defensive ability on self
                return combat.UseAbility(ability, character, character);
            }

            return false;
        }

        /// <summary>
        /// Check if we should use an offensive ability
        /// </summary>
        bool ShouldUseOffensiveAbility(CharacterData character, List<CharacterData> enemies)
        {
            // Check if we have AP for abilities
            if (character.currentAP < 2)
                return false;

            // Check if we have offensive abilities
            var offensiveAbilities = character.knownAbilities
                .Where(a => IsOffensiveAbility(a) && a.CanUse(character, character.currentAP))
                .ToList();

            return offensiveAbilities.Count > 0 && enemies.Any(e => e.IsAlive());
        }

        /// <summary>
        /// Use an offensive ability on the best target
        /// </summary>
        bool UseOffensiveAbility(CharacterData character, List<CharacterData> enemies, CombatManager combat)
        {
            var offensiveAbilities = character.knownAbilities
                .Where(a => IsOffensiveAbility(a) && a.CanUse(character, character.currentAP))
                .ToList();

            if (offensiveAbilities.Count == 0)
                return false;

            // Select best ability based on situation
            AbilityData bestAbility = SelectBestAbility(character, offensiveAbilities, enemies);
            if (bestAbility == null)
                return false;

            // Select target for the ability
            CharacterData target = SelectAbilityTarget(character, bestAbility, enemies);
            if (target == null)
                return false;

            return combat.UseAbility(bestAbility, character, target);
        }

        /// <summary>
        /// Select the best ability to use
        /// </summary>
        AbilityData SelectBestAbility(CharacterData character, List<AbilityData> abilities, List<CharacterData> enemies)
        {
            if (abilities.Count == 0)
                return null;

            // Random selection for lower intelligence
            if (Random.value > intelligence)
            {
                return abilities[Random.Range(0, abilities.Count)];
            }

            // Smart selection - consider damage vs cost
            AbilityData bestAbility = null;
            float bestScore = float.MinValue;

            foreach (var ability in abilities)
            {
                float score = EvaluateAbilityEffectiveness(ability, character, enemies);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestAbility = ability;
                }
            }

            return bestAbility;
        }

        /// <summary>
        /// Evaluate how effective an ability would be
        /// </summary>
        float EvaluateAbilityEffectiveness(AbilityData ability, CharacterData user, List<CharacterData> enemies)
        {
            float score = 0;

            // Base score from average damage
            float avgDamage = ability.damageRoll.numberOfDice * (ability.damageRoll.diceSize / 2f) + ability.damageRoll.modifier;
            if (ability.addAttributeModifier)
            {
                avgDamage += user.attributes.GetModifier(ability.damageAttributeType);
            }
            score += avgDamage * 10;

            // Penalty for high AP cost
            score -= ability.actionPointCost * 5;

            // Bonus for status effects
            score += ability.inflictsStatus.Count * 15;

            // Bonus for AOE abilities if multiple enemies
            if (ability.areaOfEffect.Contains("area") || ability.areaOfEffect.Contains("radius"))
            {
                int aliveEnemies = enemies.Count(e => e.IsAlive());
                score += aliveEnemies * 10;
            }

            // Penalty if on cooldown
            if (ability.cooldownTurns > 0)
            {
                score -= 20;
            }

            return score;
        }

        /// <summary>
        /// Select target for an ability
        /// </summary>
        CharacterData SelectAbilityTarget(CharacterData user, AbilityData ability, List<CharacterData> enemies)
        {
            // For AOE abilities, target the center of enemy group
            if (ability.areaOfEffect.Contains("area") || ability.areaOfEffect.Contains("radius"))
            {
                // Just pick a random enemy as the center
                var aliveEnemies = enemies.Where(e => e.IsAlive()).ToList();
                return aliveEnemies.Count > 0 ? aliveEnemies[Random.Range(0, aliveEnemies.Count)] : null;
            }

            // For single target, use standard targeting
            return SelectTarget(user, enemies);
        }

        bool IsDefensiveAbility(AbilityData ability)
        {
            // Check if ability is defensive (healing, buffs, shields)
            if (ability.damageType == DamageType.Healing)
                return true;

            if (ability.actionType == ActionType.Enhancement && ability.areaOfEffect.ToLower().Contains("self"))
                return true;

            if (ability.abilityName.ToLower().Contains("heal") ||
                ability.abilityName.ToLower().Contains("shield") ||
                ability.abilityName.ToLower().Contains("protect") ||
                ability.abilityName.ToLower().Contains("armor"))
                return true;

            return false;
        }

        bool IsOffensiveAbility(AbilityData ability)
        {
            // Check if ability is offensive (deals damage)
            if (ability.damageRoll.numberOfDice > 0 && ability.damageType != DamageType.Healing)
                return true;

            if (ability.inflictsStatus.Any(s => s.condition == StatusCondition.Stunned ||
                                               s.condition == StatusCondition.Paralyzed ||
                                               s.condition == StatusCondition.Poisoned))
                return true;

            return false;
        }
        #endregion

        #region Action Point Management
        /// <summary>
        /// Decide if we should save AP for next turn
        /// </summary>
        public bool ShouldBankActionPoints(CharacterData character)
        {
            // Bank AP if we're low on health and want to save for defensive actions
            float hpPercent = (float)character.currentHP / character.maxHP;
            if (hpPercent < defensiveThreshold && character.currentAP <= 2)
            {
                return true;
            }

            // Bank AP if we have expensive abilities we want to use next turn
            var expensiveAbilities = character.knownAbilities
                .Where(a => a.actionPointCost > character.currentAP)
                .ToList();

            if (expensiveAbilities.Count > 0 && character.currentAP <= 2)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region Spatial Awareness

        /// <summary>
        /// Determine if character should move and where
        /// </summary>
        bool ShouldMove(CharacterData character, List<CharacterData> enemies, CombatManager combat, out HexCoordinate targetPosition)
        {
            targetPosition = HexCoordinate.Invalid;

            if (character.currentAP < 1)
                return false;

            HexBattlefield battlefield = combat.battlefield;
            CombatantData self = combat.combatants.FirstOrDefault(c => c.character == character);

            if (self == null || battlefield == null)
                return false;

            // Find closest enemy
            CharacterData closestEnemy = null;
            int closestDistance = int.MaxValue;

            foreach (var enemy in enemies.Where(e => e.IsAlive()))
            {
                CombatantData enemyCombatant = combat.combatants.FirstOrDefault(c => c.character == enemy);
                if (enemyCombatant != null)
                {
                    int dist = battlefield.GetDistance(self.position, enemyCombatant.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestEnemy = enemy;
                    }
                }
            }

            if (closestEnemy == null)
                return false;

            CombatantData closestEnemyCombatant = combat.combatants.FirstOrDefault(c => c.character == closestEnemy);

            // Determine if we should move based on abilities
            bool hasRangedAbilities = character.knownAbilities.Any(a => a.range > 3f);
            bool hasMeleeAbilities = character.knownAbilities.Any(a => a.range <= 1.5f);

            // Melee characters: move closer if out of range
            if (!hasRangedAbilities || hasMeleeAbilities)
            {
                if (closestDistance > 1) // Not in melee range
                {
                    targetPosition = FindMoveTowardTarget(battlefield, self.position,
                                                         closestEnemyCombatant.position,
                                                         character.speed);
                    bool shouldMove = targetPosition != HexCoordinate.Invalid;
                    if (shouldMove)
                    {
                        Debug.Log($"{character.characterName} wants to move toward {closestEnemy.characterName} " +
                                 $"(distance: {closestDistance}) from {self.position} to {targetPosition}");
                    }
                    return shouldMove;
                }
            }
            // Ranged characters: maintain distance
            else if (hasRangedAbilities)
            {
                if (closestDistance <= 2) // Too close for comfort
                {
                    targetPosition = FindMoveAwayFromTarget(battlefield, self.position,
                                                           closestEnemyCombatant.position,
                                                           character.speed);
                    bool shouldMove = targetPosition != HexCoordinate.Invalid;
                    if (shouldMove)
                    {
                        Debug.Log($"{character.characterName} wants to move away from {closestEnemy.characterName} " +
                                 $"(distance: {closestDistance}) from {self.position} to {targetPosition}");
                    }
                    return shouldMove;
                }
            }

            return false;
        }

        /// <summary>
        /// Find best move toward target
        /// </summary>
        HexCoordinate FindMoveTowardTarget(HexBattlefield battlefield, HexCoordinate start,
                                           HexCoordinate target, int maxDistance)
        {
            // Search up to battlefield size to ensure we find a path
            int searchDistance = Mathf.Max(battlefield.width, battlefield.height);
            List<HexCoordinate> path = HexPathfinder.FindPath(battlefield, start, target, searchDistance, false);

            // Move as far along path as possible (up to our movement range)
            if (path != null && path.Count > 1)
            {
                // Move up to maxDistance steps along the path, but don't land on target
                int moveIndex = Mathf.Min(maxDistance, path.Count - 2);
                if (moveIndex < 1) moveIndex = 1; // At minimum, move 1 step
                return path[moveIndex];
            }

            return HexCoordinate.Invalid;
        }

        /// <summary>
        /// Find best move away from target
        /// </summary>
        HexCoordinate FindMoveAwayFromTarget(HexBattlefield battlefield, HexCoordinate start,
                                             HexCoordinate threat, int maxDistance)
        {
            List<HexCoordinate> reachable = HexPathfinder.GetReachableCells(battlefield, start, maxDistance);

            HexCoordinate bestPosition = start;
            int bestDistance = battlefield.GetDistance(start, threat);

            foreach (var pos in reachable)
            {
                int dist = battlefield.GetDistance(pos, threat);
                if (dist > bestDistance)
                {
                    bestDistance = dist;
                    bestPosition = pos;
                }
            }

            return bestPosition.Equals(start) ? HexCoordinate.Invalid : bestPosition;
        }

        /// <summary>
        /// Get enemies within ability range
        /// </summary>
        List<CharacterData> GetEnemiesInRange(CharacterData character, List<CharacterData> enemies,
                                              CombatManager combat)
        {
            List<CharacterData> inRange = new List<CharacterData>();
            HexBattlefield battlefield = combat.battlefield;
            CombatantData self = combat.combatants.FirstOrDefault(c => c.character == character);

            if (self == null)
                return enemies; // Fallback

            // Find max range of known abilities
            float maxRange = character.knownAbilities.Any() ? character.knownAbilities.Max(a => a.range) : 1.5f;
            int maxRangeHexes = Mathf.CeilToInt(maxRange / combat.metersPerHex);

            foreach (var enemy in enemies.Where(e => e.IsAlive()))
            {
                CombatantData enemyCombatant = combat.combatants.FirstOrDefault(c => c.character == enemy);
                if (enemyCombatant != null)
                {
                    int distance = battlefield.GetDistance(self.position, enemyCombatant.position);
                    if (distance <= maxRangeHexes)
                    {
                        inRange.Add(enemy);
                    }
                }
            }

            return inRange;
        }

        /// <summary>
        /// Get enemies in melee range (1 hex)
        /// </summary>
        List<CharacterData> GetEnemiesInMeleeRange(CharacterData character, List<CharacterData> enemies,
                                                   CombatManager combat)
        {
            List<CharacterData> inRange = new List<CharacterData>();
            HexBattlefield battlefield = combat.battlefield;
            CombatantData self = combat.combatants.FirstOrDefault(c => c.character == character);

            if (self == null)
                return enemies;

            foreach (var enemy in enemies.Where(e => e.IsAlive()))
            {
                CombatantData enemyCombatant = combat.combatants.FirstOrDefault(c => c.character == enemy);
                if (enemyCombatant != null)
                {
                    int distance = battlefield.GetDistance(self.position, enemyCombatant.position);
                    if (distance <= 1) // Melee range
                    {
                        inRange.Add(enemy);
                    }
                }
            }

            return inRange;
        }

        #endregion
    }

    /// <summary>
    /// Different AI personality presets
    /// </summary>
    public enum AIPersonality
    {
        Aggressive,  // High aggression, uses abilities frequently, targets low HP enemies
        Defensive,   // Low aggression, saves AP, focuses on survival
        Tactical,    // High intelligence, optimal targeting, good ability usage
        Random,      // Unpredictable, random decisions
        Balanced     // Mix of all strategies
    }

    /// <summary>
    /// Helper class to configure AI based on personality
    /// </summary>
    public static class AIPersonalityPresets
    {
        public static void ConfigureAI(BattleAI ai, AIPersonality personality)
        {
            switch (personality)
            {
                case AIPersonality.Aggressive:
                    ai.aggression = 1.0f;
                    ai.abilityUsageRate = 0.8f;
                    ai.intelligence = 0.5f;
                    ai.defensiveThreshold = 0.1f;
                    break;

                case AIPersonality.Defensive:
                    ai.aggression = 0.3f;
                    ai.abilityUsageRate = 0.4f;
                    ai.intelligence = 0.7f;
                    ai.defensiveThreshold = 0.5f;
                    break;

                case AIPersonality.Tactical:
                    ai.aggression = 0.7f;
                    ai.abilityUsageRate = 0.7f;
                    ai.intelligence = 1.0f;
                    ai.defensiveThreshold = 0.3f;
                    break;

                case AIPersonality.Random:
                    ai.aggression = Random.value;
                    ai.abilityUsageRate = Random.value;
                    ai.intelligence = Random.value;
                    ai.defensiveThreshold = Random.value * 0.5f;
                    break;

                case AIPersonality.Balanced:
                default:
                    ai.aggression = 0.6f;
                    ai.abilityUsageRate = 0.6f;
                    ai.intelligence = 0.7f;
                    ai.defensiveThreshold = 0.3f;
                    break;
            }
        }
    }
}
