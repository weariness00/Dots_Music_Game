using Define;
using Unity.Entities;
using Unity.Mathematics;

namespace Script.MusicNode
{
    public struct MusicNodeTag : IComponentData{}
    public struct MusicNodeCubeTag : IComponentData {}

    public struct MusicNodeAuthoring : IComponentData
    {
        public MusicNodeInfo NodeInfo;
    }
}