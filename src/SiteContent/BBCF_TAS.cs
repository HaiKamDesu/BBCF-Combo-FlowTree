using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

public sealed class BBCF_TAS
{
    public sealed class Config
    {
        // Timings
        public int FramesPerSecond { get; set; } = 60;   // 60fps
        public int KeyTapMs { get; set; } = 40;          // how long a tap is held down
        public int InterStepMs { get; set; } = 30;       // small gap between steps (helps stability)

        // Hotkeys
        public string PlayHotkey { get; set; } = "F6";
        public string StopHotkey { get; set; } = "F7";

        // AHK location (null/empty -> use file association, typically fine)
        public string AutoHotkeyExe { get; set; } = "";

        // Direction keys (keyboard)
        public string KeyUp { get; set; } = "Up";
        public string KeyDown { get; set; } = "Down";
        public string KeyLeft { get; set; } = "Left";
        public string KeyRight { get; set; } = "Right";

        // Attack buttons
        public string KeyA { get; set; } = "z";
        public string KeyB { get; set; } = "x";
        public string KeyC { get; set; } = "c";
        public string KeyD { get; set; } = "v";
    }

    private readonly string _path;
    private readonly Config _cfg;
    private readonly StringBuilder _runLabel = new StringBuilder(); // body of RunCombo:
    private bool _finished;

    private BBCF_TAS(string path, Config cfg)
    {
        _path = path;
        _cfg = cfg;
    }

    // Entry point
    public static BBCF_TAS CreateScript(string path)
    {
        return CreateScript(path, x => { });
    }
    public static BBCF_TAS CreateScript(string path, Action<Config> configure)
    {
        var cfg = new Config();
        configure?.Invoke(cfg);
        return new BBCF_TAS(path, cfg);
    }

    // === Public fluent helpers ===

    // Neutral normals: 5X means just press button with no direction
    public BBCF_TAS _5A() => Button("A");
    public BBCF_TAS _5B() => Button("B");
    public BBCF_TAS _5C() => Button("C");
    public BBCF_TAS _5D() => Button("D");

    // Forward normals: 6X
    public BBCF_TAS _6A() => DirPlusButton("6", "A");
    public BBCF_TAS _6B() => DirPlusButton("6", "B");
    public BBCF_TAS _6C() => DirPlusButton("6", "C");
    public BBCF_TAS _6D() => DirPlusButton("6", "D");

    // Down normals: 2X
    public BBCF_TAS _2A() => DirPlusButton("2", "A");
    public BBCF_TAS _2B() => DirPlusButton("2", "B");
    public BBCF_TAS _2C() => DirPlusButton("2", "C");
    public BBCF_TAS _2D() => DirPlusButton("2", "D");

    // Common motions
    public BBCF_TAS _236A() => MotionPlusButton("236", "A");
    public BBCF_TAS _236B() => MotionPlusButton("236", "B");
    public BBCF_TAS _236C() => MotionPlusButton("236", "C");
    public BBCF_TAS _236D() => MotionPlusButton("236", "D");

    public BBCF_TAS _214A() => MotionPlusButton("214", "A");
    public BBCF_TAS _214B() => MotionPlusButton("214", "B");
    public BBCF_TAS _214C() => MotionPlusButton("214", "C");
    public BBCF_TAS _214D() => MotionPlusButton("214", "D");

    // Jump + buttons (simple air inputs)
    public BBCF_TAS Jump() => DirTap("8");

    // Wait
    public BBCF_TAS WaitFrames(int frames)
    {
        int ms = (int)Math.Round(frames * (1000.0 / _cfg.FramesPerSecond));
        Append($"Sleep, {ms}");
        return this;
    }

    // Small safety gap
    public BBCF_TAS Pause(int ms)
    {
        Append($"Sleep, {ms}");
        return this;
    }

    // === Finalize ===
    public void Finish(bool runScript = true, bool alsoOpenInNotepad = false)
    {
        if (_finished) return;
        _finished = true;

        string script = BuildAhk();
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_path)) ?? ".");
        File.WriteAllText(_path, script, new UTF8Encoding(false));

        if (runScript)
        {
            if (!string.IsNullOrWhiteSpace(_cfg.AutoHotkeyExe))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _cfg.AutoHotkeyExe,
                    Arguments = $"\"{_path}\"",
                    UseShellExecute = false
                });
            }
            else
            {
                // Use OS association—typically starts AutoHotkey and runs the script.
                Process.Start(new ProcessStartInfo
                {
                    FileName = _path,
                    UseShellExecute = true
                });
            }
        }

        if (alsoOpenInNotepad)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "notepad.exe",
                Arguments = $"\"{_path}\"",
                UseShellExecute = false
            });
        }
    }

    // === Internals ===
    private BBCF_TAS Button(string b)
    {
        AppendAbortCheck();
        Append(TapKey(ButtonToKey(b)));
        AppendInterStep();
        return this;
    }

    private BBCF_TAS DirPlusButton(string dir, string b)
    {
        AppendAbortCheck();
        HoldDirections(dir);
        Append(TapKey(ButtonToKey(b)));
        ReleaseDirections(dir);
        AppendInterStep();
        return this;
    }

    private BBCF_TAS MotionPlusButton(string motion, string b)
    {
        AppendAbortCheck();
        // Roll the stick directions quickly then press button
        foreach (var seg in ExpandMotion(motion))
            PressDirectionCombo(seg, _cfg.KeyTapMs);

        Append(TapKey(ButtonToKey(b)));
        AppendInterStep();
        return this;
    }

    private BBCF_TAS DirTap(string dir)
    {
        AppendAbortCheck();
        PressDirectionCombo(ExpandDirection(dir), _cfg.KeyTapMs);
        AppendInterStep();
        return this;
    }

    private void Append(string line) => _runLabel.AppendLine(line);

    private void AppendAbortCheck() => _runLabel.AppendLine("if (abort) return");

    private void AppendInterStep()
    {
        if (_cfg.InterStepMs > 0)
            _runLabel.AppendLine($"Sleep, {_cfg.InterStepMs}");
    }

    private string ButtonToKey(string b) => b.ToUpperInvariant() switch
    {
        "A" => _cfg.KeyA,
        "B" => _cfg.KeyB,
        "C" => _cfg.KeyC,
        "D" => _cfg.KeyD,
        _ => throw new ArgumentException($"Unknown button '{b}'")
    };

    // Converts "2","3","6","214","236","66" etc to sequences of directional key sets
    private IEnumerable<HashSet<string>> ExpandMotion(string motion)
    {
        // Walk each digit as a step. For things like "66", you get Forward tap + Forward tap.
        foreach (char ch in motion)
            yield return ExpandDirection(ch.ToString());
    }

    private HashSet<string> ExpandDirection(string d) => d switch
    {
        "1" => new HashSet<string> { _cfg.KeyDown, _cfg.KeyLeft },
        "2" => new HashSet<string> { _cfg.KeyDown },
        "3" => new HashSet<string> { _cfg.KeyDown, _cfg.KeyRight },
        "4" => new HashSet<string> { _cfg.KeyLeft },
        "5" => new HashSet<string> { }, // neutral
        "6" => new HashSet<string> { _cfg.KeyRight },
        "7" => new HashSet<string> { _cfg.KeyUp, _cfg.KeyLeft },
        "8" => new HashSet<string> { _cfg.KeyUp },
        "9" => new HashSet<string> { _cfg.KeyUp, _cfg.KeyRight },
        _ => throw new ArgumentException($"Bad direction '{d}'")
    };

    private void HoldDirections(string dir)
    {
        var combo = ExpandDirection(dir);
        if (combo.Count == 0) return;
        var sb = new StringBuilder();
        foreach (var k in combo)
            sb.Append($"{{{k} down}}");
        Append($"Send, {sb}");
        Append($"Sleep, {_cfg.KeyTapMs}");
    }

    private void ReleaseDirections(string dir)
    {
        var combo = ExpandDirection(dir);
        if (combo.Count == 0) return;
        var sb = new StringBuilder();
        foreach (var k in combo)
            sb.Append($"{{{k} up}}");
        Append($"Send, {sb}");
    }

    private void PressDirectionCombo(HashSet<string> combo, int holdMs)
    {
        if (combo.Count == 0)
        {
            Append($"Sleep, {holdMs}");
            return;
        }
        var down = new StringBuilder();
        foreach (var k in combo) down.Append($"{{{k} down}}");
        Append($"Send, {down}");
        Append($"Sleep, {holdMs}");
        var up = new StringBuilder();
        foreach (var k in combo) up.Append($"{{{k} up}}");
        Append($"Send, {up}");
    }

    private string TapKey(string key) => $"Send, {{{key} down}}\nSleep, {_cfg.KeyTapMs}\nSend, {{{key} up}}";

    private string BuildAhk()
    {
        var keyDelay = Math.Max(0, _cfg.InterStepMs / 2);
        var sb = new StringBuilder();
        sb.AppendLine("; ===== Auto-generated by BBTas =====");
        sb.AppendLine("#NoEnv");
        sb.AppendLine("#InstallKeybdHook");
        sb.AppendLine("SetBatchLines, -1");
        sb.AppendLine("ListLines, Off");
        sb.AppendLine($"SetKeyDelay, {keyDelay}, {_cfg.KeyTapMs}");
        sb.AppendLine("global abort := false");
        sb.AppendLine();
        // Hotkeys
        sb.AppendLine($"; Play");
        sb.AppendLine($"{_cfg.PlayHotkey}::");
        sb.AppendLine("    abort := false");
        sb.AppendLine("    Gosub, RunCombo");
        sb.AppendLine("return");
        sb.AppendLine();
        sb.AppendLine($"; Stop (sets abort flag)");
        sb.AppendLine($"{_cfg.StopHotkey}::");
        sb.AppendLine("    abort := true");
        sb.AppendLine("return");
        sb.AppendLine();
        // Body
        sb.AppendLine("RunCombo:");
        sb.Append(_runLabel);
        sb.AppendLine("return");
        sb.AppendLine();
        // Optional helper to quickly exit script (Shift+Esc)
        sb.AppendLine("+Esc::ExitApp");
        sb.AppendLine("; ===== End BBTas =====");
        return sb.ToString();
    }
}