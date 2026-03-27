# PLAN.md
## ChessCheckers: The Switch — Game Jam Development Plan

**Jam Duration:** 3 Days  
**Engine:** Unity (2D)  
**Version Control:** GitHub  
**Last Updated:** Day 1  

---

## AI Tools & Their Roles

| Tool | Role | Used For |
|------|------|----------|
| **Claude (Anthropic)** | Primary AI assistant | Documentation, code scaffolding, logic design, debugging, review |

> All AI contributions are logged in `refinements-changes.md` and credited in `README.md`.

---

## Day 1 — Foundation & Documentation ✅

**Goal:** All docs done, Unity project set up, scene scaffolded.

### Milestones
- [X] Game concept locked and documented
- [X] All required documents created
- [X] Unity project initialised and pushed to GitHub
- [X] Basic scene with board grid laid out
- [X] Piece data structures designed (chess identity + checkers state)

### Task List

#### Documentation (AI-assisted)
- [X] `high-concept-document.md` — written with Claude
- [X] `plan.md` — written with Claude
- [X] `README.md` — written with Claude
- [X] `requirements.txt` — written with Claude
- [X] `refinements-changes.md` — initiated with Claude

#### Unity Setup
- [X] Create Unity 2D project
- [X] Set up GitHub repo, add `.gitignore` (Unity template)
- [X] Create folder structure: `/Assets/Scripts`, `/Assets/Sprites`, `/Assets/Scenes`, `/Assets/Audio`
- [X] Create `GameBoard` scene
- [X] Lay out 8×8 grid using Unity Tilemap or manual quad mesh
- [X] Import placeholder piece sprites

#### Code Architecture (Day 1 scaffolding)
- [X] `Piece.cs` — base class: position, chess identity, checkers state, team
- [X] `Board.cs` — grid representation, piece placement, valid move queries
- [X] `GameManager.cs` — turn counter, phase tracking, switch trigger
- [X] `SwitchController.cs` — handles the phase transition and board flip animation hook

---

## Day 2 — Playable Prototype 🔲

**Goal:** Both rulesets fully functional, switching works, win conditions detected.

### Milestones
- [X] Chess movement and capture logic complete
- [X] Checkers movement and capture logic complete (including mandatory captures)
- [X] Phase switching works correctly (2-turn trigger, board flip, piece rule swap)
- [X] Piece identity persists and restores across phases
- [X] Win condition detection working for both phases
- [X] Basic UI: turn indicator, phase indicator, switch countdown
- [X] Video demo recorded showing AI-assisted sections

### Task List

#### Chess Logic
- [X] Implement move generation per piece type (Pawn, Rook, Knight, Bishop, Queen, King)
- [X] Handle special moves: castling *(stretch)*, en passant *(stretch)*
- [X] King capture = win condition (simplified, no check/checkmate required for jam)
- [X] Highlight valid moves on click

#### Checkers Logic
- [X] Diagonal movement only (forward for regular pieces)
- [X] Mandatory jump capture — detect and enforce
- [X] Multi-jump chains
- [X] Kinging — if a checkers piece reaches back rank, it becomes a king for the duration of the checkers phase (reverts on switch)
- [X] No valid moves = loss condition

#### Switch System
- [X] Turn counter increments per player move
- [X] At turn 2 and every 2 turns thereafter: trigger switch
- [X] Board flip animation (rotate 180° on Y axis or CSS-style card flip)
- [X] Piece sprites swap between chess set and checkers disc

#### UI
- [X]Phase label: "CHESS" / "CHECKERS"
- [X] Turn indicator: "Black/White"
- [X] Switch counter: "Switch in X turn(s)"
- [ ] Captured pieces tray

#### Documents Update
- [X] Update `refinements-changes.md` with scope shifts discovered on Day 2
- [X] Record and annotate video demo

---

## Day 3 — Polish & Submission 🔲

**Goal:** Polished, bug-free, documented, ready to submit.

### Milestones
- [X] All known bugs resolved
- [X] Visual polish: animations, transitions, board themes
- [X] Audio: move sounds, switch SFX, win/lose jingle
- [ ] All documents finalised and accurate
- [ ] Build exported and tested
- [ ] Submitted to arc 

### Task List

#### Polish
- [ ] Board flip animation refined (smooth, satisfying)
- [ ] Chess-to-checkers visual theme transition
- [X] Piece selection and move highlight polish
- [X] Win screen with replay option
- [ ] Basic main menu

#### Audio
- [X] Piece move SFX
- [X] Capture SFX
- [X] Switch SFX (dramatic board flip sound)
- [-] Win/lose music sting
- [X] Background ambient (optional)

#### Bug Fixes
- [X] Edge case: what happens if King is in checkers phase? (clarify rules)
- [X] Test all piece types across multiple switch cycles
- [X] Test win conditions in both phases
- [X] Test with 2 human players for balance

#### Documentation Finalisation
- [ ] `README.md` — add final build instructions, screenshots
- [ ] `refinements-changes.md` — final log entry
- [X] `plan.md` — mark completed tasks
- [X] Ensure all AI credits are accurate

#### Submission
- [ ] Export Unity build (Windows + WebGL if time permits)
- [ ] Final GitHub commit and tag: `v1.0-jam-submission`
- [ ] Upload to ARC with screenshots and video


- Timer per turn
- Online multiplayer (unlikely in 3 days)

---

*Plan generated with AI assistance (Claude, Anthropic). Ember Jones maintains and updates this document throughout the jam.*
