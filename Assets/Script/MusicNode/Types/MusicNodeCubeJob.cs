using Unity.Entities;

namespace Script.MusicNode.Types
{
    public partial struct MusicNodeCubeJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        public float DeltaTime;
    
        private void Execute(MusicNodeAspect aspect, [ChunkIndexInQuery]int sortKey)
        {
            aspect.MoveNode(DeltaTime);
        }
    }
}
