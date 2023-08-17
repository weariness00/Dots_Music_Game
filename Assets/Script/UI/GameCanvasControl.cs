using System;
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

        private EntityManager _entityManager;
        private Entity _gmEntity;
        public void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _gmEntity = _entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
        }

        public void Update()
        {
            bool isMusicStart = _entityManager.IsComponentEnabled<MusicStartTag>(_gmEntity);

            if (isMusicStart == false) return;

            var gmAuthoring = _entityManager.GetComponentData<GameManagerAuthoring>(_gmEntity);
            score.text = gmAuthoring.Score.ToString();
            combo.text = gmAuthoring.Combo.ToString();
            if(gmAuthoring.JudgeType != JudgeType.None) judge.text = gmAuthoring.JudgeType.ToString();
        }
    }
}
