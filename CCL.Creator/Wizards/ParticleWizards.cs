using CCL.Types.Components;
using CCL.Types.Proxies.Particles;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation;
using CCL.Types;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Wizards
{
    internal class ParticleWizards
    {
        [MenuItem("GameObject/CCL/Particles/Diesel Template", false, 10)]
        public static void CreateDieselParticles(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var root = new GameObject(CarPartNames.Particles.ROOT);
            root.transform.parent = target.transform;

            var comp = root.AddComponent<ParticlesPortReadersControllerProxy>();
            var exhaust = new GameObject("ExhaustEngineSmoke").AddComponent<CopyVanillaParticleSystem>();
            exhaust.SystemToCopy = VanillaParticleSystem.DieselExhaustSmoke1;
            exhaust.transform.parent = comp.transform;
            var highTemp = new GameObject("HighTempEngineSmoke").AddComponent<CopyVanillaParticleSystem>();
            highTemp.SystemToCopy = VanillaParticleSystem.DieselHighTempSmoke;
            highTemp.transform.parent = comp.transform;
            var damage = new GameObject("DamagedEngineSmoke").AddComponent<CopyVanillaParticleSystem>();
            damage.SystemToCopy = VanillaParticleSystem.DieselDamageSmoke;
            damage.transform.parent = comp.transform;

            var de = root.transform.root.GetComponentInChildren<DieselEngineDirectDefinitionProxy>();
            var heat = root.transform.root.GetComponentInChildren<HeatReservoirDefinitionProxy>();

            comp.particlePortReaders = new List<ParticlesPortReadersControllerProxy.ParticlePortReader>
            {
                new ParticlesPortReadersControllerProxy.ParticlePortReader
                {
                    particlesParent = exhaust.gameObject,
                    particleUpdaters = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("RPM_NORMALIZED") : ".RPM_NORMALIZED",
                            inputModifier = new ParticlesPortReadersControllerProxy.ValueModifier(),
                            propertiesToUpdate = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.START_SPEED,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 5, 2.6771793f, 2.6771793f, 1/3f, 1/3f),
                                        new Keyframe(0.35365915f, 7.885854f, 19.569435f, 19.569435f, 1/3f, 1/3f),
                                        new Keyframe(0.6576919f, 12.385969f, 8.384961f, 8.384961f, 1/3f, 1/3f),
                                        new Keyframe(1, 14, 3.3680696f, 3.3680696f, 1/3f, 1/3f))
                                },
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.MAX_PARTICLES,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 70, 130, 130, 1/3f, 1/3f),
                                        new Keyframe(1, 200, 130, 130, 1/3f, 1/3f))
                                },
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.RATE_OVER_TIME,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 60, 110.55776f, 110.55776f, 1/3f, 1/3f),
                                        new Keyframe(1, 200, -5.7477827f, -5.7477827f, 0.34174556f, 1/3f))
                                }
                            }
                        },
                        new ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : ".ENGINE_ON",
                            inputModifier = new ParticlesPortReadersControllerProxy.ValueModifier(),
                            propertiesToUpdate = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0.5f, 0, 2, 2, 0, 1/3f),
                                        new Keyframe(1, 1, 2, 2, 1/3f, 0))
                                }
                            }
                        }
                    }
                },
                new ParticlesPortReadersControllerProxy.ParticlePortReader
                {
                    particlesParent = highTemp.gameObject,
                    particleUpdaters = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = heat != null ? heat.GetFullPortId("TEMPERATURE") : ".TEMPERATURE",
                            inputModifier = new ParticlesPortReadersControllerProxy.ValueModifier(),
                            propertiesToUpdate = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.START_SPEED,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(105, 0, 0.2f, 0.2f, 1/3f, 1/3f),
                                        new Keyframe(120, 5, 0.2f, 0.2f, 1/3f, 1/3f))
                                },
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(105, 0, 2/30f, 2/30f, 1/3f, 1/3f),
                                        new Keyframe(120, 1, 2/30f, 2/30f, 1/3f, 1/3f))
                                }
                            }
                        }
                    }
                },
                new ParticlesPortReadersControllerProxy.ParticlePortReader
                {
                    particlesParent = damage.gameObject,
                    particleUpdaters = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : ".ENGINE_ON",
                            inputModifier = new ParticlesPortReadersControllerProxy.ValueModifier(),
                            propertiesToUpdate = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0.5f, 0, 2, 2, 0, 1/3f),
                                        new Keyframe(1, 1, 2, 2, 1/3f, 0))
                                }
                            }
                        },
                        new ParticlesPortReadersControllerProxy.ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_HEALTH_STATE_EXT_IN") : ".ENGINE_HEALTH_STATE_EXT_IN",
                            inputModifier = new ParticlesPortReadersControllerProxy.ValueModifier(),
                            propertiesToUpdate = new List<ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.START_SIZE,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 3, -12.5f, -12.5f, 0, 1/3f),
                                        new Keyframe(0.2f, 0.5f, -12.5f, -12.5f, 1/3f, 0))
                                },
                                new ParticlesPortReadersControllerProxy.ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlesPortReadersControllerProxy.ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 1, -5, -5, 0, 1/3f),
                                        new Keyframe(0.2f, 0, -5, -5, 1/3f, 0))
                                }
                            }
                        }
                    }
                }
            };

            Undo.RegisterCreatedObjectUndo(root, "Created Diesel Particles");
        }

        [MenuItem("GameObject/CCL/Particles/Diesel Template", true, 10)]
        public static bool CreateDieselParticlesValidate()
        {
            return Selection.activeGameObject && !Selection.activeGameObject.transform.parent;
        }

        [MenuItem("GameObject/CCL/Particles/Steam Template", false, 10)]
        public static void CreateSteamParticles(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var root = new GameObject(CarPartNames.Cab.TELEPORT_ROOT);
            root.transform.parent = target.transform;

            var comp = root.AddComponent<ParticlesPortReadersControllerProxy>();

            Undo.RegisterCreatedObjectUndo(root, "Created Steam Particles");
        }

        [MenuItem("GameObject/CCL/Particles/Steam Template", true, 10)]
        public static bool CreateSteamParticlesValidate()
        {
            return Selection.activeGameObject && !Selection.activeGameObject.transform.parent;
        }
    }
}
