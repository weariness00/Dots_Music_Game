using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // if (Managers.Key.InputActionDown(KeyToAction.Setting_UI))
        // {
        //     gameObject.SetActive(!gameObject.activeSelf);
        //
        //     if (Managers.Scene.CurrenScene.Type != SceneType.Game) return;
        //
        //     if(gameObject.activeSelf)
        //     {
        //         ++GameManager.Instance.OnWindowIndex;
        //         GameManager.Instance.GamePause();
        //
        //         Cursor.visible = true;
        //         Cursor.lockState = CursorLockMode.None;
        //     }
        //     else
        //     {
        //         --GameManager.Instance.OnWindowIndex;
        //         GameManager.Instance.GameResume();
        //
        //         if (GameManager.Instance.OnWindowIndex.Equals(0))
        //         {
        //             Cursor.visible = false;
        //             Cursor.lockState = CursorLockMode.Locked;
        //         }
        //     }
        // }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}