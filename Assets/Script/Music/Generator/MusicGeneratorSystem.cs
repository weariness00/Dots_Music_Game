using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Script.Music.Generator
{
    public partial struct MusicGeneratorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicGeneratorTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var entity = SystemAPI.GetSingletonEntity<MusicGeneratorTag>();
            var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(entity);

            if (Input.GetMouseButtonDown(0))
            {
                float3 clickPosition = float3.zero;
                Vector2 mousePositino = Input.mousePosition;
                clickPosition = Camera.main.ScreenToWorldPoint();    
                // clickPosition = 0;

                foreach (var musicNodeAuthoring in SystemAPI.Query<MusicNodeAuthoring>())
                {
                    if (clickPosition.Equals(musicNodeAuthoring.StartPosition))
                    {
                        return;
                    }
                }

                var newNodeEntity = ecb.Instantiate(generatorAspect.NodeEntities[0].Entity);
                LocalTransform newNodeTransform = new LocalTransform()
                {
                    Position = clickPosition,
                    Rotation = quaternion.identity,
                    Scale = 1,
                };
                ecb.SetComponent(newNodeEntity, newNodeTransform);
            }
        }
    }
}