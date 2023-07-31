using Define;
using Unity.Entities;

namespace Script.MusicNode
{
    public struct MusicNodeTag : IComponentData{}
    public struct PistolNodeTag : IComponentData{}
    public struct RifleNodeTag : IComponentData{}
    public struct SniperNodeTag : IComponentData{}
    public struct MusicNodeCubeTag : IComponentData {}

    public struct MusicNodeAuthoring : IComponentData
    {
        public MusicNodeInfo NodeInfo;
    }
}