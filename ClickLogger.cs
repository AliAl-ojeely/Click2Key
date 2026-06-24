using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

public static class ClickLogger
{
    private static string logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Click2Key");
    private static string logFilePath = Path.Combine(logDirectory, "log.txt");

    private static Dictionary<string, int> clickStats = new Dictionary<string, int>();

    public static void LoadStats()
    {
        if (!File.Exists(logFilePath)) return;

        string[] lines = File.ReadAllLines(logFilePath);
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int count))
            {
                clickStats[parts[0]] = count;
            }
        }
    }

    public static void LogClick(string buttonName)
    {
        if (clickStats.ContainsKey(buttonName))
        {
            clickStats[buttonName]++;
        }
        else
        {
            clickStats[buttonName] = 1;
        }

        SaveStats();
    }

    private static void SaveStats()
    {
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        List<string> lines = new List<string>();
        foreach (var kvp in clickStats)
        {
            lines.Add($"{kvp.Key}:{kvp.Value}");
        }

        File.WriteAllLines(logFilePath, lines);
    }

    public static int GetCount(string buttonName)
    {
        return clickStats.ContainsKey(buttonName) ? clickStats[buttonName] : 0;
    }

    public static void OpenLogFile()
    {
        if (File.Exists(logFilePath))
        {
            System.Diagnostics.Process.Start("notepad.exe", logFilePath);
        }
        else
        {
            System.IO.File.WriteAllText(logFilePath, "");
            System.Diagnostics.Process.Start("notepad.exe", logFilePath);
        }
    }
}