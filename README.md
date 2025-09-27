# godot-game-template
A simple template to build upon, containing the bare essentials

This project contains the following:
 - A simple main menu
 - A game scene loading system to avoid recursive refrencing when loading game scenes
 - A settings system with the following settings
   - Master volume
   - Music volume
   - Sound volume
   - Windowed/Fullscreen
   - Screen Resolutions (16:9 resoulutions)
 - Persistance of settings

### Adding a new loadable game scene
1: Add scene name to the enum in res://Scripts/Data/GameScene.cs

2: Open MainScene.tscn found at res://Scenes/Administritive/MainScene.tscn

3: Select MainScene in the hirachy 

![alt text](Tutorial/image-0.png)

4: In the Inspector under SceneLoader, add a new element to the Scene Entries array

![alt text](Tutorial/image-1.png)

5: Add a new Scene Entry in the new element

6: Open the new Scene Entry

7: Set Scene Type to the scene name added in step 1

8: Drag the related .tscn file into the Scene variable
