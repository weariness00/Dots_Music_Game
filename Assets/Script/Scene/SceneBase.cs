using System;
using UnityEngine;

namespace Script.Scene
{
    public enum SceneType
    {
        Unknown,
        Login,
        Lobby,
        Game,
        MusicGenerate,
        GameEnd,
    }

    public abstract class SceneBase : MonoBehaviour
    {
        public SceneType Type { get; protected set; } = SceneType.Unknown;
        private void Awake()
        {
            Init();
        }

        protected virtual void Init()
        {
            DontDestroyOnLoad(gameObject);
        }

        public abstract void Clear();

        public void GoScene(string sceneName)
        {
            int index = 0;
            foreach (var type in Enum.GetNames(typeof(SceneType)))
            {
                if (type.Equals(sceneName))
                {
                    SceneManagerEx.Instance.LoadScene((SceneType)index);
                    break;
                }
                ++index;
            }
        }
    }
}