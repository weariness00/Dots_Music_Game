using System;
using Script.Utils;
using UnityEngine;

namespace Script.Manager
{
    public class Managers : MonoBehaviour
    {
        static Managers instance = null;
        public static Managers Instance { get { Init(); return instance; } }

        SoundManager soundManager;
        public static SoundManager Sound { get { return Instance.soundManager; } }

        public Action StartCall;
        public Action UpdateCall;
        public Action LateUpdateCall;
        public Action OnGUICall;

        private void Start()
        {
            soundManager = new SoundManager();

            StartCall?.Invoke();
        }

        private void Update()
        {
            UpdateCall?.Invoke();
        }

        private void LateUpdate()
        {
            LateUpdateCall?.Invoke();
        }

        private void OnGUI()
        {
            OnGUICall?.Invoke();
        }

        /// <summary>
        /// Prefab���� Manager�� ����� �� ��ũ��Ʈ �ֱ� 
        /// </summary>
        static void Init()
        {
            if (instance == null)
            {
                GameObject obj = GameObject.Find("Managers");

                if (obj == null)
                {
                    obj = Resources.Load("Prefabs/Managers") as GameObject;
                    obj = Instantiate(obj);
                }

                DontDestroyOnLoad(obj);
                instance = Util.GetORAddComponet<Managers>(obj);
            }
        }
    }
}