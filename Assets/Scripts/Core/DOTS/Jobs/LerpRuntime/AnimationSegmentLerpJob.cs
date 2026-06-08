using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    [WithPresent(typeof(InterruptComponent))]
    public partial struct AnimationSegmentLerpJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(DynamicBuffer<AnimationSegmentComponent> animationSegmentBuffer, ref AnimationPropertyComponent propertyComponent, in TimeComponent timeComponent, EnabledRefRO<InterruptComponent> interruptComponent, EnabledRefRO<TimeEnabledComponent> _)
        {
            if (interruptComponent.ValueRO) 
            {
                return;
            }
            float ease;
            if (timeComponent.AnimationT <= 0)
            {
                ease = 0;
            }
            else if (timeComponent.AnimationT >= 1)
            {
                ease = 1;
            }
            else
            {
                float m0 = animationSegmentBuffer[timeComponent.AnimationIndex].StartTan;
                float m1 = animationSegmentBuffer[timeComponent.AnimationIndex].EndTan;
                float a = 2 + m0 + m1;
                float b = 3 - 2 * m0 - m1;
                ease = ((a * timeComponent.AnimationT + b) * timeComponent.AnimationT + m0) * timeComponent.AnimationT;
            }
            float start, end;
            start = animationSegmentBuffer[timeComponent.AnimationIndex].StartValue;
            end = animationSegmentBuffer[timeComponent.AnimationIndex].EndValue;
            float result = start * (1 - ease) + end * ease;
            propertyComponent.Value = result;
        }
    }
}
