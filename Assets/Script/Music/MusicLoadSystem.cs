using Script.JudgePanel;
using Script.Manager;
using Script.Music.Generator;
using Script.MusicNode;
using Script.MusicNode.Canvas;
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
            state.RequireForUpdate<GameManagerTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var entity = SystemAPI.GetSingletonEntity<MusicLoadTag>();
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            var musicData = SystemAPI.ManagedAPI.GetComponent<MusicLoadAuthoring>(entity);

            {   // Set Canavs
                MusicNodeInfoCanvasController.Instance.NodeEntity = Entity.Null;
            }
            
            {   // Set Game Manager Data
                ecb.SetComponent(gmEntity, new GameManagerAuthoring(){BPM = musicData.MusicScriptableObject.BPM_Speed});
            }

            {  // prev node destory 
                foreach (var (tag, nodeEntity) in SystemAPI.Query<MusicNodeTag>().WithEntityAccess())
                    ecb.DestroyEntity(nodeEntity);
            }

            { // generate node
                var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(SystemAPI.GetSingletonEntity<MusicGeneratorTag>());
                
                generatorAspect.NodeListScriptableObject.Clear();
                foreach (var nodeInfo in musicData.MusicScriptableObject.NodeList)
                {
                    generatorAspect.NodeListScriptableObject.Add( new MusicScriptableObjectData() { NodeInfo = nodeInfo });
                    
                    var newNodeEntity = ecb.Instantiate(generatorAspect.NodeEntities[(int)nodeInfo.nodeEntityType].Entity);
                    LocalTransform newNodeTransform = new LocalTransform()
                    {
                        Position = nodeInfo.StartPosition,
                        Rotation = quaternion.identity,
                        Scale = 1,
                    };
                    
                    switch (nodeInfo.judgePanelType)
                    {
                        case JudgePanelType.Pistol:
                            ecb.AddComponent<PistolNodeTag>(newNodeEntity);
                            break;
                        case JudgePanelType.Rifle:
                            ecb.AddComponent<RifleNodeTag>(newNodeEntity);
                            break;
                        case JudgePanelType.Sniper:
                            ecb.AddComponent<SniperNodeTag>(newNodeEntity);
                            break;
                    }

                    switch (nodeInfo.nodeEntityType)
                    {
                        case 0: // Cube
                            ecb.AddComponent<MusicNodeCubeTag>(newNodeEntity);
                            break;
                    }
                    
                    ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring(){NodeInfo = nodeInfo});
                    ecb.SetComponent(newNodeEntity, newNodeTransform);
                    
                    if(nodeInfo.order == 0)
                        ecb.SetComponent(gmEntity, new NearNodeEntity()
                        {
                            PistolNode = newNodeEntity,
                        });
                }
            }
            ecb.RemoveComponent<MusicLoadAuthoring>(entity);
            ecb.RemoveComponent<MusicLoadTag>(entity);
        }
    }
}
