using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Systems;
using MNP.Helpers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace MNP.Mono
{
    public class Test : MonoBehaviour
    {
        public Texture2DArray Texture;
        public Material Material;
        public Mesh Mesh2D;
        public Mesh Mesh3D;
        public GameObject TextInstance;

        public Texture2D TestTexture;

        public Slider Changed;

        public bool EnableDependency;

        [Range(0, 10000)]
        public int testCount2D = 20;
        [Range(0, 2000)]
        public int testCount3D = 20;
        [Range(0, 200)]
        public int testCountText2D = 2;
        [Range(0, 100)]
        public int testCountText3D = 2;

        uint sourceID;

        void Start()
        {
            OutputSystem system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OutputSystem>();
            system.TestTexture = Texture;
            system.Material2D = Material;
            system.Mesh2D = Mesh2D;
        }

        public void EnableTime()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.StartTime();
        }

        public void DisableTime()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.StopTime();
        }


        public void InterruptPlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.InterruptAll();
        }

        public void ResumePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeAll();
        }

        public void PausePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.PauseTime();
        }

        public void ResumePausePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeTime();
        }
    }
}