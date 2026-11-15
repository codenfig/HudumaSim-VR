#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HudumaSimVR.Editor
{
    public static class HudumaProjectSetup
    {
        [MenuItem("HudumaSim VR/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes")) AssetDatabase.CreateFolder("Assets", "Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("HudumaSim VR Application").AddComponent<HudumaApp>();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/HudumaDemo.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/HudumaDemo.unity", true) };
            AssetDatabase.SaveAssets();
            Debug.Log("HudumaSim VR demo scene created. Press Play to start.");
        }
    }
}
#endif