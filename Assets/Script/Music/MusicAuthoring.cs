using Unity.Entities;

namespace Script.Music
{
    public struct MusicStartTag : IComponentData, IEnableableComponent {}
    public struct MusicLoadTag : IComponentData { }
    public struct MusicNodeSpawnPerfectLineTag : IComponentData, IEnableableComponent {}

    public class MusicLoadAuthoring : IComponentData
    {
        public MusicScriptableObject MusicScriptableObject;
    }
}
