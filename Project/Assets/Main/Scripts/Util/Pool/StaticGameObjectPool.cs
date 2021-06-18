using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class AutoPoolStruct
    {
        public GameObject PoolObject;
        public float PoolTime;
        public bool SetActive;

        public AutoPoolStruct(GameObject PoolObject)
        {
            this.PoolObject = PoolObject;
        }
    }

    public class StaticGameObjectPool : MonoBehaviour
    {
        public List<GameObject> PoolObjectList
        {
            get { return m_gameObjectLists; }
        }

        List<GameObject> m_gameObjectLists = new List<GameObject>();

        Queue<AutoPoolStruct> m_poolObject = new Queue<AutoPoolStruct>();

        LinkedList<AutoPoolStruct> m_autoPoolObject = new LinkedList<AutoPoolStruct>();

        Dictionary<GameObject, AutoPoolStruct> m_getedObject = new Dictionary<GameObject, AutoPoolStruct>();

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject go = transform.GetChild(i).gameObject;
                m_gameObjectLists.Add(go);

                m_poolObject.Enqueue(new AutoPoolStruct(go));
            }
        }

        private void Update()
        {
            var node = m_autoPoolObject.First;
            while (node != null)
            {
                var nodeValue = node.Value;
                if (nodeValue.PoolTime <= Time.time)
                {
                    nodeValue.PoolObject.SetActive(nodeValue.SetActive);
                    m_poolObject.Enqueue(nodeValue);
                    m_autoPoolObject.Remove(node);
                }

                node = node.Next;
            }
        }

        public GameObject GetObject()
        {
            if (m_poolObject.Count <= 0)
                return null;

            AutoPoolStruct s = m_poolObject.Dequeue();

            if(m_getedObject.ContainsValue(s) == false)
                m_getedObject.Add(s.PoolObject, s);

            return s.PoolObject;
        }

        public GameObject GetAutoPoolObject(float timer)
        {
            AutoPoolStruct s = null;

            if (m_poolObject.Count <= 0)
                return null;

            s = m_poolObject.Dequeue();
            s.PoolTime = timer + Time.time;
            s.SetActive = s.PoolObject.activeSelf;

            m_autoPoolObject.AddFirst(s);

            return s.PoolObject;
        }

        public GameObject GetAutoPoolObject(float timer, bool active)
        {
            AutoPoolStruct s = null;

            if (m_poolObject.Count <= 0)
                return null;

            s = m_poolObject.Dequeue();
            s.PoolTime = timer + Time.time;
            s.SetActive = active;

            m_autoPoolObject.AddFirst(s);

            return s.PoolObject;
        }

        public void PoolObject(GameObject o)
        {
            if (m_getedObject.ContainsKey(o) == true)
                m_poolObject.Enqueue(m_getedObject[o]);
        }
    }
}