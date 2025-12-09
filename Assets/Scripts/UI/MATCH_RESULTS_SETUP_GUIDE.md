# Match Results Screen - UI Setup Guide

This guide walks you through setting up the Match Results UI step-by-step.

## Overview

The Match Results screen displays:
- List of recent matches with scores
- Clickable rows to see detailed battle statistics
- Detail panel with combat breakdown

## Step 1: Create the Main Panel

1. In your Canvas, create a new Panel:
   - Right-click Canvas → UI → Panel
   - Rename to: `MatchResultsScreen`
   - Set to inactive initially (uncheck at top of Inspector)

2. Add the MatchResultsUI component:
   - Select `MatchResultsScreen`
   - Add Component → Scripts → MythRealFFSV2.UI → MatchResultsUI

3. Style the background:
   - Set color to semi-transparent dark (e.g., RGBA: 0, 0, 0, 200)

## Step 2: Create the Header

1. Create header container:
   - Right-click MatchResultsScreen → UI → Panel
   - Rename to: `Header`
   - Anchor: Top-stretch
   - Height: 80

2. Add title text:
   - Right-click Header → UI → Text - TextMeshPro
   - Rename to: `HeaderText`
   - Text: "Match Results - Season 2025"
   - Font Size: 32
   - Alignment: Center/Middle
   - Anchor: Stretch-stretch

## Step 3: Create the Match List Container

1. Create scroll view:
   - Right-click MatchResultsScreen → UI → Scroll View
   - Rename to: `MatchListScrollView`
   - Position below Header
   - Anchor: Stretch-stretch with margins (leave space for header and buttons)

2. Setup the Content area:
   - Select MatchListScrollView → Viewport → Content
   - Add Component → Vertical Layout Group
     - Child Alignment: Upper Center
     - Control Child Size: Width = true, Height = false
     - Child Force Expand: Width = true, Height = false
     - Spacing: 5
   - Add Component → Content Size Fitter
     - Vertical Fit: Preferred Size
   - Rename to: `MatchListContainer`

3. Configure the scroll view:
   - MatchListScrollView settings:
     - Horizontal Scrollbar: None (delete it)
     - Vertical Scrollbar: Keep it
     - Movement Type: Clamped

## Step 4: Create Match Result Row Prefab

This is the template for each match in the list.

1. Create a new Panel OUTSIDE the MatchResultsScreen (for prefab creation):
   - Right-click Canvas → UI → Panel
   - Rename to: `MatchResultRow`
   - Width: 700, Height: 80

2. Add Button component:
   - Select MatchResultRow
   - Add Component → Button
   - Transition: Color Tint
   - Highlighted Color: Light gray
   - Pressed Color: Dark gray

3. Add Horizontal Layout Group:
   - Add Component → Horizontal Layout Group
     - Padding: Left/Right = 10, Top/Bottom = 10
     - Spacing: 15
     - Child Force Expand: Width = true, Height = true
     - Child Control Size: Height = true

4. Create the text elements as children:

   **Week Text:**
   - Right-click MatchResultRow → UI → Text - TextMeshPro
   - Rename to: `WeekText`
   - Text: "Week 1"
   - Font Size: 18
   - Alignment: Left/Middle
   - Layout Element → Preferred Width: 80

   **Matchup Text:**
   - Right-click MatchResultRow → UI → Text - TextMeshPro
   - Rename to: `MatchupText`
   - Text: "Dragons vs Knights"
   - Font Size: 20
   - Alignment: Left/Middle
   - Layout Element → Flexible Width: 1

   **Score Text:**
   - Right-click MatchResultRow → UI → Text - TextMeshPro
   - Rename to: `ScoreText`
   - Text: "85 - 72"
   - Font Size: 24
   - Alignment: Center/Middle
   - Layout Element → Preferred Width: 100
   - Color: White (or highlight color)

   **Result Text:**
   - Right-click MatchResultRow → UI → Text - TextMeshPro
   - Rename to: `ResultText`
   - Text: "W: Dragons"
   - Font Size: 18
   - Alignment: Right/Middle
   - Layout Element → Preferred Width: 150

5. Create the prefab:
   - Drag MatchResultRow from Hierarchy into Assets/Prefabs/ folder
   - This creates the prefab
   - Delete the original MatchResultRow from Canvas

## Step 5: Create the Detail Panel

This panel shows when you click a match.

1. Create detail panel:
   - Right-click MatchResultsScreen → UI → Panel
   - Rename to: `DetailPanel`
   - Set to inactive initially
   - Anchor: Stretch-stretch with margins (or center with fixed size)
   - Size: 600 x 500 (or your preference)
   - Color: Darker background with higher opacity

2. Add close button:
   - Right-click DetailPanel → UI → Button - TextMeshPro
   - Rename to: `CloseButton`
   - Position: Top-right corner
   - Text: "X" or "Close"
   - Width/Height: 40x40 (for X) or 100x40 (for Close)

3. Add detail text fields:

   **Matchup Text:**
   - Right-click DetailPanel → UI → Text - TextMeshPro
   - Rename to: `DetailMatchupText`
   - Text: "Week 5: Dragons vs Knights"
   - Position: Near top
   - Font Size: 24
   - Alignment: Center

   **Score Text:**
   - Right-click DetailPanel → UI → Text - TextMeshPro
   - Rename to: `DetailScoreText`
   - Text: "Final Score: 85 - 72\nWinner: Dragons"
   - Position: Below matchup
   - Font Size: 20
   - Alignment: Center

   **Stats Text:**
   - Right-click DetailPanel → UI → Text - TextMeshPro
   - Rename to: `DetailStatsText`
   - Text: "Battle Statistics:\n\n[Stats will appear here]"
   - Position: Below score (takes up most of panel)
   - Font Size: 16
   - Alignment: Left/Top
   - Enable Rich Text: Yes
   - Auto Size: Off
   - Overflow: Scroll (or increase panel size)

4. Layout tips:
   - Use anchors to make elements responsive
   - Add padding/margins for better spacing
   - Consider adding a semi-transparent overlay behind DetailPanel

## Step 6: Create Back Button

1. Create back button:
   - Right-click MatchResultsScreen → UI → Button - TextMeshPro
   - Rename to: `BackButton`
   - Position: Bottom-left corner
   - Text: "← Back to Dashboard"
   - Size: 200 x 50

## Step 7: Assign References in Inspector

1. Select `MatchResultsScreen` in Hierarchy
2. In the MatchResultsUI component, assign:

   **Display Settings:**
   - Max Matches To Show: 10 (or your preference)

   **UI Elements:**
   - Header Text: Drag HeaderText here
   - Match List Container: Drag MatchListContainer (the Content object)
   - Match Result Prefab: Drag the prefab from Assets/Prefabs/

   **Detail Panel:**
   - Detail Panel: Drag DetailPanel
   - Detail Matchup Text: Drag DetailMatchupText
   - Detail Score Text: Drag DetailScoreText
   - Detail Stats Text: Drag DetailStatsText
   - Close Detail Button: Drag CloseButton from DetailPanel

   **Navigation:**
   - Back Button: Drag BackButton

## Step 8: Connect to UIManager

1. Select UIManager in your Canvas
2. In UIManager component:
   - Assign Match Results Screen: Drag MatchResultsScreen

## Step 9: Test the Screen

1. **In Play Mode:**
   - Start a new game
   - Play through a week of matches
   - Navigate to Match Results screen

2. **What to look for:**
   - Match list should populate with played matches
   - Clicking a match should show detail panel
   - Stats should display correctly
   - Back button returns to dashboard

## Troubleshooting

**No matches showing:**
- Check that matches have been played (`leagueManager.GetAllMatches()`)
- Verify Match Result Prefab is assigned
- Check console for errors

**Prefab not displaying correctly:**
- Verify all child text elements exist with correct names
- Check Layout Groups are configured
- Ensure prefab has Button component

**Detail panel not showing:**
- Check DetailPanel is assigned
- Verify DetailPanel starts inactive
- Check that Button onClick listeners are set up (should be automatic)

**Stats are empty:**
- Battle must complete to have statistics
- Check that `match.battleResult` is not null
- Verify battle statistics are being recorded in BattleSimulator

**Layout looks wrong:**
- Check anchor settings on all elements
- Verify Layout Groups have correct settings
- Adjust spacing and padding values

## Example Hierarchy Structure

```
Canvas
└── MatchResultsScreen (Panel + MatchResultsUI)
    ├── Header (Panel)
    │   └── HeaderText (TextMeshPro)
    ├── MatchListScrollView (Scroll View)
    │   └── Viewport
    │       └── MatchListContainer (Content with Vertical Layout)
    │           └── [Match rows created at runtime]
    ├── DetailPanel (Panel, inactive by default)
    │   ├── CloseButton (Button)
    │   ├── DetailMatchupText (TextMeshPro)
    │   ├── DetailScoreText (TextMeshPro)
    │   └── DetailStatsText (TextMeshPro)
    └── BackButton (Button)
```

## Prefab Structure

```
Assets/Prefabs/MatchResultRow (Prefab)
└── Panel + Button + Horizontal Layout Group
    ├── WeekText (TextMeshPro)
    ├── MatchupText (TextMeshPro)
    ├── ScoreText (TextMeshPro)
    └── ResultText (TextMeshPro)
```

## Styling Tips

**Color Scheme:**
- Background: Dark semi-transparent (0, 0, 0, 200)
- Headers: Accent color (e.g., gold, blue)
- Text: White or light gray
- Buttons: Highlight on hover
- Winner text: Green or team color
- Detail panel: Even darker background

**Fonts:**
- Use TextMeshPro for better quality
- Headers: 24-32 size
- Body text: 16-20 size
- Small labels: 14-16 size

**Spacing:**
- Add padding to containers (10-20px)
- Space between rows (5-10px)
- Margins around panel edges (20-30px)

## Next Steps

Once the basic screen is working:
1. Add filtering (by week, by team)
2. Add sorting (by score, by date)
3. Show character performance stats
4. Add playoff match highlighting
5. Export match history

## Additional Enhancements

**Visual Polish:**
- Add icons for wins/losses
- Color-code team names
- Highlight close matches
- Animate row expansion

**Data Display:**
- Show match location (home/away)
- Display playoff round
- Show season context
- Add team logos

**Interactivity:**
- Filter by team dropdown
- Week slider
- Export to CSV
- Share results

---

That's it! Your Match Results screen should now be functional. Test it thoroughly and adjust styling to match your game's aesthetic.
