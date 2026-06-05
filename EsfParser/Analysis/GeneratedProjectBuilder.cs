using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace EsfParser.Analysis;

/// <summary>
/// Runs <c>dotnet build</c> on a generated project as a child process, enforcing a timeout
/// (kills the process tree) and capturing the full log + error/warning counts.
/// </summary>
public sealed class GeneratedProjectBuilder
{
    // e.g. "    3 Warning(s)" / "    1 Error(s)"
    private static readonly Regex WarnLine = new(@"^\s*(\d+)\s+Warning\(s\)", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex ErrLine = new(@"^\s*(\d+)\s+Error\(s\)", RegexOptions.Multiline | RegexOptions.Compiled);

    public BuildResult Build(string projectDir, string logPath, int maxBuildSeconds)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

        var output = new StringBuilder();
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build -v minimal --nologo",
            WorkingDirectory = projectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var sw = Stopwatch.StartNew();
        Process proc;
        try
        {
            proc = Process.Start(psi)!;
        }
        catch (Exception ex)
        {
            File.WriteAllText(logPath, "Failed to start dotnet build: " + ex);
            return new BuildResult { Status = BuildStatus.Failed, LogPath = logPath, Seconds = 0 };
        }

        proc.OutputDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
        proc.ErrorDataReceived += (_, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        bool exited = proc.WaitForExit(maxBuildSeconds * 1000);
        if (!exited)
        {
            TryKillTree(proc);
            sw.Stop();
            lock (output) output.AppendLine($"\n*** Build timed out after {maxBuildSeconds}s and was killed. ***");
            File.WriteAllText(logPath, output.ToString());
            return new BuildResult { Status = BuildStatus.TimedOut, LogPath = logPath, Seconds = sw.Elapsed.TotalSeconds };
        }

        proc.WaitForExit(); // flush async buffers
        sw.Stop();

        var log = output.ToString();
        File.WriteAllText(logPath, log);

        int errors = LastCount(ErrLine, log);
        int warnings = LastCount(WarnLine, log);
        var status = proc.ExitCode == 0 ? BuildStatus.Succeeded : BuildStatus.Failed;

        return new BuildResult
        {
            Status = status,
            ErrorCount = errors,
            WarningCount = warnings,
            LogPath = logPath,
            Seconds = sw.Elapsed.TotalSeconds,
        };
    }

    private static int LastCount(Regex rx, string text)
    {
        int value = 0;
        foreach (Match m in rx.Matches(text))
            if (int.TryParse(m.Groups[1].Value, out var v)) value = v;
        return value;
    }

    private static void TryKillTree(Process proc)
    {
        try { if (!proc.HasExited) proc.Kill(entireProcessTree: true); }
        catch { /* best effort */ }
    }
}
