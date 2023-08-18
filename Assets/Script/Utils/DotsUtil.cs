using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Script.Utils
{
    public static class DotsUtil
    {
        public static Entity FindEntity<T>(T component) where T : unmanaged, IComponentData
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = entityManager.CreateEntityQuery(typeof(T));

            if (entityQuery.IsEmpty)
                throw new System.ArgumentNullException($"{typeof(T)} - FindEntity Methode");
            else if (entityQuery.HasSingleton<T>())
                return entityQuery.GetSingletonEntity();
            else
            {
                foreach (var entity in entityQuery.ToEntityArray(Allocator.Temp))
                {
                    if (component.GetHashCode().Equals(entityManager.GetComponentData<T>(entity).GetHashCode()))
                        return entity;
                }
            }
            return Entity.Null;
        }
    }
}