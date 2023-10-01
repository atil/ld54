﻿#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace JamKit
{
    public static class EditorUtils
    {
        [MenuItem("Torreng/Top View #&q", false, 10)]
        private static void TopView()
        {
            SceneView s = SceneView.sceneViews[0] as SceneView;
            s.LookAt(s.pivot, Quaternion.LookRotation(new Vector3(0, -1, 0)), s.size, true);
            s.orthographic = true;
        }

        [MenuItem("Torreng/Compile and Play #&p", false, 10)]
        private static void CompileAndPlay()
        {
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Torreng/Toggle profiler &k", false, 10)]
        private static void ToggleProfiler()
        {
            ProfilerDriver.enabled = !ProfilerDriver.enabled;
        }

        [MenuItem("Torreng/Clear playerprefs", false, 10)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }

        [MenuItem("Torreng/Deploy to itch", false, 21)]
        private static void Deploy()
        {
            if (!EditorUtility.DisplayDialog("Deploy button clicked", "Continue with WebGL build and deploy to itch?", "Yes", "Wait no."))
            {
                UnityEngine.Debug.Log("Deploy cancelled");
                return;
            }

            const string projectName = "museum-heist";
            // Also make sure that "data caching" is off and "compression format" is gzip in publishing settings

            Globals globals = Resources.Load<Globals>("Globals");
            if (globals != null)
            {
                PlayerSettings.SplashScreen.backgroundColor = globals.BuildSplashBackgroundColor;
            }

            BuildWebGL();

            Process zipProcess = Process.Start("7z.exe", "a WebGL.zip ./Build/WebGL/* -r");
            zipProcess.WaitForExit();
            UnityEngine.Debug.Log("Zipped build, proceeding with upload");
            Process butlerProcess = Process.Start("butler.exe", $"push WebGL.zip pingfromheaven/{projectName}:html5");
            butlerProcess.WaitForExit();
            if (butlerProcess.ExitCode == 0)
            {
                EditorUtility.DisplayDialog("Deploy done", "", "OK good");
                UnityEngine.Debug.Log("Deploy done");
            }
            else
            {
                EditorUtility.DisplayDialog("Deploy failed :(", "Don't know why though...", "Damn");
                UnityEngine.Debug.LogError($"Butler push failed with exit code {butlerProcess.ExitCode}");
            }

            File.Delete("WebGL.zip");
        }

        private static void BuildWebGL()
        {
            string[] scenes =
            {
                "Assets/Scenes/Splash.unity",
                "Assets/Scenes/Game.unity",
                "Assets/Scenes/End.unity",
            };

            BuildPipeline.BuildPlayer(scenes, "Build/WebGL", BuildTarget.WebGL, BuildOptions.None);
        }
    }
}
#endif