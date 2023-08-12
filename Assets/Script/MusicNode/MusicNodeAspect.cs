using Define;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.MusicNode
{
    public readonly partial struct MusicNodeAspect : IAspect
    {
        public readonly Entity Entity;

        public readonly RefRO<MusicNodeTag> Tag;    

        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRW<MusicNodeAuthoring> MusicNodeAuthoring;

        public int Order
        {
            get => MusicNodeAuthoring.ValueRO.NodeInfo.order;
            set => MusicNodeAuthoring.ValueRW.NodeInfo.order = value;
        }
        public float LenthToZero => MusicNodeAuthoring.ValueRO.NodeInfo.LenthToDestination;
        public float3 StartPosition => MusicNodeAuthoring.ValueRO.NodeInfo.StartPosition;
        
        public float3 Position
        {
            get => Transform.ValueRO.Position;
            set => Transform.ValueRW.Position = value;
        }
    }
}
