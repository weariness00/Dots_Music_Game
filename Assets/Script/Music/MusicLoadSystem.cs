using Script.JudgPanel;
using Script.Music.Generator;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.Music
{
    public partial struct MusicLoadSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MusicGeneratorTag>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicLoadTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var entity = SystemAPI.GetSingletonEntity<MusicLoadTag>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            {  // prev node destory 
                
                foreach (var (tag, nodeEntity) in SystemAPI.Query<MusicNodeTag>().WithEntityAccess())
                    ecb.DestroyEntity(nodeEntity);
            }

            { // generate node
                var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(SystemAPI.GetSingletonEntity<MusicGeneratorTag>());
                var musicData = SystemAPI.ManagedAPI.GetComponent<MusicLoadAuthoring>(entity);
                
                generatorAspect.NodeListScriptableObject.Clear();
                foreach (var nodeInfo in musicData.MusicScriptableObject.NodeList)
                {
                    generatorAspect.NodeListScriptableObject.Add( new MusicScriptableObjectData() { NodeInfo = nodeInfo });
                    
                    var newNodeEntity = ecb.Instantiate(generatorAspect.NodeEntities[nodeInfo.NodeEntityTypeIndex].Entity);
                    LocalTransform newNodeTransform = new LocalTransform()
                    {
                        Position = nodeInfo.StartPosition,
                        Rotation = quaternion.identity,
                        Scale = 1,
                    };
                    
                    switch (nodeInfo.JudgPanelType)
                    {
                        case JudgPanelType.Pistol:
                            ecb.AddComponent<PistolNodeTag>(newNodeEntity);
                            break;
                        case JudgPanelType.Rifle:
                            ecb.AddComponent<RifleNodeTag>(newNodeEntity);
                            break;
                        case JudgPanelType.Sniper:
                            ecb.AddComponent<SniperNodeTag>(newNodeEntity);
                            break;
                    }
                    
                    ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring(){NodeInfo = nodeInfo});
                    ecb.SetComponent(newNodeEntity, newNodeTransform);
                }
            }
            ecb.RemoveComponent<MusicLoadAuthoring>(entity);
            ecb.RemoveComponent<MusicLoadTag>(entity);
        }
    }
}
