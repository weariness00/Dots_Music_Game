using System;
using System.Collections;
using Script.JudgePanel;
using Script.Manager;
using Script.Music;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.UI
{
    public class GameCanvasControl : MonoBehaviour
    {
        public static GameCanvasControl Instance;
    
        public TMP_Text score;
        public TMP_Text combo;
        public TMP_Text judge;

        public void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            StartCoroutine(nameof(MusicStartUpdateCoroutine));
        }

        IEnumerator MusicStartUpdateCoroutine()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gmEntity = Entity.Null;
            while (gmEntity == Entity.Null)
            {
                yield return null;
                var query = entityManager.CreateEntityQuery(typeof(GameManagerTag));
                if(!query.IsEmpty) gmEntity = query.GetSingletonEntity();
            }

            bool isMusicStart = false;
            while (isMusicStart == false)
            {
                yield return null;
                isMusicStart = entityManager.IsComponentEnabled<MusicStartTag>(gmEntity);
            }

            while (true)
            {
                yield return null;
                var gmAuthoring = entityManager.GetComponentData<GameManagerAuthoring>(gmEntity);
                score.text = gmAuthoring.Score.ToString();
                combo.text = gmAuthoring.Combo.ToString();
                if(gmAuthoring.JudgeType != JudgeType.None) judge.text = gmAuthoring.JudgeType.ToString();
            }
        }
    }
}
