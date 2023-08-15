using Script.JudgPanel;
using Script.Manager.Game;
using Script.MusicNode;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.JudgePanel
{
    public partial struct PistolPanelJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
        [NativeDisableUnsafePtrRestriction] public JudgePanelAspect PistolPanelAspect;

        private void Execute(GameManagerAspect gmAspect, [ChunkIndexInQuery]int sortKey)
        {
            if(gmAspect.NearPistolNode == Entity.Null) return;

            var nearPistolNodeTransform = LocalTransformLookup[gmAspect.NearPistolNode];
            var dis = math.distance(nearPistolNodeTransform.Position, float3.zero);
            var judge = PistolPanelAspect.Judge(dis);
            switch(judge)
            {
                case JudgeType.None:
                    return;
                case JudgeType.Miss:
                    gmAspect.Authoring.ValueRW.Miss();
                    break;
                case JudgeType.Bad:
                    gmAspect.Authoring.ValueRW.Bad();
                    break;
                case JudgeType.Good:
                    gmAspect.Authoring.ValueRW.Good();
                    break;
                case JudgeType.Perfect:
                    gmAspect.Authoring.ValueRW.Perfect();
                    break;
            }
            
            ECB.SetComponentEnabled<PistolNodeRemoveTag>(sortKey, gmAspect.Entity, true);
        }
    }
}
