using System;
using System.Collections.Generic;

namespace MythRealFFSV2.Character
{
    /// <summary>
    /// Represents a character's skill with its calculated value
    /// Skills are calculated from attribute modifiers according to the rulebook
    /// </summary>
    [Serializable]
    public class Skill
    {
        public SkillType type;
        public int proficiencyLevel; // How many levels of proficiency the character has
        public int bonusModifier; // Additional bonuses from talents, items, etc.

        private CharacterAttributes attributes;

        public Skill(SkillType type, CharacterAttributes attributes)
        {
            this.type = type;
            this.attributes = attributes;
            this.proficiencyLevel = 0;
            this.bonusModifier = 0;
        }

        /// <summary>
        /// Calculate the skill value based on attribute modifiers
        /// According to the rulebook skill calculation table
        /// </summary>
        public int Value
        {
            get
            {
                int baseValue = CalculateBaseValue();
                return baseValue + proficiencyLevel + bonusModifier;
            }
        }

        private int CalculateBaseValue()
        {
            switch (type)
            {
                case SkillType.Stealth:
                    return (attributes.GetModifier(AttributeType.Coordination) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Athletics:
                    return (attributes.GetModifier(AttributeType.Strength) +
                            attributes.GetModifier(AttributeType.Coordination)) / 2;

                case SkillType.Deception:
                    return (attributes.GetModifier(AttributeType.Sociability) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Perception:
                    return attributes.GetModifier(AttributeType.Wits);

                case SkillType.Intimidation:
                    return (attributes.GetModifier(AttributeType.Sociability) +
                            attributes.GetModifier(AttributeType.Strength)) / 2;

                case SkillType.Entertain:
                    return (attributes.GetModifier(AttributeType.Sociability) +
                            attributes.GetModifier(AttributeType.Coordination)) / 2;

                case SkillType.Speech:
                    return attributes.GetModifier(AttributeType.Sociability);

                case SkillType.Insight:
                    return (attributes.GetModifier(AttributeType.Intelligence) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Thievery:
                    return (attributes.GetModifier(AttributeType.Coordination) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Animals:
                    return (attributes.GetModifier(AttributeType.Intelligence) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Nature:
                    return (attributes.GetModifier(AttributeType.Intelligence) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Inspect:
                    return (attributes.GetModifier(AttributeType.Intelligence) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                case SkillType.Essence:
                    // Use the higher of INT or FAI
                    return Math.Max(attributes.GetModifier(AttributeType.Intelligence),
                                   attributes.GetModifier(AttributeType.Faith));

                case SkillType.Encyclopedia:
                    return attributes.GetModifier(AttributeType.Intelligence);

                case SkillType.Survival:
                    return attributes.GetModifier(AttributeType.Wits);

                case SkillType.Religion:
                    // Use the higher of INT or FAI
                    return Math.Max(attributes.GetModifier(AttributeType.Intelligence),
                                   attributes.GetModifier(AttributeType.Faith));

                case SkillType.Medicine:
                    return (attributes.GetModifier(AttributeType.Intelligence) +
                            attributes.GetModifier(AttributeType.Wits)) / 2;

                default:
                    return 0;
            }
        }

        public void AddProficiency()
        {
            proficiencyLevel++;
        }

        public void AddBonus(int bonus)
        {
            bonusModifier += bonus;
        }

        public override string ToString()
        {
            return $"{type}: {Value} (Base: {CalculateBaseValue()}, Prof: +{proficiencyLevel}, Bonus: +{bonusModifier})";
        }
    }

    /// <summary>
    /// Manages all character attributes
    /// </summary>
    [Serializable]
    public class CharacterAttributes
    {
        private Dictionary<AttributeType, AttributeScore> attributes;

        public CharacterAttributes()
        {
            attributes = new Dictionary<AttributeType, AttributeScore>();
            foreach (AttributeType type in Enum.GetValues(typeof(AttributeType)))
            {
                attributes[type] = new AttributeScore(type, 10);
            }
        }

        public AttributeScore GetAttribute(AttributeType type)
        {
            return attributes[type];
        }

        public int GetValue(AttributeType type)
        {
            return attributes[type].Value;
        }

        public int GetModifier(AttributeType type)
        {
            return attributes[type].Modifier;
        }

        public void SetBaseValue(AttributeType type, int value)
        {
            attributes[type].baseValue = value;
        }

        public void ApplyBonus(AttributeType type, int bonus)
        {
            attributes[type].ApplyBonus(bonus);
        }

        public Dictionary<AttributeType, AttributeScore> GetAllAttributes()
        {
            return attributes;
        }
    }
}
