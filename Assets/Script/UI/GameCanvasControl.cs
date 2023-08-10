using System;
using Script.JudgePanel;
using Script.Manager;
using Script.Music;
using TMPro;
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

        public void Update()
        {
            GameManager gm = GameManager.Instance;
            bool isMusicStart = gm.EntityManager.IsComponentEnabled<MusicStartTag>(gm.Entity);

            if (isMusicStart == false) return;

            var gmAuthoring = gm.EntityManager.GetComponentData<GameManagerAuthoring>(gm.Entity);
            score.text = gmAuthoring.Score.ToString();
            combo.text = gmAuthoring.Combo.ToString();
            if(gmAuthoring.JudgeType != JudgeType.None) judge.text = gmAuthoring.JudgeType.ToString();
        }
    }
}
