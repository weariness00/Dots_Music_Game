using Unity.Entities;
using UnityEngine;

namespace Script.GameObjectSync.ActiveSync
{
    public struct ActiveSyncChangeTag : IComponentData, IEnableableComponent
    {}    // if change Active add this tag
    public class ActiveSyncAuthoring : IComponentData
    {
        public GameObject GameObject;
    }
}
