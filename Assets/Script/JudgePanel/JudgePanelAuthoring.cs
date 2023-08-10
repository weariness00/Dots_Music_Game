using Script.JudgPanel;
using Unity.Entities;
using UnityEngine;

namespace Script.JudgePanel
{
    public enum JudgeType
    {
        None,
        Miss,
        Bad,
        Good,
        Perfect,
    }
    
    public struct PistolPanelTag : IComponentData{}
    public struct RiflePanelTag : IComponentData{}
    public struct SniperPanelTag : IComponentData{}

    public struct JudgePanelAuthoring : IComponentData
    {
        public JudgeInterval Interval;
    }

    public class JudgPanelEffectSound : IComponentData
    {
        public AudioClip Clip;
    }
}
