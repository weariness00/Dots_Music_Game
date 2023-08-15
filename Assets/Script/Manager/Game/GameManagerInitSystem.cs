using Script.JudgePanel;
using Unity.Burst;
using Unity.Entities;
using Unity.VisualScripting;

namespace Script.Manager.Game
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct GameManagerInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameManagerInitTag>();
            
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
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
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            
            JudgePanelSingletonEntity judgePanelSingletonEntity;
            judgePanelSingletonEntity.PistolEntity = SystemAPI.GetSingletonEntity<PistolPanelTag>();
            judgePanelSingletonEntity.RifleEntity = SystemAPI.GetSingletonEntity<RiflePanelTag>();
            judgePanelSingletonEntity.SniperEntity = SystemAPI.GetSingletonEntity<SniperPanelTag>();
            
            ecb.AddComponent(gmEntity, judgePanelSingletonEntity);
            ecb.RemoveComponent<GameManagerInitTag>(gmEntity);
            
            state.Enabled = false;
        }
    }
}
