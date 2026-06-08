using Unity.Entities;

namespace MNP.Core.DOTS.Components.LerpRuntime
{
    public class AnimationStringPropertyComponent : IComponentData
    {
        public string Value;

        public int Index;
    }
}
