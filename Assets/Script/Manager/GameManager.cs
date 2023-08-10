using System;
using Script.Music;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public EntityManager EntityManager;
        public Entity Entity;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity = EntityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
        }
        
        public void MusicStart()
        {
            EntityManager.SetComponentEnabled<MusicStartTag>(Entity, true);
            Managers.Sound.GetAudioSource(SoundType.BGM).Play();
        }
    }
}
