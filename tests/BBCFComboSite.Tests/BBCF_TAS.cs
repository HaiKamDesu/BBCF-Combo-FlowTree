using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using NUnit.Framework; // Remove if you don't use NUnit

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

        // AHK location (null/empty -> use file association)
        public string AutoHotkeyExe { get; set; } = "";

        // Direction keys (keyboard)
        public string KeyUp { get; set; } = "Up";
        public string KeyDown { get; set; } = "Down";
        public string KeyLeft { get; set; } = "Left";
        public string KeyRight { get; set; } = "Right";

        // Attack buttons
        public string KeyA { get; set; } = "j";
        public string KeyB { get; set; } = "i";
        public string KeyC { get; set; } = "l";
        public string KeyD { get; set; } = "k";

        // Extra buttons (standalone only, no directional variants)
        public string KeyF1 { get; set; } = "0";
        public string KeyF2 { get; set; } = "P";

        // Utility
        public string KeyReset { get; set; } = "Backspace"; // stage reset
        public int DashGapMs { get; set; } = 25;            // gap between taps for dash/backdash
        public string StepHotkey { get; set; } = "F10";  // used by WaitUntil() with no args
    }

    private readonly string _path;
    private readonly Config _cfg;
    private readonly StringBuilder _runLabel = new StringBuilder(); // body of RunCombo:
    private bool _finished;

    public enum Facing { Right, Left }
    private Facing _facing = Facing.Right;

    private BBCF_TAS(string path, Config cfg)
    {
        _path = path;
        _cfg = cfg;
    }

    // Entry point
    public static BBCF_TAS CreateScript(string path) => CreateScript(path, _ => { });
    public static BBCF_TAS CreateScript(string path, Action<Config> configure)
    {
        var cfg = new Config();
        configure?.Invoke(cfg);
        return new BBCF_TAS(path, cfg);
    }

    public BBCF_TAS SetFacing(Facing facing) { _facing = facing; return this; }
    public BBCF_TAS FaceRight() => SetFacing(Facing.Right);
    public BBCF_TAS FaceLeft() => SetFacing(Facing.Left);

    // === Public fluent helpers ===
    public BBCF_TAS _F1() => Button("F1");
    public BBCF_TAS _F2() => Button("F2");

    // Neutral normals: 5X
    public BBCF_TAS _5A() => Button("A");
    public BBCF_TAS _5B() => Button("B");
    public BBCF_TAS _5C() => Button("C");
    public BBCF_TAS _5D() => Button("D");

    // Forward normals: 6X (common)
    public BBCF_TAS _6A() => DirPlusButton("6", "A");
    public BBCF_TAS _6B() => DirPlusButton("6", "B");
    public BBCF_TAS _6C() => DirPlusButton("6", "C");
    public BBCF_TAS _6D() => DirPlusButton("6", "D");

    // Down normals: 2X (common)
    public BBCF_TAS _2A() => DirPlusButton("2", "A");
    public BBCF_TAS _2B() => DirPlusButton("2", "B");
    public BBCF_TAS _2C() => DirPlusButton("2", "C");
    public BBCF_TAS _2D() => DirPlusButton("2", "D");

    // Specific directional normal that exists (e.g., Kokonoe 3C)
    public BBCF_TAS _3C() => DirPlusButton("3", "C");

    // Full numpad D placements for Drive (Kokonoe places graviton by direction)
    public BBCF_TAS _1D() => DirPlusButton("1", "D");
    public BBCF_TAS _3D() => DirPlusButton("3", "D");
    public BBCF_TAS _4D() => DirPlusButton("4", "D");
    public BBCF_TAS _7D() => DirPlusButton("7", "D");
    public BBCF_TAS _8D() => DirPlusButton("8", "D");
    public BBCF_TAS _9D() => DirPlusButton("9", "D");

    // Air buttons that DO NOT jump (assumes you're airborne)
    public BBCF_TAS jA() => Button("A");
    public BBCF_TAS jB() => Button("B");
    public BBCF_TAS jC() => Button("C");
    public BBCF_TAS jD() => Button("D");

    // Movement
    public BBCF_TAS Jump() => DirTap("8");                // simple jump if you need it
    public BBCF_TAS _27() => MotionOnly("27");            // superjump back
    public BBCF_TAS _28() => MotionOnly("28");            // superjump neutral
    public BBCF_TAS _29() => MotionOnly("29");            // superjump forward

    // Common motions
    public BBCF_TAS _236A() => MotionPlusButton("236", "A");
    public BBCF_TAS _236B() => MotionPlusButton("236", "B");
    public BBCF_TAS _236C() => MotionPlusButton("236", "C");
    public BBCF_TAS _236D() => MotionPlusButton("236", "D");

    public BBCF_TAS _214A() => MotionPlusButton("214", "A");
    public BBCF_TAS _214B() => MotionPlusButton("214", "B");
    public BBCF_TAS _214C() => MotionPlusButton("214", "C");
    public BBCF_TAS _214D() => MotionPlusButton("214", "D");

    public BBCF_TAS _22A() => MotionPlusButton("22", "A");
    public BBCF_TAS _22B() => MotionPlusButton("22", "B");
    public BBCF_TAS _22C() => MotionPlusButton("22", "C");

    public BBCF_TAS _214214A() => MotionPlusButton("214214", "A");
    public BBCF_TAS _214214B() => MotionPlusButton("214214", "B");
    public BBCF_TAS _214214C() => MotionPlusButton("214214", "C");

    public BBCF_TAS _632146D() => MotionPlusButton("632146", "D");
    public BBCF_TAS _64641236C() => MotionPlusButton("64641236", "C"); // 100% supers

    // Simultaneous/system inputs
    public BBCF_TAS Throw() => MultiTap(_cfg.KeyB, _cfg.KeyC);      // B+C
    public BBCF_TAS AirThrow() => Throw();
    public BBCF_TAS RapidCancel() => MultiTap(_cfg.KeyA, _cfg.KeyB, _cfg.KeyC); // A+B+C
    public BBCF_TAS Overdrive() => MultiTap(_cfg.KeyA, _cfg.KeyB, _cfg.KeyC, _cfg.KeyD); // A+B+C+D

    public BBCF_TAS CounterAssault()
    {
        AppendAbortCheck();
        HoldDirections(ForwardDir());
        MultiTap(_cfg.KeyA, _cfg.KeyB); // 6+A+B
        ReleaseDirections(ForwardDir());
        AppendInterStep();
        return this;
    }

    // === Kokonoe named convenience (calls the correct motions) ===
    // Drive tools (D motions are above; these are the Distortion-style device controls)
    public BBCF_TAS Activate() => _236D();                // Deploy graviton (also air OK)
    public BBCF_TAS RetrieveGraviton() => _214D();        // Retrieve graviton (also air OK)

    // Specials
    public BBCF_TAS BrokenBunkerA() => _236A();           // 236A
    public BBCF_TAS SolidWheel() => _236B();              // 236B (air OK)
    public BBCF_TAS AbsoluteZero() => _236C();            // 236C
    public BBCF_TAS FlameCageA() => _214A();              // 214A (air OK)
    public BBCF_TAS FlameCageB() => _214B();              // 214B (air OK)
    public BBCF_TAS FlameCageC() => _214C();              // 214C (air OK)
    public BBCF_TAS BanishingRaysA() => _22A();           // 22A
    public BBCF_TAS BanishingRaysB() => _22B();           // 22B
    public BBCF_TAS PlanarHaze() => _22C();               // 22C

    // Distortions / Exceed
    public BBCF_TAS PyroFlamingBelobogA() => _214214A();  // 214214A (air OK)
    public BBCF_TAS PyroFlamingBelobogB() => _214214B();  // 214214B (air OK)
    public BBCF_TAS PyroFlamingBelobogC() => _214214C();  // 214214C (air OK)
    public BBCF_TAS JammingDark() => _632146D();          // 632146D
    public BBCF_TAS DreadnoughtDestroyer() => _64641236C();           // 100% heat
    public BBCF_TAS SuperDreadnoughtExterminator() => _64641236C();   // OD version (same motion)

    // Utility actions
    public BBCF_TAS Reset() => DirPlusButton("5", "Reset");
    public BBCF_TAS MidscreenReset() => DirPlusButton("2", "Reset");
    public BBCF_TAS CornerReset() => DirPlusButton("6", "Reset");
    public BBCF_TAS BackToCornerReset() => DirPlusButton("4", "Reset");

    public BBCF_TAS ForwardDash() => DoubleTap(ForwardDir());
    public BBCF_TAS BackDash() => DoubleTap(BackDir());

    public BBCF_TAS Run(int holdFrames)
    {
        // double-tap forward, then hold forward for N frames
        DoubleTap(ForwardDir());
        HoldDirections(ForwardDir());
        int ms = (int)Math.Round(holdFrames * (1000.0 / _cfg.FramesPerSecond));
        Append($"Sleep, {ms}");
        ReleaseDirections(ForwardDir());
        AppendInterStep();
        return this;
    }

    public BBCF_TAS Crouch(int holdFrames)
    {
        HoldDirections("2");
        int ms = (int)Math.Round(holdFrames * (1000.0 / _cfg.FramesPerSecond));
        Append($"Sleep, {ms}");
        ReleaseDirections("2");
        AppendInterStep();
        return this;
    }

    public BBCF_TAS MoveForward(int holdFrames)
    {
        HoldDirections(ForwardDir());
        int ms = (int)Math.Round(holdFrames * (1000.0 / _cfg.FramesPerSecond));
        Append($"Sleep, {ms}");
        ReleaseDirections(ForwardDir());
        AppendInterStep();
        return this;
    }

    // On-screen status in AHK so you can see what is being tested
    public BBCF_TAS Show(string message) => Show("AHK Script", message);
    public BBCF_TAS Show(string title, string message, int ms = 1200, int icon = 1)
    {
        // icon: 0 = none, 1 = info, 2 = warning, 3 = error (AHK v1 TrayTip)
        AppendAbortCheck();
        Append($"; -- Show notification");
        int seconds = Math.Max(1, ms / 1000); // TrayTip expects seconds, not ms
        Append($"TrayTip, {EscapeAhk(title)}, {EscapeAhk(message)}, {seconds}, {icon}");
        Append($"Sleep, {ms}");
        Append("TrayTip"); // clear
        AppendInterStep();
        return this;
    }
    public BBCF_TAS ShowTooltip(string text, int ms = 800)
    {
        AppendAbortCheck();
        Append($"; -- ShowTooltip");
        Append($"ToolTip, {EscapeAhk(text)}");
        Append($"Sleep, {ms}");
        Append("ToolTip"); // clear
        AppendInterStep();
        return this;
    }
    // Waits
    public BBCF_TAS WaitFrames(int frames)
    {
        int ms = (int)Math.Round(frames * (1000.0 / _cfg.FramesPerSecond));
        Append($"Sleep, {ms}");
        return this;
    }
    public BBCF_TAS Pause(int ms) { Append($"Sleep, {ms}"); return this; }
    public BBCF_TAS WaitUntil(string key) => WaitUntil();

    public BBCF_TAS WaitUntil()
    {
        AppendAbortCheck();
        Append($"; -- WaitUntil (press {_cfg.StepHotkey} to continue)");
        Append("step := false");
        Append("Loop");
        Append("{");
        Append("  if (abort)");
        Append("    return");
        Append("  if (step)");
        Append("  {");
        Append("    step := false");
        Append("    break");
        Append("  }");
        Append("  Sleep, 10");
        Append("}");
        AppendInterStep();
        return this;
    }
    // === Internals ===
    private string ForwardDir() => "6";
    private string BackDir() => "4";

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

        // Special case: 22X (e.g. 22A / 22B / 22C) wants two distinct 2 taps, then button.
        if (motion == "22")
        {
            Emit22PlusButton(b);
        }
        else
        {
            // Generic behavior: tap each motion segment, then press the button.
            foreach (var seg in ExpandMotion(motion))
                PressDirectionCombo(seg, _cfg.KeyTapMs);

            Append(TapKey(ButtonToKey(b)));
        }

        AppendInterStep();
        return this;
    }

    // 22X: two very quick 2 taps, then button.
    // Intention: input display looks like "2, 2, A" and the engine reads it as 22A.
    private void Emit22PlusButton(string button)
    {
        // 2 is unaffected by facing, but we still go through ExpandDirection
        var downCombo = ExpandDirection("2");

        // Make these taps a bit snappier than the default to help 22 detection
        int tapMs = Math.Max(15, _cfg.KeyTapMs / 2); // shorter hold for each 2
        int gapMs = 10;                              // small gap between the two 2's

        // First 2 tap
        PressDirectionCombo(downCombo, tapMs);
        Append($"Sleep, {gapMs}");

        // Second 2 tap
        PressDirectionCombo(downCombo, tapMs);

        // Tiny spacer before A so it's "after" the second 2, not simultaneous
        Append("Sleep, 10");

        // Now press the button by itself
        Append(TapKey(ButtonToKey(button)));
    }


    private BBCF_TAS MotionOnly(string motion)
    {
        AppendAbortCheck();

        // Special-case superjump: 27 / 28 / 29
        if (motion.Length == 2 &&
            motion[0] == '2' &&
            (motion[1] == '7' || motion[1] == '8' || motion[1] == '9'))
        {
            EmitSuperJump(motion[1]);
        }
        else
        {
            // Generic motion: 236, 214214, etc.
            foreach (var seg in ExpandMotion(motion))
                PressDirectionCombo(seg, _cfg.KeyTapMs);
        }

        AppendInterStep();
        return this;
    }

    // Emits a 2~7 / 2~8 / 2~9 style superjump: quick down tap, then hold final dir.
    private void EmitSuperJump(char upDirDigit)
    {
        // 1) Quick down tap (2)
        var downCombo = ExpandDirection("2");
        PressDirectionCombo(downCombo, _cfg.KeyTapMs);

        // 2) Hold final diagonal / up direction
        var finalCombo = ExpandDirection(upDirDigit.ToString());

        var down = new StringBuilder();
        foreach (var k in finalCombo)
            down.Append($"{{{k} down}}");
        Append($"Send, {down}");

        // hold a bit longer so the game clearly registers the jump
        int holdMs = _cfg.KeyTapMs * 4; // tweak if needed
        Append($"Sleep, {holdMs}");

        var up = new StringBuilder();
        foreach (var k in finalCombo)
            up.Append($"{{{k} up}}");
        Append($"Send, {up}");
    }


    private BBCF_TAS DirTap(string dir)
    {
        AppendAbortCheck();
        PressDirectionCombo(ExpandDirection(dir), _cfg.KeyTapMs);
        AppendInterStep();
        return this;
    }

    private void Append(string line) => _runLabel.AppendLine(line);
    private void AppendAbortCheck() 
    {
        _runLabel.AppendLine("if (abort)");
        _runLabel.AppendLine("return");
    }

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
        "F1" => _cfg.KeyF1,
        "F2" => _cfg.KeyF2,
        "RESET" => _cfg.KeyReset,
        _ => throw new ArgumentException($"Unknown button '{b}'")
    };

    // Converts motions like "236", "214214", "632146", "64641236", "27", "29"
    private IEnumerable<HashSet<string>> ExpandMotion(string motion)
    {
        foreach (char ch in motion)
            yield return ExpandDirection(ch.ToString());
    }

    // Mirror a numpad digit if facing left: 1<->3, 4<->6, 7<->9 (2/5/8 unchanged)
    private string MirrorDigitIfNeeded(string d)
    {
        if (_facing == Facing.Right) return d;
        return d switch
        {
            "1" => "3",
            "3" => "1",
            "4" => "6",
            "6" => "4",
            "7" => "9",
            "9" => "7",
            _ => d   // "2","5","8"
        };
    }

    private HashSet<string> ExpandDirection(string d)
    {
        d = MirrorDigitIfNeeded(d);
        return d switch
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
    }

    private void HoldDirections(string dir)
    {
        var combo = ExpandDirection(dir);
        if (combo.Count == 0) return;
        var sb = new StringBuilder();
        foreach (var k in combo) sb.Append($"{{{k} down}}");
        Append($"Send, {sb}");
        Append($"Sleep, {_cfg.KeyTapMs}");
    }

    private void ReleaseDirections(string dir)
    {
        var combo = ExpandDirection(dir);
        if (combo.Count == 0) return;
        var sb = new StringBuilder();
        foreach (var k in combo) sb.Append($"{{{k} up}}");
        Append($"Send, {sb}");
    }

    private void PressDirectionCombo(HashSet<string> combo, int holdMs)
    {
        if (combo.Count == 0) { Append($"Sleep, {holdMs}"); return; }
        var down = new StringBuilder();
        foreach (var k in combo) down.Append($"{{{k} down}}");
        Append($"Send, {down}");
        Append($"Sleep, {holdMs}");
        var up = new StringBuilder();
        foreach (var k in combo) up.Append($"{{{k} up}}");
        Append($"Send, {up}");
    }

    private BBCF_TAS MultiTap(params string[] keys)
    {
        AppendAbortCheck();
        var down = new StringBuilder();
        foreach (var k in keys) down.Append($"{{{k} down}}");
        Append($"Send, {down}");
        Append($"Sleep, {_cfg.KeyTapMs}");
        var up = new StringBuilder();
        foreach (var k in keys) up.Append($"{{{k} up}}");
        Append($"Send, {up}");
        AppendInterStep();
        return this;
    }

    private BBCF_TAS DoubleTap(string dir)
    {
        AppendAbortCheck();
        var seg = ExpandDirection(dir);
        PressDirectionCombo(seg, _cfg.KeyTapMs);
        Append($"Sleep, {(_cfg.DashGapMs > 0 ? _cfg.DashGapMs : Math.Max(10, _cfg.InterStepMs))}");
        PressDirectionCombo(seg, _cfg.KeyTapMs);
        AppendInterStep();
        return this;
    }

    private string TapKey(string key) => $"Send, {{{key} down}}\nSleep, {_cfg.KeyTapMs}\nSend, {{{key} up}}";

    private static string EscapeAhk(string s) => (s ?? string.Empty).Replace(",", "`,");

    private string BuildAhk()
    {
        var sb = new StringBuilder();

        sb.AppendLine("; ===== Auto-generated by BBCF_TAS =====");
        sb.AppendLine("#NoEnv");
        sb.AppendLine("#SingleInstance Force");
        sb.AppendLine("#InstallKeybdHook");
        sb.AppendLine("SetBatchLines, -1");
        sb.AppendLine("ListLines, Off");
        sb.AppendLine("SetKeyDelay, 0, 0");
        sb.AppendLine("global abort := false");
        sb.AppendLine("global step := false");
        sb.AppendLine();

        // Play hotkey (same style as your working test script)
        sb.AppendLine($"; Play");
        sb.AppendLine($"{_cfg.PlayHotkey}::");
        sb.AppendLine("    abort := false");
        sb.AppendLine("    step := false");   // reset step flag on each run
        sb.AppendLine("    Gosub, RunCombo");
        sb.AppendLine("return");
        sb.AppendLine();

        // Stop hotkey
        sb.AppendLine($"; Stop (sets abort flag)");
        sb.AppendLine($"{_cfg.StopHotkey}::");
        sb.AppendLine("    abort := true");
        sb.AppendLine("return");
        sb.AppendLine();

        // Step hotkey (used by WaitUntil)
        sb.AppendLine($"; Step forward (used by WaitUntil)");
        sb.AppendLine($"{_cfg.StepHotkey}::");
        sb.AppendLine("    step := true");
        sb.AppendLine("return");
        sb.AppendLine();

        // Body
        sb.AppendLine("RunCombo:");
        sb.Append(_runLabel);
        sb.AppendLine("return");
        sb.AppendLine();

        // Optional helper to quickly exit script (Shift+Esc)
        sb.AppendLine("+Esc::ExitApp");
        sb.AppendLine("; ===== End BBCF_TAS =====");

        return sb.ToString();
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
}
