using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.JudgePanel
{
    [System.Serializable]
    public enum JudgePanelType
    {
        Pistol,
        Rifle,
        Sniper,
    }

    [System.Serializable]
    public struct JudgeInterval
    {
        public float Distance;
        
        public float PerfectInterval;
        public float GoodInterval;
        public float BadInterval;
        public float MissInterval;
    }
    
    public class JudgePanelMono : MonoBehaviour
    {
        public JudgePanelType type;
        public JudgeInterval judgeInterval;
        public AudioClip effectSound;
    }

    public class JudgePanelBaker : Baker<JudgePanelMono>
    {
        public override void Bake(JudgePanelMono authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent(entity, new JudgePanelAuthoring()
            {
                Interval = authoring.judgeInterval,
            });
            switch (authoring.type)
            {
                case JudgePanelType.Pistol:
                    AddComponent<PistolPanelTag>(entity);
                    break;
                case JudgePanelType.Rifle:
                    AddComponent<RiflePanelTag>(entity);
                    break;
                case JudgePanelType.Sniper:
                    AddComponent<SniperPanelTag>(entity);
                    break;
            }
            
            AddComponentObject(entity, new JudgPanelEffectSound(){Clip = authoring.effectSound});
        }
    }
}
