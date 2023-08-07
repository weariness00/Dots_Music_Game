using Unity.Entities;

namespace Script.Music
{
    public struct MusicStartTag : IComponentData, IEnableableComponent {}
    public struct MusicLoadTag : IComponentData { }

    public class MusicLoadAuthoring : IComponentData
    {
        public MusicScriptableObject MusicScriptableObject;
    }
}
