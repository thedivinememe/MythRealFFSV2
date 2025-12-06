# MythReal Fantasy Fantasy Sports - Character System Documentation

## Overview

This is a comprehensive character system for **MythReal Fantasy Fantasy Sports** - a sports management simulator set in a fantasy RPG world where you manage teams of heroes battling in arena combat.

The system is built in **Unity using C#** and implements all core mechanics from the MythReal RPG rulebook, including:

- ✅ **Character Creation** with Ancestries and Backgrounds
- ✅ **7 Primary Attributes** (COR, FAI, FRT, INT, SOC, STR, WIT)
- ✅ **16 Skills** calculated from attribute modifiers
- ✅ **Special Attributes** (HP, DEF, Initiative, Memory, etc.)
- ✅ **Tech Tree System** with 20 different trees
- ✅ **Ability & Talent Systems** with memory-based learning
- ✅ **Equipment & Inventory** system
- ✅ **Turn-Based Combat** with Action Point (AP) system
- ✅ **Character Progression** and leveling

---

## Architecture

### Core Components

#### 1. **Data Layer (ScriptableObjects)**
Located in `Assets/Scripts/Data/`

- `AncestryData.cs` - Defines the 4 ancestries (Human, Elf, Orc, Drake)
- `BackgroundData.cs` - Defines 26 backgrounds (Detective, Warrior, etc.)
- `TechTreeData.cs` - Defines 20 tech trees and their progression
- `AbilityData.cs` - Defines abilities/skills characters can use
- `TalentData.cs` - Defines talents (feats) that modify characters
- `ItemData.cs` - Defines weapons, armor, and consumables

**ScriptableObjects** allow you to create data assets in Unity's editor that designers can configure without touching code.

#### 2. **Character Layer**
Located in `Assets/Scripts/Character/`

- `Enums.cs` - All enumerations used in the system
- `AttributeScore.cs` - Handles individual attribute scores and modifiers
- `Skill.cs` - Calculates skill values from attributes
- `CharacterData.cs` - Main character class with all stats and methods

#### 3. **Combat Layer**
Located in `Assets/Scripts/Combat/`

- `CombatManager.cs` - Turn-based combat with AP system

#### 4. **Systems Layer**
Located in `Assets/Scripts/Systems/`

- `CharacterGenerator.cs` - Utility for creating characters

#### 5. **Examples**
Located in `Assets/Scripts/Examples/`

- `CharacterSystemExample.cs` - Demonstrates how to use the system

---

## Character Creation Flow

### 1. Choose Ancestry
Characters must select from 4 ancestries:

| Ancestry | Starting HP | Attribute Bonuses | Languages |
|----------|-------------|-------------------|-----------|
| **Human** | 8 | +2 to any attributes | Basic, Yanalian |
| **Elf** | 6 | +COR, +INT, +FAI, -FRT | Basic, Yanalian, Mythraic |
| **Orc** | 10 | +STR, +FRT | Basic, Yanalian, Krandalian |
| **Drake** | 10 | +STR, +WIT | Basic, Drakonic, Yanalian |

### 2. Choose Background
Select from 26 backgrounds (Acrobat, Detective, Soldier, etc.)

Each background provides:
- Choice of 2 attribute increases (pick 1)
- 2 skill proficiencies
- 1 talent
- Starting equipment
- Optional bonus languages

### 3. Assign Attributes
**Option A: Standard Array**
```
[16, 15, 14, 12, 11, 10, 8]
```

**Option B: Roll 4d6 drop lowest** (7 times)

The 7 attributes are:
- **Coordination (COR)** - Agility, reflexes, finesse
- **Faith (FAI)** - Divine magic, belief
- **Fortitude (FRT)** - Health, endurance
- **Intelligence (INT)** - Learning, memory, arcane magic
- **Sociability (SOC)** - Influence, charisma
- **Strength (STR)** - Physical power
- **Wits (WIT)** - Awareness, intuition

### 4. Calculate Skills
Skills are automatically calculated from attribute modifiers:

```csharp
Stealth = (COR + WIT) / 2
Athletics = (STR + COR) / 2
Deception = (SOC + WIT) / 2
Perception = WIT
Intimidation = (SOC + STR) / 2
Entertain = (SOC + COR) / 2
Speech = SOC
Insight = (INT + WIT) / 2
Thievery = (COR + WIT) / 2
Animals = (INT + WIT) / 2
Nature = (INT + WIT) / 2
Inspect = (INT + WIT) / 2
Essence = MAX(INT, FAI)
Encyclopedia = INT
Survival = WIT
Religion = MAX(INT, FAI)
```

### 5. Choose Primary & Secondary Attributes
These determine HP growth and progression benefits.

### 6. Assign Tech Points
At level 1, characters get **2 tech points** to invest in any of 20 tech trees:

**Combat Trees:**
- 1H Weapons, 2H Weapons, Dual Wielding, Ranged Weapons, Magic Weapons
- Martial Arts, Warfare, Sly, Hunting

**Magic Trees:**
- Wind, Water, Fire, Earth, Light, Dark, Arcane
- Summoning, Morphology, Blood, Wild

### 7. Learn Abilities
Characters start with **4 memory slots** for learning abilities.
Each ability costs memory slots based on complexity.

---

## Code Examples

### Creating a Character

```csharp
using MythRealFFSV2.Character;
using MythRealFFSV2.Data;
using MythRealFFSV2.Systems;

// Get references to ScriptableObject assets
AncestryData orcAncestry = // Load from Resources or assign in Inspector
BackgroundData detectiveBackground = // Load from Resources

// Create character template
CharacterTemplate template = new CharacterTemplate
{
    characterName = "Karr-El",
    sex = SexType.Male,
    age = 28,
    height = 7.0f,
    weight = 280f,
    alignment = AlignmentType.LawfulGood,
    catchphrase = "This case is cracked wide open!",

    ancestry = orcAncestry,
    background = detectiveBackground,
    chosenBackgroundAttribute = AttributeType.Wits,

    primaryAttribute = AttributeType.Wits,
    secondaryAttribute = AttributeType.Strength
};

// Create character using generator
CharacterGenerator generator = GetComponent<CharacterGenerator>();
CharacterData karrEl = generator.CreateFromTemplate(template);

// Invest tech points
karrEl.InvestTechPoint(TechTreeType.Sly);
karrEl.InvestTechPoint(TechTreeType.DualWielding);
```

### Running Combat

```csharp
using MythRealFFSV2.Combat;
using System.Collections.Generic;

// Create teams
List<CharacterData> team1 = new List<CharacterData> { hero1, hero2 };
List<CharacterData> team2 = new List<CharacterData> { enemy1, enemy2 };

// Start combat
CombatManager combat = GetComponent<CombatManager>();
combat.StartCombat(team1, team2);

// On a character's turn
var current = combat.GetCurrentCombatant();

// Use an ability
combat.UseAbility(fireballAbility, current.character, targetEnemy);

// Or basic attack
combat.PerformBasicAttack(current.character, targetEnemy);

// End turn
combat.EndCurrentTurn();
```

### Accessing Character Stats

```csharp
CharacterData character = // your character

// Attributes
int strValue = character.attributes.GetValue(AttributeType.Strength);
int strModifier = character.attributes.GetModifier(AttributeType.Strength);

// Skills
int stealthValue = character.GetSkillValue(SkillType.Stealth);

// Combat stats
int currentHP = character.currentHP;
int maxHP = character.maxHP;
int defense = character.defense;
int currentAP = character.currentAP;

// Display full character sheet
Debug.Log(character.GetDetailedStats());
```

---

## Combat System

### Action Point (AP) System

- Each character starts with **5 AP** per turn
- Can bank up to **2 AP** between turns
- Actions cost varying amounts of AP:
  - **Movement**: 1 AP per 5 feet (2m)
  - **Basic Attack**: 2 AP
  - **Abilities**: 1-5 AP (varies by ability)

### Turn Order

1. Roll **Initiative** (1d20 + Initiative modifier)
2. Characters act in descending initiative order
3. On each turn:
   - Start Turn (restore AP, process status effects)
   - Take actions (move, attack, use abilities)
   - End Turn (bank unused AP up to 2)

### Combat Flow

```
Initiative Roll → Sort by Initiative → Round Start
    ↓
Character 1 Turn → Character 2 Turn → ... → Character N Turn
    ↓
Round End → Check Win Conditions → Next Round or End Combat
```

---

## Special Attributes

### Defense (DEF)
```
DEF = 10 + ((MAX(STR, COR) + WIT) / 2) + Armor Bonus
```

### Hit Points (HP)
```
HP = Ancestry Base HP + (FRT Modifier × Level)
```

### Memory (Ability Slots)
```
Memory = 4 + INT Modifier (minimum 2)
```

### Initiative
```
Initiative = (COR Modifier + WIT Modifier) / 2
```

---

## Next Steps

### To Use This System:

1. **Create ScriptableObject Assets**
   - Right-click in Unity → Create → MythReal → Character → Ancestry
   - Create all 4 ancestries (Human, Elf, Orc, Drake)
   - Create backgrounds (Detective, Warrior, etc.)
   - Create abilities and talents

2. **Attach Example Script**
   - Add `CharacterSystemExample.cs` to a GameObject
   - Assign the ancestry and background assets
   - Run the scene to see character creation and combat

3. **Build UI**
   - Character creation screen
   - Lineup/roster management
   - Combat visualization
   - Stats display

4. **Implement Battle Simulation**
   - AI decision-making for abilities
   - Auto-battle system
   - Statistics tracking
   - Results display

5. **Add Progression**
   - Save/load character data
   - XP and leveling
   - Tech tree UI
   - Ability learning interface

---

## File Structure

```
Assets/
├── Scripts/
│   ├── Character/
│   │   ├── Enums.cs
│   │   ├── AttributeScore.cs
│   │   ├── Skill.cs
│   │   └── CharacterData.cs
│   ├── Combat/
│   │   └── CombatManager.cs
│   ├── Data/
│   │   ├── AncestryData.cs
│   │   ├── BackgroundData.cs
│   │   ├── TechTreeData.cs
│   │   ├── AbilityData.cs
│   │   ├── TalentData.cs
│   │   └── ItemData.cs
│   ├── Systems/
│   │   └── CharacterGenerator.cs
│   └── Examples/
│       └── CharacterSystemExample.cs
└── ScriptableObjects/
    ├── Ancestries/
    ├── Backgrounds/
    ├── TechTrees/
    ├── Abilities/
    ├── Talents/
    └── Items/
```

---

## Key Features Implemented

✅ **Character Creation**
- Ancestry selection with bonuses
- Background selection with proficiencies
- Attribute rolling (4d6 drop lowest) or standard array
- Skill calculation from attributes

✅ **Combat System**
- Turn-based with initiative
- Action Point (AP) system (5 AP per turn, bank up to 2)
- Attack rolls vs Defense
- Damage calculation with modifiers
- Status effects (poisoned, stunned, etc.)

✅ **Progression**
- XP and leveling (1000 XP per level)
- Tech point allocation
- Ability learning (memory-based)
- Talent acquisition

✅ **Data-Driven Design**
- ScriptableObjects for easy content creation
- No hardcoded character data
- Designer-friendly tools

✅ **Extensible Architecture**
- Easy to add new ancestries, backgrounds, abilities
- Modular systems
- Clean separation of concerns

---

## Notes

This system implements the core MythReal RPG rules as a foundation for the **Fantasy Fantasy Sports** game. The next major components to build are:

1. **Battle Simulation Engine** - Automated AI combat
2. **Team Management UI** - Roster, lineups, tactics
3. **Statistics Tracking** - Combat logs, player stats, season records
4. **Persistence** - Save/load character and team data
5. **Procedural Generation** - Generate random characters, teams, opponents

The architecture is designed to support all of these features with minimal modifications to the core character system.

---

## Support

For questions about the MythReal rulebook or this implementation, refer to:
- The MythReal Rulebook (provided)
- Unity Documentation: https://docs.unity3d.com/
- C# Documentation: https://docs.microsoft.com/en-us/dotnet/csharp/
