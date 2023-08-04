using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace Script.JudgPanel
{
    [BurstCompile]
    public struct JudgPanelCollisionJob : ICollisionEventsJob
    {
        public EntityCommandBuffer ECB;

        public Entity PistolPanel;
        public Entity RiflePanel;
        public Entity SniperPanel;

        public ComponentLookup<PistolNodeTag> PistolNodeLookup;
        public ComponentLookup<RifleNodeTag> RifleNodeLookup;
        public ComponentLookup<SniperNodeTag> SniperNodeLookup;
        
        private bool IsPistolNode(Entity entityA) => PistolNodeLookup.HasComponent(entityA);
        private bool IsRifleNode(Entity entity) => RifleNodeLookup.HasComponent(entity);
        private bool IsSniperNode(Entity entity) => SniperNodeLookup.HasComponent(entity);

        private void PistolNode(Entity entityA, Entity entityB)
        {
            if ((IsPistolNode(entityA) && PistolPanel.Equals(entityB)) == false) return;
     
            ECB.DestroyEntity(entityA);
        }
    
        public void Execute(CollisionEvent collisionEvent)
        {
            Debug.Log("체크");
            
            var entityA = collisionEvent.EntityA;
            var entityB = collisionEvent.EntityB;

            PistolNode(entityA, entityB);
            PistolNode(entityB, entityA);
        }
    }
}
