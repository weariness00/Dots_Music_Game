
using Unity.Entities;
using Unity.Mathematics;

namespace Define
{
    [System.Serializable]
    public struct MusicNodeInfo
    {
        public int NodeEntityTypeIndex;
        public float3 StartPosition;
        public float LenthToDestination;
    }

    public struct CurrentTime : IComponentData
    {
        public float Time;
    }
}