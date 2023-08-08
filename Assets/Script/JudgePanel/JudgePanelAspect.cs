using Script.JudgePanel;
using Unity.Entities;

namespace Script.JudgPanel
{
    public readonly partial struct JudgePanelAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<JudgePanelAuthoring> _authoring;

        public JudgeInterval JudgeInterval => _authoring.ValueRO.Interval;

        // 0 : Miss
        // 1 : Bad
        // 2 : Good
        // 3 : Perfect
        public JudgeType Judge(float dis)
        {
            if (dis < 5f + JudgeInterval.PerfectInterval && 5f - JudgeInterval.PerfectInterval < dis) return JudgeType.Perfect;
            if (dis < 5f + JudgeInterval.GoodInterval && 5f - JudgeInterval.GoodInterval < dis) return JudgeType.Good;
            if (dis < 5f + JudgeInterval.BadInterval && 5f - JudgeInterval.BadInterval < dis) return JudgeType.Bad;
            return JudgeType.Miss;
        }
    }
}
