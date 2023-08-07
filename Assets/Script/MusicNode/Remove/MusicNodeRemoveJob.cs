using Unity.Entities;

namespace Script.MusicNode.Remove
{
    public partial struct MusicNodeRemoveJob : IJobEntity
    {
        private void Execute(MusicNodeAspect aspect)
        {
            aspect.Order--;
        }
    }
}
