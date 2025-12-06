using System.Collections.Generic;
using UnityEngine;
using MythRealFFSV2.Character;

namespace MythRealFFSV2.Data
{
    /// <summary>
    /// Base class for all items in the game
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "MythReal/Items/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Information")]
        public string itemName;
        public int itemId;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;

        [Header("Economy")]
        public int valueInNotes;
        public bool isTradeable = true;

        [Header("Properties")]
        public float weight;
        public int maxStackSize = 1;
        public bool isConsumable;
        public bool isQuestItem;

        [Header("Usage")]
        public int actionPointCost = 1;
        public List<string> tags = new List<string>();
        public List<string> specialProperties = new List<string>();
    }

    /// <summary>
    /// Weapon item data
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "MythReal/Items/Weapon")]
    public class WeaponData : ItemData
    {
        [Header("Weapon Properties")]
        public WeaponType weaponType;
        public DiceRoll damageRoll;
        public DamageType damageType;

        [Header("Requirements")]
        public AttributeType primaryAttribute;
        public int minimumAttributeValue;
        public bool isTwoHanded;
        public bool isFinesse; // Can use COR instead of STR
        public bool isThrown;
        public float throwRange;

        [Header("Proficiency")]
        public TechTreeType requiredProficiency;

        public int CalculateDamage(CharacterData character)
        {
            int baseDamage = damageRoll.Roll();

            // Add attribute modifier
            int attributeBonus = character.attributes.GetModifier(primaryAttribute);

            // If finesse weapon, can use COR if higher
            if (isFinesse)
            {
                int corBonus = character.attributes.GetModifier(AttributeType.Coordination);
                attributeBonus = Mathf.Max(attributeBonus, corBonus);
            }

            return baseDamage + attributeBonus;
        }
    }

    /// <summary>
    /// Armor item data
    /// </summary>
    [CreateAssetMenu(fileName = "New Armor", menuName = "MythReal/Items/Armor")]
    public class ArmorData : ItemData
    {
        [Header("Armor Properties")]
        public ArmorType armorType;
        public EquipmentSlot equipmentSlot;
        public int armorClass; // AC bonus

        [Header("Modifiers")]
        public List<AttributeModifier> attributeModifiers = new List<AttributeModifier>();
        public bool restrictsMovement;
        public float speedPenalty; // Reduction to movement speed

        [Header("Requirements")]
        public int minimumStrength;
    }

    /// <summary>
    /// Consumable item data (potions, scrolls, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "New Consumable", menuName = "MythReal/Items/Consumable")]
    public class ConsumableData : ItemData
    {
        [Header("Consumable Properties")]
        public DiceRoll healingAmount;
        public List<StatusCondition> removesConditions = new List<StatusCondition>();
        public List<StatusEffect> appliesEffects = new List<StatusEffect>();
        public List<AttributeModifier> temporaryAttributeBoosts = new List<AttributeModifier>();
        public int effectDuration; // in turns or minutes
    }
}
