using Unity.Entities;

namespace Script.Manager.Game
{
    public readonly partial struct GameManagerAspect : IAspect
    {
        public readonly Entity Entity;

        public readonly RefRW<GameManagerAuthoring> Authoring;
        private readonly RefRW<NearNodeEntity> _nearNodeEntity;
        private readonly RefRO<JudgePanelSingletonEntity> _judgePanelSingletonEntity;

        public Entity NearPistolNode => _nearNodeEntity.ValueRO.PistolNode;
        public Entity NearRifleNode => _nearNodeEntity.ValueRO.RifleNode;
        public Entity NearSniperNode => _nearNodeEntity.ValueRO.SniperNode;
    }
}
