using System;
using Define;
using Script.JudgPanel;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Script.Music.Generator
{
    public partial struct MusicGeneratorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MusicGeneratorTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetMouseButtonDown(0) == false)
                return;

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var entity = SystemAPI.GetSingletonEntity<MusicGeneratorTag>();
            var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(entity);

            var main = Camera.main;
            Vector3 mousePosition = main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, main.nearClipPlane));
            Vector3 cameraPosition = main.transform.position;
            float3 clickDirection = mousePosition - cameraPosition;

            var ray = new RaycastInput()
            {
                Start = cameraPosition,
                End = clickDirection * 1000f,
                Filter = new CollisionFilter
                {
                    GroupIndex = 0,
                    // 1u << 6는 Physics Category Names에서 6번째의 레이어마스크이다.
                    BelongsTo = 1u << 0,
                    CollidesWith = 1u << 6,
                }
            };

            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            if (physicsWorld.CastRay(ray, out var hit))
            {
                float3 clickPosition = hit.Position;
                clickPosition.z = 0;

                foreach (var musicNodeAuthoring in SystemAPI.Query<MusicNodeAuthoring>())
                {
                    if (clickPosition.Equals(musicNodeAuthoring.NodeInfo.StartPosition))
                    {
                        return;
                    }
                }

                int entityIndex = 0;
                var entityType = generatorAspect.NodeEntities[entityIndex].Entity;
                var newNodeEntity = ecb.Instantiate(entityType);
                
                switch (entityIndex)    // index에 따른 태그 부여
                {
                    case 0:
                        ecb.AddComponent<MusicNodeCubeTag>(newNodeEntity);
                        break;
                }
                LocalTransform newNodeTransform = new LocalTransform()
                {
                    Position = clickPosition,
                    Rotation = quaternion.identity,
                    Scale = 1,
                };
                ecb.SetComponent(newNodeEntity, newNodeTransform);

                switch (generatorAspect.JudgPanelType)
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var nodeInfo = new MusicNodeInfo()
                {
                    NodeEntityTypeIndex = entityIndex,
                    JudgPanelType = generatorAspect.JudgPanelType,
                    StartPosition = clickPosition,
                    LenthToDestination = math.distance(clickPosition, float3.zero),
                };

                generatorAspect.NodeListScriptableObject.Add( new MusicScriptableObjectData() { NodeInfo = nodeInfo });
                ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring(){NodeInfo = nodeInfo});
            }
        }
    }
}