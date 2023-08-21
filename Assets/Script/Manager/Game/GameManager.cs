using Script.Music;
using Unity.Entities;
using UnityEngine;

namespace Script.Manager.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        
        public void MusicStart()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gmEntity = entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
            
            entityManager.SetComponentEnabled<MusicStartTag>(gmEntity, true);
            Managers.Sound.GetAudioSource(SoundType.BGM).Play();
        }
    }
}
