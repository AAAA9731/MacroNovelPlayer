using MNP.Core.DOTS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(BakeReadyComponent), typeof(Object3DComponent))]
    public partial struct PostprocessTransform3DJob : IJobEntity
    {
        [ReadOnly] 
        public NativeArray<float> InputArray;
        
        [BurstCompile]
        public void Execute(ref ElementComponent elementComponent)
        {
            NativeSlice<float> values = InputArray.Slice(elementComponent.PropertyIndex, 10);
            float3 pos = new(values[0], values[1], values[2]);
            quaternion rot = new(values[3], values[4], values[5], values[6]);
            float3 scl = new(values[7], values[8], values[9]);
            elementComponent.TransformMatrix = float4x4.TRS(pos, rot, scl);
        }
    }
}
