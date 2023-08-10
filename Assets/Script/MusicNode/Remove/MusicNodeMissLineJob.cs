using Script.JudgePanel;
using Script.JudgPanel;
using Script.Manager;
using Unity.Entities;
using Unity.Mathematics;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeMissLineToPistolJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        public Entity GM_Entity;
        public GameManagerAuthoring GM_Authoring;
        public JudgePanelAuthoring PistolAuthoring;
        
        private void Execute(MusicNodeAspect aspect, PistolNodeTag tag, [ChunkIndexInQuery]int sortKey)
        {
            if (aspect.Order != 0) return;
            
            var dis = math.distance(aspect.Position, float3.zero);
            if (dis < PistolAuthoring.Interval.Distance - 1f)
            {
                ECB.DestroyEntity(sortKey, aspect.Entity);
                GM_Authoring.Miss();
                
                ECB.SetComponent(sortKey, GM_Entity, GM_Authoring);
                ECB.AddComponent(sortKey, GM_Entity, new MusicNodeRemoveTag());
            }
        }
    }
}
