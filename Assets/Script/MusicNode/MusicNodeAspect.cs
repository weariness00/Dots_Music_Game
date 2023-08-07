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
        private float LenthToZero => MusicNodeAuthoring.ValueRO.NodeInfo.LenthToDestination;
        private float3 StartPosition => MusicNodeAuthoring.ValueRO.NodeInfo.StartPosition;
        
        private readonly RefRW<CurrentTime> _currentTime;
        private float CurrentTime
        {
            get => _currentTime.ValueRO.Time;
            set => _currentTime.ValueRW.Time = value;
        }
        
        public float3 Position
        {
            get => Transform.ValueRO.Position;
            set => Transform.ValueRW.Position = value;
        }

        public void MoveNode(float dt)
        {
            CurrentTime += dt;
            if (CurrentTime >= LenthToZero) CurrentTime = LenthToZero;
            Position = math.lerp(StartPosition, float3.zero, CurrentTime / LenthToZero);
        }
    }
}
