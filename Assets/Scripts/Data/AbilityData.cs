using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// ScriptableObject that defines an ability/skill that characters can use in combat or out of combat
    /// </summary>
    [CreateAssetMenu(fileName = "New Ability", menuName = "MythReal/Character/Ability")]
    public class AbilityData : ScriptableObject
    {
        [Header("Basic Information")]
        public string abilityName;
        public int abilityLevel; // Minimum level required
        [TextArea(3, 6)]
        public string description;

        [Header("Costs")]
        public int actionPointCost;
        public int memoryCost; // How many memory slots this ability takes
        public List<ComponentRequirement> components = new List<ComponentRequirement>();

        [Header("Action Properties")]
        public ActionType actionType;
        public string duration; // e.g., "Instant", "3 turns", "Combat", etc.
        public float range; // in meters
        public string areaOfEffect; // e.g., "Single target", "3x3 square", etc.

        [Header("Saving Throw")]
        public bool requiresSave;
        public AttributeType saveType;

        [Header("Damage/Healing")]
        public DiceRoll damageRoll;
        public DamageType damageType;
        public bool addAttributeModifier; // Add INT/FAI/STR/etc to damage
        public AttributeType damageAttributeType;

        [Header("Status Effects")]
        public List<StatusEffect> inflictsStatus = new List<StatusEffect>();

        [Header("Requirements")]
        public TechTreeType requiredTechTree;
        public int requiredTechTreeLevel;
        public List<string> otherRequirements = new List<string>();

        [Header("Cooldown")]
        public int cooldownTurns = 0;

        [Header("Special Properties")]
        public List<string> specialProperties = new List<string>();

        /// <summary>
        /// Calculate the damage this ability would deal with a character's modifiers
        /// </summary>
        public int CalculateDamage(CharacterData character)
        {
            int baseDamage = damageRoll.Roll();

            if (addAttributeModifier)
            {
                baseDamage += character.attributes.GetModifier(damageAttributeType);
            }

            return baseDamage;
        }

        /// <summary>
        /// Check if character can use this ability
        /// </summary>
        public bool CanUse(CharacterData character, int currentAP)
        {
            if (currentAP < actionPointCost)
                return false;

            if (character.level < abilityLevel)
                return false;

            // Check if character knows this ability
            if (!character.HasAbility(this))
                return false;

            return true;
        }
    }

    [System.Serializable]
    public class DiceRoll
    {
        public int numberOfDice = 1;
        public int diceSize = 6; // d6, d4, d8, d12, d20
        public int modifier = 0;

        public int Roll()
        {
            int total = modifier;
            for (int i = 0; i < numberOfDice; i++)
            {
                total += Random.Range(1, diceSize + 1);
            }
            return total;
        }

        public override string ToString()
        {
            string result = $"{numberOfDice}d{diceSize}";
            if (modifier != 0)
            {
                result += $" {(modifier > 0 ? "+" : "")}{modifier}";
            }
            return result;
        }
    }

    [System.Serializable]
    public class ComponentRequirement
    {
        public string componentName;
        public bool consumed; // Is the component consumed when used?
    }

    [System.Serializable]
    public class StatusEffect
    {
        public StatusCondition condition;
        public int duration; // in turns
        public string saveType; // Attribute to save against
    }
}
