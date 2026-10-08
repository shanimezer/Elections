# Vote Quest — playable 3D Unity prototype

## פתיחה מהירה
1. חלצי את קובץ ה־ZIP לתיקייה מקומית.
2. ב־Unity Hub בחרי **Add → Add project from disk** ובחרי בתיקייה VoteQuest (זו שמכילה Assets, Packages ו־ProjectSettings).
3. הפרויקט הקיים מוגדר ל־**Unity 2022.2.18f1**. הקוד נבדק בקומפילציה מול **2022.3.62f3 LTS**; הרצה בגרסה הקיימת טרם נבדקה. פתחי עם הגרסה הקיימת או שדרגי ל־2022.3.62f3 LTS. המתיני לסיום הייבוא הראשוני.
4. פתחי את `Assets/Scenes/City.unity` ולחצי **Play**. העיר נבנית כאשר המשחק מתחיל; הסצנה ריקה מבחינה חזותית מחוץ למצב Play.
5. בחרי קבוצה, לחצי **Explore a block**, לחצי על בית צבעוני שנחשף ואז **Invite a resident**.

הממשק במשחק באנגלית כדי לא להוסיף תלות בטיפול בכתב מימין לשמאל לאבטיפוס. הוראות אלו בעברית.

## Controls and loop
- Choose Coral, Azure or Amber. The other two teams get equal automated assistance every 3 simulation seconds.
- Click a revealed house to inspect it. Invite costs 8 energy and sends one resident to a nearby fictional polling hall. Affiliation never changes; inviting another team's residents helps that team and the shared goal.
- Explore costs 10 energy and reveals the nearest unknown block relative to the selected house (or town center).
- Energy refills at 1.8/second, so activity mode is entirely optional.
- WASD / arrows pan; mouse wheel zooms; Space or Pause freezes the clock, bots, residents and activity cooldown.
- Optional activity mock: manually simulate 100 steps for 20 energy, with a 15-second simulation cooldown. No sensor, GPS, step detection or verification is used.
- 240 seconds represent 08:00–22:00. Arrivals count until closing; residents still travelling at closing do not count. Restart resets the entire city.
- Goal: 76 of 108 virtual residents (at least 70%). Scores remain visible throughout; no tie-breaker is necessary for the shared goal.

## Implemented
18 colored houses, six residents per house, three fictional affiliations, three polls, visible walking capsule residents, road-based waypoint routes, two team bots, orthographic camera, lighting and placeholder trees/buildings, opaque block fog, selection and actions, live team counters and turnout, energy, optional manual activity mock, pause/restart and final results.

The shared goal aggregates all three teams **inside one local session**. It is not online multiplayer or a persistent community total. The fog hides blocks with solid stylized volumes; it is not a shader-based line-of-sight system.

## Source / extend
- `Assets/Scripts/Simulation.cs`: Unity-independent clock, energy and vote rules.
- `Assets/Scripts/CityGame.cs`: procedural city, navigation, bots, interaction and immediate-mode UI.
- `Assets/Editor/PrototypeTools.cs`: scene creation, checks and macOS build menu.
- `Assets/Scenes/City.unity`: ready-to-open entry scene.

All 3D art uses Unity primitives with built-in Standard materials. No paid assets or external code packages. UI uses Unity's built-in IMGUI. Legacy keyboard/mouse input is expected; if migrating to an Input System project, set Active Input Handling to Both.

## Verification / building
Use **Vote Quest → Run Simulation Checks** in the Unity menu. Success prints `VOTEQUEST_CHECKS_PASSED` and writes `verification.txt`. This regenerates City.unity; save any custom scene edits separately first.
Use **Vote Quest → Build macOS Player** for `Builds/VoteQuest.app`, or File → Build Settings for another installed target. Editor licence and platform build support are required. No standalone binary is included.
See TEST_REPORT.md for checks actually performed in this environment and remaining verification.

## Scope and reference fidelity
The referenced conversation was retrieved, but contained no accessible screenshot attachments. This is an interpretation of the described small city, three colors, residents, polls and fog; exact visual matching to Michael's original screenshots has not been verified.

Fictional, politically neutral simulation. No real party names, personal data, network requests, location collection, real voting claims, voting rewards or polling-place check-ins. Manual activity is independent of voting and may be used without leaving home. Real GPS, real election scheduling, civic-information links, online shared totals, mobile/touch support, accessibility polish and production art are future work, not implemented integrations.
