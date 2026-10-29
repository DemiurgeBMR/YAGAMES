using System.Linq;
using BaggageRush.Core;
using BaggageRush.DebugTools;
using BaggageRush.Platform;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BaggageRush.EditorTools
{
    /// <summary>
    /// Настройка проекта и сборка сцен из кода — чтобы проект можно было воспроизвести
    /// одной командой меню, без ручных кликов в инспекторе.
    /// </summary>
    public static class ProjectSetup
    {
        const string Menu = "YAGAMES/";
        const string MainScenePath = "Assets/_Game/Scenes/Main.unity";
        const string UiFontPath = "Assets/_Game/Fonts/Rubik-Bold.ttf";

        static readonly string[] PlatformDefines = { "YANDEX_GAMES", "RUSTORE", "GOOGLE_PLAY" };

        [MenuItem(Menu + "1. Настроить проект под WebGL", priority = 1)]
        public static void ApplyWebGLSettings()
        {
            PlayerSettings.companyName = "YAGAMES";
            PlayerSettings.productName = "Baggage Rush";
            PlayerSettings.runInBackground = false;
            PlayerSettings.defaultWebScreenWidth = 540;
            PlayerSettings.defaultWebScreenHeight = 960;
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Medium);
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.nameFilesAsHashes = false;

            SetPlatformDefine(NamedBuildTarget.WebGL, "YANDEX_GAMES");
            AssetDatabase.SaveAssets();
            Debug.Log("YAGAMES: настройки WebGL применены (Brotli, стриппинг, 540×960, YANDEX_GAMES).");
        }

        [MenuItem(Menu + "2. Собрать сцену Main", priority = 2)]
        public static void BuildMainScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var cameraGo = new GameObject("Main Camera") { tag = "MainCamera" };
            cameraGo.transform.position = new Vector3(0f, 0f, -10f);
            var camera = cameraGo.AddComponent<Camera>();
            camera.orthographic = true;
            // Арт рисуется под 1080×1920 при 100 px на юнит: по высоте 19.2 юнита.
            camera.orthographicSize = 9.6f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color32(0xdd, 0xe5, 0xee, 0xff);
            cameraGo.AddComponent<AudioListener>();

            var bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<GameBootstrap>();
            var panel = bootstrap.AddComponent<DebugPanel>();
            var serialized = new SerializedObject(panel);
            serialized.FindProperty("font").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Font>(UiFontPath);
            serialized.ApplyModifiedPropertiesWithoutUndo();

            if (!AssetDatabase.IsValidFolder("Assets/_Game/Scenes"))
                AssetDatabase.CreateFolder("Assets/_Game", "Scenes");
            EditorSceneManager.SaveScene(scene, MainScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MainScenePath, true) };
            Debug.Log($"YAGAMES: сцена {MainScenePath} собрана и добавлена в сборку.");
        }

        [MenuItem(Menu + "Платформа/Яндекс Игры (WebGL)", priority = 20)]
        public static void UseYandexGames() => SetPlatformDefine(NamedBuildTarget.WebGL, "YANDEX_GAMES");

        [MenuItem(Menu + "Сбросить локальное сохранение", priority = 40)]
        public static void DeleteLocalSave()
        {
            PlayerPrefs.DeleteKey(PlatformFactory.SaveKey);
            PlayerPrefs.Save();
            Debug.Log("YAGAMES: локальное сохранение удалено.");
        }

        static void SetPlatformDefine(NamedBuildTarget target, string define)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbols(target)
                .Split(';')
                .Where(d => d.Length > 0 && !PlatformDefines.Contains(d))
                .Append(define);
            PlayerSettings.SetScriptingDefineSymbols(target, string.Join(";", defines));
        }
    }
}
