# ShadowSwap: improvements and creative ideas

Updated 14 October 2026. This is the current focused work list. The longer
[gameplay backlog](GAMEPLAY_IDEAS.md) contains additional proposals.
Latest bug fixes and behavior notes: [project audit](PROJECT_AUDIT.md).

## Implemented first: Shadow Sense

Give swapping a recognizable identity and help players understand what is
happening in the other world:

- **Readable swap status:** "Swap ready", "Destination blocked", or "Swap is
  recharging". A rejected press explains why it did not work.
- **Shadow energy:** a shrinking bar and countdown. The last quarter of the
  existing time allowance shows "Return now" and a gentle pulse.
- **Swap echoes:** a short tinted character silhouette stays at the departure
  and arrival points, then fades. Two sprites are reused for every swap.
- **Instant camera transfer:** the camera snaps to the matching position when
  the player changes worlds, then continues its normal smooth following.
- **Reduced motion:** the HUD's **FX On / FX Off** toggle disables echo animation
  and the warning pulse. The preference is saved locally; text warnings remain.

This is presentation feedback. Swapping still uses the existing obstruction
check, cooldown, lane offset, momentum, and shadow-time limit. The timer is
initialized to its configured allowance so an immediate first-frame swap has
the same allowance as later swaps. No new movement ability or reward is added.
"Swap ready" means the existing destination overlap check is clear; it does
**not** promise a safe landing over a pit. The later audit repaired missing
shadow hazard layers and collider-offset handling in that overlap check.

### Try it and customize it

Open any gameplay level and enter Play Mode. Shadow Sense is installed
automatically on desktop and phones, alongside the existing HUD and touch
controls. Menu/finish scenes receive no gameplay overlay.

For an editable layout, choose **GameObject > ShadowSwap > Shadow Sense** in a
gameplay scene. Move/resize the **Shadow Sense HUD > Safe Area > Shadow Sense**
panel and adjust its text/colors. Save that scene or turn the HUD into a prefab.
An existing `ShadowSenseUI`, including an inactive one, suppresses automatic
HUD creation. Use the Inspector's **Swap** field to reference that level's player.

The player's `ShadowSwapEcho` component exposes duration, opacity, and world
colors. It uses the player's existing sprite/material and owns no colliders.
Effects freeze during pause and are cleaned up when their player is destroyed.
The HUD reserves only its FX button for pointer input; labels and the energy
display do not intercept touches. It respects the phone's screen safe area.

The camera transfer is implemented in the existing `CameraFollow`. A future
replacement camera controller would need its own matching transfer behavior.

## Implemented next: Echo Mastery

Give each room optional goals and a reason to replay:

- **Clean Run:** finish a complete attempt without a checkpoint death/respawn.
- **Collector:** collect every authored cookie in the room. The badge is
  unavailable when a room has no cookies; it is not awarded for doing nothing.
- **Shadow Dancer:** perform three successful midair swaps. The departure
  position determines whether a swap is airborne; rejected presses do not count.
- **Live challenges:** a small display below Shadow Sense shows attempt time,
  cookie collection, midair swaps, and clean-run status.
- **Local personal records:** each level remembers its fastest completed time,
  highest cookie count, fewest swaps, completion count, and all earned badges.
  Slower replays keep the better record, and badges accumulate across attempts.
- **Completion screen:** reaching the existing exit displays results and the
  newly earned badges, with **Continue**, **Replay**, and **Main Menu** buttons.
  Continue uses the same next build index as the previous exit behavior.

These goals are optional. They do not unlock movement powers, gate exits, or
award extra score/currency. The existing campaign score remains separate from
per-attempt cookie tracking. The later audit makes an authored cookie count once
per campaign and resets campaign score on a new Play; mastery records persist.
Paid currency/reward accounting remains a separate future task.

### Attempt rules and setup

Enter Play Mode in any saved gameplay level. The tracker and UI are installed
automatically; menu and finish scenes get no challenge overlay. To customize,
choose **GameObject > ShadowSwap > Echo Mastery**. Save the generated layout in
the scene or as a prefab. Assign the UI's **Tracker** to that scene's player.
The player's `LevelRunTracker` exposes the midair-swap target and an optional
permanent **Level Id**. Otherwise records use the scene asset path; renaming or
moving a scene changes that default identity. Use a permanent ID before release
if you plan to reorganize scenes.

Unpaused time in the level counts, including cutscenes. Pausing or backgrounding
through the existing pause menu stops the clock. A checkpoint respawn increases
this attempt's death count. A full scene reload/restart starts a fresh attempt
and restores challenge eligibility; incomplete attempts are not saved as wins.

Cookies are counted from the level's `Collectible` components, including authored
inactive ones. Each counts once per attempt. Runtime-created cookies are not
part of this authored-room challenge. Make sure inactive cookies can become
reachable before asking players to earn Collector. Body collisions, swap safety,
and the authored death/reload policies are unchanged; the later audit repairs
checkpoint state cleanup, ropes, gravity, and swap safety.

The completion screen pauses the world and clears held inputs. The pause menu
cannot resume a completed level underneath it. Navigation restores normal time
and clears inputs before loading; repeated exit triggers commit one completion.
Missing build destinations disable their buttons. Replay requires a scene in
Build Settings. Records are local to the installation, separate for each level,
and use versioned JSON in PlayerPrefs. Malformed records fall back to empty ones.

### Replay reliability fixes

The first Play Mode navigation test exposed existing effects and reference
issues. Looping orb glow/platform movement and the swap UI spin now stop their
tweens when their objects are destroyed or disabled. Orb ghost hints resolve an
omitted player reference from the active player and tolerate missing visuals.
Cross-world buttons accept either a `MovingPlatform` component or its GameObject,
preserving older serialized references such as the one in Level 2. The platform
activation rule and its movement values remain the same; scene files were not
rewritten.

## Prioritized suggestions

| Priority | Suggestion | Why it helps | Status |
| --- | --- | --- | --- |
| First | Shadow Sense feedback and swap echoes | Makes the core mechanic readable and memorable | Implemented in this change |
| First | Phone controls, interruption handling, and smooth physics rendering | Lets players complete levels with their thumbs | Implemented in the previous change; device testing remains |
| First | Repair the existing rope identification mismatch | Grab points should not depend on absent tags | Fixed in the audit; F grabs, E swaps |
| First | Consistent checkpoint/death restoration | Restore lane, gravity, velocity, and attachment state | Player cleanup fixed; puzzle object rollback remains a separate design choice |
| First | Audit swap blockers in every level | Shadow hazards were omitted and collider offsets ignored | Fixed in the audit |
| Next | A short, hands-on swap tutorial | Teach destination reading and the timer before ropes and boxes | Proposed |
| Next | Per-level results and saved best attempts | Gives players a reason to replay without adding grind | Echo Mastery implemented; currency/retry redesign remains separate |
| Next | Clear checkpoint activation feedback | A saved symbol and a brief glow make progress obvious | Proposed |
| Next | Fix and unify audio feedback | Walking state, duplicate managers, and saved volume handling | Fixed in the audit |
| Next | Small, optional secret routes | Lets confident players explore while the main route stays approachable | Proposed |
| Later | Chapter palettes, ambience, and scenery | Gives each set of rooms a distinct mood | Proposed; profile lighting on phones |

## Creative ideas to build next

### 1. Echo mastery challenges

Implemented the three badges, live progress, saved best attempts, and results
screen described above. A calibrated speed badge is a future option after
device playtests establish fair time targets for each room.

### 2. The broken-shadow story

Put optional fragments in unusual routes. Each fragment reveals one small
piece of why the two worlds separated. Completing a chapter restores a visual
detail in the world: a lit bridge, growing plants, or a repaired constellation.
Tell most of the story through scenery rather than long interruptions.

### 3. Paired-world puzzle rooms

Author short rooms where a switch in one world reveals a bridge in the other.
Start with one visible connection, then combine a box, a timed bridge, and an
optional shortcut. Use the existing switch/platform systems as the foundation.

### 4. A personal echo race

After finishing a level, optionally race a translucent recording of the best
attempt. Record positions and lane changes rather than assuming the physics
simulation replays identically. Moving platforms and resets need deliberate
handling before this becomes reliable.

### 5. Shadow postcards

Give each chapter a striking optional vista where both worlds reveal different
halves of the same image. A player-triggered photo/results screen could later
make a shareable image. Sharing must remain a deliberate player action.

### 6. Constellation secrets

Hide optional marked fragments across both worlds. Collecting a chapter's set
reveals a constellation illustration or a cosmetic shadow tint. Keep these
distinct from normal cookies and give each fragment a permanent ID so replay
cannot award it twice. Proposed; no fragments or cosmetics are implemented yet.

### 7. Alternating-world rhythm rooms

Build one authored room where the two lanes alternate between a lit safe path
and a clearly telegraphed hazard pattern. Use generous practice space, visible
countdown cues, and a consistent rhythm before increasing difficulty. Proposed;
this needs level authoring and phone playtests, not just a new timer script.

### 8. Checkpoint companion moments

At the first activation of a checkpoint, let the ghost briefly wave or leave a
small glowing footprint. Pair it with a clear saved marker and optional sound.
Respect FX Off and avoid replaying the celebration on every touch. Proposed.

## Verification and next playtest

Echo Mastery passed **54 native/model checks, 11 Play Mode navigation checks,
and all 86 previous regression checks** (151 total) in Unity 6000.3.26f1. The
checks covered duplicate pickups, badge accumulation, personal-best records,
malformed saves, departure-side airborne detection, repeated completion,
modal pause ownership, the repaired platform binding, and actual Continue,
Replay, and Main Menu transitions using Level 1 and Level 2. The exit callbacks
were invoked by the test harness; these were not full puzzle playthroughs.
Temporary test record keys were deleted. The final navigation run produced no
orb-reference exceptions, platform-reference mismatch, or destroyed-target
tween warnings found in the first run.

On a phone, play each level through to its exit, earn the three badges over
several attempts, relaunch the app to confirm records persist, and check the
challenge panel and result buttons on both landscape orientations. Check that
results stay paused when pressing Pause/Back or switching apps, and that each
navigation button restores a fresh controllable level. Android build and
device testing remain necessary.

The code compiled against Unity 6000.3.26f1. A headless Unity Editor run passed
**44 Shadow Sense checks and all 42 phone-control regression checks**. These
covered success/blocked/cooldown messages, the first-frame timer allowance,
momentum preservation, camera transfers, echo reuse/expiration, reduced motion,
saved preferences, energy geometry, touch blocking, and duplicate/menu-scene
installation. They were native Editor checks with constructed test objects,
not a full level playthrough. Actual visuals and performance still need an
Android device test; no Android build was produced.

During a playtest, check blocked/cooldown presses, repeated rapid swaps, swaps
in midair, pause/resume, and restarting each level. Check the camera instantly
lands in the new lane, echoes disappear after their duration, FX Off preserves
the warning text, the saved preference survives relaunch, and the new panel does
not overlap important HUD elements on narrow landscape screens.

Watch a new player without coaching. Record what they expect a swap to do,
whether they understand a blocked destination, and whether the timer warning
helps them return. Prioritize repeated confusion over adding more mechanics.
