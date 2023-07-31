using Unity.Entities;

namespace Script.Music
{
    public struct MusicStartTag : IComponentData {}
    public struct MusicLoadTag : IComponentData { }

    public class MusicLoadAuthoring : IComponentData
    {
        public MusicScriptableObject MusicScriptableObject;
    }
}
