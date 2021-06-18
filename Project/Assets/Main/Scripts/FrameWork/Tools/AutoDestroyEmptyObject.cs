using UnityEngine;
using System.Collections;


namespace GameBerry.Tool
{
	[ExecuteInEditMode]
	public class AutoDestroyEmptyObject : MonoBehaviour
	{
		Transform _tm;
		Coroutine _checker;


		void OnEnable()
		{
			if (_checker == null)
				_checker = StartCoroutine(CheckEmpty());
		}

		void OnDisable()
		{
			if (_checker != null)
			{
				StopCoroutine(_checker);
				_checker = null;
			}
		}
	
		IEnumerator CheckEmpty()
		{
			yield return new WaitForSeconds(1.0f);
			
			_tm = GetComponent<Transform>();

			while (_tm.childCount > 0)
			{
				yield return new WaitForSeconds(1.0f);
			}

			Destroy(gameObject);
		}

#if UNITY_EDITOR
		void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				if (_tm == null)
					_tm = GetComponent<Transform>();

				if (_tm.childCount == 0)
					DestroyImmediate(gameObject);
			}
		}
#endif
	}
}
