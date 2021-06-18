using UnityEditor;
using UnityEngine;


namespace GameBerry
{
    public static class AssetbundlesMenuItems
    {
        const string simulateAssetBundlesMenu = "_GameBerry/AssetBundles/Simulate AssetBundles";
        const string useStreamingAssetsMenu = "_GameBerry/AssetBundles/Use StreamingAssets";


        [MenuItem(simulateAssetBundlesMenu)]
        public static void ToggleSimulateAssetBundle()
        {
            AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
        }

        [MenuItem(simulateAssetBundlesMenu, true)]
        public static bool ToggleSimulateAssetBundleValidate()
        {
            Menu.SetChecked(simulateAssetBundlesMenu, AssetBundleManager.SimulateAssetBundleInEditor);
            return true;
        }

        [MenuItem(useStreamingAssetsMenu)]
        public static void ToggleUseStreamingAssets()
        {
            AssetBundleManager.UseStreamingAssetsEditor = !AssetBundleManager.UseStreamingAssetsEditor;
        }

        [MenuItem(useStreamingAssetsMenu, true)]
        public static bool ToggleUseStreamingAssetsValidate()
        {
            Menu.SetChecked(useStreamingAssetsMenu, AssetBundleManager.UseStreamingAssetsEditor);
            return true;
        }

        [MenuItem("_GameBerry/AssetBundles/Build AssetBundles", false, 100)]
        static public void BuildAssetBundles()
        {
            AssetBundleBuildScript.BuildAssetBundles();
        }

        //[MenuItem("_GameBerry/AssetBundles/CompressZip/AllBundle", false, 90)]
        //static public void CompressZipAllBundle()
        //{
        //    System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
        //    sw1.Start();

        //    if (ZipManager.ZipFiles(string.Format("{0}/AssetBundle", Application.streamingAssetsPath)
        //        , Application.streamingAssetsPath + "/" + "AssetBundle.zip"
        //        , null
        //        , false) == true)
        //    {
        //        sw1.Stop();
        //        Debug.Log(string.Format("Complete AssetBundles/CompressZip/AllBundle  {0}ms", sw1.ElapsedMilliseconds.ToString()));
        //    }
        //    else
        //    {
        //        sw1.Stop();
        //        Debug.LogError(string.Format("Error AssetBundles/CompressZip/AllBundle  {0}ms", sw1.ElapsedMilliseconds.ToString()));
        //    }
        //}

        //[MenuItem("_GameBerry/AssetBundles/CompressZip/OnlyBundleFile", false, 90)]
        //static public void CompressZipOnlyBundleFile()
        //{
        //    string[] exclusionFiles = new string[] { ".meta", ".manifest" };

        //    System.Diagnostics.Stopwatch sw1 = new System.Diagnostics.Stopwatch();
        //    sw1.Start();

        //    if (ZipManager.ZipFiles(string.Format("{0}/AssetBundle", Application.streamingAssetsPath)
        //        , Application.streamingAssetsPath + "/" + "AssetBundle.zip"
        //        , null
        //        , false
        //        , exclusionFiles) == true)
        //    {
        //        sw1.Stop();
        //        Debug.Log(string.Format("Complete AssetBundles/CompressZip/OnlyBundleFile  {0}ms", sw1.ElapsedMilliseconds.ToString()));
        //    }
        //    else
        //    {
        //        sw1.Stop();
        //        Debug.LogError(string.Format("Error AssetBundles/CompressZip/OnlyBundleFile  {0}ms", sw1.ElapsedMilliseconds.ToString()));
        //    }
        //}

        [MenuItem("_GameBerry/AssetBundles PackingTag/Set AssetBundles PackingTag")]
        static public void AssetBundles_PackingTag()
        {
            AssetBundlePackingTagScript.AssetBundlePackingTag();
        }
    }
}
