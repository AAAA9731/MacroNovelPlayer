using MNP.Core.DOTS.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PropertyLerpSystem))]
    public partial struct PostprocessingSystem : ISystem
    {
        public NativeArray<float> PropertyArray;
        
        [BurstCompile]
        void OnUpdate(ref SystemState state)
        {
            state.Dependency = new PostprocessPropertyJob()
                .ScheduleParallel(state.Dependency);
            state.Dependency = new PostprocessTransform2DJob()
            {
                InputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new PostprocessTransform3DJob()
            {
                InputArray = PropertyArray
            }.ScheduleParallel(state.Dependency);
            state.CompleteDependency();
        }

        [BurstCompile]
        void OnDestory(ref SystemState state)
        {
            PropertyArray.Dispose();
        }
    }
}
