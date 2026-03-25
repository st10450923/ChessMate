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
| **GitHub Copilot** *(optional)* | In-editor autocomplete | Boilerplate Unity C# code |
| **ChatGPT / Gemini** *(fallback)* | Secondary reference | Quick lookups if needed |

> All AI contributions are logged in `refinements-changes.md` and credited in `README.md`.

---

## Day 1 — Foundation & Documentation ✅

**Goal:** All docs done, Unity project set up, scene scaffolded.

### Milestones
- [ ] Game concept locked and documented
- [ ] All required documents created
- [ ] Unity project initialised and pushed to GitHub
- [ ] Basic scene with board grid laid out
- [ ] Piece data structures designed (chess identity + checkers state)

### Task List

#### Documentation (AI-assisted)
- [x] `high-concept-document.md` — written with Claude
- [x] `plan.md` — written with Claude
- [x] `README.md` — written with Claude
- [x] `requirements.txt` — written with Claude
- [x] `refinements-changes.md` — initiated with Claude

#### Unity Setup
- [ ] Create Unity 2D project
- [ ] Set up GitHub repo, add `.gitignore` (Unity template)
- [ ] Create folder structure: `/Assets/Scripts`, `/Assets/Sprites`, `/Assets/Scenes`, `/Assets/Audio`
- [ ] Create `GameBoard` scene
- [ ] Lay out 8×8 grid using Unity Tilemap or manual quad mesh
- [ ] Import placeholder piece sprites

#### Code Architecture (Day 1 scaffolding)
- [ ] `Piece.cs` — base class: position, chess identity, checkers state, team
- [ ] `Board.cs` — grid representation, piece placement, valid move queries
- [ ] `GameManager.cs` — turn counter, phase tracking, switch trigger
- [ ] `SwitchController.cs` — handles the phase transition and board flip animation hook

---

## Day 2 — Playable Prototype 🔲

**Goal:** Both rulesets fully functional, switching works, win conditions detected.

### Milestones
- [ ] Chess movement and capture logic complete
- [ ] Checkers movement and capture logic complete (including mandatory captures)
- [ ] Phase switching works correctly (2-turn trigger, board flip, piece rule swap)
- [ ] Piece identity persists and restores across phases
- [ ] Win condition detection working for both phases
- [ ] Basic UI: turn indicator, phase indicator, switch countdown
- [ ] Video demo recorded showing AI-assisted sections

### Task List

#### Chess Logic
- [ ] Implement move generation per piece type (Pawn, Rook, Knight, Bishop, Queen, King)
- [ ] Handle special moves: castling *(stretch)*, en passant *(stretch)*
- [ ] King capture = win condition (simplified, no check/checkmate required for jam)
- [ ] Highlight valid moves on click

#### Checkers Logic
- [ ] Diagonal movement only (forward for regular pieces)
- [ ] Mandatory jump capture — detect and enforce
- [ ] Multi-jump chains
- [ ] Kinging — if a checkers piece reaches back rank, it becomes a king for the duration of the checkers phase (reverts on switch)
- [ ] No valid moves = loss condition

#### Switch System
- [ ] Turn counter increments per player move
- [ ] At turn 2 and every 2 turns thereafter: trigger switch
- [ ] Board flip animation (rotate 180° on Y axis or CSS-style card flip)
- [ ] Piece sprites swap between chess set and checkers disc
- [ ] Chess identity stored before switch, restored after

#### UI
- [ ] Phase label: "CHESS PHASE" / "CHECKERS PHASE"
- [ ] Turn indicator: "Player 1 / Player 2"
- [ ] Switch counter: "Switch in X turn(s)"
- [ ] Captured pieces tray

#### Documents Update
- [ ] Update `refinements-changes.md` with scope shifts discovered on Day 2
- [ ] Record and annotate video demo

---

## Day 3 — Polish & Submission 🔲

**Goal:** Polished, bug-free, documented, ready to submit.

### Milestones
- [ ] All known bugs resolved
- [ ] Visual polish: animations, transitions, board themes
- [ ] Audio: move sounds, switch SFX, win/lose jingle
- [ ] All documents finalised and accurate
- [ ] Build exported and tested
- [ ] Submitted to jam platform

### Task List

#### Polish
- [ ] Board flip animation refined (smooth, satisfying)
- [ ] Chess-to-checkers visual theme transition
- [ ] Piece selection and move highlight polish
- [ ] Win screen with replay option
- [ ] Basic main menu

#### Audio
- [ ] Piece move SFX
- [ ] Capture SFX
- [ ] Switch SFX (dramatic board flip sound)
- [ ] Win/lose music sting
- [ ] Background ambient (optional)

#### Bug Fixes
- [ ] Edge case: what happens if King is in checkers phase? (clarify rules)
- [ ] Test all piece types across multiple switch cycles
- [ ] Test win conditions in both phases
- [ ] Test with 2 human players for balance

#### Documentation Finalisation
- [ ] `README.md` — add final build instructions, screenshots
- [ ] `refinements-changes.md` — final log entry
- [ ] `plan.md` — mark completed tasks
- [ ] Ensure all AI credits are accurate

#### Submission
- [ ] Export Unity build (Windows + WebGL if time permits)
- [ ] Final GitHub commit and tag: `v1.0-jam-submission`
- [ ] Upload to jam platform with screenshots and video

---

## Risk Register

| Risk | Likelihood | Mitigation |
|------|------------|------------|
| Checkers mandatory capture logic too complex | Medium | Simplify: make captures optional for jam scope |
| Board flip animation janky | Low | Use simple alpha fade + swap if 3D flip is unstable |
| Piece identity bug on switch | Medium | Thorough logging and unit tests in Day 2 |
| Scope creep (AI opponent) | High | AI opponent is explicitly a stretch goal — cut if behind |
| Art sourcing takes too long | Low | Use solid colour placeholders on Day 2, swap Day 3 |

---

## Stretch Goals (only if ahead of schedule)
- Single-player vs AI opponent (minimax or random)
- En passant and castling
- Timer per turn
- Online multiplayer (unlikely in 3 days)

---

*Plan generated with AI assistance (Claude, Anthropic). Developer maintains and updates this document throughout the jam.*
