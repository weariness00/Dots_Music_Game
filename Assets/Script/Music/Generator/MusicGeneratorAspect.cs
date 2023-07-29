using Unity.Entities;

namespace Script.Music.Generator
{
    public readonly partial struct MusicGeneratorAspect : IAspect
    {
        public readonly Entity Entity;

        public readonly DynamicBuffer<MusicGeneratorNodeEntities> NodeEntities;
        public readonly DynamicBuffer<MusicScriptableObjectData> NodeListScriptableObject;
    }
}
