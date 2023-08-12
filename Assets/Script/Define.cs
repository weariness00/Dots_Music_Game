using System.Collections.Generic;
using Script.JudgePanel;
using Script.JudgPanel;
using Script.Music.Generator;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Define
{
    [System.Serializable]
    public struct MusicNodeInfo
    {
        public int order;
        
        public int NodeEntityTypeIndex;
        public JudgePanelType judgePanelType;
        public float3 StartPosition;
        public float LenthToDestination;
    }

    public struct MusicNodeLenthToDestinationSort : IComparer<MusicScriptableObjectData>
    {
        public int Compare(MusicScriptableObjectData a, MusicScriptableObjectData b)
        {
            return a.NodeInfo.LenthToDestination.CompareTo(b.NodeInfo.LenthToDestination);
        }
    }
}