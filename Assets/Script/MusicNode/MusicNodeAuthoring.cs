using Unity.Entities;

namespace Script.MusicNode
{
    public struct MusicNodeTag : IComponentData{}
    public struct PistolNodeTag : IComponentData{}
    public struct RifleNodeTag : IComponentData{}
    public struct SniperNodeTag : IComponentData{}
    public struct MusicNodeCubeTag : IComponentData {}
    
    public struct MusicNodeInfoSingletonTag : IComponentData {}
    public struct PistolNodeRemoveTag : IComponentData, IEnableableComponent {}
    public struct RifleNodeRemoveTag : IComponentData, IEnableableComponent {}
    public struct SniperNodeRemoveTag : IComponentData, IEnableableComponent {}

    public struct MusicNodeAuthoring : IComponentData
    {
        public MusicNodeInfo NodeInfo;
    }
}