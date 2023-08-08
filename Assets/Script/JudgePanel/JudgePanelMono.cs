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
        public float PerfectInterval;
        public float GoodInterval;
        public float BadInterval;
    }
    
    public class JudgePanelMono : MonoBehaviour
    {
        public JudgePanelType type;
        [FormerlySerializedAs("judgInterval")] public JudgeInterval judgeInterval;
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
        }
    }
}
