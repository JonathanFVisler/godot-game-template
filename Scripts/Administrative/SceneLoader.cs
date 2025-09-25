using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SceneLoader : Node2D
{
    // Godot friendly Singleton
    public static SceneLoader Instance
    {
        get;
        private set;
    }

    private SceneLoader() { }
    
    public override void _EnterTree()
    {
        if (Instance != null && Instance != this)
        {
            QueueFree();
            return;
        }
        Instance = this;
    }

    public override void _ExitTree()
    {
        if (Instance == this) Instance = null;
    }

    // -----------------------------------------------------------------------

    [Export] private Godot.Collections.Array<SceneEntry> sceneEntries = new();
    private Dictionary<GameScene, PackedScene> sceneDict;
    private Dictionary<GameScene, bool> sceneExist;
    private Node2D[] loadedScenes = new Node2D[0];

    public override void _Ready()
    {
        sceneDict = new Dictionary<GameScene, PackedScene>();
        sceneExist = new Dictionary<GameScene, bool>();
        foreach (var entry in sceneEntries)
        {
            if (!sceneDict.ContainsKey(entry.SceneType))
            {
                sceneDict.Add(entry.SceneType, entry.Scene);
            }
        }
        LoadSceneAdditive(GameScene.MAINMENU);
    }

    public void LoadScene(GameScene sceneType)
    {
        UnloadAllScenes();
        LoadSceneAdditive(sceneType);
    }

    public void LoadSceneAdditive(GameScene sceneType)
    {
        if (sceneExist.TryGetValue(sceneType, out var exists) && exists)
        {
            GD.PrintErr($"Scene {sceneType} already exists in the scene tree.");
            return;
        }

        if (sceneDict.TryGetValue(sceneType, out var packedScene))
        {
            var instance = packedScene.Instantiate();
            instance.Name = sceneType.ToString();
            GetTree().Root.CallDeferred(Node.MethodName.AddChild, instance);
            sceneExist[sceneType] = true;
            GD.Print("Got to here");
            loadedScenes = loadedScenes.Append(instance as Node2D).ToArray();
            GD.Print($"Loaded scene: {sceneType}");
        }
        else
        {
            GD.PrintErr($"No scene bound for {sceneType}");
        }
    }

    public void UnloadScene(GameScene sceneType)
    {
        if (sceneExist.TryGetValue(sceneType, out var exists) && !exists)
        {
            GD.PrintErr($"Scene {sceneType} does not exist in the scene tree.");
            return;
        }

        foreach (var scene in loadedScenes)
        {
            if (scene.Name == sceneType.ToString())
            {
                scene.QueueFree();
                loadedScenes = Array.FindAll(loadedScenes, s => s != scene);
                sceneExist[sceneType] = false;
                GD.Print($"Unloaded scene: {sceneType}");
                return;
            }
        }

        GD.PrintErr($"Scene {sceneType} not found among loaded scenes.");
    }

    public void UnloadAllScenes()
    {
        foreach (var scene in loadedScenes)
        {
            scene.QueueFree();
        }
        loadedScenes = Array.Empty<Node2D>();
        sceneExist.Clear();
        GD.Print("Unloaded all scenes.");
    }

    public void UnloadAllScenesExcept(GameScene sceneType)
    {
        foreach (var scene in loadedScenes.ToList())
        {
            if (scene.Name != sceneType.ToString())
            {
                scene.QueueFree();
                loadedScenes = Array.FindAll(loadedScenes, s => s != scene);
                if (Enum.TryParse<GameScene>(scene.Name, out var parsedScene))
                {
                    sceneExist[parsedScene] = false;
                }
            }
        }
        GD.Print($"Unloaded all scenes except: {sceneType}");
    }

    public void ReloadScene(GameScene sceneType)
    {
        UnloadScene(sceneType);
        LoadSceneAdditive(sceneType);
        GD.Print($"Reloaded scene: {sceneType}");
    }

    public bool IsSceneLoaded(GameScene sceneType)
    {
        return sceneExist.TryGetValue(sceneType, out var exists) && exists;
    }
}