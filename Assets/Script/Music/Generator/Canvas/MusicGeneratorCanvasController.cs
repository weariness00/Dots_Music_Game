using System;
using System.Collections;
using System.Globalization;
using Script.JudgePanel;
using Script.Manager;
using Script.Manager.Game;
using Script.MusicNode;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Music.Generator.Canvas
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
        public Toggle spawnPerfectToggle;
        public TMP_Dropdown musicSelect;
        public TMP_Dropdown judgePanelSelect;

        private int bpm = 1;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
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
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();

            var musicData = entityManager.GetBuffer<MusicScriptableObjectData>(entity).AsNativeArray();
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
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();

            bpmField.text = musicData.BPM_Speed.ToString(CultureInfo.CurrentCulture);

            var audioSource = Managers.Sound.GetAudioSource(SoundType.BGM);
            audioSource.clip = musicData.clip;
            
            entityManager.AddComponent<MusicLoadTag>(entity);
            entityManager.AddComponentObject(entity, new MusicLoadAuthoring(){MusicScriptableObject = musicData});
        }

        public void JudgPanelType()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();
            
            entityManager.SetComponentData(entity, new MusicGeneratorPanelTypeAuthoring(){PanelType = (JudgePanelType)judgePanelSelect.value});
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
            musicPlayTimeText.text = TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");
        }

        public void SelectBGM()
        { 
            var audioSource = Managers.Sound.GetAudioSource(SoundType.BGM);
            audioSource.clip = Managers.Sound.GetORAddAudioClip($"Music/{musicSelect.captionText.text}");
        }

        public void ChangeBPM()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var gmEntity = entityManager.CreateEntityQuery(typeof(GameManagerTag)).GetSingletonEntity();
            var gmAuthoring = entityManager.GetComponentData<GameManagerAuthoring>(gmEntity);
            bpm = int.Parse(bpmField.text);
            gmAuthoring.BPM = bpm;
            entityManager.SetComponentData(gmEntity, gmAuthoring);
        }

        public void IsOnNodeDelete()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();

            entityManager.SetComponentEnabled<MusicGeneratorDeleteTag>(entity, nodeDeleteToggle.isOn); 
        }

        public void IsSpawnPerfect()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntityQuery(typeof(MusicGeneratorTag)).GetSingletonEntity();

            entityManager.SetComponentEnabled<MusicNodeSpawnPerfectLineTag>(entity, spawnPerfectToggle.isOn); 
        }
        
    }
}
