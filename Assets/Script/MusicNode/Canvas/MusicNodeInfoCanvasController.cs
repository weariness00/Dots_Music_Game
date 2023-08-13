using System.Collections;
using Script.Manager;
using Script.Music.Canvas;
using TMPro;
using Unity.Entities;
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
        public TMP_InputField perfectTime;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private Coroutine nowPositionCoroutine = null;
        private IEnumerator NowPositionUpdateCoroutine(Entity nodeEntity)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            LocalTransform nodeTransform;
            while (entityManager.Exists(nodeEntity) == false) yield return null;

            while (true)
            {
                if (nodeEntity == Entity.Null) break;

                yield return null;
                nodeTransform = entityManager.GetComponentData<LocalTransform>(nodeEntity);
                nowPosition[0].text = nodeTransform.Position.x.ToString("F6");
                nowPosition[1].text = nodeTransform.Position.y.ToString("F6");
                nowPosition[2].text = nodeTransform.Position.z.ToString("F6");
            }
        }
        
        public void SetNodeInfo(MusicNodeInfo nodeInfo)
        {
            nodeTypeText.text = nodeInfo.nodeEntityType.ToString();
            judgePanelText.text = nodeInfo.judgePanelType.ToString();
            startPosition[0].text = nodeInfo.StartPosition.x.ToString("F6");
            startPosition[1].text = nodeInfo.StartPosition.y.ToString("F6");
            startPosition[2].text = nodeInfo.StartPosition.z.ToString("F6");
            perfectTime.text = nodeInfo.perfectTime.ToString("F3");
        }

        public void SetNodeEntity(Entity nodeEntity)
        {
            if(nowPositionCoroutine !=null) StopCoroutine(nowPositionCoroutine);
            nowPositionCoroutine = StartCoroutine(NowPositionUpdateCoroutine(nodeEntity));
        }
    }
}