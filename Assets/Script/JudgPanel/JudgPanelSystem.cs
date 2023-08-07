using Script.Manager;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Script.JudgPanel
{
    public partial struct PistolPanelSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PistolPanelTag>();
            state.RequireForUpdate<GameManagerTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetKeyDown(KeyCode.A) == false) return; 

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            var gmAuthoring = SystemAPI.GetComponentRW<GameManagerAuthoring>(gmEntity);
            
            var pistolPanelEntity = SystemAPI.GetSingletonEntity<PistolPanelTag>();
            var pistolPanelAspect = SystemAPI.GetAspect<JudgPanelAspect>(pistolPanelEntity);
            
            foreach (var aspect in SystemAPI.Query<MusicNodeAspect>().WithAll<PistolNodeTag>())
            {
                var dis = aspect.LenthToZero;
                switch(pistolPanelAspect.Judg(dis))
                {
                    case JudgType.Miss:
                        gmAuthoring.ValueRW.Combo = 0;
                        break;
                    case JudgType.Bad:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 100;
                        break;
                    case JudgType.Good:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 200;
                        break;
                    case JudgType.Perfect:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 300;
                        break;
                }
                ecb.DestroyEntity(aspect.Entity);
                break;
            }
            ecb.AddComponent<MusicNodeRemoveTag>(gmEntity);

            return;
            
            var riflePanelEntity = SystemAPI.GetSingletonEntity<RiflePanelTag>();
            var sniperPanelEntity = SystemAPI.GetSingletonEntity<SniperPanelTag>();
        }
    }
}
