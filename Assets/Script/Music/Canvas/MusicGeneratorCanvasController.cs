using System;
using System.Collections;
using Define;
using Script.JudgePanel;
using Script.Manager;
using Script.Music.Generator;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Music.Canvas
{
    public class MusicGeneratorCanvasController : MonoBehaviour
    {
        public string path;
        public TMP_InputField nameField;
        public Slider musicBar;
        public Toggle musicPlayAndStop;
        public TMP_Dropdown musicSelect;

        private EntityManager _entityManager;
        private Entity _entity;

        private void Start()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = _entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();
            
            musicBar.onValueChanged.AddListener(MusicBar);
            musicPlayAndStop.onValueChanged.AddListener(MusicPlayAndStop);
        }

        public void MusicSave()
        {
            if (nameField.text.Length == 0)
            {
                Debug.LogWarning("이름을 입력해주십시오.");
                return;
            }
            
            var musicData = _entityManager.GetBuffer<MusicScriptableObjectData>(_entity).AsNativeArray();
            musicData.Sort(new MusicNodeLenthToDestinationSort());
            
            var musicScriptableObject = ScriptableObject.CreateInstance<MusicScriptableObject>();
            musicScriptableObject.NodeList = new MusicNodeInfo[musicData.Length];

            for (int i = 0; i < musicData.Length; i++)
            {
                musicScriptableObject.NodeList[i] = musicData[i].NodeInfo;
                musicScriptableObject.NodeList[i].order = i;
            }
            
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
            _entityManager.SetComponentData(_entity, new MusicGeneratorPanelTypeAuthoring(){PanelType = (JudgePanelType)index});
        }

        public void MusicPlayAndStop(bool isOn)
        {
            AudioSource musicAudio = Managers.Sound.GetAudioSource(SoundType.BGM);
            if (isOn)
            {
                if (musicAudio.clip == null)
                {
                    string path = $"Music/{musicSelect.captionText.text}";
                    Managers.Sound.Play(path, SoundType.BGM);
                }
                
                musicAudio.Play();
                StartCoroutine(nameof(PlayMusicCoroutine));
            }
            else
            {
                musicAudio.Pause();
                StopCoroutine(nameof(PlayMusicCoroutine));
            }
        }

        IEnumerator PlayMusicCoroutine()
        {
            AudioSource musicAudio = Managers.Sound.GetAudioSource(SoundType.BGM);
            float musicLenth = musicAudio.clip.length;
            while (true)
            {
                musicBar.value = musicAudio.time / musicLenth;
                yield return null;
            }
        }

        public void MusicBar(float value)
        {
            AudioSource musicAudio = Managers.Sound.GetAudioSource(SoundType.BGM);

            float musicLenth = 1f;
            if (musicAudio.clip != null)
                musicLenth = musicAudio.clip.length;
            float currentTime = value * musicLenth;

            musicAudio.time = currentTime;
        }
    }
}
