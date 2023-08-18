using System;
using System.Collections;
using System.Collections.Generic;
using Script.JudgePanel;
using Script.Manager;
using Script.MusicNode;
using Script.Utils;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Script.Music.Generator.Canvas
{
    public class MusicalScoreCanvasController : Singleton<MusicalScoreCanvasController>, IPointerClickHandler
    {
        [Space] 
        public Image beatImage;
        public RectTransform beatPerfectLineTransform;
        public GameObject beatParent;

        [Space] 
        public Image pistolNodeImage;
        public Image rifleNodeImage;
        public Image sniperNodeImage;
        public GameObject nodeParent;

        [Space] 
        public float moveIntervalDistance = 100f;
        private float _moveSecond = 0f;
        private float perfectTime = 0f;

        private void Update()
        {
            UpdateNodeStruct();
        }

        private void LateUpdate()
        {
            SetInterval();
            BeatControl();
            NodeImageControl();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                var go = eventData.pointerEnter;
                if (go.tag == "Generate Node UI")
                {
                    var nodeStruct = _nodeList.Find((node) => node.GameObject.GetInstanceID().Equals(go.GetInstanceID()));
                    if (nodeStruct != null)
                    {
                        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                        var singletonNodeEntity = entityManager.CreateEntityQuery(typeof(MusicNodeInfoSingletonTag)).GetSingletonEntity();
                        var musicNodeQuery = entityManager.CreateEntityQuery(typeof(MusicNodeAuthoring));
                        Entity selectNodeEntity = Entity.Null;

                        foreach (var entity in musicNodeQuery.ToEntityArray(Allocator.Temp))
                        {
                            var nodeInfo = entityManager.GetComponentData<MusicNodeAuthoring>(entity).NodeInfo;
                            if (nodeInfo.order == nodeStruct.Info.order)
                            {
                                selectNodeEntity = entity;
                                break;
                            }
                        }
                        
                        entityManager.RemoveComponent<MusicNodeInfoSingletonTag>(singletonNodeEntity);
                        entityManager.AddComponent<MusicNodeInfoSingletonTag>(selectNodeEntity);
                        
                        MusicNodeInfoCanvasController.Instance.SetNodeInfo(nodeStruct.Info);
                        MusicNodeInfoCanvasController.Instance.SetNodeEntity(nodeStruct.Entity);
                    }
                }
            }
        }
        
        private void SetInterval()
        {
            float screenWidth = Screen.width;
            _moveSecond = screenWidth / moveIntervalDistance;
            perfectTime = _moveSecond - beatPerfectLineTransform.position.x / moveIntervalDistance;
        }

        private class BeatStruct
        {
            public GameObject GameObject;
            public RectTransform RectTransform;
            public float CurrentTime;
        }

        private List<BeatStruct> _beatMoveList = new List<BeatStruct>();
        private float _currentTime = 0f;

        void BeatControl()
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > 1f)
            {
                _currentTime -= 1f;
                var beatGO = Instantiate(beatImage.gameObject, beatParent.transform);
                beatGO.SetActive(true);
                BeatStruct newBeat = new BeatStruct()
                {
                    GameObject = beatGO,
                    RectTransform = beatGO.GetComponent<RectTransform>(),
                    CurrentTime = 0f,
                };
                _beatMoveList.Add(newBeat);
            }

            var copyList = new List<BeatStruct>(_beatMoveList);
            Vector3 destinationPosition = Vector3.zero;
            destinationPosition.y = beatPerfectLineTransform.position.y;
            foreach (var beat in copyList)
            {
                beat.CurrentTime += Time.deltaTime;
                beat.RectTransform.position = Vector3.Lerp(beatImage.rectTransform.position, destinationPosition, beat.CurrentTime / _moveSecond);
                if (Mathf.Abs(beat.CurrentTime - _moveSecond) < 0.1f)
                {
                    _beatMoveList.Remove(beat);
                    Destroy(beat.GameObject);
                }
            }
        }

        private class MusicNodeStruct
        {
            public Entity Entity;
            public MusicNodeInfo Info;

            public GameObject GameObject;
            public RectTransform RectTransform;
        }

        private List<MusicNodeStruct> _nodeList = new List<MusicNodeStruct>();

        void NodeImageControl()
        {
            var copyList = new List<MusicNodeStruct>(_nodeList);
            var audioTime = Managers.Sound.GetAudioSource(SoundType.BGM).time;

            float moveSecondToPerfectTimeDifferentTime = _moveSecond - perfectTime;
            foreach (var node in copyList)
            {
                if (node.Info.perfectTime - audioTime > _moveSecond)
                {
                    node.RectTransform.position = new Vector3(10000f, 10000f, 10000f);
                    continue;
                }

                var currentTime = audioTime - node.Info.perfectTime + _moveSecond - moveSecondToPerfectTimeDifferentTime;
                Vector3 startPosition = Vector3.zero;
                Vector3 destinationPosition = Vector3.zero;
                switch (node.Info.judgePanelType)
                {
                    case JudgePanelType.Pistol:
                        startPosition = pistolNodeImage.rectTransform.position;
                        break;
                    case JudgePanelType.Rifle:
                        startPosition = rifleNodeImage.rectTransform.position;
                        break;
                    case JudgePanelType.Sniper:
                        startPosition = sniperNodeImage.rectTransform.position;
                        break;
                    default:
                        continue;
                }

                destinationPosition.y = startPosition.y;
                node.RectTransform.position = Vector3.Lerp(startPosition, destinationPosition, currentTime / _moveSecond);
            }
        }

        private IEnumerator FindNodeEntityCoroutine(MusicNodeStruct nodeStruct)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            while (nodeStruct.Entity == Entity.Null)
            {
                yield return null;
                var entityArray = entityManager.CreateEntityQuery(typeof(MusicNodeAuthoring)).ToEntityArray(Allocator.Temp).ToArray();
                foreach (var entity in entityArray)
                {
                    var nodeAuthoring = entityManager.GetComponentData<MusicNodeAuthoring>(entity);
                    if (nodeAuthoring.NodeInfo.id == nodeStruct.Info.id)
                    {
                        nodeStruct.Entity = entity;
                        yield break;
                    }
                }
            }
        }

        public void UpdateNodeStruct()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            foreach (var nodeStruct in _nodeList)
            {
                if(nodeStruct.Entity != Entity.Null)
                    nodeStruct.Info = entityManager.GetComponentData<MusicNodeAuthoring>(nodeStruct.Entity).NodeInfo;
            }
        }

        public void AddNodeList(MusicNodeInfo nodeInfo)
        {   
            GameObject newNodeGo = null;
            switch (nodeInfo.judgePanelType)
            {
                case JudgePanelType.Pistol:
                    newNodeGo = Instantiate(pistolNodeImage.gameObject, nodeParent.transform);
                    break;
                case JudgePanelType.Rifle:
                    newNodeGo = Instantiate(rifleNodeImage.gameObject, nodeParent.transform);
                    break;
                case JudgePanelType.Sniper:
                    newNodeGo = Instantiate(sniperNodeImage.gameObject, nodeParent.transform);
                    break;
                default:
                    return;
            }

            var newMusicNodeStruct = new MusicNodeStruct()
            {
                Entity = Entity.Null,
                Info = nodeInfo,

                GameObject = newNodeGo,
                RectTransform = newNodeGo.GetComponent<RectTransform>(),
            };

            _nodeList.Add(newMusicNodeStruct);

            StartCoroutine(FindNodeEntityCoroutine(newMusicNodeStruct));
        }

        public void RemoveNodeList(MusicNodeInfo nodeInfo)
        {
            MusicNodeStruct removeStruct = null;

            foreach (var node in _nodeList)
            {
                if (node.Info.order == nodeInfo.order)
                {
                    removeStruct = node;
                    Destroy(removeStruct.GameObject);
                    break;
                }
            }
            _nodeList.Remove(removeStruct);
        }

    }
}