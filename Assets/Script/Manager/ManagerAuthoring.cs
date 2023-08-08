using Script.JudgePanel;
using Unity.Entities;

namespace Script.Manager
{
    public struct GameManagerTag : IComponentData { }

    public struct GameManagerAuthoring : IComponentData
    {
        public int Score;
        public int Combo;
        public JudgeType JudgeType;
    }
}
