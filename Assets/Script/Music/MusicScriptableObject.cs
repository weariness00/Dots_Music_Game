﻿using Define;
using UnityEngine;

namespace Script.Music
{
    [CreateAssetMenu(fileName = "Music", menuName = "Music", order = 0)]
    public class MusicScriptableObject : ScriptableObject
    {
        public MusicNodeInfo[] NodeList;
    }
}