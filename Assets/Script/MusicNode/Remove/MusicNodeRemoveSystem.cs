using Script.Manager;
using Script.Music;
using Unity.Burst;
using Unity.Entities;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeRemoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
            state.RequireForUpdate<PistolNodeRemoveTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();

            if (SystemAPI.IsComponentEnabled<PistolNodeRemoveTag>(gmEntity) == false) return;
            SystemAPI.SetComponentEnabled<PistolNodeRemoveTag>(gmEntity, false);
            
            var removeJob = new MusicNodeRemoveJob()
            {
                ECB = ecb.AsParallelWriter(),

                GM_Entity = gmEntity,
                NearNodeEntity = SystemAPI.GetComponent<NearNodeEntity>(gmEntity),
            };
            state.Dependency = removeJob.ScheduleParallel(state.Dependency);
            
        }
    }
}