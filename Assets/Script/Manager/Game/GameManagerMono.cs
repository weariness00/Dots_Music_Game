using Script.Music;
using Script.MusicNode;
using Unity.Entities;
using UnityEngine;

namespace Script.Manager.Game
{
    public class GameManagerMono : MonoBehaviour
    {
    }
    
    public class GameManagerBaker : Baker<GameManagerMono>
    {
        public override void Bake(GameManagerMono authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent<GameManagerTag>(entity);
            AddComponent<GameManagerInitTag>(entity);
            AddComponent<PistolNodeRemoveTag>(entity);
            AddComponent<RifleNodeRemoveTag>(entity);
            AddComponent<SniperNodeRemoveTag>(entity);
            AddComponent<MusicStartTag>(entity);
            AddComponent(entity, new GameManagerAuthoring()
            {
                Score = 0,
                Combo = 0,
                BPM = 1f,
            });
            AddComponent<NearNodeEntity>(entity);
            
            SetComponentEnabled<MusicStartTag>(entity,false);
        }
    }
}
