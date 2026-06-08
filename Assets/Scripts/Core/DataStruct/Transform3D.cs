using Unity.Burst;
using Unity.Mathematics;

namespace MNP.Core.DataStruct
{
    [BurstCompile]
    public struct Transform3D
    {
        public float3 Position;
        public float4 Rotation;
        public float3 Scale;
    }
}
