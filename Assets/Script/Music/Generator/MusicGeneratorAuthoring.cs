using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Script.Music.Generator
{
    public struct MusicGeneratorInitTag : IComponentData {}
    public struct MusicGeneratorTag : IComponentData{}
    
    public struct MusicGeneratorNodeEntities : IBufferElementData
    {
        public Entity Entity;
    }

    public class MusicGeneratorNodeObjects : IComponentData
    {
        public Entity[] entities;
    }
}
