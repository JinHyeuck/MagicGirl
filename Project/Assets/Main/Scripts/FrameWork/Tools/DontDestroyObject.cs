using UnityEngine;


public class DontDestroyObject : MonoBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
