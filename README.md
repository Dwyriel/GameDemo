# GameDemo
A very basic unity game demo, where the inputs comes from a different program, which logs everything that happens.

### GameInput project
The app that hosts the server, controls the game and logs everything that is sent and received between the 2 programs.

### UnityDemo
The game itself, where you control a tank with a turret and must destroy all aircraft before the timer ends.

## Code Structure 

### GameInput (/src)
Consists of the **MainWindow** (Qt GUI, events and user interactions), the **TcpServer** (communicate with the client) and the **Logger** (logs every interaction between server and client).<br>
Global definitions (#define) are found in the **defines.h** file.<br>
**messageStructure.h** represents the possible messages the server can send and receive.<br>
**CustomPushButton** is an extension of Qt's PushButton with a few modifications to its events.

### UnityDemo (/Assets/Scripts)

- #### EnemyScripts Folder
**EnemyScript.cs** is the default enemy script that controls the collisions and added the appropriate move script to its gameObject.<br>
**EnemyMovementBase.cs** is the base class for all movement scripts containing shared attributes and methods.<br>
**EnemyHorizontalScript.cs**, **EnemyCircularScript.cs** and **EnemySinusWaveScript.cs** are all scripts that control the movement of the enemy, using state machine.

- #### Misc Folder
**ConstValuesAndUtility.cs** defines global values and utility functions.<br>
**Structure.cs** defines the classes and structs used to communicate with the TCP Server.<br>
**TcpClientScript.cs** the script that communicates with the server, used as a singleton inside a GameObject.<br>
**ScenarioScript.cs** is a very small script that sets the appropriate tag to every child of the Scenery prefab.<br>
**BulletScript.cs** controls the bullets the player shoots and its collisions.

- #### PlayerScripts Folder
**CameraScript.cs** controls the camera position and rotation, based off on the tanks position and the rotation of many of its child objects.<br>
**PlayerScript.cs** controls the player, reading inputs from a TcpServerScript's static attribute and responding to them accordingly.

- #### SceneManagers Folder
**GameSceneManager.cs** manages the GameScene, spawning enemies, recording the current game stats, updating the UI and telling the server when a game has started or ended.<br>
**IdleSceneManager.cs** manages the IdleScene, which acts as a menu. It waits for a command from the server to start the game and, after receiving it, loads and plays the scene.