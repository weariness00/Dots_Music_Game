﻿using Unity.Entities;
using Unity.Mathematics;

namespace Script.MusicNode
{
    public struct MusicNodeTag : IComponentData{}

    public struct MusicNodeAuthoring : IComponentData
    {
        public float3 StartPosition;
    }
}