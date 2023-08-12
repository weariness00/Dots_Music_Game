using Unity.Burst;
using Unity.Entities;

namespace Script.GameObjectSync.ActiveSync
{
    public partial struct ActiveSyncSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ActiveSyncChangeTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (authoring, entity) in SystemAPI.Query<ActiveSyncAuthoring>().WithAll<ActiveSyncChangeTag>().WithEntityAccess())
            {
                bool curActive = authoring.GameObject.activeSelf;
                bool changeActive = SystemAPI.IsComponentEnabled<ActiveSyncChangeTag>(entity);
                if(curActive == changeActive) continue;

                if (changeActive == true)
                {
                    authoring.GameObject.SetActive(true);
                }
                else
                {
                    authoring.GameObject.SetActive(false);
                }
            }
        }
    }
}
