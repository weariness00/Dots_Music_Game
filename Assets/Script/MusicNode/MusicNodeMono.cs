using System;
using Define;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Script.MusicNode
{
    public class MusicNodeMono : MonoBehaviour
    {
    }
    
    public class MusicNodeBaker : Baker<MusicNodeMono>
    {
        public override void Bake(MusicNodeMono authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent<MusicNodeTag>(entity);
            AddComponent(entity, new MusicNodeAuthoring(){NodeInfo = new MusicNodeInfo()});
        }
    }
}
