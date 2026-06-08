using System.Collections.Generic;
using MNP.Core.DataStruct.Animation;

namespace MNP.Core.DataStruct
{
    public class MNAnimation
    {
        public List<AnimationPropertySegmentInfo> AnimationPropertySegmentInfoList;
        public List<AnimationPropertyStringInfo> AnimationPropertyStringInfoList;
        public Dictionary<int, List<AnimationSegment>> AnimationPropertySegmentDictionary;
        public Dictionary<int, List<AnimationString>> AnimationStringSegmentDictionary;
    }
}
