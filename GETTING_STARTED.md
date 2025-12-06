# Getting Started with MythReal Fantasy Fantasy Sports

## Quick Start Guide

Follow these steps to get your game up and running!

---

## Step 1: Generate ScriptableObject Assets

The first thing you need to do is populate your project with all the game data (ancestries, backgrounds, abilities, etc.).

### How to Generate Assets:

1. **Open Unity** with your MythRealFFSV2 project
2. In the Unity menu bar, go to: **MythReal → Populate All Data**
3. Click **"Yes"** when prompted
4. Wait for Unity to create all the assets (this takes a few seconds)
5. You should see a success message!

This will create all the ScriptableObject assets in `Assets/ScriptableObjects/`:
- ✅ **4 Ancestries** (Human, Elf, Orc, Drake)
- ✅ **6 Backgrounds** (Detective, Soldier, Scholar, Criminal, Medic, Merchant)
- ✅ **8+ Abilities** (Fire Bolt, Twin Strike, Adrenaline Rush, etc.)
- ✅ **5+ Talents** (Counter, Dodge, Sneak Attack, etc.)
- ✅ **8+ Items** (Weapons, armor, potions)
- ✅ **2 Tech Trees** (Sly, Dual Wielding)

### Alternative: Create Individual Assets

If you only want to create specific data types, you can use:
- **MythReal → Create Data → Ancestries**
- **MythReal → Create Data → Backgrounds**
- **MythReal → Create Data → Abilities**
- **MythReal → Create Data → Talents**
- **MythReal → Create Data → Items**
- **MythReal → Create Data → Tech Trees**

---

## Step 2: Run the Example Scene

Now let's see the system in action!

### Option A: Character System Example

1. **Create an empty GameObject** in your scene
2. **Add the component**: `CharacterSystemExample`
3. In the Inspector, assign your created assets:
   - Drag **Ancestries** from `Assets/ScriptableObjects/Ancestries/` to the `Ancestries` list
   - Drag **Backgrounds** from `Assets/ScriptableObjects/Backgrounds/` to the `Backgrounds` list
4. **Press Play**
5. Check the **Console** to see character creation and combat simulation

### Option B: Battle Simulation Example

1. **Create an empty GameObject** in your scene
2. **Add the component**: `BattleSimulationExample`
3. Assign assets in the Inspector:
   - Ancestries
   - Backgrounds
   - Abilities
   - Talents
4. Configure battle settings:
   - Team Size: 3
   - Instant Simulation: true/false
   - Team 1 Personality: Tactical
   - Team 2 Personality: Aggressive
5. **Press Play**, then in the Inspector, click the button **"Run Example Battle"**
6. Watch the battle simulation in the Console!

---

## Step 3: Understanding the AI Personalities

The Battle AI has 5 different personalities:

| Personality | Description | Best For |
|-------------|-------------|----------|
| **Aggressive** | High damage, uses abilities frequently, targets weak enemies | Glass cannon teams |
| **Defensive** | Saves AP, focuses on survival, uses defensive abilities | Tank/support teams |
| **Tactical** | Optimal decisions, smart targeting, balanced approach | Competitive play |
| **Random** | Unpredictable, chaotic | Fun/casual battles |
| **Balanced** | Mix of all strategies | General use |

You can configure these in the `BattleSimulator` component or via code:

```csharp
simulator.team1Personality = AIPersonality.Tactical;
simulator.team2Personality = AIPersonality.Aggressive;
```

---

## Step 4: Creating Characters Programmatically

### Example: Create Karr-El from the Rulebook

```csharp
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;
using MythRealFFSV2.Systems;

// Get references (assign in Inspector or load from Resources)
AncestryData orcAncestry = // your orc ancestry asset
BackgroundData detectiveBackground = // your detective background asset

// Create character template
CharacterTemplate template = new CharacterTemplate
{
    characterName = "Karr-El",
    sex = SexType.Male,
    age = 28,
    height = 7.0f,
    weight = 280f,
    catchphrase = "This case is cracked wide open!",

    ancestry = orcAncestry,
    background = detectiveBackground,
    chosenBackgroundAttribute = AttributeType.Wits,

    primaryAttribute = AttributeType.Wits,
    secondaryAttribute = AttributeType.Strength
};

// Assign specific attribute scores
template.attributeAssignments = new Dictionary<AttributeType, int>
{
    { AttributeType.Wits, 12 },
    { AttributeType.Coordination, 15 },
    { AttributeType.Faith, 9 },
    { AttributeType.Fortitude, 15 },
    { AttributeType.Intelligence, 13 },
    { AttributeType.Sociability, 13 },
    { AttributeType.Strength, 11 }
};

// Create character
CharacterGenerator generator = GetComponent<CharacterGenerator>();
CharacterData karrEl = generator.CreateFromTemplate(template);

// Invest tech points
karrEl.InvestTechPoint(TechTreeType.Sly);
karrEl.InvestTechPoint(TechTreeType.DualWielding);
```

### Example: Create Random Character

```csharp
// Load your assets
List<AncestryData> ancestries = // your ancestry assets
List<BackgroundData> backgrounds = // your background assets

// Generate random character
CharacterGenerator generator = GetComponent<CharacterGenerator>();
CharacterData randomHero = generator.CreateRandomCharacter(ancestries, backgrounds);

Debug.Log($"Created: {randomHero.characterName}");
Debug.Log(randomHero.GetDetailedStats());
```

---

## Step 5: Running a Battle Simulation

### Basic Battle

```csharp
// Create teams
List<CharacterData> team1 = new List<CharacterData>
{
    hero1, hero2, hero3
};

List<CharacterData> team2 = new List<CharacterData>
{
    enemy1, enemy2, enemy3
};

// Get battle simulator
BattleSimulator simulator = GetComponent<BattleSimulator>();

// Configure
simulator.instantSimulation = true; // No delays
simulator.team1Personality = AIPersonality.Tactical;
simulator.team2Personality = AIPersonality.Aggressive;

// Subscribe to results
simulator.onBattleComplete.AddListener(OnBattleComplete);

// Start battle
simulator.SimulateBattle(team1, team2);
```

### Handle Battle Results

```csharp
void OnBattleComplete(BattleResult result)
{
    Debug.Log($"Winner: {result.outcome}");

    // Check who survived
    foreach (var character in result.team1FinalState)
    {
        if (character.IsAlive())
            Debug.Log($"{character.characterName} survived!");
    }

    // View statistics
    Debug.Log($"Total damage by Team 1: {result.statistics.team1TotalDamage}");
    Debug.Log($"Total damage by Team 2: {result.statistics.team2TotalDamage}");

    // View character stats
    foreach (var kvp in result.statistics.characterStats)
    {
        Debug.Log(kvp.Value.ToString());
    }
}
```

---

## Step 6: Giving Characters Abilities

Characters need abilities to be effective in combat!

### Assign Abilities Manually

```csharp
// Load ability assets
AbilityData fireBolt = // your Fire Bolt ability
AbilityData twinStrike = // your Twin Strike ability

// Learn abilities
if (character.CanLearnAbility(fireBolt))
{
    character.LearnAbility(fireBolt);
}

if (character.CanLearnAbility(twinStrike))
{
    character.LearnAbility(twinStrike);
}
```

### Assign Random Abilities

```csharp
void AssignRandomAbilities(CharacterData character, List<AbilityData> availableAbilities)
{
    int abilitiesToLearn = 3;

    for (int i = 0; i < abilitiesToLearn && availableAbilities.Count > 0; i++)
    {
        var randomAbility = availableAbilities[Random.Range(0, availableAbilities.Count)];

        if (character.CanLearnAbility(randomAbility))
        {
            character.LearnAbility(randomAbility);
        }
    }
}
```

---

## Step 7: Character Progression

### Level Up

```csharp
// Award XP
character.GainExperience(1000); // Automatically levels up at 1000 XP

// Check if leveled up
if (character.level > 1)
{
    Debug.Log($"{character.characterName} is now level {character.level}!");
}
```

### Invest Tech Points

```csharp
// Check available points
if (character.availableTechPoints > 0)
{
    // Invest in a tech tree
    character.InvestTechPoint(TechTreeType.Fire);

    // This increases the tech tree level
    int fireLevel = character.GetTechTreeLevel(TechTreeType.Fire);
}
```

### Add Talents

```csharp
TalentData dodgeTalent = // your Dodge talent asset

if (dodgeTalent.MeetsRequirements(character))
{
    character.AddTalent(dodgeTalent);
    Debug.Log($"{character.characterName} learned {dodgeTalent.talentName}!");
}
```

---

## Step 8: Equipment System

### Equip Items

```csharp
WeaponData sword = // your sword weapon
ArmorData leatherArmor = // your leather armor

// Equip weapon
character.EquipItem(sword, EquipmentSlot.MainHand);

// Equip armor
character.EquipItem(leatherArmor, EquipmentSlot.Chest);

// Defense is automatically recalculated
Debug.Log($"Defense: {character.defense}");
```

### Use Consumables

```csharp
ConsumableData healingPotion = // your healing potion

// Add to inventory
character.inventory.Add(healingPotion);

// Use healing potion (in actual combat)
character.Heal(healingPotion.healingAmount.Roll());
```

---

## Common Issues & Solutions

### Issue: "MissingReferenceException: The object of type 'AncestryData' has been destroyed"

**Solution**: You need to run **MythReal → Populate All Data** first to create the assets.

### Issue: "Character has no abilities in combat"

**Solution**: Make sure to assign abilities to characters before battle:
```csharp
AssignRandomAbilities(character, yourAbilityList);
```

### Issue: "Battle ends immediately with no winner"

**Solution**: Check that characters have HP and valid stats:
```csharp
character.CalculateMaxHP();
character.currentHP = character.maxHP;
```

### Issue: "AI doesn't use abilities"

**Solution**: Increase the `abilityUsageRate` on the BattleAI component, or check that characters actually know abilities.

---

## Next Steps

Now that you have the basics working, you can:

1. **Create More Content**
   - Add more abilities from the rulebook
   - Create all 26 backgrounds
   - Add more weapons and items
   - Build out all 20 tech trees

2. **Build UI**
   - Team management screen
   - Character creation UI
   - Battle visualization
   - Stats/results display

3. **Add Game Modes**
   - Season/league system
   - Tournament brackets
   - Story campaign
   - Character recruitment

4. **Implement Persistence**
   - Save/load characters
   - Team rosters
   - Season progress
   - Player stats

5. **Enhance AI**
   - Team formations
   - Ability synergies
   - Defensive positioning
   - Combo recognition

---

## File Structure Reference

```
Assets/
├── Scripts/
│   ├── Character/          ← Core character classes
│   ├── Combat/             ← Combat & AI systems
│   ├── Data/               ← ScriptableObject definitions
│   ├── Systems/            ← Utilities (CharacterGenerator)
│   ├── Editor/             ← DataPopulator tool
│   └── Examples/           ← Example scripts
│
└── ScriptableObjects/      ← Your data assets (created by DataPopulator)
    ├── Ancestries/
    ├── Backgrounds/
    ├── TechTrees/
    ├── Abilities/
    ├── Talents/
    └── Items/
```

---

## Key Scripts Reference

| Script | Purpose |
|--------|---------|
| `CharacterData.cs` | Main character class with all stats |
| `CharacterGenerator.cs` | Create characters from templates or randomly |
| `CombatManager.cs` | Turn-based combat system |
| `BattleAI.cs` | AI decision making |
| `BattleSimulator.cs` | Automated battle simulation |
| `DataPopulator.cs` | Generate all ScriptableObject assets |

---

## Support & Documentation

- **Full Documentation**: See `MYTHREAL_CHARACTER_SYSTEM.md`
- **Quick Reference**: See `QUICK_REFERENCE.md`
- **MythReal Rulebook**: Your provided PDF

---

Happy simulating! 🎮⚔️🏆
