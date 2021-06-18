using UnityEngine;
using System.Collections;


namespace GameBerry
{
	public abstract class AssetBundleLoadOperation : IEnumerator
	{
		public object Current
		{
			get { return null; }
		}
		public bool MoveNext()
		{
			return !IsDone();
		}

		public void Reset()
		{
		}

		abstract public bool Update();

		abstract public bool IsDone();
	}


	public class AssetBundleLoadLevelSimulationOperation : AssetBundleLoadOperation
	{
		public AssetBundleLoadLevelSimulationOperation()
		{
		}

		public override bool Update()
		{
			return false;
		}

		public override bool IsDone()
		{
			return true;
		}
	}


	public class AssetBundleLoadLevelOperation : AssetBundleLoadOperation
	{
		protected string m_AssetBundleName;
		protected string m_LevelName;
		protected bool m_IsAdditive;
		protected AsyncOperation m_Request;


		public AssetBundleLoadLevelOperation(string assetbundleName, string levelName, bool isAdditive)
		{
			m_AssetBundleName = assetbundleName;
			m_LevelName = levelName;
			m_IsAdditive = isAdditive;
		}

		public override bool Update()
		{
			if (m_Request != null)
				return false;

			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName);
			if (bundle != null)
			{
				if (m_IsAdditive)
					m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
				else
					m_Request = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_LevelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
				return false;
			}
			else
				return true;
		}

		public override bool IsDone()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null)
				return true;

			return m_Request != null && m_Request.isDone;
		}
	}


	public abstract class AssetBundleLoadAssetOperation : AssetBundleLoadOperation
	{
		public abstract T GetAsset<T>() where T : UnityEngine.Object;
		public abstract UnityEngine.Object GetAsset();
	}


	public class AssetBundleLoadAssetOperationSimulation : AssetBundleLoadAssetOperation
	{
		UnityEngine.Object m_SimulatedObject;


		public AssetBundleLoadAssetOperationSimulation(UnityEngine.Object simulatedObject)
		{
			m_SimulatedObject = simulatedObject;
		}

		public override T GetAsset<T>()
		{
			return m_SimulatedObject as T;
		}

		public override UnityEngine.Object GetAsset()
		{
			return m_SimulatedObject;
		}

		public override bool Update()
		{
			return false;
		}

		public override bool IsDone()
		{
			return true;
		}
	}


	public class AssetBundleLoadAssetOperationFull : AssetBundleLoadAssetOperation
	{
		protected string m_AssetBundleName;
		protected string m_AssetName;
		protected System.Type m_Type;
		protected AssetBundleRequest m_Request = null;


		public AssetBundleLoadAssetOperationFull(string bundleName, string assetName, System.Type type)
		{
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
		}

		public override T GetAsset<T>()
		{
			if (m_Request != null && m_Request.isDone)
				return m_Request.asset as T;
			else
				return null;
		}

		public override UnityEngine.Object GetAsset()
		{
			if (m_Request != null && m_Request.isDone)
				return m_Request.asset;
			else
				return null;
		}

		// Returns true if more Update calls are required.
		public override bool Update()
		{
			if (m_Request != null)
				return false;

			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName);
			if (bundle != null)
			{
				m_Request = bundle.m_AssetBundle.LoadAssetAsync(m_AssetName, m_Type);
				return false;
			}
			else
			{
				return true;
			}
		}

		public override bool IsDone()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Request == null)
				return true;

			return m_Request != null && m_Request.isDone;
		}
	}


	public class AssetBundleLoadAssetOperationFullNonAsync : AssetBundleLoadAssetOperation
	{
		protected string m_AssetBundleName;
		protected string m_AssetName;
		protected System.Type m_Type;
		protected Object m_Asset = null;


		public AssetBundleLoadAssetOperationFullNonAsync(string bundleName, string assetName, System.Type type)
		{
			m_AssetBundleName = bundleName;
			m_AssetName = assetName;
			m_Type = type;
		}

		public override T GetAsset<T>()
		{
			return m_Asset as T;
		}

		public override UnityEngine.Object GetAsset()
		{
			return m_Asset;
		}

		// Returns true if more Update calls are required.
		public override bool Update()
		{
			if (m_Asset != null)
				return false;

			LoadedAssetBundle bundle = AssetBundleManager.GetLoadedAssetBundle(m_AssetBundleName);
			if (bundle != null)
			{
				m_Asset = bundle.m_AssetBundle.LoadAsset(m_AssetName, m_Type);
				return false;
			}
			else
			{
				return true;
			}
		}

		public override bool IsDone()
		{
			// Return if meeting downloading error.
			// m_DownloadingError might come from the dependency downloading.
			if (m_Asset == null)
				return true;

			return m_Asset != null;
		}
	}
}
