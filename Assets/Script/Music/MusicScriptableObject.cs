using UnityEngine;
using MusicNodeInfo = Script.MusicNode.MusicNodeInfo;

namespace Script.Music
{
    [CreateAssetMenu(fileName = "Music", menuName = "Music", order = 0)]
    public class MusicScriptableObject : ScriptableObject
    {
        public float BPM_Speed = 1f;
        public AudioClip clip;
        
        public MusicNodeInfo[] NodeList;
    }
}