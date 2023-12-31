﻿using Script.JudgePanel;
using Unity.Entities;
using MusicNodeInfo = Script.MusicNode.MusicNodeInfo;

namespace Script.Music.Generator
{
    public struct MusicGeneratorInitTag : IComponentData {}
    public struct MusicGeneratorTag : IComponentData{}
    public struct MusicGeneratorDeleteTag : IComponentData, IEnableableComponent
    {}

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
