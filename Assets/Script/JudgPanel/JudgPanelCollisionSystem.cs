using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace Script.JudgPanel
{
    public partial struct JudgPanelCollisionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<SimulationSingleton>();
            
            state.RequireForUpdate<SniperPanelTag>();
            state.RequireForUpdate<RiflePanelTag>();
            state.RequireForUpdate<PistolPanelTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var judgCollisionJob = new JudgPanelCollisionJob()
            {
                ECB = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged),
                
                PistolPanel = SystemAPI.GetSingletonEntity<PistolPanelTag>(),
                RiflePanel = SystemAPI.GetSingletonEntity<RiflePanelTag>(),
                SniperPanel = SystemAPI.GetSingletonEntity<SniperPanelTag>(),
                
                PistolNodeLookup = SystemAPI.GetComponentLookup<PistolNodeTag>(true),
                RifleNodeLookup = SystemAPI.GetComponentLookup<RifleNodeTag>(true),
                SniperNodeLookup = SystemAPI.GetComponentLookup<SniperNodeTag>(true),
            };

            state.Dependency = judgCollisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
            state.Dependency.Complete();
        }
    }
}
