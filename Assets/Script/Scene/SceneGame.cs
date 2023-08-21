using System;
using System.Collections;
using Script.Game;
using Script.Manager;
using Script.Music;
using Unity.Entities;
using UnityEngine;

namespace Script.Scene
{
    public class SceneGame : SceneBase
    {
        private MusicScriptableObject _musicData;
        
        private void Start()
        {
            StartCoroutine(nameof(InitGame));
        }

        IEnumerator InitGame()
        {
            var gmEntity = Entity.Null;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            while (gmEntity == Entity.Null)
            {
                yield return null;
                entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var query = entityManager.CreateEntityQuery(typeof(GameManagerTag));
                if(!query.IsEmpty) gmEntity = query.GetSingletonEntity();
            }
            
            var gameData = GameObject.FindObjectOfType<GameData>();
            _musicData = gameData.musicData;
            
            entityManager.AddComponentData(gmEntity, new MusicLoadAuthoring(){MusicScriptableObject = _musicData});
            entityManager.AddComponent<GameInitTag>(gmEntity);
        }

        public override void Clear()
        {
        }
    }
}
