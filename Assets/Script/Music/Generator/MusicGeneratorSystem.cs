using System;
using System.Collections.Generic;
using Script.JudgePanel;
using Script.Manager;
using Script.Music.Generator.Canvas;
using Script.MusicNode;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;
using MusicNodeInfo = Script.MusicNode.MusicNodeInfo;

namespace Script.Music.Generator
{
    public partial struct MusicNodeMouseInputGeneratorSystem : ISystem
    {
        private int Total;
        
        [BurstCompile(CompileSynchronously = true)]
        Camera GetCamera() => Camera.main;

        [BurstCompile(CompileSynchronously = true)]
        AudioSource GetAudioSource() => Managers.Sound.GetAudioSource(SoundType.BGM);

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Total = 0;
            
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<GameManagerTag>();
            state.RequireForUpdate<MusicGeneratorTag>();
            state.RequireForUpdate<PistolPanelTag>();
            state.RequireForUpdate<RiflePanelTag>();
            state.RequireForUpdate<SniperPanelTag>();
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
            var mouseRay = main.ScreenPointToRay(Input.mousePosition);
            
            var ray = new RaycastInput()
            {
                Start = mouseRay.origin,
                End = mouseRay.GetPoint(100f),
                Filter = new CollisionFilter
                {
                    GroupIndex = 0,
                    // 1u << 6는 Physics Category Names에서 6번째의 레이어마스크이다.
                    BelongsTo = 1u << 0,
                    CollidesWith = (uint)LayerMask.GetMask("Music Node"),   // 9 : Music Node
                }
            };

            bool isDelete = SystemAPI.IsComponentEnabled<MusicGeneratorDeleteTag>(entity);
            var nodeInfoCanvas = MusicNodeInfoCanvasController.Instance;

            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            if (physicsWorld.CastRay(ray, out var hitNode))
            {
                if (IsUIHit()) return;
                nodeInfoCanvas.NodeEntity = Entity.Null;

                var nodeInfo = SystemAPI.GetComponent<MusicNodeAuthoring>(hitNode.Entity).NodeInfo;
                if (isDelete)
                {
                    MusicalScoreCanvasController.Instance.RemoveNodeList(nodeInfo);
                    ecb.DestroyEntity(hitNode.Entity);
                }
                else
                {
                    foreach (var (tag, nodeInfoSingletonEntity) in SystemAPI.Query<MusicNodeInfoSingletonTag>().WithEntityAccess())
                        ecb.RemoveComponent<MusicNodeInfoSingletonTag>(nodeInfoSingletonEntity);
                    ecb.AddComponent<MusicNodeInfoSingletonTag>(hitNode.Entity);
                    
                    nodeInfoCanvas.SetNodeInfo(nodeInfo);
                    nodeInfoCanvas.SetNodeEntity(hitNode.Entity);
                }
                return;
            }
            ray.Filter.CollidesWith = (uint)LayerMask.GetMask("GenerateNodePlan"); // 6 : Generator Plan
            if (physicsWorld.CastRay(ray, out var hit) && isDelete == false)
            {
                if (IsUIHit()) return;
                nodeInfoCanvas.NodeEntity = Entity.Null;
                
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

                var calStartPosition = CalStartPointToNode(clickPosition, gmAuthoring.BPM);
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
                }
                
                var nodeInfo = new MusicNodeInfo()
                {
                    id = Total++,
                    
                    nodeEntityType = (MusicNodeType)entityIndex,
                    judgePanelType = generatorAspect.JudgePanelType,
                };
                nodeInfo.SetAllFromStartPosition(calStartPosition);
                
                if(SystemAPI.IsComponentEnabled<MusicNodeSpawnPerfectLineTag>(entity)) nodeInfo.SetAllFromPerfectTime(GetAudioSource().time);

                generatorAspect.NodeListScriptableObject.Add(new MusicScriptableObjectData() { NodeInfo = nodeInfo });
                ecb.SetComponent(newNodeEntity, new MusicNodeAuthoring() { NodeInfo = nodeInfo });

                foreach (var (tag, nodeInfoSingletonEntity) in SystemAPI.Query<MusicNodeInfoSingletonTag>().WithEntityAccess())
                    ecb.RemoveComponent<MusicNodeInfoSingletonTag>(nodeInfoSingletonEntity);
                ecb.AddComponent<MusicNodeInfoSingletonTag>(newNodeEntity);
                
                nodeInfoCanvas.SetNodeInfo(nodeInfo);
                nodeInfoCanvas.SetNodeEntity();
                MusicalScoreCanvasController.Instance.AddNodeList(nodeInfo);
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