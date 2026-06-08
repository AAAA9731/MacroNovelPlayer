using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Components
{
    [BurstCompile]
    public struct Object3DComponent : IComponentData
    {
        public int MeshIndex;
        public int TextureIndex;
    }
}
