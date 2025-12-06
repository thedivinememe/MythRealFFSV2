using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// ScriptableObject that defines a Tech Tree and what it unlocks at each level
    /// </summary>
    [CreateAssetMenu(fileName = "New Tech Tree", menuName = "MythReal/Character/Tech Tree")]
    public class TechTreeData : ScriptableObject
    {
        [Header("Basic Information")]
        public TechTreeType techTreeType;
        public string techTreeName;
        [TextArea(3, 6)]
        public string description;
        public Sprite icon;

        [Header("Progression")]
        public List<TechTreeLevel> levels = new List<TechTreeLevel>();

        /// <summary>
        /// Get all abilities unlocked at a specific level
        /// </summary>
        public List<AbilityData> GetAbilitiesAtLevel(int level)
        {
            if (level > 0 && level <= levels.Count)
            {
                return levels[level - 1].abilitiesUnlocked;
            }
            return new List<AbilityData>();
        }

        /// <summary>
        /// Get all features unlocked at a specific level
        /// </summary>
        public List<TalentData> GetFeaturesAtLevel(int level)
        {
            if (level > 0 && level <= levels.Count)
            {
                return levels[level - 1].featuresUnlocked;
            }
            return new List<TalentData>();
        }

        /// <summary>
        /// Get all talents available at a specific level
        /// </summary>
        public List<TalentData> GetTalentsAtLevel(int level)
        {
            if (level > 0 && level <= levels.Count)
            {
                return levels[level - 1].talentsAvailable;
            }
            return new List<TalentData>();
        }

        /// <summary>
        /// Apply tech tree benefits when a character invests a point
        /// </summary>
        public void ApplyLevelBenefits(CharacterData character, int level)
        {
            if (level <= 0 || level > levels.Count)
                return;

            var techLevel = levels[level - 1];

            // Grant features automatically
            foreach (var feature in techLevel.featuresUnlocked)
            {
                character.AddTalent(feature);
            }

            // Abilities need to be learned (they cost memory)
            // Talents are available to be chosen

            Debug.Log($"Applied {techTreeName} level {level} benefits to {character.characterName}");
        }
    }

    [System.Serializable]
    public class TechTreeLevel
    {
        [Header("Level Information")]
        public int level;

        [Header("Unlocks")]
        [Tooltip("Abilities that become available at this level")]
        public List<AbilityData> abilitiesUnlocked = new List<AbilityData>();

        [Tooltip("Features that are automatically granted at this level")]
        public List<TalentData> featuresUnlocked = new List<TalentData>();

        [Tooltip("Talents that become available to choose at this level")]
        public List<TalentData> talentsAvailable = new List<TalentData>();

        [Header("Special Effects")]
        [TextArea(2, 4)]
        public string specialNotes;
    }
}
