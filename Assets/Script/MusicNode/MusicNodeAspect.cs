using Unity.Entities;

namespace Script.MusicNode
{
    public readonly partial struct MusicNodeAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRO<MusicNodeTag> _tag;

        public readonly RefRW<MusicNodeAuthoring> MusicNodeAuthoring;
        
    }
}
