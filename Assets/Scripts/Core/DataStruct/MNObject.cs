using UnityEngine;

namespace MNP.Core.DataStruct
{
    public class MNObject
    {
        public int ID;
        public int TextureID;
        public int TextureIndex;
        public int Object3DMeshID;
        public int Object3DMeshIndex;
        public Vector2 Object2DSize;
        public ObjectType Type;
        public MNAnimation Animations;
    }
}