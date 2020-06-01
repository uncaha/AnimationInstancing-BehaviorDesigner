using UnityEngine;
using System.Collections;
namespace AniPlayable
{
    public abstract class BaseManager: MonoBehaviour 
    {
        bool isInit = false;
        public void Init()
        {
            InitMgr();
        }

        abstract protected void InitMgr();
    }
    public abstract class SingletonMonoBehaviour<T> : BaseManager where T : BaseManager
    {
        private static T sInstance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (sInstance == null)
                {
                    lock (_lock)
                    {
                        if (sInstance == null)
                        {
                            GameObject tobj = new GameObject();
                            sInstance = tobj.AddComponent<T>();
                            sInstance.Init();
                            tobj.name = typeof(T).ToString();
                            DontDestroyOnLoad(tobj);
                        }
                    }
                }
                return sInstance;

            }
        }

        private static bool applicationIsQuitting = false;


        public static bool IsDestroy()
        {
            return applicationIsQuitting;
        }

        virtual protected void OnDestroy()
        {
            sInstance = null;
            applicationIsQuitting = true;
        }
    }

}