# ShadowSwap: gameplay improvements and idea backlog

Prepared on 8 October 2026 from the project in this repository. Companion document: [Google Play release and monetization plan](PLAYSTORE_RELEASE_AND_MONETIZATION_PLAN.md).

## 1. Recommended direction

Build ShadowSwap around one promise: **a clever swap turns an impossible jump into a satisfying escape.** Make the player understand both worlds, anticipate danger, and feel skilled when chaining jumps and swaps. Short puzzle-platforming levels with optional mastery challenges suit the existing mechanics and a phone session.

The first priorities should be responsive touch controls, reliable checkpoints, clear swap feedback, a playable tutorial, and replayable level results. Then expand combinations of mechanics you already have. A new enemy or power-up should give players a new reason to swap.

These are design proposals, not implemented features. This review read all 51 custom scripts under `Assets/Scripts`, the input assets, serialized scene script references and selected scene settings, prefabs, packages, project settings, and the README. It did not run Unity or play an Android build. Visual quality, actual difficulty, scene wiring, and device performance need playtesting. Script counts in scenes describe references, not proof that every feature works or is active.

## 2. What is already in the project

| Existing foundation | Evidence | How to build on it |
| --- | --- | --- |
| Two worlds with instant swapping | `ShadowSwap.cs`; all three gameplay scenes use a 150-unit horizontal offset, 0.15-second cooldown, and 5-second shadow limit | Create puzzles that require predicting the corresponding position in the other world |
| Preview ghost and swap status | `ShadowSwap.cs` checks overlap and colors the ghost and indicator | Add visible destination information and a reason for blocked swaps |
| Platforming forgiveness | `PlayerMovement2D.cs` already has coyote time, jump buffering, and air control | Tune these for thumbs; add variable jump height after testing |
| Health and checkpoints | `PlayerHealth.cs`, `CheckPoint.cs`, `HealthUI.cs` | Standardize deaths and restore a complete checkpoint state |
| Cross-world switches | `LaneButton.cs`, `MovingPlatform.cs` | Show a switch powering a platform in the other world |
| Hazards | Crystals, lasers, retracting spikes, death zones, gravity flips | Introduce readable patterns and combine them with swapping |
| Enemies | `EnemyPatroller2D.cs` is referenced by each gameplay scene; `EnemyOctopus.cs` contains chase/jump behavior | Start with simple enemies that create timing choices; test the octopus separately |
| Collectible and box systems | `Collectible.cs`, `GameManager.cs`, `PlayerInventory.cs`, `PushBox.cs` | Add optional routes, puzzle objects, and earned cosmetics |
| Rope and orb traversal | Rope scripts referenced in Level 3; transport orb scripts referenced in all three levels | Build set-piece moments after fixing input and state interactions |
| Presentation | DOTween indicators, orb glow/particles/audio, Timeline cutscenes, walking/jump effects | Make swapping the most recognizable audiovisual moment |
| Small campaign | Build settings contain MainMenu, Level 1, Level 2, Level 3, FinishScene | Turn these into a polished prototype, then smaller reusable challenge rooms |
| Audio settings | `AudioSettingsUI.cs` saves music/SFX preferences | Extend persistent settings and verify every sound respects them |

The README describes mirrored shadows, but the implementation currently moves between horizontally offset worlds. Use the actual offset-world behavior as the launch design. Treat true mirroring as a separate future mechanic because it changes puzzle rules.

## 3. Fixes that directly affect fun

Priority P0 means fix before wider testing; P1 means build for the first public release; P2 means consider after the core release works.

| Priority | Finding from source | Player impact | Recommended action |
| --- | --- | --- | --- |
| P0 | The gameplay `PlayerInput` references `PlayerInputActions.inputactions`, whose bindings are keyboard-only. Inventory, rope, and pause also read keyboard keys directly | The game cannot rely on a phone screen for its core actions yet | Route every gameplay action through shared input methods; wire touch buttons to those same methods |
| P0 | `RopeBuilder` assigns `RopeGrab`, while `PlayerRope` checks `RopeEnd`; `TagManager.asset` has no custom tags | Rope creation/tag lookup can throw, and grabbing uses a different tag anyway | Use one defined tag or a component identifying a grab point; test Level 3 from a fresh launch |
| P0 | E is bound to swapping and is also read directly for grabbing a rope | One press can trigger conflicting actions | Add a contextual Interact action or explicit priority rules; test interaction and swapping together |
| P0 | `ShadowSwap`, `DeathCollider`, and `EnableDeathTrigger` reload the scene; health/laser deaths call `PlayerHealth.Die()` and teleport | Different hazards erase different amounts of progress | Create one death/retry flow, with explicit rules for checkpoint and puzzle restoration |
| P0 | `Respawn()` only changes position; shadow lane, velocity, gravity, parent, rope attachment, and transport state are not restored there | Players may respawn falling, in the wrong lane, upside down, or attached to something | Save lane with the checkpoint; reset transient movement/interaction state and notify lane/UI listeners |
| P0 | `GameManager` persists score without a clear new-run reset or persistent collectible IDs | Reloaded pickups can increase score repeatedly; totals can carry into a new run | Separate attempt pickups, best per-level collection, and cosmetic currency; commit rewards once |
| P0 | `LaneObject` deactivates its own GameObject and then unsubscribes in `OnDisable` | An object can stop receiving the event needed to return | Keep the listening controller active and toggle a child visual/collider. No direct references to this script were found in the three scene files; check prefab use before relying on it |
| P0 | The swap mask is 1472 in all three levels: Ground_Over, Ground_Shadow, Hazard_Over, NoSwap; Hazard_Shadow is omitted | Some shadow hazards could be marked as a safe destination | Audit actual hazard layers and collision queries; include all intended blockers and distinguish collision safety from landing safety |
| P1 | `CameraFollow` smoothly follows world coordinates across a 150-unit teleport | Swapping may sweep across empty space and consume useful shadow time | Snap to the matching camera position on swap, or follow a logical coordinate shared by both worlds; add a short visual transition |
| P1 | Swapping checks an overlap at the target transform, without explicitly handling collider offset, rotated bounds, future landing, or rope state | A green indicator may be misleading in edge cases | Validate the actual destination collider shape; show that an unblocked position can still be over a pit; define rules for swapping while carried or attached |
| P1 | Gravity flips invert the existing value and start a coroutine for any Rigidbody2D entering | Re-entry or a death during a flip can leave unexpected state | Store a baseline and expiration, define affected object types, prevent uncontrolled stacking, and restore on retry |
| P1 | Ground detection and jump velocity assume downward gravity/upward jumping | Upside-down platforming may not behave as expected | Define movement, jump, ground checking, and sprite orientation relative to the current gravity direction |
| P1 | Movement continues to write velocity while the rope system also changes velocity/adds force | Swinging and climbing may fight the normal controller | Use explicit Normal, Rope, Orb, Cutscene, Dead, and Paused movement states |
| P1 | `ArrowIndicator` subscribes an orb event directly to its `OnDestroy` method, never unsubscribes, and calls Destroy inside that method | Hints have a fragile lifetime and can leave stale callbacks | Separate hint completion from lifecycle cleanup; unsubscribe and kill all relevant tweens safely |
| P1 | `PlayerAudio.HandleWalkSound(bool walking)` never assigns its argument to `isWalking` | Walking sound remains silent with the default field value | Use the supplied walking state; vary footsteps slightly and respect the SFX setting |
| P1 | Direct `PlayerHealth.Die()` resets health without publishing the health event | Hearts can show old health after laser deaths | Publish the restored state through the unified retry flow |
| P1 | `MusicManager` returns on duplicate instead of destroying it; audio paths differ between orb and global SFX | Duplicate objects or sounds may ignore expected settings | Ensure one persistent manager; route music, SFX, and ambience through controlled audio groups |
| P1 | `ExitDoor` uses build index + 1; `requiredScore` is not used to gate it | Completion rules are unclear and future scene ordering is fragile | Use explicit level IDs and result screens; keep collection optional unless a puzzle clearly requires a key |

The Move action declares an expected Vector2 but uses a 1D composite, while scripts read floats. Make the action and consumers agree before adding joystick/gamepad support. The default `InputSystem_Actions` asset contains other bindings, but gameplay scenes use the custom asset; its UI touch bindings do not establish mobile gameplay controls.

## 4. First-session design

Aim for the player to perform a successful, meaningful swap within their first minute. This is a design target to test, not a measured result.

| Sequence | What the player does | What they learn |
| --- | --- | --- |
| 1. Safe approach | Move and jump over a forgiving gap | Controls, jump distance, recovery |
| 2. Impossible wall | See the destination preview beyond a wall and tap Swap | The two worlds have different layouts |
| 3. Return route | Cross a shadow bridge and return to solid ground | Swapping preserves corresponding position |
| 4. Time pressure | Cross one short timed section with clear return points | The shadow timer matters |
| 5. Optional reward | Take a slightly harder route for a collectible | Exploration and mastery are optional |
| 6. Finish | See results, Replay, and Next | Progress is remembered and improvement is rewarded |

Use a compact instruction near the relevant button. Stop repeating it after success. Allow safe retries without ads or lengthy cutscenes. Introduce ropes, boxes, and gravity in later rooms instead of teaching every mechanic immediately.

## 5. Ideas that strengthen swapping

Effort estimates are relative: S = small isolated change, M = several interacting changes, L = substantial new mechanics/content. They are planning estimates, not delivery promises.

| ID | Idea and concrete example | Why it can be fun | Priority / effort |
| --- | --- | --- | --- |
| G01 | Show a small window of the destination world around the ghost or near the swap button | Players plan rather than guess | P1 / M |
| G02 | Display Ready, Blocked, and Cooling Down with distinct shapes/icons and short text | Success and failure become understandable without relying on red/green | P1 / S |
| G03 | Give a blocked swap a soft sound and brief outline of the obstruction | Failure teaches the rule without feeling like ignored input | P1 / S |
| G04 | Add a shadow timer ring, final-second pulse, and adjustable warning sound | Urgency is readable while watching the level | P1 / S |
| G05 | Make launch timer length configurable per room; offer a free relaxed mode | Beginners can learn while experts keep a demanding challenge | P1 / S |
| G06 | Preserve momentum during successful swaps and visibly demonstrate a jump-swap landing | Skilled combinations feel fluid | P1 / M |
| G07 | Add a short input buffer for swapping, only if the destination remains safe when it executes | Near-correct taps feel responsive; avoid surprise delayed teleports | P2 / M |
| G08 | Put floating rest islands in the overworld between timed shadow sections | Players alternate planning and execution | P1 / S |
| G09 | Add limited shadow refill crystals on a route, after the basic timer is understood | Creates route and timing choices | P2 / M |
| G10 | Place a visible collectible across a wall that only shadow travel can bypass | The mechanic creates an immediate reason to explore | P1 / S |
| G11 | Add one-way world gates with a clear icon and a visible return route | Makes the player plan sequence, not just position | P2 / M |
| G12 | Introduce crumbling bridges with an equivalent safe surface in the other world | Rewards fast recognition and well-timed swaps | P2 / M |
| G13 | Add a brief bridge activated by arriving in the other world | Arrival becomes part of the puzzle | P2 / M |
| G14 | Use connected light cables between a button and its opposite-world platform | Players can read cause and effect across worlds | P1 / S |
| G15 | Prototype shadow-anchoring: place the ghost at a marked anchor and move away before swapping | Adds a new spatial puzzle; needs separate tutorial and controls | P2 / L |
| G16 | Prototype a recorded echo that repeats the last few seconds and holds a switch | Creates cooperation with your past movement | P2 / L |
| G17 | Prototype mirrored rooms as a clearly signposted later chapter | Refreshes spatial reasoning after offset swapping is mastered | P2 / L |

Choose G01-G06, G08, G10, and G14 first. Prototype one of G15-G17 only after testers enjoy the basic campaign.

## 6. Movement, mobile comfort, and accessibility

| ID | Idea | Implementation/design notes | Priority / effort |
| --- | --- | --- | --- |
| G18 | Large left/right controls plus separate Jump and Swap buttons | Support moving, jumping, and swapping with multiple touches; no lost release events | P0 / M |
| G19 | Optional joystick layout and adjustable button positions | Keep left/right buttons as the initial precision option; test joystick dead zones | P2 / M |
| G20 | Contextual Interact button | Appears near rope/box actions; avoids adding permanent buttons for every mechanic | P1 / M |
| G21 | Jump height depends on hold duration | A quick tap clears a small obstacle; holding reaches a high platform | P1 / M |
| G22 | Landing feedback | Small dust puff, optional vibration, soft landing sound; do not obscure hazards | P1 / S |
| G23 | Immediate checkpoint retry with a short effect | Target roughly one second from failure to regained control; measure on device | P1 / M |
| G24 | Camera look-ahead and bounds | Show the next landing; snap appropriately across worlds; prevent off-screen required jumps | P1 / M |
| G25 | Reduced-motion and vibration settings | Reduce shake/flashes and disable haptics; leave puzzle information intact | P1 / S |
| G26 | High-contrast world markers and scalable UI | Give worlds different silhouettes/patterns as well as colors; respect phone safe areas | P1 / M |
| G27 | Clear death explanation | Briefly show Laser, Shadow time ran out, or Fell; use icons where possible | P1 / S |
| G28 | Free graduated hints | After repeated failures, first highlight a route, then show the interaction needed | P1 / M |
| G29 | Short skippable story beats | Let players skip repeat cutscenes on retries; keep full input disabled while a cutscene owns control | P1 / M |

Tune coyote time and jump buffering already present before adding double jump, dash, or wall jump. Those abilities change what obstacles swapping is needed to solve.

## 7. Puzzles, hazards, and enemy variety

| ID | Idea and example | Design rule | Priority / effort |
| --- | --- | --- | --- |
| G30 | Shadow-only patroller | Swap to avoid it, then return beyond its route | Teach the patrol path before adding time pressure | P1 / S |
| G31 | Laser warning beam | A dim beam/audio cue precedes the damaging beam | Give enough warning for a thumb response | P1 / M |
| G32 | Synchronized spike rhythm | A safe lane alternates with a dangerous lane | Make the pattern stable and visible before the player enters | P2 / M |
| G33 | Pressure plate and box | Place a box in one world to power a bridge in the other | Provide a local reset if the box becomes inaccessible | P1 / M |
| G34 | Box size trade-off | Small box fits a tunnel; big box reaches a higher ledge | Show the difference through level geometry | P2 / M |
| G35 | Directional box placement preview | Spawn/drop in the facing direction only when space is clear | Never consume the box for an invalid placement | P1 / M |
| G36 | Optional gravity route | Flip to walk a ceiling and swap to a corresponding upper platform | Teach gravity-aware jumping first; make the normal route completable | P2 / M |
| G37 | Swing-and-swap room | Leave a rope at the peak and swap onto a distant ledge | Fix rope state and provide a generous first landing | P2 / M |
| G38 | Orb ride with a planned exit | See a landing and deliberately release/swap at the destination | Define transport and swapping ownership before designing around it | P2 / M |
| G39 | Sound-sensitive shadow creature | It reacts to landing or a marked switch | Show its alert radius; avoid invisible punishment | P2 / L |
| G40 | Guardian encounter | A large enemy attacks one world; swap to activate the opposite switch | Three readable phases can reuse lasers/platforms instead of adding combat | P2 / L |
| G41 | Light beam puzzle | Move a box to block a beam and open a shadow gate | Keep beam collision and visual paths consistent | P2 / L |
| G42 | Two solution paths | A cautious timing route and a difficult momentum route reach the same exit | Reward creativity and support different skill levels | P1 / M |

Build hazards as puzzles with visible counterplay. Surprise deaths can get a laugh once, but repeated hidden traps make learning and replay less enjoyable.

## 8. Concrete level concepts

These are proposed rooms, not descriptions of the current level layouts. Target around 45-120 seconds for a successful room, with extra time for learning and optional routes. Adjust using playtest observations.

| Room | Main mechanic | Setup, solution, and optional challenge |
| --- | --- | --- |
| 1. The Other Side | Basic swap | Overworld wall, shadow opening, then a safe return pad. Optional pickup needs one extra swap |
| 2. Missing Steps | Platform correspondence | Stair platforms alternate worlds. Finish safely; mastery requires a clean jump-swap chain |
| 3. Borrowed Time | Shadow timer | Short shadow bridge ends beside a return island. Optional route needs a tighter timing margin |
| 4. Across the Wire | Cross-world switch | Touch a switch, see its cable light up, then use the other world's activated platform |
| 5. Crossing Guard | Patroller | Observe one enemy route from safety and swap behind it. Optional collectible needs waiting for a second cycle |
| 6. Laser Lesson | Telegraphing | A warning beam teaches the cycle; later beams alternate safe worlds |
| 7. Box of Tricks | Box pressure plate | Place one box on a plate to create a bridge. Reset button restores the box if misplaced |
| 8. Ceiling Detour | Gravity | Flip in a safe chamber, reach the ceiling, and swap onto an upper route. Include a checkpoint before the combination |
| 9. Swing Between | Rope momentum | Grab a rope, build swing, release, and swap into an offset landing. Practice swing in safety first |
| 10. Light Ferry | Orb | Ride an orb to a clear landing. Optional pickup is on an alternate exit timing; retry restores the orb |
| 11. Fractured Bridge | Combined traversal | Moving platforms plus a shadow timer; rest islands divide the sequence |
| 12. Gatekeeper | Chapter finale | Reuse switches, patrollers, and a laser cycle in three stages; checkpoint between stages |

For every room, write the intended learning goal, failure recovery, expected first-clear time, optional reward location, and whether its challenge remains fair on a small screen. Introduce one new rule, practice it, then combine it with a familiar rule.

## 9. Progression and reasons to replay

| ID | Idea | Rules that keep it rewarding | Priority / effort |
| --- | --- | --- | --- |
| G43 | Level select with saved unlocks | Show completion and best results; let players revisit without replaying the whole campaign | P1 / M |
| G44 | Three independent medals | Clear, find the secret, beat a target time. Earn across separate attempts | P1 / M |
| G45 | Personal best times | Show improvement and a Replay shortcut; separate assisted runs from standard records | P1 / S |
| G46 | Optional no-death or low-swap challenges | Use extra badges rather than blocking campaign progress | P2 / S |
| G47 | Cosmetic shadow trails | Unlock through unique collectibles or milestones; keep hazard visibility readable | P1 / M |
| G48 | Small lore fragments | Optional notes explain why the shadow exists without interrupting movement | P2 / S |
| G49 | Secret room per chapter | Reward observation of the other world with a small puzzle and cosmetic token | P2 / M |
| G50 | Chapter theme changes | Ruins, crystal cavern, clockwork chamber, inverted garden; each adds one mechanical identity | P2 / L |
| G51 | Challenge rotation from authored rooms | Initially bundle a set of challenge rooms; no server required | P2 / M |
| G52 | Personal ghost replay | Race your previous best locally; recorded inputs/positions need handling for moving objects | P2 / L |
| G53 | Achievements with specific feats | Examples: first midair swap, all secrets in a chapter, three clean rooms | P2 / M |
| G54 | Shareable result image | Let the player choose to share level/time/medals; do not send messages automatically | P2 / M |
| G55 | Curated remix mode | Rearrange tested room segments with guaranteed solvability; label seeds and challenge rules | P2 / L |
| G56 | Community level editor | A long-term option with validation, moderation, hosting, and discovery costs | P2 / very large |

Keep basic retries unlimited. Cosmetic currency should not require replaying the same pickup after every death. Campaign completion should depend on solving rooms, with mastery goals giving a reason to return.

## 10. Presentation ideas

1. Give the overworld and shadow world distinct palettes, lighting, ambience, and platform edge patterns. Keep the character and hazards readable in both.
2. Give swapping a recognizable sound, short outline flash, and restrained particle trail. Let players reduce flashes and vibration.
3. Keep a consistent visual language: danger edges, interactable outlines, checkpoint symbols, and cross-world connection cables.
4. Animate checkpoints lighting up and briefly show a saved icon; only celebrate the first activation.
5. Use the existing orb glow as a guide to important interactions; avoid making all decorations glow equally.
6. Add a results screen that shows actual improvements before offering Replay or Next. Keep collectible accounting clear.
7. Choose one pickup name. The source mixes coins and cookies; the story can justify either, but UI terminology should agree.
8. Add background layers with subtle parallax and a low-effects option. Profile lighting and particles on phones.
9. Build a tiny story around a shadow that helps the character recover pieces of a broken world. Tell it through rooms and optional notes.
10. Give each chapter a satisfying reveal: a restored gate, newly lit environment, or a shadow companion gesture.

## 11. Suggested first public version

Treat the following as a proposed scope, not a platform requirement:

- 12 carefully tested short rooms, adapted from the existing three levels where practical.
- A hands-on tutorial covering movement, swapping, destination reading, and the timer.
- Reliable checkpoints and one retry flow for all deaths.
- Touch controls for every required action, safe-area UI, a pause button, and background/resume handling.
- Saved level unlocks, medals, best times, settings, and collectible accounting.
- A results screen, optional secrets, and a small earned cosmetic set.
- One coherent art/audio identity and free assist settings.
- Optional monetization only after the free loop is enjoyable; details are in the companion plan.

Defer multiplayer, procedural campaigns, accounts/cloud saves, subscriptions, a level editor, and multiple complex new powers. Each creates substantial work beyond a small campaign and is easier to judge after launch data.

## 12. Practical implementation order

| Stage | Work | Completion check |
| --- | --- | --- |
| A. Reliable prototype | Fix rope tags/input conflicts, unify retry state, validate swap masks, settle scoring | Repeated deaths, swaps, ropes, gravity, and orb exits leave a controllable player and consistent score |
| B. Phone-ready core | Shared input actions, touch controls, safe areas, camera transition, pause/resume | A tester completes the prototype on a phone without a keyboard |
| C. Understandable mechanic | Tutorial, destination information, timer warning, failure reasons | New testers explain why a swap is safe or blocked and complete the first room unaided |
| D. Progress and replay | Save model, level select, results, medals, secrets | Closing/reopening restores progress; replay never duplicates banked rewards |
| E. Content | Author short rooms, introduce mechanics gradually, add one polished finale | Every room is solvable, recoverable, and fun with touch controls |
| F. Release polish | Audio consistency, accessibility, performance, store preparation | Device testing and the release checklist in the companion plan pass |

Useful future components: a player state controller, checkpoint snapshot, level-definition asset, save service with schema version, results controller, and input adapter. Keep content values such as timer length and medal targets in level data. Prefer these focused systems over one manager that owns everything.

## 13. Playtest method and success signals

Run the first tests with 5-10 people who have not seen development. Include a few experienced platformer players and people who play casually on phones. Watch without coaching, then record:

- Time to first meaningful swap and first completed room.
- Moments they could not see where a swap would lead.
- Deaths they describe as unfair or confusing, grouped by cause and room.
- Missed touch inputs, accidental Swap/Interact activation, and button occlusion.
- Whether they voluntarily replay a room or continue after failing.
- Which optional route they noticed and why they took it.
- Whether the timer adds excitement or prevents learning.
- Whether progress survives close/reopen and a restart during an interaction.

Ask: "What did you think would happen there?", "Which part would you play again?", and "What made you stop?" Prioritize repeated observations over one feature suggestion.

Proposed initial goals: most testers finish the introductory room without explanation, no known unrecoverable puzzle state, retry restores control quickly, and testers can distinguish the two worlds without color alone. These are quality gates, not measured metrics or industry benchmarks.

## 14. Next ten tasks

- [ ] Fix rope identification and separate grabbing from swapping.
- [ ] Introduce a shared input path and touch controls.
- [ ] Unify death/checkpoint restoration, including lane, physics, gravity, and attachments.
- [ ] Validate swap destination masks and actual hazard layers.
- [ ] Prevent duplicate collectible rewards across reloads and new runs.
- [ ] Fix camera movement during the 150-unit world transfer.
- [ ] Build one excellent tutorial room and test it on a phone.
- [ ] Add destination clarity, blocked reasons, and timer warnings.
- [ ] Save progress and show meaningful level results.
- [ ] Author three additional rooms using existing switches, boxes, and hazards; playtest before expanding further.
