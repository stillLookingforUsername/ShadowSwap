# Phone controls and smoother movement

The existing movement speed, jump force, air control, coyote time, jump buffer,
swap safety/cooldown, shadow timer, scoring, inventory rules, and level assets
are preserved. Player rendering now interpolates physics steps, camera follow
uses frame-rate-independent smoothing, and jumping/rope forces run in
`FixedUpdate`. Movement keeps its original acceleration response at the
project's 0.02-second physics step.

## Use the default phone controls

Android/iOS gameplay scenes get a touch Canvas automatically when they contain
an active `PlayerMovement2D`. Menu and finish scenes do not get gameplay buttons.
Phones use a 60 FPS rendering target and landscape autorotation; physics remains
at its existing rate. Actual FPS depends on the device and scene rendering cost.

- **Left/Right:** hold to move, including swinging on an attached rope.
- **Jump:** tap to jump; also releases an attached rope, matching Space.
- **Swap:** tap to invoke the existing swap ability.
- **Grab:** attach inside a `RopeGrabPoint` trigger (legacy rope tags also work).
- **Up/Down:** hold to climb an attached rope.
- **Small Box/Big Box:** use the existing inventory spawn actions.
- **Pause/Resume:** toggle the existing pause menu.

Rope and inventory buttons are hidden in scenes without their corresponding
player components. Pause requires an active `PauseMenu`. Holding one direction
while tapping Jump or Swap works with separate fingers. Opposite directions
cancel each other. Releasing a finger or dragging outside its button stops that
button's input. Holding Jump does not auto-repeat.

Controls respect the screen safe area and resize with the Canvas. App switches,
screen locks, and focus loss clear held input. On phones, interruptions pause
through the scene's `PauseMenu`; the player explicitly resumes when returning.
Pause and scene changes also clear pending jump and movement input.

## Customize the buttons in Unity

1. Open a gameplay scene.
2. Choose **GameObject > ShadowSwap > Mobile Controls**.
3. Expand **Mobile Controls > Safe Area > Gameplay Buttons**.
4. Move/resize the button RectTransforms and change their Images/text.
5. Save the scene. Repeat for each level, or save the Canvas as a prefab and add
   it to each gameplay scene.

The menu creates the layout as editable scene objects and supports Undo.
**Preview In Editor** on `MobileControlsUI` shows the controls in Play Mode.
The automatically generated phone layout is skipped when the scene already
contains `MobileControlsUI` or `MobileControlButton`, including inactive objects.
Keep a complete custom layout active in phone gameplay scenes.

For your own UI, add **MobileControlButton** to a raycastable UI Image or Button,
then choose its **Control** in the Inspector. It handles press, release, multi-touch,
and cancellation itself. Leave the Button's **On Click** empty for gameplay
actions to avoid dispatching twice; set **Transition = None** for the component's
pressed tint and **Navigation = None** to avoid keyboard submit activating it.
Labels should have **Raycast Target** disabled. Use a Canvas with a
GraphicRaycaster and an EventSystem with **InputSystemUIInputModule**; the
existing level EventSystems already use that module. A new EventSystem is
created only when none is present.

Keyboard bindings: A/D, Space, E (Swap), F (Grab), W/S, 1/2, Escape.
Swap and Grab now have separate keyboard keys as well as separate phone buttons.
The Move action declares Axis to match the float input read by the movement code;
Unity regenerates its input wrapper from that asset.

## Verify before your Android release

The scripts were compiled against the installed Unity 6000.3.26f1 assemblies.
A headless Unity Editor run passed 42 native checks covering independent touch
owners, multiple fingers on one button, release/exit/reset cancellation, pause
and resume, movement acceleration, jump events, and swap safety, cooldown, and
momentum. These were automated Editor checks, not a full level playthrough or
an Android device test. Android Build Support is not installed in this Editor,
so no APK or App Bundle was produced.

In Unity Play Mode, check each level with the editable layout. Then test an
Android development build on an actual phone:

- Hold Left/Right while tapping Jump and Swap with another finger.
- Hold a button with two fingers; releasing one must keep it held.
- Drag off a movement button; movement must stop.
- Pause while moving, resume, restart, and change levels; no input should stick.
- Switch apps or lock the phone while in the shadow lane; its timer must remain
  paused until Resume.
- Check both landscape orientations, screen cutouts, and different aspect ratios.
- Compare jumping, swapping, box placement, and level outcomes with keyboard play.
- Check rope grab, climb, and release in Level 3. The tag mismatch is repaired
  with a `RopeGrabPoint` component. Level 3's RopeAnchor is currently authored
  inactive; enable it when you want to include the rope in that level.
- Profile on the phone to confirm the 60 FPS target is sustainable.

These changes supply phone input and movement smoothing. Building/signing an
Android App Bundle and submitting to Google Play remain separate release steps.

Implementation references: [Unity Rigidbody2D interpolation](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Rigidbody2D-interpolation.html),
[Unity mobile frame rate](https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Application-targetFrameRate.html).
