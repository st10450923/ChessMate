# HIGH CONCEPT DOCUMENT
## ChessCheckers: The Switch

**Genre:** Two-Player Strategy / Hybrid Board Game  
**Platform:** PC (Unity)  
**Players:** 2 (Local)  
**Game Jam Theme:** Switch  
**Version:** 1.0 — Day 1 Draft  
**Document Author:** Claude (AI) + Developer  

---

## Elevator Pitch

*Two games. One board. Every two turns, the rules change.*

**ChessCheckers: The Switch** is a two-player strategy game where the board physically flips and the rules alternate between Chess and Checkers every two turns. Pieces retain their positions but their identities — and movement rules — transform with each switch. Master both games to win.

---

## Core Concept

A standard 8×8 board is shared between Chess and Checkers. Every **two full turns** (one turn per player), the board "switches" — animating a flip — and all pieces change their ruleset:

| Phase     | Movement Rules         | Capture Rules             |
|-----------|------------------------|---------------------------|
| Chess     | Standard chess piece moves | Standard chess captures   |
| Checkers  | Diagonal moves only    | Mandatory jump captures   |

**Piece Identity Persistence:** Each piece has a hidden "chess identity" (Pawn, Rook, Knight, Bishop, Queen, King). During Checkers phase, all pieces behave as checkers pieces. When Chess phase returns, they revert to their original chess identity. Pieces captured during either phase are permanently removed.

---

## Gameplay Loop

1. **Chess Phase (2 turns):** Players take turns moving pieces by chess rules.
2. **Switch!** Board flip animation triggers. All pieces become checkers pieces.
3. **Checkers Phase (2 turns):** Players take turns moving by checkers rules. Mandatory captures apply.
4. **Switch!** Board flips back. Pieces revert to their chess identities.
5. Repeat until one player's King is captured (Chess) or all opposing pieces are eliminated (Checkers) or a player has no valid moves.

---

## Win Conditions

- **Chess win:** Capture the opponent's King during a Chess phase
- **Checkers win:** Capture all opponent's pieces OR leave opponent with no valid moves during a Checkers phase

---

## Visual Style

Clean, readable board aesthetic with two distinct visual themes that transition during the flip:
- **Chess Phase:** Classic dark/light wood-style board, detailed piece sprites
- **Checkers Phase:** Bold, flat colour board, simplified disc-style pieces
- Art assets sourced from free/open-licence online repositories (no AI-generated art) for consistent visual style

---

## AI Integration (Brief Requirement)

| Tool | Use Case |
|------|----------|
| Claude (Anthropic) | Game design documentation, code scaffolding, logic review, README/plan generation |
| Claude (in-game) | *Stretch goal:* Single-player AI opponent suggesting moves or providing hints |

---

## Unique Selling Point

The Switch mechanic forces players to think in two strategic languages simultaneously. A dominant chess position can be dismantled by aggressive checkers play, and vice versa. Planning must span both rulesets — not just the current one.

---

## Scope (3-Day Jam)

| Day | Target |
|-----|--------|
| Day 1 | All documentation, project setup, Unity scene scaffold |
| Day 2 | Playable prototype — both rulesets, switching mechanic, win conditions |
| Day 3 | Polish, UI, audio, bug fixes, submission prep |

---

*Document generated with AI assistance (Claude, Anthropic). Reviewed and approved by developer.*
