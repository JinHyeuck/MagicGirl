#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace GameBerry
{
	public sealed class AssetBundleLoader : MonoSingleton<AssetBundleLoader>
	{
		const string kAssetBundlesPath = "/AssetBundle/";
		bool initializeComplete = false;


		// Use this for initialization.
		void Start()
		{
            Initialize();
            initializeComplete = true;
		}

		// Initialize the downloading url and AssetBundleManifest object.
		void Initialize()
		{
#if UNITY_EDITOR
			Debug.LogFormat("We are {0}", (AssetBundleManager.SimulateAssetBundleInEditor ? "in Editor simulation mode" : "in normal mode"));
#endif

			string platformFolderForAssetBundles;
#if UNITY_EDITOR
			platformFolderForAssetBundles = GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
			platformFolderForAssetBundles = GetPlatformFolderForAssetBundles(Application.platform);
#endif

			// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
			AssetBundleManager.Initialize(platformFolderForAssetBundles);
		}

		//public string GetRelativePath()
		//{
		//	if (Application.isEditor)
		//		return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
		//	else // For standalone player.
		//	    return "file://" + Application.streamingAssetsPath;
		//}

#if UNITY_EDITOR
		public static string GetPlatformFolderForAssetBundles(BuildTarget target)
		{
			switch (target)
			{
			case BuildTarget.Android:
				return "Android";

			case BuildTarget.iOS:
				return "iOS";

            case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneWindows64:
				return "Windows";

			//case BuildTarget.StandaloneOSXIntel:
			//case BuildTarget.StandaloneOSXIntel64:
			//case BuildTarget.StandaloneOSXUniversal:
			//	return "OSX";

			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
			default:
				return null;
			}
		}
#endif

        static string GetPlatformFolderForAssetBundles(RuntimePlatform platform)
		{
			switch (platform)
			{
			case RuntimePlatform.Android:
				return "Android";

			case RuntimePlatform.IPhonePlayer:
				return "iOS";

            case RuntimePlatform.WindowsPlayer:
				return "Windows";

			case RuntimePlatform.OSXPlayer:
				return "OSX";

			// Add more build platform for your own.
			// If you add more platforms, don't forget to add the same targets to GetPlatformFolderForAssetBundles(BuildTarget) function.
			default:
				return null;
			}
		}

		public IEnumerator LoadAsync<T>(string assetBundleName, string assetName, System.Action<UnityEngine.Object> onComplete)
		{
			while (!initializeComplete)
				yield return null;

			yield return StartCoroutine(LoadAsyncInternal<T>(assetBundleName.ToLower(), assetName, onComplete));
		}

        public IEnumerator LoadAsync<T>(string assetBundleName, string assetName)
        {
            while (!initializeComplete)
                yield return null;

            yield return StartCoroutine(LoadAsyncInternal<T>(assetBundleName.ToLower(), assetName, null));
        }

        IEnumerator LoadAsyncInternal<T>(string assetBundleName, string assetName, System.Action<UnityEngine.Object> onComplete)
		{
			//Debug.LogFormat("Start to load {0} at frame {1}", assetName, Time.frameCount);

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(T));
			if (request == null)
				yield break;

			yield return StartCoroutine(request);

			// Get the asset.
			UnityEngine.Object prefab = request.GetAsset();

			if (prefab != null)
			{
				//Debug.LogFormat("{0} is loaded successfully at frame {1}", assetName, Time.frameCount);

				if (onComplete != null)
					onComplete(prefab);
			}
			else
			{
				Debug.LogErrorFormat("{0} isn't loaded successfully at frame {1}", assetName, Time.frameCount);
			}
		}

        // 오직 번들 로드만 해둔다
        public void LoadOnlyBundle(string assetBundleName, System.Action<bool> onComplete)
        {
            AssetBundleManager.LoadOnlyBundle(assetBundleName.ToLower(), onComplete);
        }

        // 오직 번들 로드만 해둔다
        public IEnumerator LoadOnlyBundleAsync(string assetBundleName, System.Action<bool> onComplete)
        {
            while (!initializeComplete)
                yield return null;

            yield return StartCoroutine(AssetBundleManager.LoadOnlyBundleAsync(assetBundleName.ToLower(), onComplete));
        }

        public void Load<T>(string assetBundleName, string assetName, System.Action<UnityEngine.Object> onComplete)
		{
			LoadInternal<T>(assetBundleName.ToLower(), assetName, onComplete);
		}

		void LoadInternal<T>(string assetBundleName, string assetName, System.Action<UnityEngine.Object> onComplete)
		{
			//Debug.LogFormat("Start to load {0} at frame {1}", assetName, Time.frameCount);

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(T));
            if (request == null)
                return;

			// Get the asset.
			UnityEngine.Object prefab = request.GetAsset();

			if (prefab != null)
			{
				//Debug.LogFormat("{0} is loaded successfully at frame {1}", assetName, Time.frameCount);

				if (onComplete != null)
					onComplete(prefab);
			}
			else
			{
				Debug.LogErrorFormat("{0} isn't loaded successfully at frame {1}", assetName, Time.frameCount.ToString());
			}
		}

        public void Unload(string assetBundleName)
		{
			AssetBundleManager.UnloadAssetBundle(assetBundleName.ToLower());
		}

#if UNITY_EDITOR
		protected override void Release()
		{
            AssetBundleManager.UnloadAll();
			AssetBundleManager.LogNotUnloadingAssetBundles();
			Destroy(AssetBundleManager.instance);
		}
#endif
	}
}
