using Define;
using Script.JudgePanel;
using Script.JudgPanel;
using Unity.Entities;
using UnityEngine.UI;

namespace Script.Music.Generator
{
    public struct MusicGeneratorInitTag : IComponentData {}
    public struct MusicGeneratorTag : IComponentData{}

    public struct MusicGeneratorPanelTypeAuthoring : IComponentData
    {
        public JudgePanelType PanelType;
    }
    
    public struct MusicGeneratorNodeEntities : IBufferElementData
    {
        public Entity Entity;
    }
    public class MusicGeneratorNodeObjects : IComponentData
    {
        public Entity[] Entities;
    }

    public struct MusicScriptableObjectData : IBufferElementData
    {
        public MusicNodeInfo NodeInfo;
    }
}
