using System;
using Script.JudgPanel;
using Script.Manager;
using Script.MusicNode;
using Script.UI;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Script.JudgePanel
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
            var pistolPanelAspect = SystemAPI.GetAspect<JudgePanelAspect>(pistolPanelEntity);
            var pistolPanelSound = SystemAPI.ManagedAPI.GetComponent<JudgPanelEffectSound>(pistolPanelEntity);
            
            foreach (var aspect in SystemAPI.Query<MusicNodeAspect>().WithAll<PistolNodeTag>())
            {
                var dis = math.distance(aspect.Position, float3.zero);
                var judge = pistolPanelAspect.Judge(dis);
                switch(judge)
                {
                    case JudgeType.Miss:
                        gmAuthoring.ValueRW.Combo = 0;
                        break;
                    case JudgeType.Bad:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 100;
                        break;
                    case JudgeType.Good:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 200;
                        break;
                    case JudgeType.Perfect:
                        gmAuthoring.ValueRW.Combo++;
                        gmAuthoring.ValueRW.Score += 300;
                        break;
                }

                gmAuthoring.ValueRW.JudgeType = judge;
                ecb.DestroyEntity(aspect.Entity);
                break;
            }
            ecb.AddComponent<MusicNodeRemoveTag>(gmEntity);
            Managers.Sound.Play(pistolPanelSound.Clip, SoundType.Effect);
            
            return;
        }
    }
}
