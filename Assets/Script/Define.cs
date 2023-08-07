using System.Collections.Generic;
using Script.JudgPanel;
using Script.Music.Generator;
using Unity.Entities;
using Unity.Mathematics;

namespace Define
{
    [System.Serializable]
    public struct MusicNodeInfo
    {
        public int order;
        
        public int NodeEntityTypeIndex;
        public JudgPanelType JudgPanelType;
        public float3 StartPosition;
        public float LenthToDestination;
    }

    public struct CurrentTime : IComponentData
    {
        public float Time;
    }

    public struct MusicNodeLenthToDestinationSort : IComparer<MusicScriptableObjectData>
    {
        public int Compare(MusicScriptableObjectData a, MusicScriptableObjectData b)
        {
            return a.NodeInfo.LenthToDestination.CompareTo(b.NodeInfo.LenthToDestination);
        }
    }
}