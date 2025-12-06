using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// ScriptableObject that defines a character background
    /// There are 26 backgrounds in the Alpha test
    /// </summary>
    [CreateAssetMenu(fileName = "New Background", menuName = "MythReal/Character/Background")]
    public class BackgroundData : ScriptableObject
    {
        [Header("Basic Information")]
        public int backgroundId;
        public string backgroundName;
        [TextArea(3, 6)]
        public string description;

        [Header("Attribute Increases")]
        [Tooltip("Player can choose one of these attribute increases")]
        public AttributeType attributeIncreaseOption1;
        public AttributeType attributeIncreaseOption2;

        [Header("Skill Proficiencies")]
        public SkillType skillProficiency1;
        public SkillType skillProficiency2;

        [Header("Talents")]
        public TalentData proficiencyTalent;

        [Header("Languages")]
        public int additionalLanguages = 0;

        [Header("Starting Equipment")]
        [Tooltip("Items this background starts with")]
        public List<ItemData> startingEquipment = new List<ItemData>();
        public int startingNotes = 50;

        [Header("Background Story Elements")]
        [TextArea(2, 4)]
        public string typicalBackstory;
        public List<string> suggestedFlaws = new List<string>();
        public List<string> suggestedIdeals = new List<string>();
        public List<string> suggestedBonds = new List<string>();

        /// <summary>
        /// Apply background bonuses to character
        /// </summary>
        public void ApplyBackgroundBonuses(CharacterData character, AttributeType chosenAttribute)
        {
            // Apply chosen attribute increase
            character.attributes.ApplyBonus(chosenAttribute, 1);

            // Apply skill proficiencies
            character.AddSkillProficiency(skillProficiency1);
            character.AddSkillProficiency(skillProficiency2);

            // Apply talent if available
            if (proficiencyTalent != null)
            {
                character.AddTalent(proficiencyTalent);
            }
        }
    }
}
