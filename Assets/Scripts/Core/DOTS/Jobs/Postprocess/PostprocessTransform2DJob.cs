using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(BakeReadyComponent), typeof(Object2DComponent))]
    public partial struct PostprocessTransform2DJob : IJobEntity
    {
        [ReadOnly] 
        public NativeArray<float> InputArray;
        
        [BurstCompile]
        public void Execute(ref ElementComponent elementComponent)
        {
            NativeSlice<float> values = InputArray.Slice(elementComponent.PropertyIndex, 5);
            float3 pos = new(values[0], values[1], 0);
            quaternion rot = quaternion.RotateZ(math.radians(values[2]));
            float3 scl = new(values[3], values[4], 1);
            elementComponent.TransformMatrix = float4x4.TRS(pos, rot, scl);
        }
    }
}
