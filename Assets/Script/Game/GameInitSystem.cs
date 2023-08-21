using Script.JudgePanel;
using Script.Manager;
using Script.Manager.Game;
using Script.Music;
using Script.Music.Generator;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Script.Game
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(MusicGeneratorInitSystem))]
    public partial struct GameInitSystem : ISystem
    {
        EntityCommandBuffer ecb;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameManagerTag>();
            state.RequireForUpdate<GameInitTag>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicGeneratorTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            var musicData = SystemAPI.ManagedAPI.GetComponent<MusicLoadAuthoring>(gmEntity);

            {
                // Set Game Manager Data
                ecb.SetComponent(gmEntity, new GameManagerAuthoring() { BPM = musicData.MusicScriptableObject.BPM_Speed });
            }

            {
                // generate node
                var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(SystemAPI.GetSingletonEntity<MusicGeneratorTag>());
                GenerateNode(gmEntity, generatorAspect, musicData);
            }
            ecb.RemoveComponent<MusicLoadAuthoring>(gmEntity);
            ecb.RemoveComponent<GameInitTag>(gmEntity);
            
            GameManager.Instance.MusicStart();
        }

        void GenerateNode(Entity gmEntity, MusicGeneratorAspect generatorAspect, MusicLoadAuthoring musicData)
        {
            foreach (var nodeInfo in musicData.MusicScriptableObject.NodeList)
            {
                generatorAspect.NodeListScriptableObject.Add(new MusicScriptableObjectData() { NodeInfo = nodeInfo });

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

                ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring() { NodeInfo = nodeInfo });
                ecb.SetComponent(newNodeEntity, newNodeTransform);

                if (nodeInfo.order == 0)
                    ecb.SetComponent(gmEntity, new NearNodeEntity()
                    {
                        PistolNode = newNodeEntity,
                    });
            }
        }
    }
}