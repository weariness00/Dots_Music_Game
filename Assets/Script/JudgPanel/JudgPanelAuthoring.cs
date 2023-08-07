using Unity.Entities;

namespace Script.JudgPanel
{
    public enum JudgType
    {
        Miss,
        Bad,
        Good,
        Perfect,
    }
    
    public struct PistolPanelTag : IComponentData{}
    public struct RiflePanelTag : IComponentData{}
    public struct SniperPanelTag : IComponentData{}

    public struct JudgPanelAuthoring : IComponentData
    {
        public JudgInterval Interval;
    }
}
