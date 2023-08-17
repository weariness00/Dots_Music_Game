using Script.JudgePanel;
using Unity.Entities;

namespace Script.Music.Generator
{
    public readonly partial struct MusicGeneratorAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<MusicGeneratorPanelTypeAuthoring> _judgPanelTypeAuthoring;
        public JudgePanelType JudgePanelType => _judgPanelTypeAuthoring.ValueRO.PanelType;

        public readonly DynamicBuffer<MusicGeneratorNodeEntities> NodeEntities;
        public readonly DynamicBuffer<MusicScriptableObjectData> NodeListScriptableObject;
    }
}
