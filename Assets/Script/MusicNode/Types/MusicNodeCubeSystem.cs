using Script.Manager;
using Unity.Burst;
using Unity.Entities;

namespace Script.MusicNode.Types
{
    public partial struct MusicNodeCubeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
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
            var gmAuthoring = SystemAPI.GetComponent<GameManagerAuthoring>(gmEntity);
            
            var cubeNodeJob = new MusicNodeCubeJob()
            {
                ECB = ecb.AsParallelWriter(),
                CurrentTime = GetBGMTime(),
                BPM = gmAuthoring.BPM,
            };

            state.Dependency = cubeNodeJob.ScheduleParallel(state.Dependency);
        }

        [BurstCompile(CompileSynchronously = true)]
        float GetBGMTime() => Managers.Sound.GetAudioSource(SoundType.BGM).time;
    }
}
