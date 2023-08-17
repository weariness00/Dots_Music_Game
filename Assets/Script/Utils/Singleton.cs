using UnityEngine;

namespace Script.Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this as T;
        }
    }
}
