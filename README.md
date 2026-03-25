# ChessCheckers: The Switch

> \\\*Two games. One board. Every two turns, the rules change.\\\*

A two-player local strategy game built for a 3-day game jam on the theme **"Switch"**.

\---

## Overview

**ChessCheckers: The Switch** places two players on a standard 8×8 board, alternating every two turns between Chess rules and Checkers rules. Pieces hold their positions between phases, but their movement and capture abilities transform with each switch.

Can you outmanoeuvre your opponent across two rulesets at once?

\---

## How to Play

### Setup

* Player 1 (White) and Player 2 (Black) begin in standard Chess starting positions
* The game opens in **Chess Phase**

### Turn Structure

1. Each player takes one turn per phase
2. After **2 turns total** (1 per player), the board **Switches**
3. The board flip animation plays — the ruleset changes
4. After another 2 turns, it Switches back
5. This cycle continues until a win condition is met

### Chess Phase Rules

* All standard chess movement applies
* Each piece moves according to its chess identity (Pawn, Rook, Knight, Bishop, Queen, King)
* Win by **capturing the opponent's King**
* No formal check/checkmate — the King can be captured directly

### Checkers Phase Rules

* All pieces move diagonally, one square at a time (forward only for non-kings)
* **Mandatory capture** — if a jump is available, you must take it
* Multi-jump chains are allowed in a single turn
* A piece reaching the opponent's back rank becomes a **Checkers King** (moves freely diagonally) for the duration of this Checkers phase; it reverts to its chess identity on the next Switch
* Win by **capturing all opponent pieces** or leaving them with **no valid moves**

### Switching

* Piece **positions are preserved** across every switch
* Piece **chess identities are preserved** — they always return to the same type
* Pieces **captured in either phase are permanently removed**

\---

## Controls

|Action|Input|
|-|-|
|Select piece|Left click|
|Move piece|Left click on highlighted square|
|Deselect|Right click or click elsewhere|

\---

## Building \& Running

### Requirements

* Unity 2022.3 LTS or newer
* See `requirements.txt` for full dependency list

### Run from Source

1. Clone the repository: `git clone https://github.com/st10450923/GameJam`
2. Open the project in Unity Hub
3. Open the `GameBoard` scene in `/Assets/Scenes/`
4. Press **Play** in the Unity Editor

### Play a Build

1. Download the latest build from the [Releases](../../releases) page
2. Extract and run `ChessCheckers.exe` (Windows) or the appropriate binary

\---

## Project Structure

```
/Assets
  /Scenes         — Unity scenes (GameBoard, MainMenu)
  /Scripts        — All C# game logic
    Piece.cs              — Piece data: identity, position, team
    Board.cs              — Grid, placement, valid move queries
    GameManager.cs        — Turn/phase management, win detection
    SwitchController.cs   — Phase transition and animation
    MoveHighlighter.cs    — Visual move highlighting
  /Sprites        — Art assets (sourced from open-licence repositories)
  /Audio          — SFX and music
  /Prefabs        — Piece and board prefabs
/Docs
  high-concept-document.md
  plan.md
  refinements-changes.md
requirements.txt
README.md
```

\---

## Known Issues / Limitations

* No formal check detection — King capture is the win condition
* En passant and castling not implemented in jam version
* No AI opponent (single-player stretch goal not reached in jam scope)

\---

## Art Assets

All visual assets sourced from free and open-licence repositories. No AI-generated art was used in this project, to ensure a consistent visual style.

Sources include:

* https://game-icons.net/ — chess/board sprites
* [Kenney.nl](https://kenney.nl) - Chess sprites
* [Kenney.nl](https://kenney.nl) - UI elements and additional sprites
* Kenney.nl - Board game info icons

\---

## Credits

**Developer:** Ember Jones
**Engine:** Unity 2022.3 LTS
**Jam Theme:** Switch

\---

## AI Credits

This project used AI tools as part of the game jam brief requiring AI integration.

|Tool|Contribution|
|-|-|
|**Claude (Anthropic) — claude.ai**|Game design documentation (this README, `high-concept-document.md`, `plan.md`, `requirements.txt`, `refinements-changes.md`); code architecture planning; logic design for the Switch mechanic; review and debugging assistance throughout development|
|**GitHub Copilot** *(if used)*|In-editor C# code autocomplete|

AI was used as a **collaborative tool** — all design decisions, final code, and creative direction were made by the developer. AI output was reviewed and edited before inclusion in the project.

A video demonstration of AI-assisted development moments is included with the submission.

\---

## License

This project was made for a game jam. Assets sourced from third parties remain under their respective licences. Original code and design © Ember Jones 2026.

