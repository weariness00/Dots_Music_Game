using System.Collections.Generic;
using Script.JudgePanel;
using Script.Music.Generator;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Script.MusicNode
{
    [System.Serializable]
    public enum MusicNodeType
    {
        None,
        Cube,
    }
    
    [System.Serializable]
    public struct MusicNodeInfo
    {
        public int order;
        
        public MusicNodeType nodeEntityType;
        public JudgePanelType judgePanelType;
        public float3 StartPosition;
        public float LenthToDestination;

        public float perfectTime;
    }

    public struct MusicNodeLenthToDestinationSort : IComparer<MusicScriptableObjectData>
    {
        public int Compare(MusicScriptableObjectData a, MusicScriptableObjectData b)
        {
            return a.NodeInfo.LenthToDestination.CompareTo(b.NodeInfo.LenthToDestination);
        }
    }
}