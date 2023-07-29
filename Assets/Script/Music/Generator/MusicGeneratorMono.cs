using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Music.Generator
{
    public class MusicGeneratorMono : MonoBehaviour
    {
        public GameObject[] nodeObjects;
    }
    
    public class MusicGeneratorBaker : Baker<MusicGeneratorMono>
    {
        public override void Bake(MusicGeneratorMono authoring)
        {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

            AddComponent<MusicGeneratorInitTag>(entity);
            AddComponent<MusicGeneratorTag>(entity);

            Entity[] entities = new Entity[authoring.nodeObjects.Length];
            for (int i = 0; i < authoring.nodeObjects.Length; i++)
                entities[i] = GetEntity(authoring.nodeObjects[i], TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new MusicGeneratorNodeObjects(){ entities = entities});
        }
    }
}
