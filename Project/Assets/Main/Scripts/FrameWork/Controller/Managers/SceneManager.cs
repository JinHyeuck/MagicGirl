using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameBerry;
using GameBerry.Scene;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace GameBerry.Managers
{
    public class SceneManager : MonoSingleton<SceneManager>
    {
        public Constants.SceneName startScene;
        public Constants.SceneName nowScene;

        // TODO: 한번에 여러 씬을 로딩하는 경우를 위해 구현한 부분인데 이제는 그럴일이 없다. 수정하자.
        List<GameObject> _inSettingsScenes;
        int _inSettingsScenesCount;

        Dictionary<Constants.SceneName, GameObject> _scenes;
        LinkedList<Constants.SceneName> _showList;

        Transform _root;

        //App초기화가 되었는지
        bool completeAppInit = false;

        #region Initialize
        //------------------------------------------------------------------------------------
        protected override void Init()
        {
            _inSettingsScenes = new List<GameObject>();
            _scenes = new Dictionary<Constants.SceneName, GameObject>();
            _showList = new LinkedList<Constants.SceneName>();

            _root = GetComponent<Transform>();

            if (!completeAppInit)
                StartCoroutine(InitializeApp());
            else
                LoadStartScene();
        }
        //------------------------------------------------------------------------------------

        //[Serializable]
        //public class asdf
        //{
        //    public int a;
        //    public string b;
        //    public double c;
        //}

        //[Serializable]
        //public class asdfconta
        //{
        //    public List<asdf> viewList = new List<asdf>();
        //    public List<asdf> viewList1 = new List<asdf>();
        //    public List<asdf> viewList2 = new List<asdf>();
        //}

        //public asdfconta container = new asdfconta();

        //[ContextMenu("testsa")]
        //private void testsa()
        //{
        //    asdfconta asdflist = new asdfconta();

        //    for (int i = 0; i < 10; ++i)
        //    {
        //        asdflist.viewList.Add(new asdf { a = i, b = i.ToString(), c = i });
        //    }

        //    for (int i = 0; i < 10; ++i)
        //    {
        //        asdflist.viewList1.Add(new asdf { a = i, b = i.ToString(), c = i });
        //    }

        //    for (int i = 0; i < 10; ++i)
        //    {
        //        asdflist.viewList2.Add(new asdf { a = i, b = i.ToString(), c = i });
        //    }

        //    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        //    sw.Start();

        //    string s = LitJson.JsonMapper.ToJson(asdflist);
        //    sw.Stop();
        //    Debug.Log(s);
        //    Debug.Log(sw.ElapsedMilliseconds.ToString());



        //    LitJson.JsonData chartJson = LitJson.JsonMapper.ToObject(s);

        //    var raw = chartJson["viewList"];

        //    for (int i = 0; i < raw.Count; ++i)
        //    {
        //        asdf data = null;

        //        if (raw[i] != null)
        //        {
        //            data = new asdf
        //            {
        //                a = raw[i]["a"].ToString().FastStringToInt(),

        //                b = raw[i]["b"].ToString(),

        //                c = System.Convert.ToDouble(raw[i]["c"].ToString()),
        //            };
        //        }

        //        container.viewList.Add(data);
        //    }

        //    raw = chartJson["viewList1"];

        //    for (int i = 0; i < raw.Count; ++i)
        //    {
        //        asdf data = null;

        //        if (raw[i] != null)
        //        {
        //            data = new asdf
        //            {
        //                a = raw[i]["a"].ToString().FastStringToInt(),

        //                b = raw[i]["b"].ToString(),

        //                c = System.Convert.ToDouble(raw[i]["c"].ToString()),
        //            };
        //        }

        //        container.viewList1.Add(data);
        //    }

        //    raw = chartJson["viewList2"];

        //    for (int i = 0; i < raw.Count; ++i)
        //    {
        //        asdf data = null;

        //        if (raw[i] != null)
        //        {
        //            data = new asdf
        //            {
        //                a = raw[i]["a"].ToString().FastStringToInt(),

        //                b = raw[i]["b"].ToString(),

        //                c = System.Convert.ToDouble(raw[i]["c"].ToString()),
        //            };
        //        }

        //        container.viewList2.Add(data);
        //    }








        //    Dictionary<EquipmentType, Dictionary<int, asdf>> asdfdic = new Dictionary<EquipmentType, Dictionary<int, asdf>>();

        //    asdfdic.Add(EquipmentType.Weapon, new Dictionary<int, asdf>());
        //    asdfdic.Add(EquipmentType.Necklace, new Dictionary<int, asdf>());
        //    asdfdic.Add(EquipmentType.Ring, new Dictionary<int, asdf>());

        //    foreach (KeyValuePair<EquipmentType, Dictionary<int, asdf>> pair in asdfdic)
        //    {
        //        for (int i = 0; i < 10; ++i)
        //        {
        //            asdf asd = new asdf { a = i, b = i.ToString(), c = i };
        //            pair.Value.Add(asd.a, asd);
        //        }
        //    }
        //    sw.Start();

        //    string d = LitJson.JsonMapper.ToJson(asdfdic);
        //    sw.Stop();
        //    Debug.Log(d);
        //    Debug.Log(sw.ElapsedMilliseconds.ToString());

        //    chartJson = LitJson.JsonMapper.ToObject(d);

        //    Dictionary<EquipmentType, Dictionary<int, asdf>> viewdic = new Dictionary<EquipmentType, Dictionary<int, asdf>>();

        //    for (int i = 0; i < (int)EquipmentType.Max; ++i)
        //    {
        //        raw = chartJson[((EquipmentType)i).ToString()];

        //        Dictionary<int, asdf> templist = new Dictionary<int, asdf>();

        //        for (int j = 0; j < raw.Count; ++j)
        //        {
        //            asdf data = null;

        //            if (raw[j] != null)
        //            {
        //                data = new asdf
        //                {
        //                    a = raw[j]["a"].ToString().FastStringToInt(),

        //                    b = raw[j]["b"].ToString(),

        //                    c = System.Convert.ToDouble(raw[j]["c"].ToString()),
        //                };
        //            }

        //            templist.Add(data.a, data);
        //        }

        //        viewdic.Add((EquipmentType)i, templist);
        //    }
        //}

        void LoadStartScene()
        {
            if (startScene == Constants.SceneName.None)
                return;

            nowScene = startScene;
            var scene = GetRoot(nowScene);
            if (scene == null)
            {
                var fullpath = string.Format("Scenes/{0}", nowScene);
                StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>(fullpath, nowScene.ToString(), o => OnPostLoadProcess(o)));
            }
        }
        //------------------------------------------------------------------------------------
        protected override void Release()
        {
            UnloadAll();
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        // 혹시라도 yield return StartCoroutine 사용해서 싱글톤 초기화를 할 수 있기 때문에 IEnumerator로 냅둠
        IEnumerator InitializeApp()
        {
            GameObject AppObj = new GameObject("AppInitialize");
            AppInitialize AppInit = AppObj.AddComponent<AppInitialize>();

            AppInit.Init();

            if(AppInit.InitBackEnd() == false)
                yield break;

            bool completeLogin = false;

            AppInit.DoLogin(() =>
            {
                completeLogin = true;
            });

            while (completeLogin == false)
                yield return null;


            Debug.Log("End InitializeApp");

            AppInit.Release();

            Destroy(AppObj);

            completeAppInit = true;

            LoadStartScene();
        }
        //------------------------------------------------------------------------------------
        GameObject GetRoot(Constants.SceneName sceneName)
        {
            GameObject scene;
            _scenes.TryGetValue(sceneName, out scene);
            return scene;
        }
        //------------------------------------------------------------------------------------
        public T GetScript<T>(Constants.SceneName sceneName)
        {
            var scene = GetRoot(sceneName);
            if (scene != null)
                return scene.GetComponent<T>();

            return default(T);
        }
        //------------------------------------------------------------------------------------
        public T GetScript<T>()
        {
            var scene = GetRoot(EnumExtensions.Parse<Constants.SceneName>(typeof(T).Name.Replace("Scene", "")));
            if (scene != null)
                return scene.GetComponent<T>();

            return default(T);
        }
        //------------------------------------------------------------------------------------
        #region Load/Unload
        //------------------------------------------------------------------------------------
        public bool Load(Constants.SceneName sceneName, bool unload = true)
        {
            if (sceneName == nowScene)
                return false;

            LoadRoot(sceneName, unload);
            return true;
        }
        //------------------------------------------------------------------------------------
        void LoadRoot(Constants.SceneName sceneName, bool unload)
        {
            if (unload)
                UnloadAll();

            nowScene = sceneName;
            var scene = GetRoot(sceneName);
            if (scene == null)
            {
                var fullpath = string.Format("Scenes/{0}", sceneName);
                StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>(fullpath, sceneName.ToString(), o => OnPostLoadProcess(o)));
            }
            else
            {
                var sceneScript = scene.GetComponent<IScene>();
                SetupScene(scene, sceneScript.contentsList);
            }
        }
        //------------------------------------------------------------------------------------
        void OnPostLoadProcess(UnityEngine.Object o)
        {
            var scene = Instantiate(o) as GameObject;

            var sceneScript = scene.GetComponent<IScene>();
            scene.name = sceneScript.sceneName.ToString();
            scene.transform.SetParent(_root);

            _scenes.Add(sceneScript.sceneName, scene);
            SetupScene(scene, sceneScript.contentsList);
        }
        //------------------------------------------------------------------------------------
        void SetupScene(GameObject scene, List<string> enterContentList)
        {
            _inSettingsScenes.Add(scene);
            _inSettingsScenesCount++;

            scene.SetActive(true);

            var sceneScript = scene.GetComponent<IScene>();
            sceneScript.LoadAssets(enterContentList,
                () =>
                {
                    BringToTop(sceneScript.sceneName);
                    _inSettingsScenes.Remove(scene);
                });
        }
        //------------------------------------------------------------------------------------
        public void Unload(Constants.SceneName sceneName)
        {
            var scene = GetRoot(sceneName);
            if (scene != null)
            {
                scene.GetComponent<IScene>().Unload();

                _scenes.Remove(sceneName);
                _showList.Remove(sceneName);

                var fullpath = string.Format("Scenes/{0}", scene.name);
                if (AssetBundleLoader.isAlive)
                    AssetBundleLoader.Instance.Unload(fullpath);
            }
        }
        //------------------------------------------------------------------------------------
        public void UnloadAll()
        {
            LinkedListNode<Constants.SceneName> node;

            while (true)
            {
                node = _showList.First;
                if (node == null)
                    break;

                Unload(node.Value);
            }
        }
        //------------------------------------------------------------------------------------
        #endregion
        //------------------------------------------------------------------------------------
        public void Show(Constants.SceneName sceneName)
        {
            BringToTop(sceneName);
        }
        //------------------------------------------------------------------------------------
        void BringToTop(Constants.SceneName sceneName)
        {
            _showList.Remove(sceneName);
            _showList.AddFirst(sceneName);

            var scene = GetRoot(sceneName);
            scene.transform.SetAsFirstSibling();
        }
        //------------------------------------------------------------------------------------
        public void OnApplicationQuit()
        {
            Application.Quit();
        }
        //------------------------------------------------------------------------------------
        public void OnApplicationRestart()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
                intent.Call<AndroidJavaObject>("setFlags", 0x20000000);//Intent.FLAG_ACTIVITY_SINGLE_TOP

                AndroidJavaClass pendingIntent = new AndroidJavaClass("android.app.PendingIntent");
                AndroidJavaObject contentIntent = pendingIntent.CallStatic<AndroidJavaObject>("getActivity", currentActivity, 0, intent, 0x8000000); //PendingIntent.FLAG_UPDATE_CURRENT = 134217728 [0x8000000]
                AndroidJavaObject alarmManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "alarm");
                AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
                long currentTime = system.CallStatic<long>("currentTimeMillis");
                alarmManager.Call("set", 1, currentTime + 1000, contentIntent); // android.app.AlarmManager.RTC = 1 [0x1]

                Debug.LogError("alarm_manager set time " + currentTime + 1000);
                currentActivity.Call("finish");

                AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
                int pid = process.CallStatic<int>("myPid");
                process.CallStatic("killProcess", pid);
            }
        }
        //------------------------------------------------------------------------------------
        public void GoStore()
        {
#if UNITY_ANDROID
            Application.OpenURL(string.Format("market://details?id={0}", Application.identifier));
#elif UNITY_IPHONE
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1234567890")
#else
            Application.OpenURL(string.Format("http://play.google.com/store/apps/details?id={0}", Application.identifier));
#endif
            OnApplicationQuit();
        }
        //------------------------------------------------------------------------------------
    }
}
