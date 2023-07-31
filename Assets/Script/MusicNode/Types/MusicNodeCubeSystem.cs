using Script.Music;
using Script.MusicNode.Types;
using Unity.Burst;
using Unity.Entities;

namespace Script.MusicNode
{
    public partial struct MusicNodeCubeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicNodeCubeTag>();
            state.RequireForUpdate<MusicStartTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var dt = SystemAPI.Time.DeltaTime;

            var cubeNodeJob = new MusicNodeCubeJob()
            {
                ECB = ecb.AsParallelWriter(),
                DeltaTime = dt,
            };

            state.Dependency = cubeNodeJob.ScheduleParallel(state.Dependency);
        }
    }
}
