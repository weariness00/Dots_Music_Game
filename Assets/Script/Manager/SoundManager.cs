using System;
using UnityEngine;

namespace Script.Manager
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;
        public AudioSource rhythmGameMusic;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void Start()
        {
            GameManager.Instance.musicStartCall.AddListener(()=> rhythmGameMusic.Play());
        }
    }
}
