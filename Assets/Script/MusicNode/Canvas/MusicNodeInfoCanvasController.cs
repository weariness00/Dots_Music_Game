using System;
using System.Collections;
using Script.JudgePanel;
using Script.Manager;
using Script.Music.Canvas;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Script.MusicNode.Canvas
{
    public class MusicNodeInfoCanvasController : MonoBehaviour
    {
        public static MusicNodeInfoCanvasController Instance;

        public TMP_Text nodeTypeText;
        public TMP_Text judgePanelText;
        public TMP_InputField[] startPosition;
        public TMP_InputField[] nowPosition;
        public TMP_InputField perfectTimeField;

        public Entity NodeEntity = Entity.Null;

        private bool isSetInfo = false;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private Coroutine nowPositionCoroutine = null;
        private IEnumerator NowPositionUpdateCoroutine(Entity entity)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            LocalTransform nodeTransform;
            NodeEntity = entity;

            while (NodeEntity == Entity.Null)
            {
                yield return null;
                var nodeQuery = entityManager.CreateEntityQuery((ComponentType)typeof(MusicNodeInfoSingletonTag));
                if (nodeQuery.IsEmpty == false)
                    NodeEntity = nodeQuery.GetSingletonEntity();
            }
            
            while (true)
            {
                yield return null;

                if (NodeEntity == Entity.Null) break;
                nodeTransform = entityManager.GetComponentData<LocalTransform>(NodeEntity);
                nowPosition[0].text = nodeTransform.Position.x.ToString("F6");
                nowPosition[1].text = nodeTransform.Position.y.ToString("F6");
                nowPosition[2].text = nodeTransform.Position.z.ToString("F6");
            }
        }
        
        public void SetNodeInfo(MusicNodeInfo nodeInfo)
        {
            StartCoroutine(nameof(SetInfoTime));
            
            nodeTypeText.text = nodeInfo.nodeEntityType.ToString();
            judgePanelText.text = nodeInfo.judgePanelType.ToString();
            startPosition[0].text = nodeInfo.StartPosition.x.ToString("F6");
            startPosition[1].text = nodeInfo.StartPosition.y.ToString("F6");
            startPosition[2].text = nodeInfo.StartPosition.z.ToString("F6");
            perfectTimeField.text = nodeInfo.perfectTime.ToString("F3");
        }

        IEnumerator SetInfoTime()
        {
            isSetInfo = true;
            yield return null;
            isSetInfo = false;
        }

        public void SetNodeEntity(Entity entity)
        {
            if(nowPositionCoroutine !=null) StopCoroutine(nowPositionCoroutine);
            nowPositionCoroutine = StartCoroutine(NowPositionUpdateCoroutine(entity));
        }
        public void SetNodeEntity()
        {
            if(nowPositionCoroutine !=null) StopCoroutine(nowPositionCoroutine);
            nowPositionCoroutine = StartCoroutine(NowPositionUpdateCoroutine(Entity.Null));
        }

        public void ChangeNodeStartPosition()
        {
            if (isSetInfo) return;
            if(NodeEntity == Entity.Null) return;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var nodeAuthoring = entityManager.GetComponentData<MusicNodeAuthoring>(NodeEntity);

            float3 newStartPosition;
            newStartPosition.x = float.Parse(startPosition[0].text);
            newStartPosition.y = float.Parse(startPosition[1].text);
            newStartPosition.z = float.Parse(startPosition[2].text);
            nodeAuthoring.NodeInfo.SetAllFromStartPosition(newStartPosition);

            SetNodeInfo(nodeAuthoring.NodeInfo);
            
            entityManager.SetComponentData(NodeEntity, nodeAuthoring);
        }

        public void ChangeNodePerfectTime()
        {
            if (isSetInfo) return;
            if(NodeEntity == Entity.Null) return;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gmEntity = entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
            var gmAuthoring = entityManager.GetComponentData<GameManagerAuthoring>(gmEntity);
            var judgeEntities = entityManager.GetComponentData<JudgePanelSingletonEntity>(gmEntity);
            var nodeInfo = entityManager.GetComponentData<MusicNodeAuthoring>(NodeEntity).NodeInfo;
            
            float perfectTime = float.Parse(perfectTimeField.text);

            var moveDirToSecond = nodeInfo.StartPosition * gmAuthoring.BPM / nodeInfo.LenthToDestination;
            var newStartPosition = moveDirToSecond * perfectTime;

            float judgePanelDistance = 1f;
            switch (nodeInfo.judgePanelType)
            {
                case JudgePanelType.Pistol:
                    judgePanelDistance = entityManager.GetComponentData<JudgePanelAuthoring>(judgeEntities.PistolEntity).Interval.Distance;
                    break;
                case JudgePanelType.Rifle:
                    judgePanelDistance = entityManager.GetComponentData<JudgePanelAuthoring>(judgeEntities.RifleEntity).Interval.Distance;
                    break;
                case JudgePanelType.Sniper:
                    judgePanelDistance = entityManager.GetComponentData<JudgePanelAuthoring>(judgeEntities.SniperEntity).Interval.Distance;
                    break;
            }
            newStartPosition += moveDirToSecond * judgePanelDistance / gmAuthoring.BPM;

            nodeInfo.SetAllFromStartPosition(newStartPosition);
            
            SetNodeInfo(nodeInfo);
            
            entityManager.SetComponentData(NodeEntity, new MusicNodeAuthoring(){NodeInfo = nodeInfo});
        }
    }
}