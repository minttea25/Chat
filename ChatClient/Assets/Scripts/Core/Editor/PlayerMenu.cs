using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMenu
{
    [MenuItem("Tools/Run/1")]
    static void BuildAndRun1()
    {
        BuildAndRun(1);
    }

    [MenuItem("Tools/Run/2")]
    static void BuildAndRun2()
    {
        BuildAndRun(2);
    }

    [MenuItem("Tools/Run/3")]
    static void BuildAndRun3()
    {
        BuildAndRun(3);
    }

    static void BuildAndRun(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        var configs = GetSettings();

        for(int i=0; i<playerCount; i++)
        {
            string path = configs.Item1 ?
                $"{configs.Item2}/{GetProjectName()}_{DateTime.Now:yy-MM-dd-HH-mm}_{i}/{configs.Item3}_{i}.exe"
                : $"{configs.Item2}/{GetProjectName()}_{i}/{configs.Item3}_{i}.exe";

            BuildPipeline.BuildPlayer(
                GetLevels(),
                path,
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    static string[] GetLevels()
    {
        string[] levels = new string[EditorBuildSettings.scenes.Length];

        for(int i=0; i< levels.Length;i++)
        {
            levels[i] = EditorBuildSettings.scenes[i].path;
        }
        return levels;
    }

    static Tuple<bool, string, string> GetSettings()
    {
        PlayerConfig config = Resources.Load<PlayerConfig>("PlayerConfig");
        return new(config.UseTimestamp, config.LocationPath, config.ExecutableFileName);
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/'); // ~~Assets  full °æ·Î
        foreach (var ss in s)
        {
            Debug.Log(ss);
        }
        return s[^2];
    }
}
