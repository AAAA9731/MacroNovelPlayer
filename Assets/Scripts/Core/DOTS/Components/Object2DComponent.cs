using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Object2DComponent : IComponentData
    {
        public float TextureIndex;
        public float2 BaseSize;
    }
}
