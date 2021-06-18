using UnityEngine;
using UnityEditor;
using System.IO;


namespace GameBerry
{
    public class SetAssetBundleNames
    {
        [MenuItem("Assets/_GameBerry/AssetBundle Names/Set AssetBundle Names - Folder", false, 500)]
        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - Folder", false, 34)]
        static void SetAssetBundleName()
        {
            if (Selection.assetGUIDs.Length == 0)
                return;

            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            SetNames(path);
        }

        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - Folder", true)]
        static bool IsSelectedAssets()
        {
            if (Selection.assetGUIDs.Length == 0)
                return false;

            return true;
        }
        //------------------------------------------------------------------------------------
        // 클릭된 폴더 이름으로 하위 에셋들의 번들테그를 셋팅한다
        [MenuItem("Assets/_GameBerry/AssetBundle Names/Set AssetBundle Names - IsName", false, 500)]
        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - IsName", false, 34)]
        static void SetAssetBundleName_IsName()
        {
            if (Selection.assetGUIDs.Length == 0)
                return;

            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            SetNamesIsName(path);
        }

        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - IsName", true)]
        static bool IsSelectedAssets_IsName()
        {
            if (Selection.assetGUIDs.Length == 0)
                return false;

            return true;
        }
        //------------------------------------------------------------------------------------
        [MenuItem("Assets/_GameBerry/AssetBundle Names/Set AssetBundle Names - File", false, 500)]
        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - File", false, 34)]
        static void SetAssetBundleName_File()
        {
            if (Selection.assetGUIDs.Length == 0)
                return;

            SetNamesFile();
        }

        [MenuItem("_GameBerry/AssetBundle Names/Set AssetBundle Names - File", true)]
        static bool IsSelectedAssets_File()
        {
            if (Selection.assetGUIDs.Length == 0)
                return false;

            return true;
        }

        public static void SetNames(string path)
        {
            System.IO.DirectoryInfo Info = new System.IO.DirectoryInfo(path);

            if (Info.Exists)
            {
                System.IO.DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.AllDirectories);
                if (CInfo.Length == 0)
                {
                    foreach (System.IO.FileInfo File in Info.GetFiles())
                    {
                        if (File.Name.Contains("meta"))
                            continue;

                        string FullFileName = File.FullName;
                        FullFileName = FullFileName.Replace("\\", "/");

                        string FileNameOnly = Path.GetFileNameWithoutExtension(FullFileName);
                        int pathIndex = FullFileName.IndexOf("Assets");
                        FullFileName = FullFileName.Substring(pathIndex, FullFileName.Length - pathIndex);

                        var importer = AssetImporter.GetAtPath(FullFileName);
                        FullFileName = FullFileName.Replace("Assets/AssetBundle/", "");
                        FullFileName = FullFileName.Replace(File.Name, "");

                        importer.assetBundleName = string.Format("{0}{1}", FullFileName, FileNameOnly);
                    }
                }
                else
                {
                    foreach (System.IO.DirectoryInfo info in CInfo)
                    {
                        string temp = info.FullName;
                        temp = temp.Replace("\\", "/");
                        SetNames(temp);
                    }

                    foreach (System.IO.FileInfo File in Info.GetFiles())
                    {
                        if (File.Name.Contains("meta"))
                            continue;

                        string FullFileName = File.FullName;
                        FullFileName = FullFileName.Replace("\\", "/");

                        string FileNameOnly = Path.GetFileNameWithoutExtension(FullFileName);
                        int pathIndex = FullFileName.IndexOf("Assets");
                        FullFileName = FullFileName.Substring(pathIndex, FullFileName.Length - pathIndex);

                        var importer = AssetImporter.GetAtPath(FullFileName);
                        FullFileName = FullFileName.Replace("Assets/AssetBundle/", "");
                        FullFileName = FullFileName.Replace(File.Name, "");

                        importer.assetBundleName = string.Format("{0}{1}", FullFileName, FileNameOnly);
                    }
                }
            }
        }

        public static void SetNamesIsName(string originpath, string path = "")
        {
            System.IO.DirectoryInfo Info = new System.IO.DirectoryInfo(path == "" ? originpath : path);

            if (Info.Exists)
            {
                System.IO.DirectoryInfo[] CInfo = Info.GetDirectories("*", System.IO.SearchOption.AllDirectories);
                if (CInfo.Length == 0)
                {
                    foreach (System.IO.FileInfo File in Info.GetFiles())
                    {
                        if (File.Name.Contains("meta"))
                            continue;

                        string FullFileName = File.FullName;
                        FullFileName = FullFileName.Replace("\\", "/");

                        int pathIndex = FullFileName.IndexOf("Assets");
                        FullFileName = FullFileName.Substring(pathIndex, FullFileName.Length - pathIndex);

                        var importer = AssetImporter.GetAtPath(FullFileName);
                        FullFileName = originpath.Replace("Assets/AssetBundle/", "");

                        importer.assetBundleName = FullFileName;
                    }
                }
                else
                {
                    foreach (System.IO.DirectoryInfo info in CInfo)
                    {
                        string temp = info.FullName;
                        temp = temp.Replace("\\", "/");
                        SetNamesIsName(originpath, temp);
                    }

                    foreach (System.IO.FileInfo File in Info.GetFiles())
                    {
                        if (File.Name.Contains("meta"))
                            continue;

                        string FullFileName = File.FullName;
                        FullFileName = FullFileName.Replace("\\", "/");

                        int pathIndex = FullFileName.IndexOf("Assets");
                        FullFileName = FullFileName.Substring(pathIndex, FullFileName.Length - pathIndex);

                        var importer = AssetImporter.GetAtPath(FullFileName);
                        FullFileName = originpath.Replace("Assets/AssetBundle/", "");

                        importer.assetBundleName = FullFileName;
                    }
                }
            }
        }

        public static void SetNamesFile()
        {
            foreach (var guid in Selection.assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = AssetImporter.GetAtPath(path);

                var fileName = Path.GetFileNameWithoutExtension(path);
                var fileNameExtension = Path.GetFileName(path);
                path = path.Replace("Assets/AssetBundle/", "");
                path = path.Replace(fileNameExtension, "");

                var newName = string.Format("{0}{1}", path, fileName);
                importer.assetBundleName = newName;
            }
        }
    }

    public class InputAssetBundlePathnameWindow : EditorWindow
    {
        static InputAssetBundlePathnameWindow _instance = null;

        string pathname;
        string bundleName;
        bool makeIntoOneBundleName = false;


        public static void OpenWindow()
        {
            _instance = GetWindow<InputAssetBundlePathnameWindow>("AssetBundlePath");
            _instance.minSize = new Vector2(300.0f, 40.0f);
            _instance.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Pathname:", GUILayout.Width(70));
                pathname = EditorGUILayout.TextField(pathname);
            }
            EditorGUILayout.EndHorizontal();

            makeIntoOneBundleName = GUILayout.Toggle(makeIntoOneBundleName, "Make into one BundleName");
            if (makeIntoOneBundleName)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Bundle Name:", GUILayout.Width(90));
                    bundleName = EditorGUILayout.TextField(bundleName);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("OK"))
            {
                SetAssetBundleNames.SetNamesFile();

                _instance.Close();
                _instance = null;
            }

            EditorGUILayout.EndVertical();
        }
    }


    public class AssetBundleNameChangeNotifier : AssetPostprocessor
    {
        void OnPostprocessAssetbundleNameChanged(string path, string previous, string next)
        {
            Debug.LogFormat("ChangeAssetBundleNames - Target: {0}, Old: {1}, New: {2}", path, previous, string.IsNullOrEmpty(next) ? "None" : next);
        }
    }
}