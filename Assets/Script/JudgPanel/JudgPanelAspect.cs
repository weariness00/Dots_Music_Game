using Unity.Entities;

namespace Script.JudgPanel
{
    public readonly partial struct JudgPanelAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<JudgPanelAuthoring> _authoring;

        public JudgInterval JudgInterval => _authoring.ValueRO.Interval;

        // 0 : Miss
        // 1 : Bad
        // 2 : Good
        // 3 : Perfect
        public JudgType Judg(float dis)
        {
            if (dis < 5f + JudgInterval.PerfectInterval && 5f - JudgInterval.PerfectInterval < dis) return JudgType.Perfect;
            else if (dis < 5f + JudgInterval.GoodInterval && 5f - JudgInterval.GoodInterval < dis) return JudgType.Good;
            else if (dis < 5f + JudgInterval.BadInterval && 5f - JudgInterval.BadInterval < dis) return JudgType.Bad;

            return JudgType.Miss;
        }
    }
}
