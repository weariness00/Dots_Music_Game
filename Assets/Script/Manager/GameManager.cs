using System;
using Script.Music;
using Unity.Entities;
using UnityEngine;

namespace Script.Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private EntityManager _entityManager;
        private Entity _entity;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
        }

        public void MusicStart()
        {
            _entityManager.SetComponentEnabled<MusicStartTag>(_entity, true);
        }
    }
}
