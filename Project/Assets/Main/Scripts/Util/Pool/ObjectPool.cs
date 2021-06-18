using System.Collections.Generic;

namespace GameBerry.Utility
{
    public interface IPoolObject
    {
        void Reset();
    }

    public static class ObjectPoolManager
    {
        static System.Action _ClearHandlers;

        public static void Add_ClearHandler(System.Action handler)
        {
            _ClearHandlers += handler;
        }

        public static void Clear_AllObjectPool()
        {
            if (_ClearHandlers != null)
                _ClearHandlers();
        }
    }

    public static class ObjectPool<T> where T : class, IPoolObject, new()
    {
        static Queue<T> _Pool = new Queue<T>();

        static ObjectPool()
        {
            ObjectPoolManager.Add_ClearHandler(Clear);
        }

        public static T GetObject()
        {
            T obj = null;

            if (_Pool.Count > 0)
                obj = _Pool.Dequeue();
            else
                obj = new T();

            obj.Reset();

            return obj;
        }

        public static void ReturnObject(T obj)
        {
            _Pool.Enqueue(obj);
        }

        public static void Clear()
        {
            _Pool.Clear();
        }
    }
}