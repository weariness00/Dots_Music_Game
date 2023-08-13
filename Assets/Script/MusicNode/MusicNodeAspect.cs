using Script.JudgePanel;
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

        public MusicNodeType NodeType => MusicNodeAuthoring.ValueRO.NodeInfo.nodeEntityType;
        public JudgePanelType JudgePanelType => MusicNodeAuthoring.ValueRO.NodeInfo.judgePanelType;
        public float LenthToZero => MusicNodeAuthoring.ValueRO.NodeInfo.LenthToDestination;
        public float3 StartPosition => MusicNodeAuthoring.ValueRO.NodeInfo.StartPosition;
        public float PerfectTime => MusicNodeAuthoring.ValueRO.NodeInfo.perfectTime;
        
        public float3 Position
        {
            get => Transform.ValueRO.Position;
            set => Transform.ValueRW.Position = value;
        }
    }
}
