refinements-changes.md
ChessCheckers: The Switch — Scope Shifts & Design Decisions Log
Running log of all design decisions, scope changes, and refinements made during development. Updated throughout the jam.

DAY 1 — Foundation
Entry 1 — Initial Concept Lock
Date: Day 1
Type: Design Decision
Author: Ember Jones + Claude (AI)
Decision: Locked core concept as described in high-concept-document.md.
Key design choices made at concept stage:

2 turns per phase (1 per player) rather than a fixed timer — keeps it turn-based and predictable
King capture (not checkmate) as the chess win condition — checkmate detection is too complex for a 3-day jam
Piece identity hidden during checkers phase — all pieces appear as checkers discs, adding strategic uncertainty
No AI opponent in base scope — explicitly listed as stretch goal

AI Contribution: Claude drafted the initial high concept document and this log template. Developer reviewed and approved all content.

Entry 2 — Switch Timing Clarification
Date: Day 1
Type: Design Decision
Author: Ember Jones
Decision: "2 turns" = 1 action per player (a full round), not 2 individual player moves.
Rationale: "Every 2 turns" was ambiguous. Settled on 1 action per player per phase before the switch. Each player gets exactly one move in each ruleset before it changes — keeps the Switch mechanic impactful and the game fast.

Entry 3 — Checkers King Persistence Rule (Revised Day 2)
Date: Day 1 (revised Day 2)
Type: Design Decision
Author: Ember Jones
Decision: Checkers Kings are initially planned as temporary. Revised on Day 2 to be permanent — a piece kinged during the Checkers phase retains its king status in all subsequent Checkers phases. It still reverts to its chess identity and movement rules during Chess phases.
Rationale: Temporary kinging was the original design to avoid complexity, but playtesting revealed that permanent kinging better rewards genuine strategic play during the Checkers phase and gives the phase more lasting consequence. The code change was minimal — removing the king reset on phase switch. Since kinging does not affect Chess phase movement at all, there is no conflict with piece identity preservation.

Entry 4 — En Passant and Castling (Scope Restored Day 2)
Date: Day 1 (cut), Day 2 (restored)
Type: Scope Addition
Author: Ember Jones + Claude (AI)
Decision: En passant and castling were originally cut from Day 1 scope. Both were implemented during Day 2 as the prototype was ahead of schedule.
Rationale: Core mechanics were solid early enough on Day 2 to allow time for these additions. Both features meaningfully improve the chess phase's strategic depth and bring it closer to standard chess rules, which is important for players who know chess well.
AI Contribution: Claude provided full implementation for both features — en passant target tracking in Board.cs, pawn move generation updates in MoveResolver.cs, castling rights tracking, and the rook teleport logic in TryExecuteChessMove.

Entry 5 — Art Direction: No AI Art
Date: Day 1
Type: Design Decision
Author: Ember Jones
Decision: All visual assets sourced from free online repositories (game-icons.net, Kenney). No AI-generated art used.
Rationale: AI-generated art produces inconsistent style across different piece types and UI elements. Curated open-source packs provide a more cohesive visual result within the jam timeline.

DAY 2 — Prototype
Entry 6 — King Capture Win Condition in Checkers Phase
Date: Day 2
Type: Scope Addition
Author: Ember Jones
Decision: Added capturing the opponent's King as a win condition during the Checkers phase, mirroring the Chess phase win condition.
Rationale: Determined through playtesting that the Checkers phase felt lower stakes without it. With pieces retaining their chess identities underneath, the King is always present on the board during Checkers — allowing it to be captured and ending the game immediately makes both phases feel equally consequential and keeps the strategic tension consistent across the Switch.
AI Contribution: Claude identified the correct location in TryExecuteCheckersMove to add the king capture check and provided the code change, including the early return false to prevent further move processing after a game-ending capture.

Entry 7 — Move Log UI Added
Date: Day 2
Type: Scope Addition
Author: Ember Jones + Claude (AI)
Decision: Added a move log panel to the right side UI panel, displaying moves in standard algebraic notation (e.g. Ne2-e4, e2xe3).
Rationale: The move log improves readability and gives players a record of what has happened — useful for a strategy game where both players need to track the game state across two rulesets. It also adds polish to the prototype ahead of Day 3.
AI Contribution: Claude designed and implemented the full move log system — LogMove and PieceInitial methods in Board.cs, AddMoveLogEntry and RefreshMoveLog in UIManager.cs, and the full-move numbering logic that pairs White and Black moves on the same line.

Entry 8 — Prototype Playable and Feature Complete
Date: Day 2
Type: Milestone
Author: Ember Jones
Decision: Day 2 prototype is feature complete. All planned mechanics are implemented and functional: Chess movement (all piece types), Checkers movement with mandatory capture and multi-jump, phase switching, both win conditions in both phases, move log, full UI, and restart.
Rationale: No scope cuts were required on Day 2. The prototype exceeded the planned Day 2 deliverables by also including en passant, castling, and the checkers king capture win condition.
Remaining for Day 3: Video demo recording, visual polish, audio, bug fixing, build export, and final document updates.

DAY 3 — Polish & Submission
(Update this section during Day 3 development)
Entry N — [Title]
Date: Day 3
Type: [Design Decision / Scope Cut / Bug Fix / Polish]
Author:
Decision:
Rationale:

Entry Template
### Entry N — [Title]
**Date:** Day X
**Type:** [Design Decision / Scope Cut / Bug Fix / Scope Addition / Polish]
**Author:** Developer / Developer + Claude / Claude

**Decision:**
[What was decided or changed]

**Rationale:**
[Why this decision was made]

**AI Contribution (if any):**
[What Claude or other AI tools contributed to this decision]

Log initiated with AI assistance (Claude, Anthropic). All entries reviewed and approved by developer.
