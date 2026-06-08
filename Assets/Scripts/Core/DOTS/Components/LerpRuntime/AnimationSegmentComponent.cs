using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    [BurstCompile]
    public struct AnimationSegmentComponent : IBufferElementData
    {
        public float StartValue;
        public float EndValue;
        public float StartTan;
        public float EndTan;
        public float StartTime;
        public float DurationTime;
    }
}
