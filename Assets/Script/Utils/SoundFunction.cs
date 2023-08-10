using Script.Manager;
using UnityEngine;

namespace Script.Utils
{
    public class SoundFunction : MonoBehaviour
    {
        public void Play_Effect(AudioClip clip)
        {
            Managers.Sound.Play(clip, SoundType.Effect);
        }
        public void Play_BGM(AudioClip clip)
        {
            Managers.Sound.Play(clip, SoundType.BGM);
        }
    }
}