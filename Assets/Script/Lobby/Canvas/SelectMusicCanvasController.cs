using System.IO;
using Script.Game;
using Script.Music;
using Script.Scene;
using Script.Utils;
using TMPro;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Lobby.Canvas
{
    public class SelectMusicCanvasController : Singleton<SelectMusicCanvasController>
    {
        public MusicToggles Toggles = new MusicToggles();
        public MusicList musicList;

        public ScrollRect musicInfoView;
        public ToggleGroup toggleGroup;

        protected override void Awake()
        {
            musicList.Init();
            Toggles.UIBind(musicInfoView.content.gameObject);
        }

        private void Start()
        {
            SetMusicInfoInToggles();
        }
        
        void SetMusicInfoInToggles()
        {
            for (int i = 0; i < musicList.musics.Length; i++)
            {
                Toggles.GetToggle(i).GetComponentInChildren<TMP_Text>().text = musicList.musics[i].name;
            }
        }
        
        void SelectToggle(Toggle toggle)
        {
            var lobbyScene = SceneManagerEx.Instance.CurrentScene as SceneLobby;
            foreach (var musicData in musicList.musics)
            {
                if (musicData.name == toggle.GetComponentInChildren<TMP_Text>().text)
                {
                    GameObject obj = new GameObject() { name = "MusicData Object" };
                    var gameData = obj.AddComponent<GameData>();
                    gameData.musicData = musicData;
                    DontDestroyOnLoad(obj);
                    break;
                }
            }
        }

        public void GameStart()
        {
            foreach (var toggle in toggleGroup.ActiveToggles())
            {
                if (toggle.isOn)
                {
                    SelectToggle(toggle);
                    break;
                }
            }
            SceneManagerEx.Instance.LoadScene(SceneType.Game);
        }

        public class MusicToggles : UIUtil
        {
            public void UIBind(GameObject bindParent)
            {
                BindAll<Toggle>(bindParent);
            }

            public Toggle GetToggle(int index) => Get<Toggle>(index);
        }
    }
    
    [System.Serializable]
    public class MusicList
    {
        public MusicScriptableObject[] musics;

        public void Init()
        {
            string directoryPath = "Assets/Resources/Music";
            string[] guids = AssetDatabase.FindAssets("t:MusicScriptableObject", new[] { directoryPath });
            
            musics = new MusicScriptableObject[guids.Length];
            for(int i = 0; i < guids.Length; ++i)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                musics[i] = AssetDatabase.LoadAssetAtPath<MusicScriptableObject>(assetPath);
            }
        }

        int CountFilesInDirectory(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] files = directoryInfo.GetFiles();
            return files.Length;
        }
    }
}
