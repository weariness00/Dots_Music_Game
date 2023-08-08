using Script.JudgPanel;
using Unity.Entities;

namespace Script.JudgePanel
{
    public enum JudgeType
    {
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
}
