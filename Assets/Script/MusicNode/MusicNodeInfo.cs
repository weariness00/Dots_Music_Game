using System;
using System.Collections.Generic;
using Script.JudgePanel;
using Script.Manager;
using Script.Music.Generator;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Script.MusicNode
{
    [System.Serializable]
    public enum MusicNodeType
    {
        None,
        Cube,
    }
    
    [System.Serializable]
    public struct MusicNodeInfo
    {
        public int order;
        
        public MusicNodeType nodeEntityType;
        public JudgePanelType judgePanelType;
        public float3 StartPosition;
        public float LenthToDestination;    // 무조건 bpm = 1 기준

        public float perfectTime;
        
        public void SetStartPosition(float3 startPosition)
        {
            StartPosition = startPosition;
            LenthToDestination = math.distance(startPosition, float3.zero);
            SetPerfectTime();
        }
        
        public void SetJudgePanelType(JudgePanelType type, Entity nodeEntity)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            judgePanelType = type;
            switch (type)
            {
                case JudgePanelType.Pistol:
                    entityManager.AddComponent<PistolNodeTag>(nodeEntity);
                    break;
                case JudgePanelType.Rifle:
                    entityManager.AddComponent<RifleNodeTag>(nodeEntity);
                    break;
                case JudgePanelType.Sniper:
                    entityManager.AddComponent<SniperNodeTag>(nodeEntity);
                    break;
            }
        }

        public void SetPerfectTime()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gmEntity = entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
            var gmAuthoring = entityManager.GetComponentData<GameManagerAuthoring>(gmEntity);
            var judgePanelEntities = entityManager.GetComponentData<JudgePanelSingletonEntity>(gmEntity);
            perfectTime = LenthToDestination / gmAuthoring.BPM;
            
            switch (judgePanelType)
            {
                case JudgePanelType.Pistol:
                    perfectTime -= entityManager.GetComponentData<JudgePanelAuthoring>(judgePanelEntities.PistolEntity).Interval.Distance / gmAuthoring.BPM;
                    break;
                case JudgePanelType.Rifle:
                    perfectTime -= entityManager.GetComponentData<JudgePanelAuthoring>(judgePanelEntities.RifleEntity).Interval.Distance / gmAuthoring.BPM;
                    break;
                case JudgePanelType.Sniper:
                    perfectTime -= entityManager.GetComponentData<JudgePanelAuthoring>(judgePanelEntities.SniperEntity).Interval.Distance / gmAuthoring.BPM;
                    break;
            }
        }
    }

    public struct MusicNodeLenthToDestinationSort : IComparer<MusicScriptableObjectData>
    {
        public int Compare(MusicScriptableObjectData a, MusicScriptableObjectData b)
        {
            return a.NodeInfo.LenthToDestination.CompareTo(b.NodeInfo.LenthToDestination);
        }
    }
}