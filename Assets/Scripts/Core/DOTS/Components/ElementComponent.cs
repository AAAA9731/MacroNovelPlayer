using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct ElementComponent : IComponentData
    {
        public int ID;
        public int ParentID;
        public float4x4 TransformMatrix;
        public int PropertyIndex;
    }
}
