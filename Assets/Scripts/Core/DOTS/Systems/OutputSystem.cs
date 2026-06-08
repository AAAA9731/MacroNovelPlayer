using System.Collections.Generic;
using MNP.Core.DOTS.Components;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PostprocessingSystem))]
    public partial class OutputSystem : SystemBase
    {
        public Texture2DArray TestTexture;
        public int TestTextureIndex;
        public List<Texture2DArray> Texture2Ds;
        public List<Texture2D> Texture3Ds;
        public List<Mesh> Mesh3Ds;
        public Material Material2D;
        public Material Material3D;
        public Mesh Mesh2D;

        private List<Matrix4x4> Matrix4x42Ds;
        private List<float> TextureIndexs;
        private List<float> TextureWidths;
        private List<float> TextureHeights;
        private List<TextMeshPro> TextList;
        
        MaterialPropertyBlock PropertyBlock = new();

        protected override void OnCreate()
        {
            RequireForUpdate<ElementComponent>();
            RequireForUpdate<BakeReadyComponent>();
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponents(out NativeList<TextureComponent> textures, Allocator.TempJob);
            EntityManager.GetAllUniqueSharedComponents(out NativeList<MeshComponent> meshs, Allocator.TempJob);

            Matrix4x42Ds.Clear();
            foreach (TextureComponent texture in textures)
            {
                PropertyBlock.Clear();
                PropertyBlock.SetTexture("_MainTex", Texture2Ds[texture.TextureID]);
                foreach ((var element, var object2D) in SystemAPI.Query<RefRO<ElementComponent>, RefRO<Object2DComponent>>()
                                                                 .WithSharedComponentFilter(texture))
                {
                    Matrix4x42Ds.Add(element.ValueRO.TransformMatrix);
                    TextureIndexs.Add(object2D.ValueRO.TextureIndex);
                    TextureWidths.Add(object2D.ValueRO.BaseSize.x);
                    TextureHeights.Add(object2D.ValueRO.BaseSize.y);
                }
                PropertyBlock.SetFloatArray("_TextureIndex", TextureIndexs);
                PropertyBlock.SetFloatArray("_TextureWidth", TextureWidths);
                PropertyBlock.SetFloatArray("_TextureHeight", TextureHeights);
                Graphics.DrawMeshInstanced(Mesh2D,
                                           0,
                                           Material2D,
                                           Matrix4x42Ds.ToArray(),
                                           Matrix4x42Ds.Count,
                                           PropertyBlock);

                PropertyBlock.Clear();
                foreach (MeshComponent mesh in meshs)
                {
                    foreach ((var element, var object3D) in SystemAPI.Query<RefRO<ElementComponent>, RefRO<Object3DComponent>>()
                                                                     .WithSharedComponentFilter(texture)
                                                                     .WithSharedComponentFilter(mesh))
                    {
                        PropertyBlock.Clear();
                        PropertyBlock.SetTexture("_MainTex", Texture3Ds[texture.TextureID]);
                        Graphics.DrawMesh(Mesh3Ds[mesh.MeshID],
                                          element.ValueRO.TransformMatrix,
                                          Material3D,
                                          0,                               // Layer
                                          null,                            // Camera (null = 主相机)
                                          object3D.ValueRO.MeshIndex);     // Submesh index
                    }
                }
            }
            
            SystemHandle handle = World.Unmanaged.GetExistingUnmanagedSystem<PostprocessingSystem>();
            ref PostprocessingSystem system = ref World.Unmanaged.GetUnsafeSystemRef<PostprocessingSystem>(handle);
            new OutputTextJob()
            {
                PropertyArray = system.PropertyArray
            }.Run();
        }
    }
}
