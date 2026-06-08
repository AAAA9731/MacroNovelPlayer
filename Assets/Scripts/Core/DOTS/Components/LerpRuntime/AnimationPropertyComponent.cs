using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct AnimationPropertyComponent : IComponentData
    {
        public float Value;
        public int PropertyOffsetIndex;
    }
}
