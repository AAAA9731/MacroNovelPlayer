using System;
using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using Unity.Entities;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace MNP.Core
{
    public class SceneBaker
    {
        public int PropertyIndexCounter;

        public GameObject TextInstance;

        public async UniTask BakeElements(List<MNObject> objects, IProgress<float> progress)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            for (int i = 0; i < objects.Count; i++)
            {
                MNObject mnObject = objects[i];
                Entity entity = manager.CreateEntity();
                manager.AddComponentData(entity, new CleanComponent());
                ElementComponent elementComponent = new()
                {
                    ID = mnObject.ID,
                    ParentID = -1,
                    PropertyIndex = -1
                };
                switch (mnObject.Type)
                {
                    case ObjectType.Empty2D:
                    case ObjectType.Object2D:
                        SeperateAnimationObject2D(objects, mnObject, mnObject.Animations, manager, entity, ref elementComponent);
                        break;
                    case ObjectType.Empty3D:
                    case ObjectType.Object3D:
                        SeperateAnimationObject3D(objects, mnObject, mnObject.Animations, manager, entity, ref elementComponent);
                        break;
                    case ObjectType.Text2D:
                        SeperateAnimationText2D(objects, mnObject.Animations, manager, entity, ref elementComponent);
                        break;
                    case ObjectType.Text3D:
                        SeperateAnimationText3D(objects, mnObject.Animations, manager, entity, ref elementComponent);
                        break;
                }
                manager.AddComponentData(entity, elementComponent);
                manager.AddComponentData(entity, new BakeReadyComponent());
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
                progress?.Report((float)i / objects.Count);
                await UniTask.Yield();
            }
        }

        #region 2D

        private void SeperateAnimationObject2D(List<MNObject> objects, 
                                               MNObject current,
                                               MNAnimation animationListComponent,
                                               EntityManager manager,
                                               Entity entity,
                                               ref ElementComponent element)
        {
            SeperateCustomProperty2D(objects, animationListComponent, manager, ref element);
            manager.AddSharedComponent(entity, new TextureComponent()
            {
                TextureID = current.TextureID
            });
            manager.AddComponentData(entity, new Object2DComponent()
            {
                TextureIndex = current.Type == ObjectType.Empty2D ? -1 : current.TextureIndex,
                BaseSize = new(current.Object2DSize)
            });
        }

        private void SeperateCustomProperty2D(List<MNObject> objects, 
                                              MNAnimation animationListComponent,
                                              EntityManager manager,
                                              ref ElementComponent element)
        {
            element.PropertyIndex = PropertyIndexCounter;
            foreach (AnimationPropertySegmentInfo property in animationListComponent.AnimationPropertySegmentInfoList)
            {
                Entity entity = manager.CreateEntity();
                manager.AddComponentData(entity, new CleanComponent());
                AnimationPropertyComponent propertyComponent = new()
                {
                    PropertyOffsetIndex = PropertyIndexCounter - element.PropertyIndex
                };
                PropertyIndexCounter++;
                if (property.IsStatic)
                {
                    propertyComponent.Value = property.StaticValue.Value;
                    manager.AddComponentData(entity, propertyComponent);
                    continue;
                }

                List<AnimationSegment> animationList = animationListComponent.AnimationPropertySegmentDictionary[property.ID];

                manager.AddBuffer<AnimationSegmentComponent>(entity);
                DynamicBuffer<AnimationSegmentComponent> animationBuffer = manager.GetBuffer<AnimationSegmentComponent>(entity);
                foreach (AnimationSegment animation in animationList)
                {
                    AnimationSegmentComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        StartTan = animation.StartTan,
                        EndTan = animation.EndTan,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    animationBuffer.Add(component);
                }
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }
                
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                manager.AddComponentData(entity, propertyComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.AddComponentData(entity, new BakeReadyComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        #endregion

        #region 3D

        private void SeperateAnimationObject3D(List<MNObject> objects, 
                                               MNObject current,
                                               MNAnimation animationListComponent,
                                               EntityManager manager,
                                               Entity entity,
                                               ref ElementComponent element)
        {
            SeperateCustomProperty3D(objects, animationListComponent, manager, ref element);
            manager.AddSharedComponent(entity, new TextureComponent()
            {
                TextureID = current.TextureID
            });
            manager.AddSharedComponent(entity, new MeshComponent()
            {
                MeshID = current.Object3DMeshID
            });
            manager.AddComponentData(entity, new Object3DComponent()
            {
                TextureIndex = current.Type == ObjectType.Empty2D ? -1 : current.TextureIndex,
                MeshIndex = current.Object3DMeshIndex
            });
        }

        private void SeperateCustomProperty3D(List<MNObject> objects, 
                                              MNAnimation animationListComponent,
                                              EntityManager manager,
                                              ref ElementComponent element)
        {
            element.PropertyIndex = PropertyIndexCounter;
            foreach (AnimationPropertySegmentInfo property in animationListComponent.AnimationPropertySegmentInfoList)
            {
                Entity entity = manager.CreateEntity();
                manager.AddComponentData(entity, new CleanComponent());
                AnimationPropertyComponent propertyComponent = new()
                {
                    PropertyOffsetIndex = PropertyIndexCounter - element.PropertyIndex
                };
                PropertyIndexCounter++;
                if (property.IsStatic)
                {
                    propertyComponent.Value = property.StaticValue.Value;
                    manager.AddComponentData(entity, propertyComponent);
                    continue;
                }
                List<AnimationSegment> animationList = animationListComponent.AnimationPropertySegmentDictionary[property.ID];
                manager.AddBuffer<AnimationSegmentComponent>(entity);
                DynamicBuffer<AnimationSegmentComponent> animationBuffer = manager.GetBuffer<AnimationSegmentComponent>(entity);
                foreach (AnimationSegment animation in animationList)
                {
                    AnimationSegmentComponent component = new()
                    {
                        StartValue = animation.StartValue,
                        EndValue = animation.EndValue,
                        StartTan = animation.StartTan,
                        EndTan = animation.EndTan,
                        StartTime = animation.StartTime,
                        DurationTime = animation.DurationTime
                    };
                    animationBuffer.Add(component);
                }
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };
                manager.AddComponentData(entity, propertyComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.AddComponentData(entity, new BakeReadyComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        #endregion

        #region Text

        private void SeperateAnimationText2D(List<MNObject> objects, 
                                             MNAnimation animationListComponent,
                                             EntityManager manager,
                                             Entity entity,
                                             ref ElementComponent element)
        {
            SeperateCustomProperty2D(objects, animationListComponent, manager, ref element);
            SeperateCustomStringProperty(animationListComponent, entity, manager);
            manager.AddComponent(entity, typeof(Text2DComponent));
        }

        private void SeperateAnimationText3D(List<MNObject> objects, 
                                             MNAnimation animationListComponent,
                                             EntityManager manager,
                                             Entity entity,
                                             ref ElementComponent element)
        {
            SeperateCustomProperty3D(objects, animationListComponent, manager, ref element);
            SeperateCustomStringProperty(animationListComponent, entity, manager);
            manager.AddComponent(entity, typeof(Text3DComponent));
        }

        private void SeperateCustomStringProperty(MNAnimation animationListComponent,
                                                  Entity elementEntity,
                                                  EntityManager manager)
        {
            foreach (AnimationPropertyStringInfo property in animationListComponent.AnimationPropertyStringInfoList)
            {
                Entity entity = manager.CreateEntity();
                manager.AddComponentData(entity, new CleanComponent());
                TextMeshPro tmp = UnityEngine.Object.Instantiate(TextInstance).GetComponent<TextMeshPro>();
                PropertyStringComponent propertyStringComponent = new()
                {
                    Value = property.StaticValue,
                    OutputText = tmp
                };
                if (property.IsStatic)
                {
                    manager.AddComponentData(entity, propertyStringComponent);
                    continue;
                }

                List<AnimationString> animationList = animationListComponent.AnimationStringSegmentDictionary[property.ID];
                AnimationStringListComponent animationComponent = new()
                {
                    Animations = animationList
                };
                
                manager.AddBuffer<InterruptTimeComponent>(entity);
                DynamicBuffer<InterruptTimeComponent> interruptTimeBuffer = manager.GetBuffer<InterruptTimeComponent>(entity);
                for (int i = 0; i < property.AnimationInterruptTimeList.Count; i++)
                {
                    InterruptTimeComponent component = new()
                    {
                        InterruptTime = property.AnimationInterruptTimeList[i]
                    };
                    interruptTimeBuffer.Add(component);
                }
                
                PropertyInfoComponent propertyInfoComponent = new()
                {
                    StartTime = property.StartTime,
                    EndTime = property.EndTime
                };
                TimeComponent timeComponent = new()
                {
                    Time = 0,
                    InterrputedTime = 0
                };

                manager.AddComponentData(entity, animationComponent);
                manager.AddComponentData(elementEntity, propertyStringComponent);
                manager.AddComponentData(entity, propertyStringComponent);
                manager.AddComponentData(entity, propertyInfoComponent);
                manager.AddComponentData(entity, timeComponent);
                manager.AddComponentData(entity, new TimeEnabledComponent());
                manager.AddComponentData(entity, new InterruptComponent());
                manager.AddComponentData(entity, new BakeReadyComponent());
                manager.SetComponentEnabled<InterruptComponent>(entity, false);
                manager.SetComponentEnabled<TimeEnabledComponent>(entity, false);
            }
        }

        #endregion
    }
}