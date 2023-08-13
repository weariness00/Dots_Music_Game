using System;
using System.Collections;
using Script.JudgePanel;
using Script.Manager;
using Script.Music.Generator;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using MusicNodeInfo = Script.MusicNode.MusicNodeInfo;
using MusicNodeLenthToDestinationSort = Script.MusicNode.MusicNodeLenthToDestinationSort;

namespace Script.Music.Canvas
{
    public class MusicGeneratorCanvasController : MonoBehaviour
    {
        public static MusicGeneratorCanvasController Instance;
        
        public string path;
        public TMP_InputField nameField;
        public TMP_InputField bpmField;
        
        [Space]
        public Slider musicBar;
        public Toggle musicPlayAndStop;
        public TMP_Text musicPlayTimeText;
        
        [Space]
        public Toggle nodeDeleteToggle;
        public TMP_Dropdown musicSelect;

        private int bpm = 1;
        
        private EntityManager _entityManager;
        private Entity _entity;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

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

            var clip = Managers.Sound.GetORAddAudioClip($"Music/{musicSelect.captionText.text}");
            if (clip == null)
            {
                Debug.LogWarning("Is not Find Music.so i can't save");
                return;
            }
            
            var musicData = _entityManager.GetBuffer<MusicScriptableObjectData>(_entity).AsNativeArray();
            musicData.Sort(new MusicNodeLenthToDestinationSort());
            
            var musicScriptableObject = ScriptableObject.CreateInstance<MusicScriptableObject>();
            musicScriptableObject.clip = clip;
            musicScriptableObject.BPM_Speed = bpm;

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

            var audioSource = Managers.Sound.GetAudioSource(SoundType.BGM);
            audioSource.clip = musicData.clip;
            
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
            if (musicAudio.clip == null)
            {
                Debug.LogWarning("is not find select Music");
                return;
            }
            
            if (isOn)
            {
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
                musicPlayTimeText.text = TimeSpan.FromSeconds(musicAudio.time).ToString(@"mm\:ss");
                musicBar.value = musicAudio.time / musicLenth;
                yield return null;
            }
        }

        public void MusicBar(float value)
        {
            AudioSource musicAudio = Managers.Sound.GetAudioSource(SoundType.BGM);
            if (musicAudio.clip == null) return;
            
            float musicLenth = musicAudio.clip.length;
            float currentTime = value * musicLenth;

            musicAudio.time = currentTime;
        }

        public void SelectBGM()
        { 
            var audioSource = Managers.Sound.GetAudioSource(SoundType.BGM);
            audioSource.clip = Managers.Sound.GetORAddAudioClip($"Music/{musicSelect.captionText.text}");
        }

        public void ChangeBPM()
        {
            var gmEntity = GameManager.Instance.Entity;
            var gmAuthoring = _entityManager.GetComponentData<GameManagerAuthoring>(gmEntity);
            bpm = int.Parse(bpmField.text);
            gmAuthoring.BPM = bpm;
            _entityManager.SetComponentData(gmEntity, gmAuthoring);
        }

        public void IsOnNodeDelete()
        {
            _entityManager.SetComponentEnabled<MusicGeneratorDeleteTag>(_entity, nodeDeleteToggle.isOn); 
        }
        
    }
}
