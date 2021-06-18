using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


namespace GameBerry
{
    public static class UnityExtensions
    {

        /// <summary>
        /// Extension method to check if a layer is in a layermask
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }

    public static class TransformExtensions
	{
		public static float DistanceOnGround(this Transform origin, Transform target)
		{
			Vector3 originPos = origin.position;
			Vector3 targetPos = target.position;
			originPos.y = 0.0f;
			targetPos.y = 0.0f;

			return (originPos - targetPos).magnitude;
		}

		public static float SqrDistanceOnGround(this Transform origin, Transform target)
		{
			Vector3 originPos = origin.position;
			Vector3 targetPos = target.position;
			originPos.y = 0.0f;
			targetPos.y = 0.0f;

			return (originPos - targetPos).sqrMagnitude;
		}

		public static float DistanceOnGround(this Transform origin, Vector3 target)
		{
			Vector3 originPos = origin.position;
			originPos.y = 0.0f;
			target.y = 0.0f;

			return (originPos - target).magnitude;
		}

		public static float SqrDistanceOnGround(this Transform origin, Vector3 target)
		{
			Vector3 originPos = origin.position;
			originPos.y = 0.0f;
			target.y = 0.0f;

			return (originPos - target).sqrMagnitude;
		}

		public static float DistanceToLine(this Transform point, Vector3 linePoint, Vector3 lineDirection)
		{
			return Vector3.Cross(lineDirection, point.position - linePoint).magnitude;
		}

		public static float SqrDistanceToLine(this Transform point, Vector3 linePoint, Vector3 lineDirection)
		{
			return Vector3.Cross(lineDirection, point.position - linePoint).sqrMagnitude;
		}

		public static Vector3 DirectionOnGround(this Transform origin, Transform target)
		{
			Vector3 originPos = origin.position;
			Vector3 targetPos = target.position;
			originPos.y = 0.0f;
			targetPos.y = 0.0f;

			return Vector3.Normalize(targetPos - originPos);
		}

		public static Vector3 DirectionOnGround(this Transform origin, Vector3 target)
		{
			Vector3 originPos = origin.position;
			originPos.y = 0.0f;
			target.y = 0.0f;

			return Vector3.Normalize(target - originPos);
		}

		public static void DestroyChildren(this Transform t)
		{
			bool isPlaying = Application.isPlaying;

			while (t.childCount != 0)
			{
				Transform child = t.GetChild(0);

				if (isPlaying)
				{
					child.SetParent(null);
					UnityEngine.Object.Destroy(child.gameObject);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(child.gameObject);
				}
			}
		}

		public static List<Transform> FindChildAll(this Transform root, string childName, bool containsWord = false)
		{
			var result = new List<Transform>();

			for (int i = 0; i < root.childCount; ++i)
			{
				Transform child = root.GetChild(i);
				if (child != null)
				{
					if (containsWord)
					{
						if (child.name.Contains(childName))
							result.Add(child);
					}
					else
					{
						if (child.name == childName)
							result.Add(child);
					}
				}
			}

			return result;
		}

		public static Transform FindChildRecursive(this Transform root, string childName, bool containsWord = false)
		{
			Transform result = null;

            if (containsWord == false)
            {
                result = root.Find(childName);
            }
            else
            {
                for (int i = 0; i < root.childCount; ++i)
                {
                    Transform child = root.GetChild(i);
                    if (child != null)
                    {
                        if (child.name.Contains(childName))
                        {
                            result = child;
                            break;
                        }
                    }
                }
            }

			if (result == null)
			{
				for (int i = 0; i < root.childCount; ++i)
				{
					Transform child = root.GetChild(i);
					if (child != null)
						result = child.FindChildRecursive(childName, containsWord);

					if (result != null)
						break;
				}
			}

			return result;
		}

		public static void ResetLocal(this Transform tm)
		{
			tm.localPosition = Vector3.zero;
			tm.localRotation = Quaternion.identity;
			tm.localScale = Vector3.one;
		}

		public static void ResetWorld(this Transform tm)
		{
			tm.position = Vector3.zero;
			tm.rotation = Quaternion.identity;
			tm.localScale = Vector3.one;
		}

		public static void CopyValue(this Transform tm, Transform from)
		{
			tm.position = from.position;
			tm.rotation = from.rotation;
			tm.localScale = from.localScale;
		}

		//public static void SetAttachTransform(this Transform tm, Transform attachRoot, string attachNodeName, Vector3 pos, Vector3 rot)
		//{
		//	if (attachRoot == null)
		//	{
		//		tm.localRotation = Quaternion.Euler(rot);
		//		tm.localPosition = pos;
		//	}
		//	else
		//	{
		//		if (string.IsNullOrEmpty(attachNodeName))
		//		{
		//			// DONE: 이 오브젝트에 AnimationClip이 적용되고 있다면
		//			// 애니메이션 데이터가 아래에서 잡을 위치와 방향을 덮어버리는 문제가 발생한다.
		//			// 그래서 더미 오브젝트를 추가해서 아래에서 설정한 정보를 유지하도록 변경한다.
		//			var tempRoot = new GameObject(tm.name + "_Root");
		//			tempRoot.AddComponent<Tool.AutoDestroyEmptyObject>();

		//			var rootTM = tempRoot.transform;
		//			tm.parent = rootTM;
		//			tm.localRotation = Quaternion.Euler(rot);
		//			tm.localPosition = pos;

		//			rootTM.localRotation = attachRoot.rotation;
		//			rootTM.localPosition = attachRoot.position;
		//		}
		//		else if (attachNodeName == "Root")
		//		{
		//			tm.parent = attachRoot;
		//			tm.localRotation = Quaternion.Euler(rot);
		//			tm.localPosition = pos;
		//		}
		//		else if (attachNodeName == "WorldRoot")
		//		{
		//			tm.parent = null;
		//			tm.localRotation = Quaternion.Euler(rot);
		//			tm.localPosition = pos;
		//		}
		//		else if (attachNodeName.ToUpper().Contains("PROJECTILE"))
		//		{
		//			var tempRoot = new GameObject(tm.name + "_Root");
		//			tempRoot.AddComponent<Tool.AutoDestroyEmptyObject>();

		//			var rootTM = tempRoot.transform;
		//			Transform parent = attachRoot.FindChildRecursive(attachNodeName);
		//			if (parent != null)
		//			{
		//				tm.parent = rootTM;
		//				tm.localRotation = Quaternion.Euler(rot);
		//				tm.localPosition = pos;

		//				rootTM.localRotation = parent.rotation;
		//				rootTM.localPosition = parent.position;
		//			}
		//		}
		//		else
		//		{
		//			Transform parent = attachRoot.FindChildRecursive(attachNodeName);
		//			if (parent != null)
		//			{
		//				tm.parent = parent;
		//				tm.localRotation = Quaternion.Euler(rot);
		//				tm.localPosition = pos;
		//			}
		//		}
		//	}
		//}

		public static bool AlmostEqual(this Transform origin, Transform target, float tolerance = float.Epsilon)
		{
			return (origin.position.AlmostEqual(target.position, tolerance) && origin.rotation.AlmostEqual(target.rotation, tolerance) && origin.localScale.AlmostEqual(target.localScale, tolerance));
		}

		public static RectTransform RectTransform(this Component cp)
		{
			return cp.transform as RectTransform;
		}

		public static void FromScreenPoint(this RectTransform target, Vector2 screenPoint)
		{
			var canvas = target.GetComponentInParent<Canvas>();
			if (canvas == null)
			{
				Debug.LogWarning("Can't found Canvas.");
				return;
			}

			Vector3 wp;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(target, screenPoint, canvas.worldCamera, out wp);

			target.position = wp;
		}

		public static void SetLayerInChildren(this Transform parent, int layer)
		{
			parent.gameObject.layer = layer;

			for (int i = 0; i < parent.childCount; ++i)
				parent.GetChild(i).SetLayerInChildren(layer);
		}

        public static List<T> GetComponentsInAllChildren<T>(this Transform tm) where T : Component
		{
            List<T> cc = new List<T>();

            T vp = tm.GetComponent<T>();

            if (vp != null)
                cc.Add(vp);

            for (int i = 0; i < tm.childCount; ++i)
                cc.AddRange(tm.GetChild(i).GetComponentsInAllChildren<T>());

            return cc;
		}

        public static T FindChildComponent<T>(this Transform tm, string name) where T : Component
        {
            Transform childTrans = tm.Find(name);
            if (childTrans == null)
                return null;

            return childTrans.GetComponent<T>();
        }

        public static void SetLocalRotationToEulerAngles(this Transform tm, Vector3 v)
        {
            Quaternion quaternion = tm.localRotation;
            quaternion.eulerAngles = v;
            tm.localRotation = quaternion;
        }
	}


	public static class GameObjectExtensions
	{
		public static void SetLayerInChildren(this GameObject parent, int layer)
		{
			parent.layer = layer;

			for (int i = 0; i < parent.transform.childCount; ++i)
				parent.transform.GetChild(i).SetLayerInChildren(layer);
		}

		public static GameObject FindChildGameObjectWithTag(this GameObject go, string tag)
		{
			if (go.CompareTag(tag))
				return go;
			
			for (int i = 0; i < go.transform.childCount; ++i)
			{
				var child = go.transform.GetChild(i);
				var found = child.gameObject.FindChildGameObjectWithTag(tag);
				if (found != null)
					return found;
			}

			return null;
		}
	}


	public static class Vector3Extensions
	{
		public static float DistanceToLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
		{
			return Vector3.Cross(lineDirection, point - linePoint).magnitude;
		}

		public static float SqrDistanceToLine(this Vector3 point, Vector3 linePoint, Vector3 lineDirection)
		{
			return Vector3.Cross(lineDirection, point - linePoint).sqrMagnitude;
		}

        public static float DistanceOnGround(this Vector3 origin, Vector3 target)
        {
            origin.y = 0.0f;
            target.y = 0.0f;

            return Vector3.Distance(origin, target);
        }

        public static Vector3 Direction(this Vector3 origin, Vector3 target)
        {
            return Vector3.Normalize(target - origin);
        }

        public static Vector3 DirectionOnGround(this Vector3 origin, Vector3 target)
		{
			origin.y = 0.0f;
			target.y = 0.0f;

			return Vector3.Normalize(target - origin);
		}

        public static float AngleToRad(this Vector3 myDir, Vector3 targetDir)
        {
            return Vector3.Dot(myDir, targetDir);
        }

        public static float AngleAcosToRad(this Vector3 myDir, Vector3 targetDir)
        {
            return Mathf.Acos(Vector3.Dot(myDir, targetDir));
        }

        public static float AngleAcosToDeg(this Vector3 myDir, Vector3 targetDir)
        {
            return Mathf.Acos(Vector3.Dot(myDir, targetDir)) * Mathf.Rad2Deg;
        }

        public static bool AlmostEqual(this Vector3 origin, Vector3 target, float tolerance = float.Epsilon)
		{
            //Debug.LogFormat("Diff : {0:0.####}, {1:0.####}, {2:0.####}, Tol: {3:0.####}", MathDatas.Abs(origin.x - target.x), MathDatas.Abs(origin.y - target.y), MathDatas.Abs(origin.z - target.z), tolerance);
            return (MathDatas.Abs(origin.x - target.x) <= tolerance && MathDatas.Abs(origin.y - target.y) <= tolerance && MathDatas.Abs(origin.z - target.z) <= tolerance);
		}
	}


	public static class QuaternionExtensions
	{
		public static bool AlmostEqual(this Quaternion origin, Quaternion target, float tolerance = 1.0f)
		{
            //Debug.LogFormat("Angle : {0:0.####}, Tol: {1:0.####}", MathDatas.Abs(Quaternion.Angle(origin, target)), tolerance);
            return (MathDatas.Abs(Quaternion.Angle(origin, target)) <= tolerance);
		}
	}


	public static class StringExtensions
	{

		public static int FastStringToInt(this string s)
		{ // 문자열에 공백이 없는 경우에만 써야한다.
			int outValue = 0;

			for (int i = 0; i < s.Length; i++)
			{
				outValue = outValue * 10 + (s[i] - '0');
			}

			return outValue;
		}
	}


	public static class FloatExtensions
	{
		public static Vector3 ToVector3(this float v)
		{
			return new Vector3(v, v, v);
		}

		public static bool AlmostEqual(this float origin, float target, float tolerance = float.Epsilon)
		{
			return (MathDatas.Abs(origin - target) <= tolerance);
		}
	}


	public static class EnumExtensions
	{
		public static T Parse<T>(string value, bool ignoreCase, T defaultValue) where T : struct
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, ignoreCase);
			}
			catch
			{
				return defaultValue;
			}
		}

		public static T Parse<T>(string value) where T : struct
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return default(T);
			}
		}

		public static int ParseToInt<T>(string value) where T : struct
		{
			try
			{
				return (int)Enum.Parse(typeof(T), value, true);
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return 0;
			}
		}

		public static bool TryParseToInt<T>(string value, out int number) where T : struct
		{
			try
			{
				number = (int)Enum.Parse(typeof(T), value, true);
				return true;
			}
			catch
			{
				/* Empty */
			}

			number = 0;
			return false;
		}
	}


	public static class ListExtension
	{
		static System.Random _random = new System.Random();


		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = _random.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}
