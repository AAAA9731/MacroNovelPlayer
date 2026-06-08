using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs
{
    [BurstCompile]
    public partial struct PostprocessPropertyJob : IJobEntity
    {
        [NativeDisableParallelForRestriction]
        [WriteOnly] public NativeArray<float> OutputArray;

        [BurstCompile]
        public void Execute(in ElementComponent elementComponent, in AnimationPropertyComponent animationPropertyComponent, EnabledRefRO<TimeEnabledComponent> enabled)
        {
            if (!enabled.ValueRO)
            {
                return;
            }
            OutputArray[elementComponent.PropertyIndex + animationPropertyComponent.PropertyOffsetIndex] = animationPropertyComponent.Value;
        }
    }
}
