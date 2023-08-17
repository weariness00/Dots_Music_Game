using Script.JudgePanel;
using Script.Manager;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeMissLineToPistolJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;

        public Entity GM_Entity;
        public LocalTransform NearPistolNodeTransform;
        public GameManagerAuthoring GM_Authoring;
        public JudgePanelAuthoring PistolAuthoring;
        
        private void Execute(NearNodeEntity nearNodeEntity, [ChunkIndexInQuery]int sortKey)
        {
            if (nearNodeEntity.PistolNode == Entity.Null) return;
            
            var dis = math.distance(NearPistolNodeTransform.Position, float3.zero);
            if (dis < PistolAuthoring.Interval.Distance - 1f)
            {
                GM_Authoring.Miss();
                
                ECB.SetComponent(sortKey, GM_Entity, GM_Authoring);
                ECB.SetComponentEnabled<PistolNodeRemoveTag>(sortKey, GM_Entity, true);
            }
        }
    }
}
