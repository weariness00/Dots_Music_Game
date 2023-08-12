using Unity.Entities;
using Unity.Mathematics;

namespace Script.MusicNode.Types
{
    public partial struct MusicNodeCubeJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public float CurrentTime;
        public float BPM;
    
        private void Execute(MusicNodeAspect aspect, MusicNodeCubeTag tag, [ChunkIndexInQuery]int sortKey)
        {
            MoveNode(aspect);
        }
        
        private void MoveNode(MusicNodeAspect aspect)
        {
            float time = CurrentTime * BPM;
            if (time >= aspect.LenthToZero) time = aspect.LenthToZero;
            aspect.Position = math.lerp(aspect.StartPosition, float3.zero, time / aspect.LenthToZero);
        }
    }
}
