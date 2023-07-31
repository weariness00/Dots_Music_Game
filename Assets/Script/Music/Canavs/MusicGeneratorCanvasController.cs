using System;
using Define;
using Script.JudgPanel;
using Script.Music.Generator;
using TMPro;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Music.Canavs
{
    public class MusicGeneratorCanvasController : MonoBehaviour
    {
        public string path;
        public TMP_InputField nameField;
        
        private EntityManager _entityManager;
        private Entity _entity;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();
        }

        public void MusicSave()
        {
            var musicData = _entityManager.GetBuffer<MusicScriptableObjectData>(_entity);
            
            var musicScriptableObject = ScriptableObject.CreateInstance<MusicScriptableObject>();
            musicScriptableObject.NodeList = new MusicNodeInfo[musicData.Length];
            for (int i = 0; i < musicData.Length; i++)
                musicScriptableObject.NodeList[i] = musicData[i].NodeInfo;
            
            var curPath = $"{path}/{nameField.text}.asset";
            AssetDatabase.CreateAsset(musicScriptableObject, curPath);
        }

        public void MusicLoad()
        {
            var curPath = $"Music/{nameField.text}";
            var musicData = Resources.Load<MusicScriptableObject>(curPath);

            if (musicData == null)
            {
                Debug.LogWarning($"{curPath}에\n{nameField.text}라는 이름의 Music ScriptableObject가 존재하지 않습니다.");
                return;
            }
            
            _entityManager.AddComponent<MusicLoadTag>(_entity);
            _entityManager.AddComponentObject(_entity, new MusicLoadAuthoring(){MusicScriptableObject = musicData});
        }

        public void JudgPanelType(Int32 index)
        {
            _entityManager.SetComponentData(_entity, new MusicGeneratorPanelTypeAuthoring(){PanelType = (JudgPanelType)index});
        }
    }
}
