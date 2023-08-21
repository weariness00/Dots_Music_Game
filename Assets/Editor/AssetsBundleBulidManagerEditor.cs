using UnityEditor;
using System.IO;
using UnityEngine;

namespace Editor
{
    public class AssetBundleBulidManagerEditor : EditorWindow
    {
        [MenuItem("MyTool/AssetBundle Build")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetBundleBulidManagerEditor>();
            window.titleContent = new GUIContent("Asset Bundle Manager");
            window.Show();
        }

        private void CreateGUI()
        {
            
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Asset Bundle Build"))
            {
                AssetBundleBuild();
            }
        }

        public static void AssetBundleBuild()
        {
            string directory = "./Bundle";

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            BuildPipeline.BuildAssetBundles(directory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

            EditorUtility.DisplayDialog("Assets Bundle Build", "Build Complicate", "exits");
        }
    }
}