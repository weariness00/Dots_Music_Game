using Script.Music;
using Unity.Entities;
using UnityEngine;

namespace Script.Manager
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
            AddComponent<MusicStartTag>(entity);
            AddComponent(entity, new GameManagerAuthoring()
            {
                Score = 0,
                Combo = 0,
            });
            AddComponent<NearNodeEntity>(entity);
            
            SetComponentEnabled<MusicStartTag>(entity,false);
        }
    }
}
