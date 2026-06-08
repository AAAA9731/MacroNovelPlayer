using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PreprocessingSystem))]
    public partial struct PropertyLerpSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new AnimationSegmentLerpJob().ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }
    }
}
