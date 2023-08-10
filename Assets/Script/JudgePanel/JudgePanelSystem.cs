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
            
            Managers.Sound.Play(pistolPanelSound.Clip, SoundType.Effect);

            var nearPistolNodeEntity = SystemAPI.GetComponent<NearNodeEntity>(gmEntity).PistolNode;
            var nearPistolNodeAspect = SystemAPI.GetAspect<MusicNodeAspect>(nearPistolNodeEntity);
            var dis = math.distance(nearPistolNodeAspect.Position, float3.zero);
            var judge = pistolPanelAspect.Judge(dis);
            switch(judge)
            {
                case JudgeType.None:
                    return;
                case JudgeType.Miss:
                    gmAuthoring.ValueRW.Miss();
                    break;
                case JudgeType.Bad:
                    gmAuthoring.ValueRW.Bad();
                    break;
                case JudgeType.Good:
                    gmAuthoring.ValueRW.Good();
                    break;
                case JudgeType.Perfect:
                    gmAuthoring.ValueRW.Perfect();
                    break;
            }
            ecb.DestroyEntity(nearPistolNodeEntity);
            
            ecb.AddComponent<MusicNodeRemoveTag>(gmEntity);
        }
    }
}
