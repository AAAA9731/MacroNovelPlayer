using System;
using System.Collections.Generic;
using Unity.Burst;

namespace MNP.Helpers
{
    [BurstCompile]
    public static class UtilityHelper
    {
        public const float InterruptTorloance = 0.005f;

        public static void GetFloorIndexInContainer<T>(IList<T> valueContainer, Func<T, float> converter, float referenceValue, out int resultIndex)
        {
            int index = 0;
            for (int i = 1; i < valueContainer.Count; i++)
            {
                if (referenceValue.CompareTo(converter.Invoke(valueContainer[i])) < 0)
                {
                    index = i - 1;
                    break;
                }
                else
                {
                    if (i >= valueContainer.Count - 1)
                    {
                        index = i;
                        break;
                    }
                }
            }
            resultIndex = index;
        }
    }
}
