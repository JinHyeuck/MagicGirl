using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class CAssetBundleManager : MonoBehaviour
{
    private static CAssetBundleManager instance = null;
    public static CAssetBundleManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(CAssetBundleManager)) as CAssetBundleManager;
                if (instance == null)
                {
                    instance = new GameObject("AssetBundleManager", typeof(CAssetBundleManager)).GetComponent<CAssetBundleManager>();
                }
            }
            return instance;
        }
    }
    // 번들 다운 받을 서버의 주소(필자는 임시방편으로 로컬 파일 경로 쓸 것임)
    private const string bundleURL = "https://d29631ebvl5ku4.cloudfront.net/kimegg/";
    private string[] bundleName = { "popup" };
    private string bundlePath = "";
    private int index;
    [SerializeField] TextMeshProUGUI loadingText;

    private Dictionary<string, AudioClip> dicAudio = new Dictionary<string, AudioClip>();
    private Dictionary<string, GameObject> dicGameObject = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> dicSprite = new Dictionary<string, Sprite>();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

#if UNITY_ANDROID && !UNITY_EDITOR
        bundlePath = Application.persistentDataPath + "/";
#elif UNITY_IOS && !UNITY_EDITOR
//filePath = Application.dataPath + "/Raw";
bundlePath = Application.persistentDataPath + "/";
#else
        bundlePath = Application.persistentDataPath + "/";
#endif
    }

    public void setNewBundleData()
    {
        index = 0;
        StartCoroutine(onStartDownloadAssets());
    }


    private IEnumerator onStartDownloadAssets()
    {        
        while (!Caching.ready)
            yield return null;

        while (bundleName.Length > index)
        {
            UnityWebRequest www = UnityWebRequest.Get(bundleURL + bundleName[index]);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                File.WriteAllBytes(Path.Combine(bundlePath, bundleName[index]), www.downloadHandler.data);
                Debug.Log("파일이 "+ bundlePath +"에 저장되었습니다.");
            }
            yield return new WaitForSeconds(0.1f);

            loadingText.text = string.Format("리소스를 불러오는중...({0}/{1})", index++, bundleName.Length);
        }
        //MessageManager.Instance.sendMessage(MESSAGE_TYPE.Refresh, RECIVER_TYPE.Title);
    }


    public GameObject getAssetGameObject(string type, string resourcename)
    {
        GameObject obj = null;
        if (!dicGameObject.ContainsKey(resourcename))
        {
//#if UNITY_EDITOR
            obj = Resources.Load<GameObject>("Prefabs/" + type + "/" + resourcename);
//#elif !UNITY_EDITOR && UNITY_ANDROID || UNITY_IOS
//            obj = onStartGameObjectAsset(type, resourcename);
//#endif
        }

        return obj;
    }


    public Sprite getAssetSprite(string type, string resourcename)
    {
        Sprite sprite = null;

        if (!dicSprite.ContainsKey(resourcename))
        {
//#if UNITY_EDITOR
            sprite = Resources.Load<Sprite>(type + "/" + resourcename);
//#elif !UNITY_EDITOR && UNITY_ANDROID || UNITY_IOS
//            sprite = onStartSpriteAsset(type, resourcename);
//#endif      
        }
        return sprite;
    }

    
    public AudioClip getAssetAudioClip(string type, string resourcename)
    {
        if (!dicAudio.ContainsKey(resourcename))
        {
            AudioClip audio;
//#if UNITY_EDITOR
            audio = Resources.Load(string.Format("{0}/{1}", type, resourcename)) as AudioClip;
//#elif !UNITY_EDITOR && UNITY_ANDROID || UNITY_IOS
//            audio = onStartAudioAsset(type, resourcename);
//#endif
            Debug.Log("오디오 : " + audio.name);
            dicAudio.Add(resourcename, audio);
        }

        return dicAudio[resourcename];
    }

    private GameObject onStartGameObjectAsset(string type, string resourcename)
    {
        GameObject obj;
        
        var filestream = new FileStream(Path.Combine(bundlePath, type), FileMode.Open, FileAccess.Read);
        var bundleloadrequest = AssetBundle.LoadFromStreamAsync(filestream);        
        var loadbundle = bundleloadrequest.assetBundle;
        if (loadbundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }

        var assetLoadRequest = loadbundle.LoadAssetAsync<GameObject>(resourcename);        
        obj = assetLoadRequest.asset as GameObject;
        loadbundle.Unload(false);
        return obj;
    }


    private Sprite onStartSpriteAsset(string type, string resourcename)
    {
        Sprite sprite = null;
        var filestream = new FileStream(Path.Combine(Application.streamingAssetsPath, type), FileMode.Open, FileAccess.Read);
        var bundleloadrequest = AssetBundle.LoadFromStreamAsync(filestream);
        var loadbundle = bundleloadrequest.assetBundle;
        if (loadbundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }

        var assetLoadRequest = loadbundle.LoadAssetAsync<Sprite>(resourcename);
        sprite = assetLoadRequest.asset as Sprite;
        loadbundle.Unload(false);

        return sprite;
    }

    private AudioClip onStartAudioAsset(string type, string resourcename)
    {
        AudioClip audio = null;
        var filestream = new FileStream(Path.Combine(Application.streamingAssetsPath, type), FileMode.Open, FileAccess.Read);
        var bundleloadrequest = AssetBundle.LoadFromStreamAsync(filestream);
        var loadbundle = bundleloadrequest.assetBundle;
        if (loadbundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
        }

        var assetLoadRequest = loadbundle.LoadAssetAsync<AudioClip>(resourcename);
        audio = assetLoadRequest.asset as AudioClip;
        loadbundle.Unload(false);

        return audio;
    }



}
