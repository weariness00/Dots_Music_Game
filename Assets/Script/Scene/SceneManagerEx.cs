using Script.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Scene
{
    public class SceneManagerEx : Singleton<SceneManagerEx>
    {
        public SceneBase CurrentScene {get { return GameObject.FindObjectOfType<SceneBase>(); } }

        public void LoadScene(SceneType type)
        {
            CurrentScene.Clear();
            SceneManager.LoadScene($"{type.ToString()}Scene");
            Destroy(CurrentScene.gameObject);
        }
    }
}
