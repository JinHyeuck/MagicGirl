#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;


namespace GameBerry
{
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadedAssetBundle
	{
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;

		public LoadedAssetBundle(AssetBundle assetBundle)
		{
			m_AssetBundle = assetBundle;
			m_ReferencedCount = 1;
		}
	}

	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
	public class AssetBundleManager : MonoBehaviour
	{
		static bool initializeComplete = false;
		public static GameObject instance { get; private set; }

#if UNITY_EDITOR
		static int m_SimulateAssetBundleInEditor = -1;
		const string kSimulateAssetBundles = "SimulateAssetBundles";

        static int m_UseStreamingAssetsEditor = -1;
        const string kUseStreamingAssets = "UseStreamingAssets";
#endif

        static Dictionary<string, LoadedAssetBundle> m_LoadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        static Dictionary<Constants.SceneName, Queue<string>> m_LoadedAssetBundleNamesAtScene = new Dictionary<Constants.SceneName, Queue<string>>();

#if UNITY_EDITOR
        // Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        public static bool SimulateAssetBundleInEditor
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}

        public static bool UseStreamingAssetsEditor
        {
            get
            {
                if (m_UseStreamingAssetsEditor == -1)
                    m_UseStreamingAssetsEditor = EditorPrefs.GetBool(kUseStreamingAssets, true) ? 1 : 0;

                return m_UseStreamingAssetsEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != m_UseStreamingAssetsEditor)
                {
                    m_UseStreamingAssetsEditor = newValue;
                    EditorPrefs.SetBool(kUseStreamingAssets, value);
                }
            }
        }
#endif

        // Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
        public static LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName)
		{
			LoadedAssetBundle bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			return bundle;
		}

		// Load AssetBundleManifest.
		public static void Initialize(string manifestAssetBundleName)
		{
			if (initializeComplete)
				return;

			instance = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			instance.hideFlags = HideFlags.HideInHierarchy;
			DontDestroyOnLoad(instance);

			initializeComplete = true;
		}

		// Unload assetbundle and its dependencies.
		public static void UnloadAssetBundle(string assetBundleName)
		{
#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return;
#endif

			//Debug.LogFormat("--> {0} assetbundle(s) in memory before unloading \"{1}\"", m_LoadedAssetBundles.Count, assetBundleName);
			UnloadAssetBundleInternal(assetBundleName);
		}

		protected static int UnloadAssetBundleInternal(string assetBundleName)
		{
			LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName);
			if (bundle == null)
				return 0;

			int refCount = --bundle.m_ReferencedCount;
			if (refCount == 0)
			{
				bundle.m_AssetBundle.Unload(false);
				m_LoadedAssetBundles.Remove(assetBundleName);
				//Debug.LogFormat("AssetBundle {0} has been unloaded successfully", assetBundleName);
			}

			return refCount;
		}

		// Load asset from the given assetBundle.
		public static AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
		{
			AssetBundleLoadAssetOperation operation = null;

#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				//Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);

				Object target = null;
				for (int i = 0; i < assetPaths.Length; ++i)
				{
					target = AssetDatabase.LoadAssetAtPath(assetPaths[i], type);
					if (target != null)
						break;
				}

				if (target == null)
				{
					Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
					return null;
				}

				operation = new AssetBundleLoadAssetOperationSimulation(target);
			}
			else
#endif
			{
                Object target = GetAsset(assetBundleName, assetName);
                operation = new AssetBundleLoadAssetOperationSimulation(target);
			}

			return operation;
		}

        public static IEnumerator LoadOnlyBundleAsync(string assetBundleName, System.Action<bool> onComplete)
        {
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
            }
            else
#endif
            {
                AssetBundle assetBundle = null;

                yield return LoadAssetBundleAsync(assetBundleName, o =>
                {
                    assetBundle = o;
                });

                if (assetBundle == null)
                {
                    if (onComplete != null)
                        onComplete(false);

                    yield break;
                }
            }

            if (onComplete != null)
                onComplete(true);
        }

        public static void LoadOnlyBundle(string assetBundleName, System.Action<bool> onComplete)
        {
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
            }
            else
#endif
            {
                if (m_LoadedAssetBundles.ContainsKey(assetBundleName) == true)
                {
                    if (onComplete != null)
                        onComplete(true);

                    return;
                }

                var assetBundle = LoadAssetBundle(assetBundleName);

                if (assetBundle == null)
                {
                    if (onComplete != null)
                        onComplete(false);

                    return;
                }
            }

            if (onComplete != null)
                onComplete(true);
        }

        // Load asset from the given assetBundle.
        public static AssetBundleLoadAssetOperation LoadAsset(string assetBundleName, string assetName, System.Type type)
		{
			AssetBundleLoadAssetOperation operation = null;

#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
                    return null;
                }

                // @TODO: Now we only get the main object from the first asset. Should consider type also.
                //Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);

                Object target = null;
                for (int i = 0; i < assetPaths.Length; ++i)
                {
                    target = AssetDatabase.LoadAssetAtPath(assetPaths[i], type);
                    if (target != null)
                        break;
                }

                if (target == null)
                {
                    Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", assetName, assetBundleName);
                    return null;
                }

                operation = new AssetBundleLoadAssetOperationSimulation(target);
            }
            else
#endif
            {
                Object target = GetAsset(assetBundleName, assetName);
                operation = new AssetBundleLoadAssetOperationSimulation(target);
            }

			return operation;
		}

        protected static Object GetAsset(string assetBundleName, string assetName)
        {
            AssetBundle assetBundle = LoadAssetBundle(assetBundleName);

            if (assetBundle != null)
                return assetBundle.LoadAsset<Object>(assetName);

            return null;
        }

        private static AssetBundle LoadAssetBundle(string assetBundleName)
        {
            if (m_LoadedAssetBundles.ContainsKey(assetBundleName) == true)
            {
                return m_LoadedAssetBundles[assetBundleName].m_AssetBundle;
            }

            string path = "";

#if UNITY_EDITOR
            if (UseStreamingAssetsEditor == true)
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
            else
                path = string.Format("{0}/AssetBundle/{1}", Application.persistentDataPath, assetBundleName);

#else
            if (Debug.isDebugBuild == true)
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
            else
            {
#if UNITY_ANDROID || UNITY_IPHONE
                path = string.Format("{0}/AssetBundle/{1}", Application.persistentDataPath, assetBundleName);
#else
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
#endif
            }
#endif



            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
                return null;

            m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle));
            AddLoadedAssetBundleNamesAtScene(assetBundleName);
            return assetBundle;
        }

        private static IEnumerator LoadAssetBundleAsync(string assetBundleName, System.Action<AssetBundle> onComplete)
        {
            if (m_LoadedAssetBundles.ContainsKey(assetBundleName) == true)
            {
                if (onComplete != null)
                    onComplete(m_LoadedAssetBundles[assetBundleName].m_AssetBundle);

                yield break;
            }

            string path = "";

#if UNITY_EDITOR
            if (UseStreamingAssetsEditor == true)
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
            else
                path = string.Format("{0}/AssetBundle/{1}", Application.persistentDataPath, assetBundleName);

#else
            if (Debug.isDebugBuild == true)
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
            else
            {
#if UNITY_ANDROID || UNITY_IPHONE
                path = string.Format("{0}/AssetBundle/{1}", Application.persistentDataPath, assetBundleName);
#else
                path = string.Format("{0}/AssetBundle/{1}", Application.streamingAssetsPath, assetBundleName);
#endif
            }
#endif

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);

            if(request == null)
                yield break;

            yield return request;

            AssetBundle assetBundle = request.assetBundle;
            if (assetBundle == null)
            {
                if (onComplete != null)
                    onComplete(null);

                yield break;
            }

            m_LoadedAssetBundles.Add(assetBundleName, new LoadedAssetBundle(assetBundle));
            AddLoadedAssetBundleNamesAtScene(assetBundleName);

            if (onComplete != null)
                onComplete(assetBundle);
        }

        private static void AddLoadedAssetBundleNamesAtScene(string assetBundleName)
        {
            if (m_LoadedAssetBundleNamesAtScene.ContainsKey(Managers.SceneManager.Instance.nowScene) == false)
            {
                m_LoadedAssetBundleNamesAtScene.Add(Managers.SceneManager.Instance.nowScene, new Queue<string>());
            }
            m_LoadedAssetBundleNamesAtScene[Managers.SceneManager.Instance.nowScene].Enqueue(assetBundleName);
        }

#if UNITY_EDITOR
        public static void LogNotUnloadingAssetBundles()
		{
			foreach (var keyValue in m_LoadedAssetBundles)
			{
				var assetBundle = keyValue.Value;
				if (assetBundle != null && assetBundle.m_AssetBundle != null)
				{
					Debug.LogWarningFormat("Not unloading assetBundle : \"{0}\", RefCount : {1}", keyValue.Key, assetBundle.m_ReferencedCount);
				}
			}
		}
#endif

        public static void UnloadSceneLoadedBundles(Constants.SceneName sceneName)
        {
            if (m_LoadedAssetBundleNamesAtScene.ContainsKey(sceneName) == false)
                return;

            Queue<string> bundlenames = m_LoadedAssetBundleNamesAtScene[sceneName];

            while(bundlenames.Count > 0)
                UnloadAssetBundle(bundlenames.Dequeue());
        }

		public static void UnloadAll()
		{
            List<string> LoadedBundleKey = new List<string>(m_LoadedAssetBundles.Keys);

            for (int i = 0; i < LoadedBundleKey.Count; ++i)
                UnloadAssetBundle(LoadedBundleKey[i]);
		}
	}
}
