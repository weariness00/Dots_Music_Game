using Script.Manager;
using Unity.Entities;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeRemoveJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        
        public Entity GM_Entity;
        public NearNodeEntity NearNodeEntity;

        private void Execute(MusicNodeAspect aspect, [ChunkIndexInQuery]int sortKey)
        {
            aspect.Order--;
            if (aspect.Order == 0)
            {
                NearNodeEntity.PistolNode = aspect.Entity;
                ECB.SetComponent(sortKey, GM_Entity, NearNodeEntity);
            }
            else if (aspect.Order < 0)
                ECB.DestroyEntity(sortKey, aspect.Entity);
        }
    }
}
