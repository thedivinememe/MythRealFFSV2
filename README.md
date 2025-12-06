# MythReal Fantasy Fantasy Sports

A Unity-based sports management simulator set in a fantasy RPG world where you manage teams of heroes battling in arena combat.

![Unity](https://img.shields.io/badge/Unity-2021.3+-black?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-10.0-purple?style=flat&logo=c-sharp)
![License](https://img.shields.io/badge/License-Custom-blue?style=flat)

---

## 🎮 What Is This?

**MythReal Fantasy Fantasy Sports (MRFFS)** is similar to **Out of the Park Baseball**, but instead of baseball players, you manage teams of **fantasy heroes** who battle in arena combat as a sport!

- 🏆 **Manage teams** of heroes with unique abilities
- ⚔️ **Simulate battles** with turn-based combat AI
- 📊 **Track statistics** for players and teams
- 🎲 **RPG mechanics** based on the MythReal tabletop RPG system
- 🤖 **Intelligent AI** with multiple personality types

---

## ✨ Features Implemented

### ✅ Core Character System
- **7 Primary Attributes** (COR, FAI, FRT, INT, SOC, STR, WIT)
- **16 Skills** auto-calculated from attributes
- **Special Attributes** (HP, Defense, Initiative, Memory, Speed)
- **4 Ancestries** (Human, Elf, Orc, Drake)
- **26 Backgrounds** (Detective, Soldier, Scholar, etc.)
- **20 Tech Trees** for character progression
- **Character progression** with XP and leveling

### ✅ Combat System
- **Turn-based combat** with initiative order
- **Action Point (AP) system** (5 AP per turn, bank up to 2)
- **Attack rolls** vs Defense
- **Damage calculation** with critical hits
- **Status effects** (poisoned, stunned, burning, etc.)

### ✅ Battle Simulation AI
- **5 AI Personalities** (Aggressive, Defensive, Tactical, Random, Balanced)
- **Smart targeting** based on threat assessment
- **Ability usage** with tactical decision-making
- **Action point management**
- **Defensive behavior** when low on health

### ✅ Data Management
- **ScriptableObject-based** data architecture
- **Easy content creation** without coding
- **Data populator tool** to generate all assets
- **Extensible system** for adding content

### ✅ Statistics Tracking
- **Battle results** with win/loss/draw outcomes
- **Character statistics** (damage dealt, healing, abilities used)
- **Team statistics** (total damage, abilities used, attacks)
- **Top performers** tracking

---

## 🚀 Quick Start

### 1. Generate Game Data
```
Unity Menu → MythReal → Populate All Data
```
This creates all ancestries, backgrounds, abilities, talents, and items.

### 2. Run Example Scene
1. Create an empty GameObject
2. Add component: `BattleSimulationExample`
3. Assign the generated ScriptableObject assets
4. Press Play
5. Click "Run Example Battle" in Inspector

### 3. View Results
Check the Console to see:
- Team rosters
- Battle play-by-play
- Final results and statistics

---

## 📖 Documentation

| Document | Description |
|----------|-------------|
| **[GETTING_STARTED.md](GETTING_STARTED.md)** | Step-by-step tutorial for beginners |
| **[MYTHREAL_CHARACTER_SYSTEM.md](MYTHREAL_CHARACTER_SYSTEM.md)** | Complete technical documentation |
| **[QUICK_REFERENCE.md](QUICK_REFERENCE.md)** | Quick lookup tables and formulas |

---

## 🎯 Example Usage

### Create a Character
```csharp
CharacterTemplate template = new CharacterTemplate
{
    characterName = "Karr-El",
    sex = SexType.Male,
    ancestry = orcAncestry,
    background = detectiveBackground,
    chosenBackgroundAttribute = AttributeType.Wits
};

CharacterData character = generator.CreateFromTemplate(template);
```

### Run a Battle
```csharp
List<CharacterData> team1 = new List<CharacterData> { hero1, hero2, hero3 };
List<CharacterData> team2 = new List<CharacterData> { enemy1, enemy2, enemy3 };

BattleSimulator simulator = GetComponent<BattleSimulator>();
simulator.team1Personality = AIPersonality.Tactical;
simulator.team2Personality = AIPersonality.Aggressive;
simulator.SimulateBattle(team1, team2);
```

### Handle Results
```csharp
simulator.onBattleComplete.AddListener((result) =>
{
    Debug.Log($"Winner: {result.outcome}");
    Debug.Log($"Total Damage: {result.statistics.team1TotalDamage}");
});
```

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────┐
│              Battle Simulator                    │
│  ┌────────────────────────────────────────────┐ │
│  │           Combat Manager                    │ │
│  │  ┌──────────────┐    ┌──────────────┐     │ │
│  │  │  Battle AI   │    │  Battle AI   │     │ │
│  │  │   Team 1     │    │   Team 2     │     │ │
│  │  └──────────────┘    └──────────────┘     │ │
│  └────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                      ▲
                      │
        ┌─────────────┴─────────────┐
        │    Character System        │
        ├───────────────────────────┤
        │  • Attributes & Skills     │
        │  • Abilities & Talents     │
        │  • Equipment & Inventory   │
        │  • Progression & Leveling  │
        └────────────────────────────┘
                      ▲
                      │
        ┌─────────────┴─────────────┐
        │   ScriptableObject Data    │
        ├───────────────────────────┤
        │  • Ancestries              │
        │  • Backgrounds             │
        │  • Abilities               │
        │  • Talents                 │
        │  • Items                   │
        │  • Tech Trees              │
        └────────────────────────────┘
```

---

## 📊 Stats & Calculations

### Attributes → Skills
```
Stealth = (COR + WIT) / 2
Athletics = (STR + COR) / 2
Deception = (SOC + WIT) / 2
```

### Combat Stats
```
Defense = 10 + ((MAX(STR, COR) + WIT) / 2) + Armor
HP = Ancestry Base HP + (FRT Modifier × Level)
Initiative = (COR Modifier + WIT Modifier) / 2
```

### Action Points
```
Base AP: 5 per turn
Bank Limit: 2 AP
Movement: 1 AP per 5 feet
Basic Attack: 2 AP
Abilities: 1-5 AP (varies)
```

---

## 🧠 AI System

The Battle AI evaluates:
- **Threat assessment** of enemies
- **Target prioritization** (low HP, healers, high damage)
- **Ability effectiveness** (damage vs cost)
- **AP management** (saving for big moves)
- **Defensive behavior** (when low on health)

### AI Personalities

| Personality | Aggression | Intelligence | Ability Usage | Best For |
|-------------|------------|--------------|---------------|----------|
| Aggressive | 100% | 50% | 80% | Glass cannons |
| Defensive | 30% | 70% | 40% | Tanks/support |
| Tactical | 70% | 100% | 70% | Optimal play |
| Random | Random | Random | Random | Chaos! |
| Balanced | 60% | 70% | 60% | General use |

---

## 🛠️ Tech Stack

- **Unity 2021.3+**
- **C# 10.0**
- **ScriptableObjects** for data
- **Coroutines** for battle simulation
- **UnityEvents** for extensibility

---

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Character/          # Core character classes
│   │   ├── CharacterData.cs
│   │   ├── AttributeScore.cs
│   │   ├── Skill.cs
│   │   └── Enums.cs
│   │
│   ├── Combat/             # Combat & AI systems
│   │   ├── CombatManager.cs
│   │   ├── BattleAI.cs
│   │   └── BattleSimulator.cs
│   │
│   ├── Data/               # ScriptableObject definitions
│   │   ├── AncestryData.cs
│   │   ├── BackgroundData.cs
│   │   ├── AbilityData.cs
│   │   ├── TalentData.cs
│   │   └── ItemData.cs
│   │
│   ├── Systems/            # Utilities
│   │   └── CharacterGenerator.cs
│   │
│   ├── Editor/             # Unity Editor tools
│   │   └── DataPopulator.cs
│   │
│   └── Examples/           # Example usage scripts
│       ├── CharacterSystemExample.cs
│       └── BattleSimulationExample.cs
│
└── ScriptableObjects/      # Game data (generated)
    ├── Ancestries/         # Human, Elf, Orc, Drake
    ├── Backgrounds/        # Detective, Soldier, etc.
    ├── Abilities/          # Fire Bolt, Twin Strike, etc.
    ├── Talents/            # Dodge, Counter, etc.
    ├── Items/              # Weapons, armor, potions
    └── TechTrees/          # Sly, Dual Wielding, etc.
```

---

## 🎲 Based on MythReal RPG

This system implements the **MythReal tabletop RPG** ruleset:
- 7 primary attributes (COR, FAI, FRT, INT, SOC, STR, WIT)
- Skill-based character system
- Tech tree progression (no classes)
- Turn-based combat with Action Points
- Memory-based ability learning

---

## 🔮 Roadmap

### Completed ✅
- [x] Core character system
- [x] Combat mechanics
- [x] Battle simulation AI
- [x] Statistics tracking
- [x] Data population tools

### In Progress 🚧
- [ ] UI/UX for team management
- [ ] Season/league system
- [ ] Save/load functionality

### Planned 📋
- [ ] Character recruitment system
- [ ] Training/progression UI
- [ ] Match scheduling
- [ ] Standings and playoffs
- [ ] Advanced AI formations
- [ ] Multiplayer support

---

## 🤝 Contributing

This is a personal project, but suggestions and feedback are welcome!

---

## 📝 License

Custom License - See project documentation

---

## 🙏 Credits

- **Game Design**: Based on the MythReal RPG system
- **Development**: Built with Unity and C#
- **Inspiration**: Out of the Park Baseball, Football Manager

---

## 📧 Contact

For questions or feedback about this project, please refer to the documentation files or the MythReal rulebook.

---

**Happy Managing!** ⚔️🏆🎮
