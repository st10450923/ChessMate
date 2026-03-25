# refinements-changes.md

## ChessCheckers: The Switch — Scope Shifts \& Design Decisions Log

Running log of all design decisions, scope changes, and refinements made during development. Updated throughout the jam.

\---

## DAY 1 — Foundation

### Entry 1 — Initial Concept Lock

**Date:** Day 1
**Type:** Design Decision
**Author:** Ember Jones + Claude (AI)

**Decision:** Locked core concept as described in `high-concept-document.md`.

Key design choices made at concept stage:

* **2 turns per phase** (1 per player) rather than a fixed timer — keeps it turn-based and predictable
* **King capture (not checkmate)** as the chess win condition — checkmate detection is too complex for a 3-day jam
* **Piece identity hidden during checkers phase** — all pieces appear as checkers discs, adding strategic uncertainty
* **No AI opponent in base scope** — explicitly listed as stretch goal

**AI Contribution:** Claude drafted the initial high concept document and this log template. Developer reviewed and approved all content.

\---

### Entry 2 — Switch Timing Clarification

**Date:** Day 1
**Type:** Design Decision
**Author:** Ember Jones

**Decision:** "2 turns" = 1 action per player (a full round), not 2 individual player moves.

**Rationale:** "Every 2 turns" was ambiguous. Settled on 1 action per player per phase before the switch. Each player gets exactly one move in each ruleset before it changes — keeps the Switch mechanic impactful and the game fast.

\---

### Entry 3 — Checkers King Persistence Rule

**Date:** Day 1
**Type:** Design Decision
**Author:** Ember Jones

**Decision:** Checkers Kings are temporary — a piece kinged during the Checkers phase reverts to its chess identity on the next Switch.

**Rationale:** Permanent kinging would conflict with piece identity preservation. A Pawn that gets kinged in checkers is still a Pawn in chess. Temporary kinging avoids a "permanently kinged" state and keeps the ruleset clean.

\---

### Entry 4 — No En Passant / Castling (Jam Scope)

**Date:** Day 1
**Type:** Scope Cut
**Author:** Ember Jones

**Decision:** En passant and castling are out of scope for the jam build.

**Rationale:** Both require significant additional state tracking. Cut in favour of getting core mechanics solid within the 3-day timeline. Remain in stretch goals.

\---

### Entry 5 — Art Direction: No AI Art

**Date:** Day 1
**Type:** Design Decision
**Author:** Ember Jones

**Decision:** All visual assets sourced from free online repositories (game-icons.net, Kenney). No AI-generated art used.

**Rationale:** AI-generated art produces inconsistent style across different piece types and UI elements. Curated open-source packs provide a more cohesive visual result within the jam timeline.

\---

## DAY 2 — Prototype

*(Update this section during Day 2 development)*

### Entry 6 — \[Title]

**Date:** Day 2
**Type:** \[Design Decision / Scope Cut / Bug Fix / Scope Addition]
**Author:**

**Decision:**

**Rationale:**

**AI Contribution (if any):**

\---

## DAY 3 — Polish \& Submission

*(Update this section during Day 3 development)*

### Entry N — \[Title]

**Date:** Day 3
**Type:** \[Design Decision / Scope Cut / Bug Fix / Polish]
**Author:**

**Decision:**

**Rationale:**

\---

## Entry Template

```
### Entry N — \[Title]
\*\*Date:\*\* Day X
\*\*Type:\*\* \[Design Decision / Scope Cut / Bug Fix / Scope Addition / Polish]
\*\*Author:\*\* Developer / Developer + Claude / Claude

\*\*Decision:\*\*
\[What was decided or changed]

\*\*Rationale:\*\*
\[Why this decision was made]

\*\*AI Contribution (if any):\*\*
\[What Claude or other AI tools contributed to this decision]
```

\---

*Log initiated with AI assistance (Claude, Anthropic). All entries reviewed and approved by developer.*

