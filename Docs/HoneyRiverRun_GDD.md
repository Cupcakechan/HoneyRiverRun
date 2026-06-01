# 🐝 Honey River Run — Game Design Document & Roadmap

**Project:** Game #3 of the 20 Games Challenge — Cocolito Collective
**Engine:** Unity 6 (C# only)
**Genre:** River Raid-style vertical-scrolling shooter (constant downward scroll, left/right movement, upward shooting)
**Repo:** `cupcakechan/HoneyRiverRun`
**Target:** WebGL build for itch.io (`mrcanela/honey-river-run:webgl`) + portfolio showcase
**Status:** ✅ **LOCKED** (v4)

---

## 1. Research Foundation (Original River Raid, Activision 1982)

River Raid is the backbone, not a straitjacket — we deviate where it makes the game more fun or more *us*.

| River Raid (1982) | Honey River Run Translation |
|---|---|
| Jet anchored low, world scrolls down endlessly | Captain Bumble anchored low, honey river scrolls down endlessly |
| Move left/right only | Move left/right only |
| Speed up / slow down (never fully stop) | **Throttle** — speed up / slow (LOCKED) |
| Unlimited-ammo cannon firing straight up | Pollen Darts firing straight up, unlimited |
| Fuel drains constantly; refuel by flying *over* depots | Honey tank drains constantly; refuel over Bloom Orbs |
| Riverbanks = instant death | Flower-canyon banks = instant death (independent box-collider walls) |
| Islands / varying river width as obstacles | **Island obstacles + river-shape variety** (planned — see §13/§15) |
| Bridges end sections, must be destroyed, act as checkpoints | **Honeycomb Gates** — section dividers + checkpoints (LOCKED) |
| Progressive difficulty: narrower river, rarer fuel, faster enemies | Same |
| Limited lives, high-score chase | 3 lives (honey jars), high-score chase with initials entry |

---

## 2. Project Identity

- **Title:** Honey River Run
- **Player:** Captain Bumble — queen-bee aviator with goggles, red scarf, flapping wings, pollen sparkle trail
- **Theme:** Cute 16-bit retro bee aviator down a glowing honey river through a flower canyon. Bright, sticky, joyful Cocolito vibe.
- **Art:** Pure 16-bit SNES style. 16×16 tiles. Shimmering honey-river mid-background.
- **Tone:** Cheerful surface, classic high-tension "one more run" arcade pull underneath.

---

## 3. Purpose

1. **Portfolio:** a complete, polished arcade loop — endless scroll, fuel resource management, shooting, escalating difficulty, score persistence.
2. **Skill growth:** endless-scroll architecture, object pooling, spawn systems, clean state management in Unity 6.
3. **Fun target:** River Raid's "fuel anxiety + dodge + shoot" tension in a joyful original skin.

---

## 4. Core Loop

```
Descend the honey river  →  dodge banks, islands & enemies
        ↑                              ↓
  refuel on Bloom Orbs   ←   shoot Pollen Darts upward
        ↑                              ↓
  watch the honey tank   →   blast Honeycomb Gates (checkpoints)
        ↑                              ↓
        └──────  crash or run dry  →  lose a jar  ──────┘
                          ↓
              all jars gone → Game Over → enter initials → high score
```

Tension triangle, every second: **Where's my next Bloom Orb? / What's coming at me? / Am I about to clip a bank or island?**

---

## 5. Technical Specs (Locked)

| Setting | Value |
|---|---|
| Reference resolution | **960 × 540** |
| Camera | Orthographic + **Pixel Perfect Camera** (2D Pixel Perfect package) |
| — Assets Pixels Per Unit | **16** |
| — Reference Resolution | **320 × 180** (exactly 3× → 960×540, clean integer scaling) |
| — Upscale Render Texture | **ON** (quantizes all rendering to the pixel grid — kills seams/shimmer) |
| — Effective view | ortho size ≈ **5.625**, visible world ≈ **20 × 11.25 units** |
| Color space | **Gamma** (best for Built-in 2D pixel art) |
| Render pipeline | **Built-in** (Option A — lightest WebGL) |
| Build target | WebGL |
| Input | Unity 6 **Input System** (shared `Move` action: X = left/right, Y = throttle) |
| Text | **TextMeshPro** (UI baseline font size 50) |
| Physics | Kinematic Rigidbody2D + trigger colliders; Bumble uses **Full Kinematic Contacts** |

### World layout (current)
- **River channel:** x −5 … +5 (10 units wide) — the navigable play space
- **Grass banks:** x ±5 … ±10 (5 units each side) — visual only; lethality comes from two independent box-collider walls (inner faces at x = ±5)
- **Player anchor:** Captain Bumble at y ≈ −3.5, moves left/right only

### Color Palette (hex)
| Role | Name | Hex |
|---|---|---|
| Primary accent | Lime Green | `#8FD94A` |
| Secondary accent | Hot Pink | `#FF4FA3` |
| Highlight / UI | Sunny Yellow | `#FFD23F` |
| Honey river / fuel | Honey Amber | `#FFB627` |
| Danger / low fuel | Warning Red | `#E84B3C` |
| Deep shadow / outline | Dark Plum | `#3A2A4D` |

---

## 6. Menu Flow

```
[ Boot ] → MainMenu Scene
                ├── PLAY        → Gameplay Scene  (GameManager.StartRun → resets lives)
                ├── HOW TO PLAY → Instructions panel (overlay)
                ├── HIGH SCORES → Top scores panel (overlay)
                └── QUIT        → (hidden/disabled on WebGL)
```
- MainMenu is always the entry point and the only scene entered first when testing (the persistent `GameManager` is created here).
- Scene management via `GameManager` (persistent singleton) + `SceneLoader`.

---

## 7. Gameplay Flow

1. Fade from Main Menu into Gameplay.
2. Honey river scrolls down at cruise speed; Captain Bumble spawns anchored low-center.
3. Honey tank starts full, drains continuously.
4. Enemies, islands, and Bloom Orbs spawn from the top and scroll down with the world.
5. Player throttles, dodges, shoots upward, and flies over Bloom Orbs to refuel.
6. Honeycomb Gates periodically block the river — blast to advance a section, bank a checkpoint, bump difficulty.
7. Crash (bank/island/enemy/gate) or empty tank → lose a jar, brief respawn invulnerability, respawn at center (last Gate later).
8. All 3 jars gone → Game Over → score → 3-letter initials if high score → back to Main Menu.

---

## 8. Controls (Input System)

| Action | Keyboard | Gamepad |
|---|---|---|
| Move Left / Right | A / D or ← / → | Left Stick / D-Pad X |
| Throttle (speed up / slow) | W / S or ↑ / ↓ | Triggers |
| Shoot Pollen Dart | Space | South (A) |
| Pause | Esc | Start |
| Confirm initials | Enter | South (A) |

---

## 9. Entities

### Player — Captain Bumble
One-hit death (with brief respawn invulnerability). Pollen sparkle trail, flapping wings. Constant-drain honey tank.
**Animations:** Fly (default), Fire (shooting), Death (on death), Hurt (reserved for future non-lethal hits), Front Idle (reserved for future menu character-select).

### Fuel — Bloom Orbs
Flying *over* one refills the tank (no shooting). Spawn frequency drops as difficulty climbs. Currently a self-recycling scroller; will move onto the shared Spawner.

### Enemies (roster — introduced one at a time)
| Enemy | Behavior intent | Hits |
|---|---|---|
| **Wasp** ✅ *(first, implemented)* | Spawns as **south-dive** (tops, falls at player), **side-drift** (enters L→east / R→west toward center), or **static** (carried down by the scroll). On hit: **Hurt white-flash → despawn** (no death anim) | 1 |
| Ant Bandits | Basic drifter | 1 |
| Web Weavers | Strings hazards / slows lanes | 1 |
| Stinger Squads | Fast, grouped, weaving | 1 |
| Tanky Ladybugs | Slow, armored | 2–3 |
| Itch Bombers | Drops / launches hazards | 1 |
| Cub Chargers | Charges the player's lane | 1 |

**Wasp animations:** FlySouth / FlyEast / FlyWest (set on spawn via `animator.Play`) + Hurt (white flash). No Death anim.

### Hazards / Structures
- **Flower Canyon Banks** — visual grass tilemap; lethality from two independent static box-collider trigger walls (contact = death).
- **Island Obstacles** — mid-river landmasses (planned). Contact = death.
- **Honeycomb Gate** — section divider + checkpoint; must be destroyed to pass.

---

## 10. Win / Lose

- No traditional win — endless high-score chase.
- Lose a jar on: bank, island, enemy, Gate contact, or empty tank.
- Game Over when all 3 jars are spent.
- Goal: maximize score before Game Over.

---

## 11. Scoring (proposed — tunable)

| Action | Points |
|---|---|
| Distance / survival | continuous tick |
| Wasp | 40 |
| Ant Bandit | 30 |
| Web Weaver / Itch Bomber | 60 |
| Stinger Squad member | 50 |
| Tanky Ladybug | 80 |
| Cub Charger | 100 |
| Honeycomb Gate destroyed | 500 |
| Bloom Orb collected | small bonus or fuel only |

High scores persist via **PlayerPrefs** (WebGL IndexedDB), saved on meaningful events.

---

## 12. Feature List

**Core — Vertical Slice:** Main Menu + scene management · endless honey-river scroll · Bumble left/right movement · throttle · pooled pollen-dart shooting · lethal banks + death · fuel system · one enemy type (Wasp) · lives (3 honey jars) + death feedback · score + HUD · Game Over + high-score initials + persistence.

**Core — Expansion (post-slice):** Honeycomb Gates + checkpoints · progressive difficulty + **world variety** · full enemy roster.

**Polish:** particles (deaths, fuel, pollen trail) · screen shake, hit-stop, SFX/music, menu juice · final art + transitions.

---

## 13. Cocolito Design Ideas (Suggestions — NOT locked)

Each goes through the normal 2–3 options + pick process before implementation.

1. **Throttle ↔ Fuel synergy** — slowing over a Bloom Orb refuels faster/more.
2. **Risk/reward Bloom Orb** — shooting an orb pops it for bonus points but forfeits the refuel.
3. **Overflow bonus** — collecting an orb at a full tank converts to score.
4. **Pollen combo multiplier** — chained kills build a score multiplier; pollen trail intensifies.
5. **Gate variety** — some Honeycomb Gates have a narrow gap you can thread instead of blasting.
6. **Island obstacles** — landmasses in the channel that scroll down and kill on contact (same trigger-death as the banks). Simplest as **pooled scrolling hazards**, reusing the shared Spawner + scroll pattern — independent of river generation, so it can be added shortly after the enemy spawner exists. Primary non-enemy difficulty lever.
7. **River-shape variety (narrowing / widening / branches)** — evolve the world from one repeating segment into **streamed, hand-authored river chunks** (wide, narrow, island-heavy, branching) that cycle in. This is the chunk-streaming approach (flagged in Phase 2 as "right idea, wrong time"); a bigger change that belongs with the Phase 13 world-variety work. Drives spatial difficulty beyond enemies.

---

## 14. Architecture Notes (Unity 6 best practices)

- **GameManager** (State: Menu/Playing/Paused/GameOver) — lightweight, persistent singleton; also owns **Lives** + `OnLivesChanged` + `StartRun()`/`LoseLife()`, and the high score. Created in MainMenu.
- **Object pooling** for Pollen Darts, enemies, Bloom Orbs, islands (recycle via `SetActive`; keep activation/spawn logic **out of `OnEnable`** — use the `ISpawnable.OnSpawned()` the Spawner calls, to dodge the pool-construction gotcha).
- **Generic `Spawner` + `ISpawnable`** — one prefab-agnostic pooled spawner (timer + pool); the spawned object configures itself in `OnSpawned()`. Reused for Wasps now, Bloom Orbs and islands later (just swap the prefab).
- **Single source of truth** `WorldScroll.Speed` — river, banks, enemies, orbs, islands all read it; throttle + difficulty modify it.
- **Bank collision** — lethal banks use **two independent static Box Collider 2D trigger walls** (one composite can't be tuned per-side — single Offset breaks symmetry). The grass tilemap is purely visual.
- **Event-driven UI** — `FuelBarUI`/`LivesUI` subscribe to system events and also self-initialize in `Start` (events can fire before the Gameplay scene/UI exists).
- **ScriptableObjects** for enemy definitions, difficulty tiers, river chunks where it cuts hard-coding (grow into this at Phase 14).
- **Single responsibility:** scroller, spawner, player movement, throttle, shooter, health, fuel, lives, score, UI are separate components.
- **Physics:** player + hazards use Kinematic Rigidbody2D; lethal geometry uses trigger colliders; player has **Full Kinematic Contacts** enabled so kinematic-kinematic/static triggers fire. (Kinematic bodies have no Gravity Scale field — gravity doesn't apply.)
- **Build Profiles:** edit splash/icon/resolution inside the active **Web Build Profile**, not just global Player Settings.

---

## 15. Roadmap (slow & steady — one core feature per step)

> Each step first offers 2–3 options (simple → advanced) with pros/cons + a recommendation, then waits for the pick. After each feature: "Ready for the next core feature?"

### Vertical Slice
- ✅ **Phase 0 — Project Setup** (Unity 6, Built-in 2D, folders, packages, .gitignore, repo, Web profile)
- ✅ **Phase 1 — Main Menu + Scene Management** (GameManager + SceneLoader)
- ✅ **Phase 2 — Endless Honey-River Downscroll** (leapfrog river + grass-bank tilemap, pixel-perfect camera)
- ✅ **Phase 3 — Captain Bumble Movement** (left/right, kinematic RB, Fly anim)
- ✅ **Phase 4 — Throttle** (smooth speed control driving WorldScroll.Speed)
- ✅ **Phase 5 — Pollen Dart Shooting** (object pooling, Fire animation)
- ✅ **Phase 6 — Flower Canyon Banks + Death** (independent box-collider trigger walls, death/respawn)
- ✅ **Phase 7 — Fuel System** (HoneyTank drain + Bloom Orb refuel + low-fuel warning; empty → death)
- ✅ **Phase 8 — First Enemy (Wasp)** (generic pooled Spawner + ISpawnable; Wasp variants; darts deal hits) — establishes the spawner reused for orbs & islands
- ✅ **Phase 9 — Lives + Death Feedback** (3 honey jars, lost-jar blink, respawn invulnerability, placeholder Game Over)
- ⏳ **Phase 10 — Score + HUD** (distance tick + per-kill points on the HUD Canvas, feeds high score)
- **Phase 11 — Game Over + High Score** (initials + persistence)

**Vertical slice completes at Phase 11 — fully playable before anything below.**

### Core Expansion (pre-polish)
- **Phase 12 — Honeycomb Gates + Checkpoints**
- **Phase 13 — Progressive Difficulty + World Variety** — **river narrowing/widening/branches via streamed authored chunks**, **island obstacles (pooled scrolling hazards)**, rarer Bloom Orbs, faster/denser enemies, faster baseline scroll — tied to Gates passed. *(Islands may land earlier, reusing the Phase 8 Spawner.)*
- **Phase 14 — Full Enemy Roster** (one type at a time; grow into ScriptableObject enemy defs + spawn tables here)

### Polish
- **Phase 15 — Juice** (particles, screen shake, hit-stop, audio, menu animation, upgraded lives/jar effects)
- **Phase 16 — Final Art + Transitions**
- **Phase 17 — Ship** (WebGL build → `update-HoneyRiverRun.bat` → itch.io)

---

*End of GDD & Roadmap v4 — LOCKED.*
