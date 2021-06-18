using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameBerry.Contents
{
    public class ContentsLoader : MonoSingleton<ContentsLoader>
    {
        Dictionary<string, IContent> _contentsMap = new Dictionary<string, IContent>();
        List<string> _inLoadProgressing = new List<string>();

        public delegate void OnComplete(GameObject ui);
        Dictionary<string, System.Delegate> _completeEventMap = new Dictionary<string, System.Delegate>();

        private bool _loadComplete = false;

        public IEnumerator Load(string contentName, OnComplete loadComplete)
        {
            if (_contentsMap.ContainsKey(contentName))
            {
                if (loadComplete != null)
                    loadComplete(_contentsMap[contentName].gameObject);

                yield break;
            }

            if (_inLoadProgressing.Contains(contentName))
            {
                AddEvent(contentName, loadComplete);
                yield break;
            }
            else
            {
                _inLoadProgressing.Add(contentName);
                _loadComplete = false;

                var fullpath = string.Format("Contents/{0}", contentName);
                yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>(fullpath, contentName, o => SetupContent(o, contentName, loadComplete)));

                while (!_loadComplete)
                    yield return null;

                _inLoadProgressing.Remove(contentName);
            }
        }

        void SetupContent(Object o, string contentName, OnComplete loadComplete)
        {
            if (!_contentsMap.ContainsKey(contentName))
            {
                var content = Instantiate(o) as GameObject;
                content.name = contentName;
                content.transform.SetParent(gameObject.transform);

                var contentScript = content.GetComponent<IContent>();
                contentScript.Load(OnLoadComplete);
                AddEvent(contentName, loadComplete);

                _contentsMap.Add(contentName, contentScript);
            }
        }

        void OnLoadComplete(GameObject content)
        {
            _loadComplete = true;
            RaiseEvent(content);
        }

        void AddEvent(string contentName, OnComplete loadComplete)
        {
            if (loadComplete == null)
                return;

            System.Delegate events;
            if (_completeEventMap.TryGetValue(contentName, out events))
            {
                _completeEventMap[contentName] = (OnComplete)_completeEventMap[contentName] + loadComplete;
            }
            else
            {
                _completeEventMap.Add(contentName, loadComplete);
            }
        }

        void RaiseEvent(GameObject content)
        {
            System.Delegate events;
            _completeEventMap.TryGetValue(content.name, out events);
            if (events == null)
                return;

            var onComplete = (OnComplete)events;
            onComplete(content);

            _completeEventMap.Remove(content.name);
        }

        public void Unload(string contentName)
        {
            IContent contents;
            _contentsMap.TryGetValue(contentName, out contents);
            if (contents != null && !contents.dontDestroy)
            {
                contents.Unload();
                Destroy(contents.gameObject);

                _contentsMap.Remove(contentName);

                var fullpath = string.Format("Contents/{0}", contentName);
                if (AssetBundleLoader.isAlive)
                    AssetBundleLoader.Instance.Unload(fullpath);
            }
        }

        public void ForceUnload(string contentName)
        {
            IContent contents;
            _contentsMap.TryGetValue(contentName, out contents);
            if (contents != null)
            {
                contents.Unload();
                Destroy(contents.gameObject);

                _contentsMap.Remove(contentName);

                var fullpath = string.Format("Contents/{0}", contentName);
                AssetBundleLoader.Instance.Unload(fullpath);
            }
        }

        public void UnloadAll()
        {
            foreach (var contents in _contentsMap)
            {
                contents.Value.Unload();
                Destroy(contents.Value.gameObject);
            }

            _contentsMap.Clear();
        }

        public bool IsActiveContent<T>() where T : IContent
        {
            IContent content;
            _contentsMap.TryGetValue(typeof(T).Name, out content);

            return content != null && content.isActive;
        }
    }
}
