# ShadowSwap: Google Play release and monetization plan

Prepared on **8 October 2026**. Companion document: [Gameplay improvements and ideas](GAMEPLAY_IDEAS.md).

This is a plan for the existing Unity project, not a completed release. No Android build, Unity playtest, ad account, billing integration, or Play Console account was verified during this review. Official policy sources were checked for this document; check them again immediately before submission because requirements change.

## 1. Recommended business approach

Launch a small, polished free puzzle-platformer whose appeal is swapping between parallel worlds. Make the tutorial and normal retries free. Add optional rewarded ads for extra cosmetic rewards or an optional detailed hint, and a clearly defined one-time supporter purchase. Consider paid chapter packs after players finish and enjoy the initial campaign.

Your first commercial goal should be evidence that people understand the mechanic, return to play, and complete levels. Income depends on active players, geography, ad availability, retention, and purchase conversion. Publishing alone does not produce dependable income. Use small experiments to learn before committing a large marketing budget.

Proposed launch scope: around 12 short rooms, one tutorial, level select, reliable saves/checkpoints, optional medals/secrets, and a small cosmetic collection. This is a design recommendation, not a Google Play requirement.

## 2. Current project readiness

| Area | What the repository currently shows | Work needed |
| --- | --- | --- |
| Engine | Unity `6000.2.7f2` in `ProjectVersion.txt` | Check current supported patches and release notes, back up, then test any upgrade |
| Campaign | MainMenu, three gameplay scenes, FinishScene in build settings | Define a polished launch campaign and reliable final/replay navigation |
| Android platform | Android settings exist; an installed toolchain/build profile was not verified | Install Android Build Support and validate a real device build |
| Android identity | `DefaultCompany`; identifier block only shows a Standalone template identifier | Set a permanent Android package identifier and developer identity |
| Target/minimum API | Target value `0` means automatic selection; minimum is `23` | Explicitly verify target API compliance; choose minimum based on SDK support and device tests |
| Architecture/backend | Android architecture value `2` and backend `1`, consistent with ARM64 and IL2CPP | Confirm these in the Android build profile and inspect the produced bundle |
| Signing | Keystore and alias fields are empty | Create and securely back up the upload key; configure Play App Signing |
| Inputs | Gameplay scenes use the keyboard-bound custom input asset; several scripts also read keys directly | Implement touch for every required action and multi-touch |
| Persistence | Audio preferences use PlayerPrefs; no campaign save system was found in custom scripts | Save unlocks, results, cosmetics, and checkpoint policy; keep paid entitlements restorable |
| Monetization | No ads or IAP integration found in custom scripts/packages; no `.aar`/`.so`/`.jar` plugins found in the Assets scan | Integrate and test an ad SDK and billing solution only after choosing products |
| Reliability | Death paths differ; rope tags disagree; swap masks omit Hazard_Shadow; score persists without clear replay rules | Complete the P0 fixes in the gameplay document |
| Store preparation | No store release checklist or policy documents were found in the reviewed Markdown files | Prepare listing, privacy policy, app declarations, support contact, and testing evidence |

The absence of plugin binaries under Assets does not mean the final Unity build has no native libraries. Unity/IL2CPP and future SDKs make bundle compatibility checks necessary.

## 3. Current publishing requirements that affect this game

### Developer account

Play Console registration requires an adult account owner, a one-time US$25 registration fee, and identity verification. Choose Personal or Organization according to your actual situation. New personal accounts also have device verification requirements. Use the instructions shown in your account rather than guessing which documents are required. [Google: Get started with Play Console](https://support.google.com/googleplay/android-developer/answer/6112435?hl=en).

### Testing for new personal accounts

For personal accounts created after 13 November 2023, Google requires at least **12 testers continuously opted into a closed test for 14 days** before applying for production access. An internal test does not substitute for this requirement. Approval is a separate review; meeting the elapsed-time minimum alone does not guarantee acceptance. Open testing is available after production access. [Google: App testing requirements](https://support.google.com/googleplay/android-developer/answer/14151465).

### Android target API

Since 31 August 2026, new phone apps and updates must target **Android 16 / API 36 or higher**. ShadowSwap should explicitly verify its generated target SDK against this requirement; automatic selection is insufficient evidence. Target API and minimum supported API are different settings. [Google: Target API requirements](https://support.google.com/googleplay/android-developer/answer/11926878).

### Native library compatibility

The 16 KB page-size requirement applies to new apps and updates targeting Android 15+ from 1 November 2025. Verify the actual release and every native SDK; an engine version alone is not proof of compatibility. [Android Developers: Play 16 KB requirement](https://android-developers.googleblog.com/2025/05/prepare-play-apps-for-devices-with-16kb-page-size.html).

### Bundle and signing

Publish this new game as an **Android App Bundle (`.aab`)**, with Play App Signing configured. The upload key authenticates uploaded releases; the app signing key signs packages delivered to players. Back up the upload keystore and keep its passwords out of source control. [Android: App Bundles](https://developer.android.com/guide/app-bundle), [Google: Play App Signing](https://support.google.com/googleplay/android-developer/answer/9842756?hl=en-GB).

### Billing compatibility

If adding purchases, use a supported billing integration. Billing Library 7 reached its normal new-app/update deadline on 31 August 2026; plan for **Billing Library 8 or newer**, and verify the version actually bundled by the chosen Unity IAP/package release. Do not build a new launch around an extension. [Android: Billing deprecation schedule](https://developer.android.com/google/play/billing/deprecation-faq).

## 4. Development phases and estimated schedule

These are rough solo-developer planning ranges. They depend on available hours, skill, existing level quality, and test results. Some preparation can overlap; production-access and app reviews add uncertain time.

| Phase | Suggested range | Deliverable | Gate before moving on |
| --- | --- | --- | --- |
| 1. Stabilize the prototype | 1-2 weeks | Consistent player/death/input state and working core interactions | Every existing scene can be completed without blocking bugs |
| 2. Mobile controls and onboarding | 1-2 weeks | Touch controls, phone layout, tutorial, swap/camera feedback | Fresh players can learn and finish a room on a real phone |
| 3. Saves and launch content | 2-4 weeks | Level select, results, medals, secrets, around 12 rooms | Progress survives close/reopen and replay rewards are correct |
| 4. Monetization and declarations | 1-2 weeks | Tested optional ads, defined IAP, policy and listing drafts | Offline play works; rewards/purchases recover correctly |
| 5. Internal/closed testing | 2-3+ weeks | Device feedback, fixes, testing record | Quality gates and applicable account testing requirements satisfied |
| 6. Limited launch and review | Variable | Approved production release in selected markets | Stable operation and acceptable player feedback |
| 7. Wider launch and iteration | Ongoing | Better onboarding, content, store conversion | Expansion supported by retention and technical data |

Start developer verification and tester recruitment early. Avoid announcing a fixed public launch date until the build and review process are ready.

## 5. Make the Unity project ready for phones

### Gameplay and UX checklist

- [ ] Complete the P0 fixes in the companion document: rope identification, input conflicts, swap masks, death restoration, and reward accounting.
- [ ] Use one shared action path for keyboard, controller, and on-screen controls.
- [ ] Support holding movement while tapping Jump and Swap with other fingers.
- [ ] Add a visible Pause button and deliberate handling for Android Back.
- [ ] Clear held inputs on focus loss; pause gameplay/timers when appropriate and require deliberate resume.
- [ ] Ensure an ad, app switch, call interruption, or screen lock does not drain the shadow timer unfairly.
- [ ] Test standard landscape and reverse landscape on phones; choose orientation deliberately.
- [ ] Respect `Screen.safeArea`, display cutouts, gesture navigation, and multiple aspect ratios.
- [ ] Save after meaningful progress and on application suspension; do not depend only on quit callbacks.
- [ ] Add Continue, level select, results, Replay, and a sensible final-scene return path.
- [ ] Confirm every required action is usable without a keyboard, including rope climbing/release and box placement.
- [ ] Add free assist, reduced motion, vibration, and independent audio settings.

### Save design

Store a schema version, unlocked level IDs, best results per level, collected secret IDs, owned earned cosmetics, current selection, and settings. Write robustly, handle missing/corrupt saves with a safe recovery, and test migration before updates. Decide whether Continue resumes a level from its start or restores a checkpoint, then describe that behavior accurately.

Maintain separate values for temporary attempt pickups and banked currency. A level restart must not duplicate banked rewards. Paid entitlements should be recoverable from the billing provider, with local caching for offline use after verification. Plain PlayerPrefs flags should not be the sole proof of a purchase.

### Performance goals

Suggested internal goals, not official platform thresholds:

- Aim for stable 60 fps on the main target devices; offer a consistent lower setting when needed.
- Profile longer sessions on a modest phone, checking thermal slowdown and battery use.
- Target a small initial download where practical; measure the Play-generated download instead of only the local AAB size.
- Pool frequently spawned effects/objects, limit 2D lights and particles, and compress textures/audio appropriately.
- Check shader variants, unused assets, and package/native SDK contributions before optimizing blindly.
- Remove excessive development logging from release paths and turn off Development Build/Script Debugging.

Use the Unity Profiler and Android logs to identify actual bottlenecks. A 2D game can still be expensive when lighting, overdraw, physics, or duplicated managers are uncontrolled.

## 6. Android build procedure

1. Back up the project and record the Unity/package versions used for the release. Keep unrelated editor changes intact.
2. Install the compatible Android Build Support toolchain through Unity Hub, including the recommended SDK/NDK/OpenJDK components.
3. Create or confirm the Android Build Profile. Check scenes and required assets against the intended campaign.
4. Set your real developer/company name, ShadowSwap product name, and a permanent Android identifier such as `com.yourstudio.shadowswap`. Replace the example with an identifier you own/control.
5. Set a visible version such as `1.0.0`, and a strictly increasing Android version code for each upload.
6. Set/verify target API 36 or higher using compatible tooling. Choose minimum API based on the engine and all SDK requirements, then test that oldest supported OS.
7. Confirm IL2CPP, ARM64, graphics settings, texture compression choices, icons, orientation, and safe-area behavior.
8. Generate an upload keystore, store two secure backups, and document recovery access privately. Configure signing without committing credentials.
9. Build an AAB with release settings. Keep debug symbols needed to diagnose release crashes.
10. Inspect the bundle's generated manifest, permissions, target SDK, architectures, version code, and native libraries. Check 16 KB support and install behavior on an appropriate test environment.
11. Upload to an internal testing track, install from Play, and test those delivered packages. A locally installed APK does not validate the whole Play distribution path.
12. Repeat the main regression checks after adding ads/IAP, upgrading SDKs, or changing the Unity version.

For 16 KB verification, use the official guide's bundle/ELF alignment checks and a 16 KB emulator/device. `adb shell getconf PAGE_SIZE` should return `16384` in that environment. Verify both packaging and runtime, including ad and billing screens. Follow engine-supported build configurations when updating toolchains. [Android: Support 16 KB page sizes](https://developer.android.com/guide/practices/page-sizes).

## 7. Monetization options and recommendation

| Model | Fit for ShadowSwap | Trade-off | Recommendation |
| --- | --- | --- | --- |
| Optional rewarded ads | Extra cosmetic tokens or a detailed optional hint | Revenue requires ad engagement; loading/consent adds work | Start with one or two carefully placed opportunities |
| Forced interstitials | Natural breaks after completed rooms | Can interrupt replay and reduce retention | Start without them; test only after observing the core experience |
| Banners | Menu or result areas | Small-screen clutter and accidental touch risk | Skip in the first release |
| One-time supporter purchase | Cosmetic bundle and clearly defined perks | Needs billing, restoration, and useful perceived value | Good small initial product |
| Paid chapter packs | More authored puzzles after the free campaign | Requires substantial quality content and demand | Strong candidate after launch |
| Paid upfront game | Complete premium campaign | Harder to attract players without reputation/demo | Alternative if you choose a premium identity |
| Subscription | Continuous content/service | Creates an ongoing content promise | Poor initial fit for this small campaign |
| Selling retries/timer advantage | Can monetize frustration | Undermines learning and fair mastery | Keep ordinary retries and assist options free |

For Play-distributed digital items such as cosmetics, chapters, and ad removal, plan to use Google Play Billing. Regional alternative payment programs have specific eligibility and terms; the initial plan should use the standard billing path. [Google: Payments policy](https://support.google.com/googleplay/android-developer/answer/9858738).

### Suggested initial offers

These are experiment prices, not observed market rates or guaranteed optimal prices. Set local prices in Console and show the provider's localized price string in-game.

| Offer | Proposed content | Illustrative price to test |
| --- | --- | --- |
| Free campaign | Tutorial, normal retries, around 12 rooms, earnable cosmetics | Free |
| Supporter pack | Exclusive cosmetic set; optional ad rewards replaceable by a clearly stated free claim if included | Try an India-local range of INR 99-199 or separately test US$1.99-2.99 in the US |
| Future chapter | A substantial set of additional polished rooms, for example 8-12 | Test local pricing after proving demand |

If launching with rewarded ads only, do not sell a vague "Remove ads" product. Explain exactly what purchase does. If it also replaces optional rewarded ads, define limits and rewards precisely so players never need an ad for an advertised paid benefit. If later adding forced ads, disclose which ad formats removal affects.

### Rewarded ad placements

| Placement | Clear player offer | Free behavior | Guardrail |
| --- | --- | --- | --- |
| Results screen | Watch an ad for extra cosmetic tokens | Keep the normal earned result | Bonus granted once per eligible result |
| Optional hint screen | Watch an ad for a detailed route demonstration | Basic hints/assist remain available | Avoid making level completion require ad availability |
| Cosmetic preview | Watch an ad for a temporary cosmetic trial | Keep currently owned cosmetic | Show duration and do not misrepresent permanent ownership |

Checkpoint revival is a poor first ad placement because ordinary death is part of learning. Keep recovery free rather than making players wait through an ad each time they miss a jump.

## 8. Ads integration and operations

AdMob is one possible first provider. Use the current compatible Google Mobile Ads Unity plugin and avoid integrating several networks before there is enough traffic to assess benefit.

### Implementation checklist

- [ ] Create your ad account and Android app entry; distinguish the app ID from individual ad unit IDs.
- [ ] Install a compatible SDK and resolve Android dependencies; record SDK versions.
- [ ] Configure privacy/consent handling before requesting ads when required.
- [ ] Start with rewarded ads at an intentional screen chosen by the player.
- [ ] Maintain states for unavailable, loading, ready, showing, closed, and failed.
- [ ] Pause gameplay and audio appropriately; restore the prior state on close/failure/focus changes.
- [ ] Grant a reward through the SDK's earned-reward callback, once per offer; do not grant merely because a screen closed.
- [ ] Handle callbacks on the appropriate Unity thread, using the chosen SDK's current guidance.
- [ ] Clear/destroy used ad objects and retry load failures with bounded backoff.
- [ ] Keep gameplay and navigation usable with no network, no fill, or blocked ads.

The rewarded SDK guide describes loading, checking availability, earned-reward callbacks, and cleaning up one-use ad instances. [Google: Rewarded ads for Unity](https://developers.google.com/admob/unity/rewarded).

Use test ad units and configured test devices during development. Never click live ads to test them or ask testers to generate revenue by clicking. Verify the release configuration carefully before replacing test units. [Google: Enable test ads](https://developers.google.com/admob/unity/test-ads).

Use the User Messaging Platform or another appropriate supported consent solution for the markets served. In Google's UMP flow, refresh consent information at launch, show required forms, check whether ads can be requested, and expose privacy options where required. Do not treat a saved boolean as permanent permission. [Google: UMP for Unity](https://developers.google.com/admob/unity/privacy).

### AdMob app verification

New apps set up in AdMob require app-ads.txt verification and app readiness approval to fully serve ads. Host the supplied publisher line at `/app-ads.txt` on the developer website associated with the store listing, ensure it is crawlable, link the exact Play app, and verify status in AdMob. Budget time for verification/review; test ads working does not establish live monetization readiness. [Google: Verify an app with app-ads.txt](https://support.google.com/admob/answer/14538460?hl=en).

Do not place unexpected full-screen ads during a jump, at level start, or after tapping an unrelated control. Evaluate any future interstitial only at a predictable natural break with conservative frequency. [Google: Ads policy](https://support.google.com/googleplay/android-developer/answer/9857753).

## 9. Purchases and entitlement handling

Use a supported Unity IAP integration or another well-maintained Play Billing integration. Verify its actual Android Billing Library version before submission.

1. Create stable product IDs, for example `supporter_pack` and later `chapter_02`, with matching IDs in the game and Play Console.
2. Choose non-consumable products for permanent cosmetics/chapter access. Avoid consumable currencies for the first release to reduce complexity.
3. Retrieve product metadata and display localized prices from billing, not hardcoded currency text.
4. Treat initiated, pending, completed, canceled, and failed purchases differently. Grant entitlement only for a verified completed purchase.
5. Validate receipts/tokens using the integration's supported approach. For stronger purchase protection and refund synchronization, plan a small server-side entitlement service and include its operating cost. [Android: Purchase security](https://developer.android.com/google/play/billing/security).
6. Acknowledge completed purchases promptly; an unacknowledged purchase is automatically refunded after three days. Make processing idempotent so reconnects do not duplicate rewards. Follow the current provider workflow rather than writing an ad hoc callback chain. [Android: Purchase acknowledgement](https://developer.android.com/google/play/billing/security).
7. Query/restore owned non-consumables after reinstall, returning online, or changing device. Offer a visible Restore/Check purchases action where useful.
8. Handle refunded/revoked products through appropriate purchase queries/server notifications and define how cached offline access is reconciled. [Android: One-time purchase lifecycle](https://developer.android.com/google/play/billing/lifecycle/one-time).
9. Test through Play with license testers, including cancellation, pending payment, interruption, reinstall, and restore.

Keep campaign progress separate from billing entitlements. A corrupted local save should not permanently remove a purchased chapter. Never use a purchase to bypass a broken puzzle or inaccessible control.

Technical reference: [Android: Integrate Google Play Billing](https://developer.android.com/google/play/billing/integrate). Recheck that guide and the chosen integration's documentation during implementation; no purchase system was added in this review.

## 10. Privacy, audience, assets, and app declarations

### Accurate audience and privacy

Choose the audience based on the game's actual art, marketing, content, and intended players. If children are in the target audience, apply the Families requirements, including appropriate SDKs and ad handling. Selecting an older age group to avoid the rules does not change who the game is actually designed for. [Google: Families policy](https://support.google.com/googleplay/android-developer/answer/9893335).

Prepare a public privacy policy and an in-game link. Explain the actual developer/contact, data use, SDKs/recipients, retention/deletion, and user options. Even apps that do not collect personal data need a privacy policy under Google's policy. Adding accounts later brings additional account-deletion obligations. [Google: User data policy](https://support.google.com/googleplay/android-developer/answer/10144311?hl=en-gb).

Complete Data safety using an inventory of the SDKs and actual configuration. Gameplay that works offline can still send data through ads, analytics, or crash reporting. Review each provider's disclosures and your own events; update the form when integrations change. [Google: Data safety](https://support.google.com/googleplay/android-developer/answer/10787469).

### Asset/license audit

The included `Assets/kenney_platformer-art-extended-tileset/license.txt` explicitly permits commercial use under CC0. Keep that license record. It does not establish rights for every sprite, font, song, sound effect, or plugin in the project.

Before release, inventory each asset's author, original source, license, download/purchase record, required credit, and permission for commercial games. Pay particular attention to the MP3/WAV files, GoogleFonts files, other textures/sprites, and third-party code such as DOTween. Retain required notices. Check the current Unity license terms for your usage and eligibility before budgeting; do not assume engine/editor costs from this plan.

### Console declarations

- [ ] Ads declaration matches the release, including rewarded ads.
- [ ] Data safety and privacy policy match all included SDKs.
- [ ] Target audience and content rating questionnaire are accurate.
- [ ] App access explains the reviewer path; supply working credentials only if login is introduced.
- [ ] Permission declarations match the final merged manifest, including permissions added by SDKs.
- [ ] Pricing, purchase terms, supported countries, and distribution settings are deliberate.
- [ ] Support email is monitored, and all required links are public and functional.

Use the current app dashboard as the source of truth for required forms. [Google: Create and set up an app](https://support.google.com/googleplay/android-developer/answer/9859152).

## 11. Store listing and promotional assets

Prepare a recognizable split-world icon, a feature graphic that shows the swap concept, and real gameplay screenshots. Current specifications include a **512 x 512** 32-bit PNG app icon (up to 1024 KB), a **1024 x 500** JPEG or 24-bit PNG feature graphic without alpha, and a short description of at most **80 characters**. Check screenshot requirements for each supported device type; prepare 4-8 good phone screenshots as a marketing target. [Google: Preview asset guidelines](https://support.google.com/googleplay/android-developer/answer/9866151?hl=en-en).

Proposed listing text, to edit against the features actually shipped:

**Title:** ShadowSwap

**Short description:** Swap between worlds, solve puzzles, and escape before your shadow fades

**Full-description draft:**

> A wall blocks your path. A bridge exists only in the shadows. One swap changes the way forward.
>
> ShadowSwap is a 2D puzzle-platformer about moving between parallel worlds. Read the route, time your jumps, and return before your shadow time runs out.
>
> Explore handcrafted rooms, activate switches across worlds, discover optional secrets, and replay for better results.

Add the shipped monetization terms and accurate connectivity requirements. Use "offline campaign" only after verifying that gameplay remains available without ads, network, or login. Do not list planned bosses, cosmetics, level counts, accessibility features, or chapters as if already shipped.

### Screenshot sequence

1. Show a clear swap solving an otherwise impossible obstacle.
2. Show the visual difference between corresponding worlds.
3. Show a cross-world switch/platform puzzle.
4. Show a distinctive timed or gravity section, if polished and shipped.
5. Show secrets, level selection, or mastery results.
6. Show the touch layout on actual phone gameplay.

Create a 15-30 second optional trailer using real gameplay: opening obstacle, successful swap, two puzzle variations, then a clear ending. Keep the central mechanic visible immediately. This duration and sequence are creative recommendations.

## 12. Testing plan and production access

### Internal testing

Use an internal track for rapid checks with trusted testers. Install the Play-delivered build, then test fresh start, every room, retries, saves, ads, and purchases. Read pre-launch reports and resolve actionable crashes, ANRs, and device/layout issues before wider testing.

### Closed testing

Recruit 15-20 willing testers as a practical buffer, with a mix of phones and skill levels. Give them a clear opt-in/install guide, a feedback channel, and several test sessions across the period. Keep a dated record of reported issues, device/version, fixes, and retest outcomes. Follow your account's applicable continuous opt-in requirement and use real feedback when answering the production-access application. This recruitment target is a recommendation, not a second platform minimum.

### Test matrix

| Area | Cases | Expected behavior |
| --- | --- | --- |
| Controls | Multiple touches, sliding off buttons, quick alternating inputs | No stuck movement or ignored critical action |
| World state | Swap on edge, on platform, near wall, near both hazard layers | Correct destination and honest feedback |
| Death/retry | Falling, laser, damage, timer expiry; death while upside down/carried/attached | Consistent restored checkpoint and controllable player |
| Puzzle recovery | Lose a box, move platform, take optional route | Reset/retry offers a valid solution |
| Persistence | Force-close after completion, background during save, corrupt/missing save | Progress restores safely; currency is not duplicated |
| Navigation | Pause, Back, restart, replay, final scene | No frozen timescale or invalid scene load |
| Network | Offline, slow network, connection loss, ad no-fill | Campaign works; no permanent spinner |
| Ads | No ad ready, failed show, earned reward, duplicate/delayed callback | Reward follows confirmed eligibility and is granted once |
| Billing | Completed, pending, canceled, interrupted, restore, refund handling | Correct entitlement and clear player feedback |
| Devices | Modest phone, current phone, oldest supported OS, latest OS, varied screen sizes | Playable layout and acceptable measured performance |
| Compatibility | Play-installed AAB, ARM64, 16 KB environment | Installs, launches, and exercises SDKs correctly |
| Long session | 20-30 minutes, repeated scene reloads, lock/resume | No accumulating managers, broken audio, or unacceptable slowdown |

Do not ask testers for positive public reviews or ad clicks. For game feedback, ask what confused them, where they quit, and which room they replayed voluntarily.

## 13. Publishing steps

1. Complete developer verification and create the app in Play Console with the intended language and game category.
2. Enter store text/assets, contact details, policy links, and app declarations.
3. Configure Play App Signing; upload the signed release AAB to internal testing.
4. Fix build warnings and test installation through Play. Inspect pre-launch reports and app bundle details.
5. Publish the closed test to the chosen tester group; collect and act on feedback.
6. Apply for production access when your account's requirements are satisfied. Keep the testing record available.
7. If approved, prepare a production release with accurate notes and selected launch countries. Review warnings and send the release for review.
8. Use a limited geographic first launch if appropriate for your audience and support capacity. Include devices representative of future markets; one country's ad revenue is not a global forecast.
9. Confirm the public listing, actual install, purchases, ad verification, support links, and analytics after release.
10. For subsequent updates, increment version code, regression-test, and use staged rollouts where available. If a regression appears, halt expansion and issue a corrected higher-version build; plan recovery before launching.

A first production release has different rollout options from later updates. Check the options Console actually offers rather than assuming a new app can begin with a 5% rollout.

## 14. Revenue estimates: explicit assumptions

All figures below are hypothetical sensitivity examples, not forecasts, market eCPM benchmarks, or promised earnings. Currency is USD for consistent arithmetic; the India prices above are separate product experiments, not exchange-rate conversions.

### Advertising formula

`Monthly ad revenue = average daily active users x actual impressions per active user per day x days x publisher eCPM / 1,000`

Use actual paid/showed impressions, not ad requests or installs. If estimating from offers instead, also account for player opt-in, fill, and successful show rates. Use publisher-side eCPM so the provider share is already represented; do not subtract it again.

| Scenario | Average DAU | Actual impressions / DAU / day | Assumed publisher eCPM | Days | Illustrative monthly ad revenue |
| --- | --- | --- | --- | --- | --- |
| Small audience | 100 | 0.5 | US$1 | 30 | US$1.50 |
| Growing audience | 1,000 | 1.0 | US$3 | 30 | US$90 |
| Larger audience | 5,000 | 1.5 | US$5 | 30 | US$1,125 |

The table shows why a download count alone is a weak income measure. A short campaign needs replay value, new content, or ongoing discovery to maintain active players. Do not increase intrusive ads simply to reach the assumed impression count.

### Purchase example

Suppose 10,000 new players install in a month, 1% purchase a US$2.99 non-consumable supporter pack, and a 15% service fee applies:

- Buyers: `10,000 x 0.01 = 100`.
- Gross sales: `100 x US$2.99 = US$299`.
- After assumed service fee only: `US$299 x 0.85 = US$254.15`.

This excludes taxes, refunds, regional pricing differences, exchange rates, payment adjustments, and operating/marketing costs. The DAU and monthly-install assumptions above describe different measurements; do not add scenarios together without a consistent player cohort model.

Google's published 15% fee tier applies to enrolled developers' first US$1 million annual revenue under its terms; service fees vary by program and situation. Verify enrollment and actual rates for your account. Ad revenue is a separate monetization stream and is not subject to that Play digital-purchase commission. [Google: Service fees](https://support.google.com/googleplay/android-developer/answer/112622?ctx=topic&hl=en&topic=15867).

### Unit economics before paid marketing

Track net revenue per acquired player over time (LTV), after purchase fees and expected refunds. Compare it with acquisition cost (CPI) for the same country, channel, and cohort. As a purely hypothetical example, paying US$0.50 per install for players who generate US$0.10 net LTV loses money before development costs.

Do not infer lifetime value from one week's revenue without acknowledging the unobserved future. Increase marketing only when retention, technical quality, and measured economics support it.

## 15. Budget and getting paid

| Cost area | Planning approach |
| --- | --- |
| Play registration | US$25 one-time, subject to the signup/payment process |
| Test devices | Use available phones first; consider buying only for a specific coverage gap |
| Art/audio | Audit existing assets; budget for a distinctive icon, missing sounds, or licensed replacements |
| Website | Host privacy/support pages and app-ads.txt on a reliable public site; budget domain/hosting only if needed |
| Engine/tools | Check your actual Unity license eligibility and paid tool needs |
| Backend | Can avoid accounts/leaderboards initially; purchase validation service may introduce a hosting cost |
| Marketing | Start with organic clips; set a small, capped experimental budget after evidence of retention |
| Maintenance | Reserve ongoing time for support, device regressions, SDK/API changes, and content fixes |

A lean cash budget can start with registration and available equipment, but development/testing time is a real cost. Record spending and revenue monthly. Separate gross estimates, finalized earnings, and cash received.

Set up the applicable Google Play payments/merchant profile for paid products and the separate AdMob payment setup for ads. Complete the identity, bank, address, and tax-information steps requested for your account. Check each dashboard's payment thresholds, timing, and holds rather than treating estimated daily revenue as money already available. If operating from India, confirm the correct local business/tax treatment with a qualified adviser; this plan assumes no particular registration or tax status.

## 16. Marketing and discovery

### Before release

- Record short clips where one swap visibly solves a problem. Start with gameplay rather than a long logo animation.
- Build 5-10 variations: impossible wall, last-second return, cross-world switch, secret route, clean speedrun.
- Share progress in relevant communities where promotion is permitted and request specific gameplay feedback.
- Keep a simple developer page with gameplay, contact, privacy policy, and eventually the store link.
- Collect interested testers voluntarily; do not scrape contact lists or send unsolicited messages.
- Ask small creators who cover puzzle/platformer games whether they want to try it; outreach remains your choice and is not performed by this plan.

### After release

- Lead with the game's swap mechanic in screenshots and clips.
- Ask for a review at a natural moment after enjoyable play, using an appropriate review flow. Do not incentivize or screen for positive ratings.
- Read support messages and reviews for repeated issues; prioritize controls, confusing rules, and lost progress.
- Experiment with one store asset at a time when there is enough traffic to interpret results.
- Localize UI and store text for languages you can support well; choose the first expansion using actual audience data.
- Promote a meaningful update with a new room/challenge and a short gameplay clip.

Treat virality and store featuring as upside, not a financial plan. A small audience that enjoys the game gives more useful direction than a large audience that quits during the tutorial.

## 17. Measurement and decision rules

Add a small analytics set only if you can implement and disclose it appropriately. Local tester logs and Play's reports are useful before adding another SDK.

| Measurement | Definition / useful fields | Decision it supports |
| --- | --- | --- |
| Tutorial completion | Completed / started among new players | Whether onboarding works |
| First successful swap | Time from starting the tutorial | Whether the main mechanic appears early enough |
| Level completion | Level ID, attempts, duration, assist mode | Where difficulty or confusion blocks progress |
| Death causes | Level/checkpoint ID and cause | Whether one hazard or timer needs redesign |
| Replay | Voluntary replay after clearing | Whether mastery goals add value |
| Day 1 / Day 7 retention | Fraction of an install cohort returning on that exact later day | Whether players have reason to return |
| Ads | Offers, opt-ins, loaded/showed, rewards, revenue by format/country | Whether an offer is valuable and technically reliable |
| Purchases | Offer views, started/completed/restored, revenue/refunds | Whether pricing and products fit the audience |
| Stability | Crash/ANR reports by build/device | Whether release expansion should stop |
| Store conversion | Listing visitors who install, segmented by acquisition source | Whether the listing communicates the mechanic |

Avoid collecting personal information in event payloads. Review data collection and retention for every analytics/crash SDK before enabling it.

Use cohorts and sample size; 12 closed testers cannot establish reliable monetization rates. Choose internal targets after collecting a baseline. Examples of sensible decisions: simplify the tutorial when new players quit before learning swap; repair an unfair room before selling content; remove an ad placement if it disrupts continuation; pause acquisition when measured net LTV is below CPI.

## 18. Final release gates

- [ ] Core game is fun in fresh-player phone tests, with no known blocking bugs.
- [ ] Every launch level has been cleared on a Play-installed release build.
- [ ] All required touch actions, retry states, and puzzle resets work.
- [ ] Saves survive interruptions and upgrades without duplicate banked rewards.
- [ ] Android target API, ARM64, and 16 KB compatibility are verified.
- [ ] Permanent package ID, version code, icons, signing, and secure key backups are ready.
- [ ] Purchase, restore, pending, cancel, and failure paths pass if IAP ships.
- [ ] Ad reward/failure/offline paths pass if ads ship; consent is configured.
- [ ] Live ad setup and app-ads.txt/readiness status have been checked separately from test ads.
- [ ] Privacy, Data safety, audience, ads, rating, access, and permission declarations match the actual build.
- [ ] Commercial asset rights and required notices are documented.
- [ ] Store claims/screenshots reflect shipped features.
- [ ] Applicable account testing and production-access requirements are satisfied.
- [ ] Support contact, review monitoring, and a corrective-update procedure are ready.
- [ ] Estimated income is separated from finalized payouts and profit.

## 19. First month after release

| Period | Main work | Evidence to review |
| --- | --- | --- |
| Days 1-3 | Check install, gameplay, saves, ads/IAP, links, and crashes | Build-specific reports, support messages, verified purchase/ad behavior |
| Days 4-7 | Fix the largest onboarding/control issues | Tutorial exits, confusing death causes, repeated feedback |
| Week 2 | Improve one weak area and verify the update | Cohort completion/retention and device stability |
| Week 3 | Test one store or monetization change with a small scope | Conversion, continuation, reward reliability, revenue quality |
| Week 4 | Decide whether to add rooms, widen distribution, or continue polishing | Retention, player requests, maintenance load, measured economics |

Prepare the next small content update while supporting the launch, but let observed player problems decide the order. Revisit this checklist before every SDK, policy, or major gameplay update.
