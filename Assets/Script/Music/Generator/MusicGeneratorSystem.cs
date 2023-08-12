using System;
using System.Collections.Generic;
using Define;
using Script.JudgePanel;
using Script.JudgPanel;
using Script.Manager;
using Script.Music.Canvas;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.Music.Generator
{
    public partial struct MusicNodeMouseInputGeneratorSystem : ISystem
    {
        [BurstCompile(CompileSynchronously = true)]
        Camera GetCamera() => Camera.main;

        [BurstCompile(CompileSynchronously = true)]
        AudioSource GetAudioSource() => Managers.Sound.GetAudioSource(SoundType.BGM);

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
            state.RequireForUpdate<MusicGeneratorTag>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            var gmEntity = SystemAPI.GetSingletonEntity<GameManagerTag>();
            if (SystemAPI.IsComponentEnabled<MusicStartTag>(gmEntity) == true) return;
            if (Input.GetMouseButtonDown(0) == false) return;
            if (GetAudioSource().clip == null) return; 

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var entity = SystemAPI.GetSingletonEntity<MusicGeneratorTag>();
            var generatorAspect = SystemAPI.GetAspect<MusicGeneratorAspect>(entity);
            var gmAuthoring = SystemAPI.GetComponent<GameManagerAuthoring>(gmEntity);

            var main = GetCamera();
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
                    CollidesWith = 1u << 9,   // 9 : Music Node
                }
            };

            bool isDelete = SystemAPI.IsComponentEnabled<MusicGeneratorDeleteTag>(entity);

            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            if (physicsWorld.CastRay(ray, out var hitNode))
            {
                if(isDelete) ecb.DestroyEntity(hitNode.Entity);
                return;
            }
            ray.Filter.CollidesWith = 1u << 6; // 6 : Generator Plan
            if (physicsWorld.CastRay(ray, out var hit) && isDelete == false)
            {
                if (IsUIHit()) return;

                float3 clickPosition = hit.Position;

                foreach (var musicNodeAuthoring in SystemAPI.Query<MusicNodeAuthoring>())
                    if (clickPosition.Equals(musicNodeAuthoring.NodeInfo.StartPosition))
                        return;

                int entityIndex = 0;
                var entityType = generatorAspect.NodeEntities[entityIndex].Entity;
                var newNodeEntity = ecb.Instantiate(entityType);

                switch (entityIndex) // index에 따른 태그 부여
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

                switch (generatorAspect.JudgePanelType) // 판단 노드에 따른 tag 부여
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
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var calStartPosition = CalStartPointToNode(clickPosition, gmAuthoring.BPM);
                var nodeInfo = new MusicNodeInfo()
                {
                    NodeEntityTypeIndex = entityIndex,
                    judgePanelType = generatorAspect.JudgePanelType,
                    StartPosition = calStartPosition,
                    LenthToDestination = math.distance(calStartPosition, float3.zero),
                };

                generatorAspect.NodeListScriptableObject.Add(new MusicScriptableObjectData() { NodeInfo = nodeInfo });
                ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring() { NodeInfo = nodeInfo });
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        float3 CalStartPointToNode(float3 clickPosition, float bpm)
        {
            float currentTime = Managers.Sound.GetAudioSource(SoundType.BGM).time;

            float curDistance = currentTime * bpm;

            float3 dirToZero = math.normalize(clickPosition - float3.zero);
            return dirToZero * curDistance + clickPosition;
        }

        bool IsUIHit()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition,
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count != 0) return true;
            return false;
        }
    }
}