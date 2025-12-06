# MythReal Fantasy Sports - UI Setup Guide

This guide explains how to set up the Unity UI system for MythReal Fantasy Sports.

## Overview

The UI system consists of multiple screens managed by a central UIManager singleton:
- **Main Menu** - New game, load game, quit
- **League Dashboard** - Standings, season controls, navigation hub
- **Match Results** - View recent match results and battle statistics
- **Team Roster** - View team rosters and character details
- **Draft Board** - View draft picks and available characters
- **Confirmation Dialog** - Reusable dialog for important actions

## Quick Start

### 1. Create the Canvas Hierarchy

Create a Canvas with the following structure:

```
Canvas (Screen Space - Overlay)
├── UIManager (attach UIManager.cs)
├── MainMenuScreen
│   └── MainMenuUI (attach MainMenuUI.cs)
│       ├── Title
│       ├── NewGameButton
│       ├── LoadGameButton
│       └── QuitButton
├── LeagueDashboardScreen
│   └── LeagueDashboardUI (attach LeagueDashboardUI.cs)
│       ├── Header
│       ├── StandingsPanel
│       ├── ControlButtons
│       └── NavigationButtons
├── MatchResultsScreen
│   └── MatchResultsUI (attach MatchResultsUI.cs)
│       ├── MatchList
│       └── DetailPanel
├── TeamRosterScreen
│   └── TeamRosterUI (attach TeamRosterUI.cs)
│       ├── TeamDropdown
│       ├── RosterList
│       └── DetailPanel
├── DraftBoardScreen
│   └── DraftBoardUI (attach DraftBoardUI.cs)
│       ├── PicksList
│       └── AvailableCharacters
└── ConfirmationDialog (attach ConfirmationDialog.cs)
    └── DialogPanel
        ├── Title
        ├── Message
        ├── ConfirmButton
        └── CancelButton
```

### 2. UIManager Setup

1. Create an empty GameObject named "UIManager" on the Canvas
2. Attach the `UIManager.cs` script
3. Assign the screen GameObjects in the inspector:
   - Main Menu Screen
   - League Dashboard Screen
   - Match Results Screen
   - Draft Board Screen
   - Team Roster Screen

### 3. Main Menu Setup

**GameObject: MainMenuScreen**
- Create a Panel as the background
- Add `MainMenuUI.cs` component

**Required UI Elements:**
- Title (TextMeshProUGUI)
- Version Text (TextMeshProUGUI)
- New Game Button
- Load Game Button
- Quit Button

**Inspector Assignments:**
- Assign all UI elements to MainMenuUI component
- Set default values:
  - Default Number Of Teams: 4
  - Default Draft Rounds: 5
  - Default Character Pool Size: 30
- Assign data assets (Ancestries, Backgrounds, Abilities lists)

### 4. League Dashboard Setup

**GameObject: LeagueDashboardScreen**
- Create a Panel as the background
- Add `LeagueDashboardUI.cs` component

**Required UI Elements:**

*Header Section:*
- League Name Text (TextMeshProUGUI)
- Season Info Text (TextMeshProUGUI)
- Schedule Info Text (TextMeshProUGUI)

*Standings Section:*
- Standings Container (Vertical Layout Group)
- Standings Row Prefab (create as prefab with these TextMeshProUGUI elements):
  - RankText
  - TeamNameText
  - RecordText
  - WinPctText
  - DiffText

*Control Buttons:*
- Play Next Week Button
- Simulate Season Button
- Start New Season Button
- Play Playoffs Button

*Navigation Buttons:*
- View Match Results Button
- View Team Rosters Button
- Save Game Button
- Main Menu Button

### 5. Match Results Setup

**GameObject: MatchResultsScreen**
- Create a Panel as the background
- Add `MatchResultsUI.cs` component

**Required UI Elements:**

*Main View:*
- Header Text (TextMeshProUGUI)
- Match List Container (Scroll View with Vertical Layout Group)
- Match Result Prefab (create as prefab with these elements):
  - WeekText (TextMeshProUGUI)
  - MatchupText (TextMeshProUGUI)
  - ScoreText (TextMeshProUGUI)
  - ResultText (TextMeshProUGUI)

*Detail Panel:*
- Detail Panel (GameObject, initially inactive)
- Detail Matchup Text (TextMeshProUGUI)
- Detail Score Text (TextMeshProUGUI)
- Detail Stats Text (TextMeshProUGUI)
- Close Button

*Navigation:*
- Back Button

### 6. Team Roster Setup

**GameObject: TeamRosterScreen**
- Create a Panel as the background
- Add `TeamRosterUI.cs` component

**Required UI Elements:**

*Team Selection:*
- Team Dropdown (TMP_Dropdown)
- Team Info Text (TextMeshProUGUI)

*Roster Display:*
- Roster Container (Scroll View with Vertical Layout Group)
- Character Row Prefab (create as prefab with these elements):
  - NameText (TextMeshProUGUI)
  - LevelText (TextMeshProUGUI)
  - AncestryText (TextMeshProUGUI)
  - BackgroundText (TextMeshProUGUI)
  - HPText (TextMeshProUGUI)
  - StatusText (TextMeshProUGUI)

*Detail Panel:*
- Detail Panel (GameObject, initially inactive)
- Detail Name Text (TextMeshProUGUI)
- Detail Stats Text (TextMeshProUGUI)
- Detail Abilities Text (TextMeshProUGUI)
- Close Button

*Navigation:*
- Back Button

### 7. Draft Board Setup

**GameObject: DraftBoardScreen**
- Create a Panel as the background
- Add `DraftBoardUI.cs` component

**Required UI Elements:**

*Draft Info:*
- Draft Title Text (TextMeshProUGUI)
- Draft Status Text (TextMeshProUGUI)

*Draft Picks:*
- Team Filter Dropdown (TMP_Dropdown)
- Picks Container (Scroll View with Vertical Layout Group)
- Pick Row Prefab (create as prefab with these elements):
  - PickNumText (TextMeshProUGUI)
  - RoundText (TextMeshProUGUI)
  - TeamText (TextMeshProUGUI)
  - CharacterText (TextMeshProUGUI)

*Available Characters:*
- Available Container (Scroll View with Grid Layout Group)
- Character Card Prefab (create as prefab with these elements):
  - NameText (TextMeshProUGUI)
  - DetailsText (TextMeshProUGUI)
  - RatingText (TextMeshProUGUI)

*Navigation:*
- Back Button
- Start Draft Button (for future manual draft feature)

### 8. Confirmation Dialog Setup

**GameObject: ConfirmationDialog**
- Create as a separate GameObject on Canvas (not a screen)
- Add `ConfirmationDialog.cs` component

**Required UI Elements:**
- Dialog Panel (Panel with semi-transparent background)
  - Title Text (TextMeshProUGUI)
  - Message Text (TextMeshProUGUI)
  - Confirm Button
    - Confirm Button Text (TextMeshProUGUI)
  - Cancel Button
    - Cancel Button Text (TextMeshProUGUI)

**Usage in Other Scripts:**
```csharp
ConfirmationDialog.ShowYesNo(
    "Quit Game",
    "Are you sure you want to quit?",
    onYes: () => Application.Quit()
);
```

## Manager Setup

The UI system requires these managers to exist in the scene:
- **TeamManager** - Manages teams
- **LeagueManager** - Manages seasons and matches
- **DraftManager** - Manages drafts
- **SaveLoadManager** - Handles save/load
- **BattleSimulator** - Simulates battles

These can all be on a single "GameManager" GameObject, or organized however you prefer.

## Testing the UI

1. **In Unity Editor:**
   - Open your main scene
   - Ensure all managers are present
   - Press Play
   - The Main Menu should appear automatically

2. **Test New Game Flow:**
   - Click "New Game"
   - Teams should be created and drafted
   - League Dashboard should appear showing standings

3. **Test Season Simulation:**
   - Click "Start New Season"
   - Click "Play Next Week" to play one week
   - Click "View Match Results" to see match details

4. **Test Save/Load:**
   - Play a few weeks
   - Click "Save Game"
   - Return to Main Menu
   - Click "Load Game"
   - Progress should be restored

## Customization Tips

### Styling
- Use TextMeshPro for all text (better quality than standard UI Text)
- Create a consistent color scheme for your team colors
- Use Unity's UI Layout Groups for automatic positioning

### Prefab Creation
- Each row/card prefab should have a specific naming convention for child elements
- Consider using a standard prefab template for consistency
- Add hover effects to buttons for better UX

### Performance
- Use object pooling for frequently created/destroyed UI elements (match rows, character cards)
- Disable inactive screens rather than destroying them
- Limit the number of visible elements in scroll views

## Next Steps

After setting up the basic UI:

1. **Add Polish:**
   - Transitions between screens
   - Loading indicators
   - Sound effects for buttons
   - Animations for important events

2. **Add Features:**
   - Manual draft mode (interactive character selection)
   - Team customization (edit names, colors)
   - Statistics tracking (player stats, team records)
   - Playoff bracket visualization

3. **Improve UX:**
   - Tooltips for abilities and stats
   - Color coding for injured/KO'd characters
   - Sortable standings table
   - Search/filter for characters

## Troubleshooting

**Screen doesn't show up:**
- Check that the screen GameObject is assigned in UIManager
- Verify the screen is active in the hierarchy initially
- Check that UIManager.Instance exists (singleton setup)

**Buttons don't work:**
- Ensure Button components have the correct onClick listeners
- Check that referenced managers exist in the scene
- Verify EventSystem is present in the scene

**Text doesn't display:**
- Make sure you're using TextMeshProUGUI (not regular Text)
- Import TextMeshPro essentials if prompted
- Check that fonts are assigned

**Prefabs don't populate:**
- Verify prefab references are assigned in inspector
- Check that container Transform is assigned
- Ensure prefabs have the correct child element names

## Reference

All UI scripts are located in `Assets/Scripts/UI/`:
- `UIManager.cs` - Main UI controller
- `MainMenuUI.cs` - Main menu controller
- `LeagueDashboardUI.cs` - Dashboard controller
- `MatchResultsUI.cs` - Match results controller
- `TeamRosterUI.cs` - Team roster controller
- `DraftBoardUI.cs` - Draft board controller
- `ConfirmationDialog.cs` - Reusable confirmation dialog
