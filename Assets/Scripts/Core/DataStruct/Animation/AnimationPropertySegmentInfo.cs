using System.Collections.Generic;

namespace MNP.Core.DataStruct.Animation
{
    public class AnimationPropertySegmentInfo
    {
        public int ID;
        public float StartTime;
        public float EndTime;
        public bool IsStatic;
        public float? StaticValue;
        public List<float> AnimationInterruptTimeList;
        public int Index;
    }
}
