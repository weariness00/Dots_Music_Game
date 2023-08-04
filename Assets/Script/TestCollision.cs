using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public partial struct TestCollision : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Dependency = new TestCollisionJob() { }.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

public struct TestCollisionJob : ICollisionEventsJob
{
    public void Execute(CollisionEvent collisionEvent)
    {
        Debug.Log($"Entity A : {collisionEvent.EntityA}, Entity B : {collisionEvent.EntityB}");
    }
}
