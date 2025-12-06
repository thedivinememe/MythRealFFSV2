# MythReal Quick Reference Guide

## Attribute Modifiers

| Score | Modifier |
|-------|----------|
| 19-20 | +5 |
| 17-18 | +4 |
| 15-16 | +3 |
| 13-14 | +2 |
| 11-12 | +1 |
| 9-10  | 0 |
| 7-8   | -1 |
| 5-6   | -2 |
| 3-4   | -3 |
| 1-2   | -4 |

## Skill Calculations

| Skill | Formula |
|-------|---------|
| Stealth | (COR + WIT) / 2 |
| Athletics | (STR + COR) / 2 |
| Deception | (SOC + WIT) / 2 |
| Perception | WIT |
| Intimidation | (SOC + STR) / 2 |
| Entertain | (SOC + COR) / 2 |
| Speech | SOC |
| Insight | (INT + WIT) / 2 |
| Thievery | (COR + WIT) / 2 |
| Animals | (INT + WIT) / 2 |
| Nature | (INT + WIT) / 2 |
| Inspect | (INT + WIT) / 2 |
| Essence | MAX(INT, FAI) |
| Encyclopedia | INT |
| Survival | WIT |
| Religion | MAX(INT, FAI) |
| Medicine | (INT + WIT) / 2 |

## Special Attributes

| Attribute | Formula |
|-----------|---------|
| Defense (DEF) | 10 + ((MAX(STR, COR) + WIT) / 2) + Armor |
| Hit Points (HP) | Ancestry Base HP + (FRT Modifier × Level) |
| Initiative | (COR Modifier + WIT Modifier) / 2 |
| Memory | 4 + INT Modifier (min 2) |
| Speed | 2m per AP |
| Critical Hit | Natural 20 (5% chance) |

## Ancestries

| Ancestry | HP | Attributes | Languages |
|----------|----|-----------|-----------|
| Human | 8 | +2 any | Basic, Yanalian |
| Elf | 6 | +COR, +INT, +FAI, -FRT | Basic, Yanalian, Mythraic |
| Orc | 10 | +STR, +FRT | Basic, Yanalian, Krandalian |
| Drake | 10 | +STR, +WIT | Basic, Drakonic, Yanalian |

## Backgrounds

| ID | Name | Attr Options | Skills | Talent ID |
|----|------|--------------|--------|-----------|
| 1 | Acrobat | STR/COR | Athletics, Entertain | 14 |
| 2 | Entertainer | SOC/WIT | Entertain, Speech | 17 |
| 3 | Artist | COR/INT | Entertain, Insight | 22 |
| 4 | Barkeep | FRT/SOC | Entertain, Insight | 19 |
| 5 | Bodyguard | STR/FRT | Intimidation, Survival | 23 |
| 6 | Artisan | STR/INT | Insight, Encyclopedia | 22 |
| 7 | Criminal | COR/INT | Deception, Stealth | 24 |
| 8 | Detective | WIT/INT | Inspect, Insight | 25 |
| 9 | Medic | FRT/WIT | Medicine, Encyclopedia | 26 |
| 10 | Drifter | INT/SOC | Thievery, Deception | 27 |
| 11 | Emissary | INT/SOC | Religion, Speech | 20 |
| 12 | Farmer | FRT/WIT | Animals, Nature | 15 |
| 13 | Gambler | COR/SOC | Speech, Insight | 28 |
| 14 | Hermit | FRT/INT | Medicine, Religion | 29 |
| 15 | Inventor | COR/INT | Insight, Encyclopedia | 22 |
| 16 | Merchant | INT/SOC | Speech, Insight | 21 |
| 17 | Miner | STR/WIT | Athletics, Survival | 35 |
| 18 | Nomad | FRT/WIT | Survival, Nature | 31 |
| 19 | Sailor | STR/COR | Athletics, Survival | 16 |
| 20 | Scholar | INT/WIT | Encyclopedia, Essence | 32 |
| 21 | Shepherd | FRT/WIT | Insight, Animals | 33 |
| 22 | Prisoner | STR/FRT | Intimidation, Survival | 24 |
| 23 | Soldier | STR/FRT | Athletics, Intimidation | 23 |
| 24 | Street Rat | COR/FRT | Thievery, Stealth | 34 |
| 25 | Teacher | INT/SOC | Encyclopedia, Insight | 30 |
| 26 | Zealot | FAI/STR | Religion, Insight | 13 |

## Tech Trees (20 Total)

### Combat
- 1H Weapons
- 2H Weapons
- Dual Wielding
- Ranged Weapons
- Magic Weapons
- Martial Arts
- Warfare
- Sly
- Hunting

### Magic
- Wind
- Water
- Fire
- Earth
- Light
- Dark
- Arcane
- Summoning
- Morphology
- Blood
- Wild

## Combat

### Action Points (AP)
- Start with: **5 AP**
- Bank limit: **2 AP**
- Refresh: **Each turn**

### Common Actions
| Action | AP Cost |
|--------|---------|
| Move 5 ft | 1 AP |
| Basic Attack | 2 AP |
| Use Ability | 1-5 AP (varies) |
| Use Item | 1 AP |

### Attack Roll
```
1d20 + Attribute Modifier vs Target Defense
```

### Damage Roll
```
Weapon Dice + Attribute Modifier
```

### Critical Hit
```
Natural 20 = Double Damage Dice
```

## Progression

### Leveling
- XP Required: **1000 per level**
- Rewards per level:
  - +1 Tech Point
  - Recalculate HP, DEF, Memory
  - Possible talent/ability unlocks

### Level 1-5 Progression
| Level | Features |
|-------|----------|
| 1 | Ancestry, Background, 2 Tech Points, 4 Memory, Primary/Secondary Attr |
| 2 | 1 Skill Talent, 1 Tech Talent, 1 Tech Point |
| 3 | +Memory, 1 General Talent, Skill Increase, 1 Tech Point |
| 4 | 1 Skill Talent, 1 Tech Talent, 1 Tech Point |
| 5 | 2 Attr Improvements, Ancestry Talent, Skill Increase, +Memory, 1 Tech Talent, 1 Tech Point |

## Status Conditions

| Condition | Effect |
|-----------|--------|
| Poisoned | Disadvantage on checks, 1d4 poison damage/turn |
| Stunned | Can't move, auto-fail STR/COR saves |
| Paralyzed | Incapacitated, auto-fail STR/COR, attacks have advantage |
| Blinded | Can't see, fail sight checks, disadvantage on attacks |
| Charmed | Can't attack charmer |
| Frightened | Disadvantage while source in sight |
| Burning | 1d6 fire damage/turn |
| Frozen | Speed = 0, disadvantage on rolls, 1d4 cold damage/turn |
| Asleep | Unconscious until damaged or shaken |
| Weakened | Half damage on attacks |

## Standard Attribute Array
```
[16, 15, 14, 12, 11, 10, 8]
```

## Random Attribute Roll
```
4d6 drop lowest (roll 7 times)
```

## Starting Resources
- **Tech Points**: 2
- **Memory Slots**: 4
- **Currency**: 50 notes
- **Abilities**: 3 (based on tech points)
