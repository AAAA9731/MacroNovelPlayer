using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[WithAll(typeof(BakeReadyComponent), typeof(Text2DComponent))]
public partial struct OutputTextJob : IJobEntity
{
    [ReadOnly]
    public NativeArray<float> PropertyArray;

    public void Execute(in ElementComponent elementComponent, PropertyStringComponent stringComponent, EnabledRefRO<TimeEnabledComponent> timeEnabledComponent)
    {
        if (!timeEnabledComponent.ValueRO)
        {
            return;
        }
        stringComponent.OutputText.text = stringComponent.Value;
    }
}
