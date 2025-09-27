using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

[Tool]
public partial class SceneLoader : Node2D
{
    // Godot friendly Singleton
    private static SceneLoader _instance;
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
        if (Engine.IsEditorHint()) { return; }
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
        if (Engine.IsEditorHint()) { return; }
        UnloadAllScenes();
        LoadSceneAdditive(sceneType);
    }

    public void LoadSceneAdditive(GameScene sceneType)
    {
        if (Engine.IsEditorHint()) { return; }
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
        if (Engine.IsEditorHint()) { return; }
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
        if (Engine.IsEditorHint()) { return; }
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
        if (Engine.IsEditorHint()) { return; }
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
        if (Engine.IsEditorHint()) { return; }
        UnloadScene(sceneType);
        LoadSceneAdditive(sceneType);
        GD.Print($"Reloaded scene: {sceneType}");
    }

    public bool IsSceneLoaded(GameScene sceneType)
    {
        if (Engine.IsEditorHint()) { return false; }
        return sceneExist.TryGetValue(sceneType, out var exists) && exists;
    }


    //-------------------------------------------------------
    // In Engine fuctionality
    //-------------------------------------------------------
    [ExportSubgroup("Editor Tools")]
    [Export] public string gameSceneToAdd;
    [ExportToolButton("Update GameScene Enum")]
    public Callable Button => new Callable(this, nameof(UpdateGameSceneEnum));

    public void UpdateGameSceneEnum()
    {
        if (Engine.IsEditorHint())
        {
            GameSceneEditorTools.UpdateGameSceneEnum(gameSceneToAdd);
        }
    }
}

public partial class GameSceneEditorTools : EditorScript
{
    public static void UpdateGameSceneEnum(string gameSceneToAdd)
    {
        if(!Engine.IsEditorHint())
        {
            GD.PrintErr("UpdateGameSceneEnum can only be run in the editor.");
            return;
        }

        if (string.IsNullOrWhiteSpace(gameSceneToAdd))
        { GD.PrintErr("Game scene name cannot be empty."); return; }

        string enumFilePath = "Scripts/Data/GameScene.cs";
        if (!File.Exists(enumFilePath))
        { GD.PrintErr($"File not found: {enumFilePath}"); return; }

        var lines = File.ReadAllLines(enumFilePath).ToList();

        // 1) Find the enum declaration line
        int enumDeclIndex = lines.FindIndex(l => l.Contains("public enum GameScene"));
        if (enumDeclIndex == -1)
        { GD.PrintErr("Could not find 'public enum GameScene' in GameScene.cs"); return; }

        // 2) Find the opening brace '{' for the enum (may be on same line or a following line)
        int openIdx = -1, openCharCol = -1;
        for (int i = enumDeclIndex; i < lines.Count; i++)
        {
            int col = lines[i].IndexOf('{');
            if (col >= 0) { openIdx = i; openCharCol = col; break; }
        }
        if (openIdx == -1)
        { GD.PrintErr("Could not find opening '{' for GameScene enum."); return; }

        // 3) Find the matching closing brace '}' using brace depth
        int depth = 0, closeIdx = -1;
        for (int i = openIdx; i < lines.Count; i++)
        {
            // Walk characters to avoid matching braces in comments/strings too aggressively.
            // (Lightweight heuristic: ignore braces after '//' on a line)
            string line = lines[i];
            int commentPos = line.IndexOf("//");
            string scanSegment = commentPos >= 0 ? line.Substring(0, commentPos) : line;

            for (int c = 0; c < scanSegment.Length; c++)
            {
                if (scanSegment[c] == '{') depth++;
                else if (scanSegment[c] == '}')
                {
                    depth--;
                    if (depth == 0) { closeIdx = i; break; }
                }
            }
            if (closeIdx != -1) break;
        }
        if (closeIdx == -1)
        { GD.PrintErr("Could not find closing '}' for GameScene enum."); return; }

        // 4) Prepare the sanitized member name
        string sanitisedSceneName = gameSceneToAdd.Trim().ToUpper().Replace(" ", "_");
        if (string.IsNullOrEmpty(sanitisedSceneName) ||
            (!char.IsLetter(sanitisedSceneName[0]) && sanitisedSceneName[0] != '_'))
        {
            GD.PrintErr("Scene name must start with a letter or underscore.");
            return;
        }

        // 5) Collect existing member lines between braces (exclusive)
        int membersStart = openIdx + 1;
        int membersEnd = closeIdx - 1;

        // Helper to get the "token" name of a member line: strip comments, comma, initializer
        string MemberNameFromLine(string rawLine)
        {
            int commentPos = rawLine.IndexOf("//");
            string code = (commentPos >= 0 ? rawLine[..commentPos] : rawLine).Trim();
            if (code.Length == 0) return string.Empty;
            code = code.TrimEnd(',');
            int eq = code.IndexOf('=');
            if (eq >= 0) code = code[..eq].Trim();
            // If the line starts with attributes or something weird, ignore it
            // Valid enum member should be an identifier
            // We'll do a quick check:
            var m = Regex.Match(code, @"^[_\p{L}][_\p{L}\p{Nd}]*$");
            return m.Success ? code : string.Empty;
        }

        // 6) Check for duplicate inside the enum block only
        for (int i = membersStart; i <= membersEnd; i++)
        {
            string name = MemberNameFromLine(lines[i]);
            if (name == sanitisedSceneName)
            {
                GD.PrintErr($"GameScene enum already contains an entry for {gameSceneToAdd}");
                return;
            }
        }

        // 7) Determine indentation for members
        string GuessIndent()
        {
            // Use first non-empty member's leading whitespace, else use 4 spaces more than enum line
            for (int i = membersStart; i <= membersEnd; i++)
            {
                var s = lines[i];
                if (string.IsNullOrWhiteSpace(s)) continue;
                var m = Regex.Match(s, @"^\s+");
                if (m.Success) return m.Value;
            }
            // fallback: indent one level deeper than the line with '{'
            string openLine = lines[openIdx];
            string baseIndent = Regex.Match(openLine, @"^\s*").Value;
            return baseIndent + "    ";
        }
        string indent = GuessIndent();

        // 8) Ensure the last actual member ends with a comma (preserve comment & indent)
        int lastMemberLineIndex = -1;
        for (int i = membersEnd; i >= membersStart; i--)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            // Skip pure comment lines
            if (Regex.IsMatch(lines[i].TrimStart(), @"^//")) continue;

            // If this line yields a valid member name, treat as last member
            if (!string.IsNullOrEmpty(MemberNameFromLine(lines[i])))
            {
                lastMemberLineIndex = i;
                break;
            }
        }

        if (lastMemberLineIndex != -1)
        {
            string original = lines[lastMemberLineIndex];

            // Split off inline comment
            int commentPos = original.IndexOf("//");
            string beforeComment = commentPos >= 0 ? original[..commentPos] : original;
            string comment = commentPos >= 0 ? original[commentPos..] : string.Empty;

            // Preserve leading whitespace
            var leadMatch = Regex.Match(beforeComment, @"^\s*");
            string leading = leadMatch.Value;
            string rest = beforeComment[leading.Length..];

            // Remove trailing spaces and any existing comma(s)
            rest = rest.TrimEnd();
            bool hasComma = rest.EndsWith(",");
            if (!hasComma)
            {
                // Also remove extra trailing commas/spaces just in case
                rest = Regex.Replace(rest, @",\s*$", "");
                // Rebuild line
                lines[lastMemberLineIndex] = leading + rest + "," + (comment.Length > 0 ? " " + comment : string.Empty);
            }
        }

        // 9) Insert the new member just before the closing brace line
        string newEntry = $"{indent}{sanitisedSceneName},";
        lines.Insert(closeIdx, newEntry);

        // 10) Write back
        File.WriteAllLines(enumFilePath, lines);
        GD.Print($"Added {sanitisedSceneName} to GameScene enum.");
        gameSceneToAdd = "";

        // 11) Rebuild the project to refresh the enum in the editor
        ForceCSharpBuild();
    }

    
    private static void ForceCSharpBuild()
    {
        // Project root (res://) and your .csproj
        var root = ProjectSettings.GlobalizePath("res://");
        var csproj = Directory.GetFiles(root, "*.csproj").FirstOrDefault();
        if (csproj == null)
        {
            GD.PrintErr("Could not find a .csproj at project root.");
            return;
        }

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{csproj}\" -v:m",
                WorkingDirectory = root,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            using var p = Process.Start(psi);
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();

            GD.Print(stdout);
            if (!string.IsNullOrWhiteSpace(stderr))
                GD.PrintErr(stderr);

            if (p.ExitCode != 0)
            {
                GD.PrintErr($"dotnet build failed with exit code {p.ExitCode}");
            }
            else
            {
                // Nudge the editor to pick up the new assemblies
                EditorInterface.Singleton.GetResourceFilesystem().Scan();
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to run dotnet build: {ex.Message}");
        }
    }
}