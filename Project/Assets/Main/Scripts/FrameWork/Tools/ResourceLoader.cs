using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GameBerry
{
	public class ResourceLoader : MonoSingleton<ResourceLoader>
	{
		public class LoadedResource
		{
			public UnityEngine.Object m_Resource;
			public int m_ReferencedCount;

			public LoadedResource(UnityEngine.Object resource)
			{
				m_Resource = resource;
				m_ReferencedCount = 1;
			}
		}

		Dictionary<string, LoadedResource> loadedResources = new Dictionary<string, LoadedResource>(); // 동기 비동기로 로드된 오브젝트들이 들어감
		Dictionary<string, ResourceRequest> inProgressOperations = new Dictionary<string, ResourceRequest>(); // 비동기로 로드된 오브젝트들만 들어감

        // 비동기 로드
        public IEnumerator LoadAsync<T>(string resourceName, System.Action<UnityEngine.Object> onComplete)
		{
			//Debug.LogFormat("Start to load {0} at frame {1}", resourceName, Time.frameCount);

			while (inProgressOperations.ContainsKey(resourceName))
				yield return null;

			if (loadedResources.ContainsKey(resourceName))
			{
				var resource = loadedResources[resourceName];
				if (resource != null && resource.m_Resource != null)
				{
					resource.m_ReferencedCount++;

					if (onComplete != null)
						onComplete(resource.m_Resource);
				}
				else
				{
					Debug.LogError("Resource already loaded but actual data is null. Name: " + resourceName);
				}
			}
			else
			{
				ResourceRequest request = Resources.LoadAsync(resourceName, typeof(T));
				inProgressOperations.Add(resourceName, request);

				yield return request;

				inProgressOperations.Remove(resourceName);

				if (request.asset != null)
				{
					var resource = new LoadedResource(request.asset);
					loadedResources.Add(resourceName, resource);

					if (onComplete != null)
						onComplete(request.asset);
				}
				else
				{
					Debug.LogError("Asynchronously loaded resource is null. Name: " + resourceName);
				}
			}
		}

        // 동기 로드
        public void Load<T>(string resourceName, System.Action<UnityEngine.Object> onComplete)
        {
            if (loadedResources.ContainsKey(resourceName))
            {
                var resource = loadedResources[resourceName];
                if (resource != null && resource.m_Resource != null)
                {
                    resource.m_ReferencedCount++;

                    if (onComplete != null)
                        onComplete(resource.m_Resource);
                }
                else
                {
                    Debug.LogError("Resource already loaded but actual data is null. Name: " + resourceName);
                }
            }
            else
            {
                Object request = Resources.Load(resourceName, typeof(T));
                if (request != null)
                {
                    var resource = new LoadedResource(request);
                    loadedResources.Add(resourceName, resource);

                    if (onComplete != null)
                        onComplete(request);
                }
                else
                {
                    Debug.LogError("loaded resource is null. Name: " + resourceName);
                }
            }
        }

		public float GetProgress(string resourceName)
		{
			if (loadedResources.ContainsKey(resourceName))
				return 1.0f;

			ResourceRequest request;
			inProgressOperations.TryGetValue(resourceName, out request);
			if (request != null)
			{
				return request.progress;
			}

			return 0.0f;
		}

		public void Unload(string resourceName)
		{
			return;
		}

		public void UnloadAll()
		{
			loadedResources.Clear();
			Resources.UnloadUnusedAssets();
		}

#if UNITY_EDITOR
		void LogNotUnloadingResources()
		{
			foreach (var resPair in loadedResources)
			{
				var res = resPair.Value;
				if (res != null && res.m_Resource != null)
				{
					Debug.LogWarningFormat("Not unloading resource : \"{0}\", RefCount : {1}", resPair.Key, res.m_ReferencedCount);
				}
			}
		}

		protected override void Release()
		{
			UnloadAll();
			LogNotUnloadingResources();
		}
#endif
	}
}
