using Unity.Burst;
using Unity.Entities;

namespace Script.Music.Generator
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct MusicGeneratorInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicGeneratorInitTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var entity = SystemAPI.GetSingletonEntity<MusicGeneratorInitTag>();
            
            ecb.SetComponentEnabled<MusicGeneratorDeleteTag>(entity, false);
            
            ecb.AddBuffer<MusicScriptableObjectData>(entity);
            var nodeEntities = ecb.AddBuffer<MusicGeneratorNodeEntities>(entity);
            foreach (var objects in SystemAPI.Query<MusicGeneratorNodeObjects>())
            {
                foreach (var node in objects.Entities)
                {
                    nodeEntities.Add(new MusicGeneratorNodeEntities(){Entity = node});
                }
            }
            
            ecb.RemoveComponent<MusicGeneratorNodeObjects>(entity);
            ecb.RemoveComponent<MusicGeneratorInitTag>(entity);
        }
    }
}
