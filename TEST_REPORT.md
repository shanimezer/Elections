# Verification report — 2026-10-08

## Passed
- Unity-independent simulation compiled with Mono C# compiler and executed successfully: **123 assertions passed**. Coverage: 08:00 opening, action energy deduction, insufficient energy, paused time/arrivals, manual activity energy, invalid team rejection, all 108 residents counted once through the model capacity limit, 22:00 closing, rejection of post-close arrivals/actions.
- Runtime and editor C# scripts compiled together against the installed **Unity 2022.3.62f3 engine/editor assemblies and .NET Standard 2.1**: compiler exit 0, no errors or warnings. This is an API/syntax compile check outside the Unity import pipeline.
- Project archive checked for required scene, scripts, package manifest, pinned editor version, matching scene/script identifiers and build scene registration.

## Blocked / untested
- Started Unity in batch mode to import the project and invoke its checks. Unity timed out connecting to the local licensing client after 60 seconds and aborted **before importing the project**. Log excerpt: `IPC channel to LicensingClient doesn't exist; aborting`.
- Consequently Unity import, scene deserialization, Play-mode lifecycle, actual 3D rendering, mouse targeting, UI sizing, bot/path animation and standalone player builds are **not runtime-verified**. No screenshot or successful player build is claimed.
- Original screenshot attachments were unavailable in the retrieved reference conversation; exact visual fidelity remains unverified.
- GPS, real step detection, online multiplayer, persistent totals and mobile controls are not implemented and therefore not tested.

## First editor smoke test
1. Open City.unity and Play at a landscape resolution of at least 1100×720.
2. Choose any team; observe the two other counters rising after residents reach polls.
3. Explore a block, select its house, invite a resident and watch that house's remaining count fall and its team score rise on arrival.
4. Pause: verify clock, bots and walkers freeze. Resume and test WASD/scroll camera controls.
5. Enable mock activity and check energy increases by up to 20; check 15-second cooldown. Disable it and verify passive energy continues.
6. Let 4 minutes elapse: verify 22:00, frozen results and disabled gameplay actions. Restart and verify all counts reset.
7. Run Vote Quest → Run Simulation Checks, then optionally build a player.

`SimulationTests.cs.txt` contains the standalone test source (kept outside Assets to avoid being compiled into the game). Editor checks are in PrototypeTools.cs. Both are included for repeatability.
