using Script.Manager;
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
            state.RequireForUpdate<MusicNodeRemoveTag>();
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
            var removeJob = new MusicNodeRemoveJob() { };

            state.Dependency = removeJob.ScheduleParallel(state.Dependency);

            ecb.RemoveComponent<MusicNodeRemoveTag>(gmEntity);
        }
    }
}
