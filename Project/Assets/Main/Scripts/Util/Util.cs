using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameBerry
{
    public static class Util
    {
        static int lastCallFrameCount;
        static float interpolationFactor;

        public static float InterpolationFactor
        {
            get
            {
                if (Time.frameCount != lastCallFrameCount)
                {
                    interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;
                    lastCallFrameCount = Time.frameCount;
                }

                return interpolationFactor;
            }
        }

        public static int FixedUpdateCount
        {
            get
            {
                return Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
            }
        }

        public static Sprite ConvertTexture2DToSprite(Texture2D tex)
        {
            /*
            if (tex == null)
                return new Sprite();
            */
            Rect rect = new Rect(0, 0, tex.width, tex.height);

            return Sprite.Create(tex, rect, new Vector2(0, 0));
        }

        /// <summary>
        /// 리턴값이 false면 message에 어떤 에러메세지인지 알려준다.
        /// <returns></returns>
        public static bool CheckNickName(string nickname, System.Action<string> message)
        {
            if (string.IsNullOrEmpty(nickname))
            {
                if (message != null)
                    message("닉네임을 입력해 주세요.");
                return false;
            }
            else if (nickname.Contains(" "))
            {
                if (message != null)
                    message("빈문자를 사용할 수 없습니다.");
                return false;
            }
            else if (nickname.Length < 3 || nickname.Length > 25)
            {
                if (message != null)
                    message("3글자 이상 25글자 이하");
                return false;
            }

            return true;
        }

        public static void GetSpriteOnAssetBundle(string path, string imagenane, out Sprite outSprite)
        {
            Sprite texture = null;
            AssetBundleLoader.Instance.Load<Sprite>(path, imagenane, o =>
            {
#if (UNITY_EDITOR || UNITY_EDITOR_64)
                if (AssetBundleManager.SimulateAssetBundleInEditor == false)
                {
                    if (o.GetType().ToString().Contains("Sprite"))
                        texture = o as Sprite;
                    else
                        texture = ConvertTexture2DToSprite(o as Texture2D);
                }
                else
                {
                    texture = o as Sprite;
                }
#else
            if (o.GetType().ToString().Contains("Sprite"))
                texture = o as Sprite;
            else
                texture = ConvertTexture2DToSprite(o as Texture2D);
#endif
            });

            outSprite = texture;
        }

        public static Sprite GetSpriteOnAssetBundle(string path, string imagenane)
        {
            Sprite texture = null;
            AssetBundleLoader.Instance.Load<Sprite>(path, imagenane, o =>
            {
#if (UNITY_EDITOR || UNITY_EDITOR_64)
                if (AssetBundleManager.SimulateAssetBundleInEditor == false)
                {
                    if (o.GetType().ToString().Contains("Sprite"))
                        texture = o as Sprite;
                    else
                        texture = ConvertTexture2DToSprite(o as Texture2D);
                }
                else
                {
                    texture = o as Sprite;
                }
#else
            if (o.GetType().ToString().Contains("Sprite"))
                texture = o as Sprite;
            else
                texture = ConvertTexture2DToSprite(o as Texture2D);
#endif
            });

            return texture;
        }

//        public static WebViewObject StartWebView(RectTransform target, string url)
//        {
//#if UNITY_ANDROID || UNITY_IPHONE
//            try
//            {
//                string strUrl = url;
//                WebViewObject webViewObject;
//                webViewObject = target.gameObject.AddComponent<WebViewObject>();
//                webViewObject.Init((msg) =>
//                {
//                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
//                });

//                webViewObject.LoadURL(strUrl);
//                webViewObject.SetVisibility(true);

//                float y = (float)Screen.height / 720.0f;
//                Vector2 size = target.rect.size;
//                size = size * y;
//                float width = (Screen.width - size.x) / 2.0f;
//                float height = (Screen.height - size.y) / 2.0f;

//                Vector3 pos = UI.UIManager.Instance.screenCanvasCamera.WorldToScreenPoint(target.transform.position);

//                Vector2 anchpos = new Vector2();
//                anchpos.x = pos.x - (Screen.width / 2);
//                anchpos.y = pos.y - (Screen.height / 2);

//                int left = (int)(width + anchpos.x);
//                int right = (int)(width - anchpos.x);

//                int top = (int)(height - anchpos.y);
//                int bottom = (int)(height + anchpos.y);

//                webViewObject.SetMargins(left, top, right, bottom);

//                return webViewObject;
//            }
//            catch
//            {
//                return null;
//            }
//#else
//            return null;
//#endif
//        }

//        public static WebViewObject StartWebView(Transform target, string url, int left, int top, int right, int bottom)
//        {
//#if UNITY_ANDROID || UNITY_IPHONE
//            try
//            {
//                string strUrl = url;
//                WebViewObject webViewObject;
//                webViewObject = target.gameObject.AddComponent<WebViewObject>();
//                webViewObject.Init((msg) =>
//                {
//                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
//                });

//                webViewObject.LoadURL(strUrl);
//                webViewObject.SetVisibility(true);
//                webViewObject.SetMargins(left, top, right, bottom);

//                return webViewObject;
//            }
//            catch
//            {
//                return null;
//            }
//#else
//            return null;
//#endif
//        }

//        public static WebViewObject StartWebView(Transform target, string url, Vector2 center, Vector2 scale)
//        {
//#if UNITY_ANDROID || UNITY_IPHONE
//            try
//            {
//                string strUrl = url;
//                WebViewObject webViewObject;
//                webViewObject = target.gameObject.AddComponent<WebViewObject>();
//                webViewObject.Init((msg) =>
//                {
//                    Debug.Log(string.Format("CallFromJS[{0}]", msg));
//                });

//                webViewObject.LoadURL(strUrl);
//                webViewObject.SetVisibility(true);
//                webViewObject.SetCenterPositionWithScale(center, scale);

//                return webViewObject;
//            }
//            catch
//            {
//                return null;
//            }

//#else
//            return null;
//#endif
//        }

        public static string GetResourceFullPath(Object go, string replaceStr = "")
        {
            if (go == null)
                return "";

#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(go);
            // Assets/Resources/Enemy/pfGoblinGreen.prefab
            string file = GetFileTitle(path);
            // pfGoblinGreen
            path = Path.GetDirectoryName(path);
            // Assets/Resources/Enemy

            if (replaceStr == "")
            { // Assets/Resources/Enemy/pfGoblinGreen
                path += file;
                return path;
            }

            path += "/";
            // Assets/Resources/Enemy/
            path = path.Replace(replaceStr, "");
            // Enemy/
            path += file;
            // Enemy/pfGoblinGreen
            return path;
#else
            return go.name;
#endif
        }

        public static string GetResourcePath(Object go, string replaceStr = "")
        {
            if (go == null)
                return "";

#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GetAssetPath(go);
            // Assets/Resources/Enemy/pfGoblinGreen.prefab

            path = Path.GetDirectoryName(path);
            // Assets/Resources/Enemy

            if (replaceStr == "")
                return path;

            path += "/";
            // Assets/Resources/Enemy/
            path = path.Replace(replaceStr, "");
            // Enemy/
            return path;
#else
            return go.name;
#endif
        }

        public static string GetResourceBundleTag(Object go)
        {
            if (go == null)
                return "";

#if UNITY_EDITOR

            string fullpath = UnityEditor.AssetDatabase.GetAssetPath(go);

            string bundlepath = UnityEditor.AssetDatabase.GetImplicitAssetBundleName(fullpath);

            return bundlepath;
#else
            return go.name;
#endif
        }

        public static string GetFileTitle(string path)
        {
            // Assets/Resources/Enemy/pfGoblinGreen.prefab -> pfGoblinGreen
            return Path.GetFileNameWithoutExtension(path);
        }

        public static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;


            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();
                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.

            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static string GetStreamingAssetPath()
        {
            //return Application.streamingAssetsPath;

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            { // 아이폰은 테스트해봐야함
                return "file:" + Application.dataPath + "/Raw";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return "jar:file://" + Application.dataPath + "!/assets";
            }
            else
            {
                return Application.streamingAssetsPath;
            }
        }

        public static void ResetOthersAndSetTrigger(this Animator anim, string name)
        {
            if (anim.parameters == null)
                return;

            for (int i = 0; i < anim.parameters.Length; ++i)
            {
                if (anim.parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    anim.ResetTrigger(anim.parameters[i].name);
                }
            }

            anim.SetTrigger(name);
        }

        public static float DirToEulerAngleY(Vector3 dir)
        {
            return Quaternion.LookRotation(dir).eulerAngles.y;
        }

        public static  Vector3 EulerAngleYToDir(float angleY)
        {
            return Quaternion.Euler(0.0f, angleY, 0.0f) * Vector3.forward;
        }

        public static float RotationToEulerAngleY(Quaternion rot)
        {
            return rot.eulerAngles.y;
        }

        public static Quaternion EulerAngleYToRotation(float angleY)
        {
            return Quaternion.Euler(0.0f, angleY, 0.0f);
        }

        static float CoordinateMinValue = -50.0f;
        /// <summary>
        /// (50(MaxValue) - (-50(MinValue))) / Precision + 1 = 65535 (ushort)
        /// </summary>
        static float CoordinatePrecision = 0.001526f;
        public static ushort CompressCoordinate(float coordinate)
        {
            return (ushort)((coordinate - CoordinateMinValue) / CoordinatePrecision);
        }
        public static float DecompressCoordinate(ushort value)
        {
            return value * CoordinatePrecision + CoordinateMinValue;
        }

        /// <summary>
        /// (360 - 0) / Precision + 1 = 65535 (ushort)
        /// </summary>
        static float RotationEulerAnglePrecision = 0.005494f; 
        public static ushort CompressRotationEulerAngle(float eulerAngle)
        {
            return (ushort)(eulerAngle / RotationEulerAnglePrecision);
        }
        public static float DecompressRotationEulerAngle(ushort value)
        {
            return value * RotationEulerAnglePrecision;
        }

        public static short PackToShort(short bytePack, int byteIndex, byte value)
        {
            int shiftNum = byteIndex * 8;
            int targetBytePack = bytePack;
            targetBytePack &= ~(byte.MaxValue << shiftNum);
            targetBytePack |= value << shiftNum;

            return (short)targetBytePack;
        }
        public static byte UnpackToByte(short bytePack, int byteIndex)
        {
            int shiftNum = byteIndex * 8;
            return (byte)(bytePack >> shiftNum);
        }

        public static int PackToInt(int bytePack, int byteIndex, byte value)
        {
            int shiftNum = byteIndex * 8;
            int targetBytePack = bytePack;
            targetBytePack &= ~(byte.MaxValue << shiftNum);
            targetBytePack |= value << shiftNum;

            return targetBytePack;
        }
        public static byte UnpackToByte(int bytePack, int byteIndex)
        {
            int shiftNum = byteIndex * 8;
            return (byte)(bytePack >> shiftNum);
        }

        public static int PackToInt(int bytePack, int byteIndex, sbyte value)
        {
            return PackToInt(bytePack, byteIndex, (byte)value);
        }
        public static sbyte UnpackToSByte(int bytePack, int byteIndex)
        {
            return (sbyte)UnpackToByte(bytePack, byteIndex);
        }

        public static int PackToInt(int bytePack, int byteIndex, ushort value)
        {
            int shiftNum = byteIndex * 8;
            int targetBytePack = bytePack;
            targetBytePack &= ~(ushort.MaxValue << shiftNum);
            targetBytePack |= value << shiftNum;

            return targetBytePack;
        }
        public static ushort UnpackToUShort(int bytePack, int byteIndex)
        {
            int shiftNum = byteIndex * 8;
            return (ushort)(bytePack >> shiftNum);
        }

        public static int PackToInt(int bytePack, int byteIndex, short value)
        {
            return PackToInt(bytePack, byteIndex, (ushort)value);
        }
        public static short UnpackToShort(int bytePack, int byteIndex)
        {
            return (short)UnpackToUShort(bytePack, byteIndex);
        }

        public static long PackToLong(long bytePack, int byteIndex, byte value)
        {
            int shiftNum = byteIndex * 8;
            long targetBytePack = bytePack;
            targetBytePack &= ~((long)byte.MaxValue << shiftNum); // 부호 있는 정수의 부호 확장시 앞에 채워지는 비트의 값이 달라서 생기는 버그를 없애기 위해 unsigned 형으로 변환.
            targetBytePack |= (long)value << shiftNum; // 부호 있는 정수의 부호 확장시 앞에 채워지는 비트의 값이 달라서 생기는 버그를 없애기 위해 unsigned 형으로 변환.

            return targetBytePack;
        }
        public static byte UnpackToByte(long bytePack, int byteIndex)
        {
            int shiftNum = byteIndex * 8;
            return (byte)(bytePack >> shiftNum);
        }

        public static long PackToLong(long bytePack, int byteIndex, ushort value)
        {
            int shiftNum = byteIndex * 8;
            long targetBytePack = bytePack;
            targetBytePack &= ~((long)ushort.MaxValue << shiftNum); // 부호 있는 정수의 부호 확장시 앞에 채워지는 비트의 값이 달라서 생기는 버그를 없애기 위해 unsigned 형으로 변환.
            targetBytePack |= (long)value << shiftNum; // 부호 있는 정수의 부호 확장시 앞에 채워지는 비트의 값이 달라서 생기는 버그를 없애기 위해 unsigned 형으로 변환.

            return targetBytePack;
        }
        public static ushort UnpackToUShort(long bytePack, int byteIndex)
        {
            int shiftNum = byteIndex * 8;
            return (ushort)(bytePack >> shiftNum);
        }

        public static long PackToLong(long bytePack, int byteIndex, short value)
        {
            return PackToLong(bytePack, byteIndex, (ushort)value);
        }
        public static short UnpackToShort(long bytePack, int byteIndex)
        {
            return (short)UnpackToUShort(bytePack, byteIndex);
        }

        public static long PackToLongWithCompression(long bytePack, int byteIndex, Vector3 value)
        {
            bytePack = PackToLong(bytePack, byteIndex, CompressCoordinate(value.x));
            bytePack = PackToLong(bytePack, byteIndex + 2, CompressCoordinate(value.y));
            bytePack = PackToLong(bytePack, byteIndex + 4, CompressCoordinate(value.z));

            return bytePack;
        }
        public static Vector3 UnpackToVectorWithDecompression(long bytePack, int byteIndex)
        {
            Vector3 pos = Vector3.zero;
            pos.x = DecompressCoordinate(UnpackToUShort(bytePack, byteIndex));
            pos.y = DecompressCoordinate(UnpackToUShort(bytePack, byteIndex + 2));
            pos.z = DecompressCoordinate(UnpackToUShort(bytePack, byteIndex + 4));

            return pos;
        }

        /// <summary>
        /// 위치값과 회전 Y값을 합쳐서 패킹한다.
        /// 일반적으로 캐릭터의 움직임을 하나의 값에 모아서 네트워크로 보낼 때 사용한다.
        /// 캐릭터의 움직임은 위치값과 회전 Y값 만으로도 표현할 수 있기 때문이다.
        /// 이런식의 움직임을 압축해서 하나의 값에 패킹한다.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static long ConvertStandUpMovementToLongBytePack(Vector3 pos, Quaternion rot)
        {
            long bytePack = 0;
            bytePack = PackToLongWithCompression(bytePack, 0, pos);
            bytePack = PackToLong(bytePack, 6, CompressRotationEulerAngle(rot.eulerAngles.y));

            return bytePack;
        }
        public static Vector3 UnpackStandUpMovementToPos(long bytePack)
        {
            return UnpackToVectorWithDecompression(bytePack, 0);
        }
        public static Quaternion UnpackStandUpMovementToRot(long bytePack)
        {
            return EulerAngleYToRotation(DecompressRotationEulerAngle((UnpackToUShort(bytePack, 6))));
        }

        public static void SetActiveGameObjects(List<GameObject> gobj_List, bool isActive)
        {
            for (int i = 0; i < gobj_List.Count; ++i)
                gobj_List[i].SetActive(isActive);
        }

        public static T CheckAndAddOneComponent<T>(GameObject gameObj) where T : UnityEngine.Component
        {
            T component = null;
            component = gameObj.GetComponent<T>();
            if (component == null)
                component = gameObj.AddComponent<T>();

            return component;
        }


        static Vector2 CoinFlipDistance = new Vector2(1.5f, 2.2f);
        static int _coinFloorCheckCount = 50;
        public static Vector3 GetNextCoinPosition(Vector3 startPos, int seed)
        {
            Vector3 pos = startPos;
            Random.InitState(seed);
            for (int i = 0; i < _coinFloorCheckCount; ++i)
            {
                pos = startPos + Quaternion.Euler(0.0f, Random.Range(0, 360), 0.0f) * Vector3.forward * Random.Range(CoinFlipDistance.x, CoinFlipDistance.y);
                RaycastHit hit;
                if (Physics.Raycast(pos + Vector3.up, Vector3.down, out hit, float.MaxValue, LayerMask.GetMask("Floor"), QueryTriggerInteraction.Ignore))
                {
                    pos = hit.point;
                    break;
                }
                else
                {
                    pos = startPos;
                }
            }

            return pos;
        }
    }
}