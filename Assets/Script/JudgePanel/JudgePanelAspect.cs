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
            if (dis < JudgeInterval.Distance + JudgeInterval.PerfectInterval && JudgeInterval.Distance - JudgeInterval.PerfectInterval < dis) return JudgeType.Perfect;
            if (dis < JudgeInterval.Distance + JudgeInterval.GoodInterval && JudgeInterval.Distance - JudgeInterval.GoodInterval < dis) return JudgeType.Good;
            if (dis < JudgeInterval.Distance + JudgeInterval.BadInterval && JudgeInterval.Distance - JudgeInterval.BadInterval < dis) return JudgeType.Bad;
            if (dis < JudgeInterval.Distance + JudgeInterval.MissInterval && JudgeInterval.Distance - JudgeInterval.MissInterval < dis) return JudgeType.Miss;
            return JudgeType.None;
        }
    }
}
