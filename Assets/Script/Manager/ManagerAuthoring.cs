using Script.JudgePanel;
using Unity.Entities;
using Unity.VisualScripting;

namespace Script.Manager
{
    public struct GameManagerTag : IComponentData { }
    public struct GameManagerInitTag : IComponentData {}

    public struct NearNodeEntity : IComponentData
    {
        public Entity PistolNode;
    }

    public struct JudgePanelSingletonEntity : IComponentData
    {
        public Entity PistolEntity;
        public Entity RifleEntity;
        public Entity SniperEntity;
    }
    
    public struct GameManagerAuthoring : IComponentData
    {
        public int Score;
        public int Combo;
        public JudgeType JudgeType;

        public float BPM;

        public void Miss()
        {
            Combo = 0;
            JudgeType = JudgeType.Miss;
        }

        public void Bad()
        {
            Combo++;
            Score += 100;
            JudgeType = JudgeType.Bad;
        }

        public void Good()
        {
            Combo++;
            Score += 200;
            JudgeType = JudgeType.Good;
        }

        public void Perfect()
        {
            Combo++;
            Score += 200;
            JudgeType = JudgeType.Perfect;
        }
    }
}
