using Script.JudgePanel;
using Script.JudgPanel;
using Script.Manager;
using Script.Music;
using Unity.Burst;
using Unity.Entities;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeMissLineSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
            state.RequireForUpdate<PistolPanelTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            if (SystemAPI.IsComponentEnabled<MusicStartTag>(gmEntity) == false) return;
            
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            var pistolMissLineJob = new MusicNodeMissLineToPistolJob()
            {
                ECB = ecb.AsParallelWriter(),
                
                GM_Entity = gmEntity,
                GM_Authoring = SystemAPI.GetComponent<GameManagerAuthoring>(gmEntity),
                PistolAuthoring = SystemAPI.GetComponent<JudgePanelAuthoring>(SystemAPI.GetSingletonEntity<PistolPanelTag>()),
            };

            state.Dependency = pistolMissLineJob.ScheduleParallel(state.Dependency);
        }
    }
}
