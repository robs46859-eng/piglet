# PIGLET — The Crash

Unity 6 (6000.6.0f1) scene built from the supplied Tripo3D FBX assets. Open this folder in Unity Hub, open `Assets/Scenes/TheCrash.unity`, then press Play.

The SS Acorn has crashed in a woodland clearing. Piglet approaches Bear, they meet, and Blue, Red, and Purple chase them. After the sequence, use WASD or arrow keys to move, Tab to switch between Piglet and Bear, and R to replay.

## Correct model and animation setup

- Import the five animated characters as **Generic** rigs with **Avatar Definition: No Avatar** and **Import Animation** enabled. Their clips bind directly to the `Armature` bone hierarchy. Creating an Avatar prevented bone movement in this project's Play mode check.
- Enable **Loop Time** on the imported clips used by the scene.
- Put an **Animator** and the matching controller on each character's root object. Keep **Apply Root Motion** off; `PigletOpening.cs` moves the roots along the map while the clips pose the bones.
- Rotate each root to face its target. The models' visual forward direction is local **+Z**. Piglet and Bear face each other at the meeting; the enemies face the pair. Do not align every actor to the camera.

| Character in scene | Supplied ZIP | Embedded clips | Scene use |
|---|---|---|---|
| Piglet | `pig+character+3d+model.zip` | `agree.001`, `angry_02.001` | Agree at meeting; angry when enemies appear |
| Bear | `winnie.zip` | `angry_02.001` | Angry when enemies appear |
| Blue | `blue+rhinoceros+figure+3d+model.zip` | `wait.001`, `turn.001`, `swagger.001`, `angry_02.001` | Wait, turn, threaten, then swagger during pursuit |
| Red | `bull+character+3d+model.zip` | `greet_01.001`, `defeat_03.001`, `angry_01.001`, `angry_02.001` | Greet, threaten, chase |
| Purple | `cute+purple+bull+3d+model.zip` | `idle.001`, `look_around.001`, `greet_02.001`, `angry_02.001` | Idle, look around, threaten, chase |

The supplied `redbull.zip` and `purpbull.zip` contain static meshes with no embedded clips. The scene uses the animated red and purple bull versions listed above. The characters do not have dedicated walk or run clips; pursuit uses scripted movement, and Piglet and Bear return to their neutral poses while running. This is a playable opening scene prototype, not a complete RPG movement or combat system.

`Preview.png` shows the opening position. `Meeting.png`, `Chase.png`, and `Escape.png` were captured during Play mode. To regenerate the scene, choose **PIGLET > Build Opening Scene** in Unity. That command overwrites `Assets/Scenes/TheCrash.unity`.
