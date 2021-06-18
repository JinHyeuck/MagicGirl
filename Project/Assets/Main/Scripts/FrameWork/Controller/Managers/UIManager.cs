using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace GameBerry.UI
{
	public class UIManager : MonoSingleton<UIManager>
	{
        public Canvas screenCanvas;
        public CanvasScaler screenCanvasScaler;
        public Camera screenCanvasCamera;
        public GameObject screenCanvasContent;
        public GameObject screenCanvasGlobalContent;

        Dictionary<string, GameObject> _uis;

		public delegate void OnComplete(GameObject ui);
		Dictionary<string, System.Delegate> _onCompleteEvents;

		const string ASSET_PATH = "ContentResources";

        protected override void Init()
		{
			_uis = new Dictionary<string, GameObject>();
			_onCompleteEvents = new Dictionary<string, System.Delegate>();
        }

		protected override void Release()
		{
			UnloadAll();
		}

		public GameObject Get(string uiName)
		{
			GameObject ui;
			_uis.TryGetValue(uiName, out ui);
			return ui;
		}

		public void Load(string uiName, string contentName, OnComplete onComplete)
		{
			AddEvent(uiName, onComplete);

			GameObject ui = Get(uiName);
			if (ui == null)
			{
                var bundleName = string.Format("{0}/{1}", ASSET_PATH, contentName);
                StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>(bundleName, uiName, OnPostLoadProcess));
			}
			else
			{
				ui.SetActive(true);

				AttachToCanvas(ui);
				RaiseEvent(ui);
			}
		}

		void OnPostLoadProcess(Object o)
		{
			var ui = Instantiate(o) as GameObject;

			ui.SetActive(true);
			ui.name = o.name;

            Common.ShaderHelper.SetupShader(ui);

			AttachToCanvas(ui);

			_uis.Add(ui.name, ui);
			RaiseEvent(ui);
		}

		void AttachToCanvas(GameObject ui)
		{
			int sibling = EnumExtensions.ParseToInt<UISibling>(ui.name);

			var obj = GetGameObjectBySiblingIndex(sibling);
			ui.transform.SetParent(obj.transform, false);
			ui.SetLayerInChildren(obj.layer);

			//ui.transform.localPosition = Vector3.zero;
			ui.transform.localScale = Vector3.one;
		}

		public void BackgroundLoad(string uiName)
		{
			// TODO: Background Load 기능 추가 구현
		}

        // 현재 사용하지 않음
		public float GetProgress(string uiName)
		{
			GameObject ui;
			_uis.TryGetValue(uiName, out ui);
			if (ui != null)
				return 1.0f;

			float progress = 0.0f;
			var fullpath = string.Format("{0}{1}", ASSET_PATH, uiName);
			progress = ResourceLoader.Instance.GetProgress(fullpath);

			return progress * 0.9f; // 리소스 로딩 90%, OnPostLoadProcess()에서 후처리 10%
		}

		public void Unload(string uiName)
		{
			GameObject ui = Get(uiName);
			if (ui != null)
			{				
				Destroy(ui);
				_uis.Remove(uiName);

				var fullpath = string.Format("{0}{1}", ASSET_PATH, uiName);
				if (AssetBundleLoader.isAlive)
                    AssetBundleLoader.Instance.Unload(fullpath);
			}
		}

		public void UnloadAll()
		{
			foreach (var ui in _uis)
			{
				Destroy(ui.Value);
			}
			_uis.Clear();
		}

		void AddEvent(string uiName, OnComplete onComplete)
		{
			if (onComplete == null)
				return;

			System.Delegate events;
			if (_onCompleteEvents.TryGetValue(uiName, out events))
			{
				_onCompleteEvents[uiName] = (OnComplete)_onCompleteEvents[uiName] + onComplete;
			}
			else
			{
				_onCompleteEvents.Add(uiName, onComplete);
			}
		}

		void RaiseEvent(GameObject ui)
		{
			System.Delegate events;
			_onCompleteEvents.TryGetValue(ui.name, out events);
			if (events == null)
				return;

			var onComplete = (OnComplete)events;
			onComplete(ui);
             
			_onCompleteEvents.Remove(ui.name);
		}

		public void LastSibling(string uiName)
		{
			GameObject ui;
			_uis.TryGetValue(uiName, out ui);

			if (ui != null)
				ui.transform.SetSiblingIndex(_uis.Count);
		}

		public void SetSibling(RectTransform source, int sourceSibling)
		{
//			Debug.LogFormat("This Target SIBLING {0}-[{1}]", sourceSibling, source.name);
			bool addChild = false;

			var siblingObj = GetGameObjectBySiblingIndex(sourceSibling);
			source.gameObject.layer = siblingObj.gameObject.layer;
			Transform ctf = siblingObj.transform;

			for (int i = 0; i < ctf.childCount; ++i)
			{					
				var dest = ctf.GetChild(i);
				if (dest == source)
					continue;

				int destSibling = EnumExtensions.ParseToInt<UISibling>(dest.name);
				if (destSibling > sourceSibling)
				{
//					Debug.LogFormat("Sort SIBLING {0}-{1}-{2}", destSibling, dest.GetSiblingIndex(), dest.name);
					source.SetSiblingIndex(dest.GetSiblingIndex());			
					addChild = true;
					break;
				}
				else
				{
//					Debug.LogFormat("PASS SIBLING {0}-{1}-{2}", destSibling, dest.GetSiblingIndex(), dest.name);
					continue;
				}
			}

			if (!addChild)
			{
				source.SetAsLastSibling();
			}
		}

		GameObject GetGameObjectBySiblingIndex(int index)
		{
			GameObject obj = null;
            if (1 <= index && index < 500)
                obj = screenCanvasContent;
            else if (501 <= index && index < 1000)
                obj = screenCanvasGlobalContent;

			return obj;
		}
	}
}
