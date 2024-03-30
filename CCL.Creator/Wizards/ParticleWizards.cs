using CCL.Types.Components;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation;
using CCL.Types;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CCL.Types.Proxies.VFX;
using CCL.Types.Proxies.Simulation.Steam;
using static CCL.Types.Proxies.VFX.ParticlesPortReadersControllerProxy;

namespace CCL.Creator.Wizards
{
    internal class ParticleWizards
    {
        #region Diesel

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

            comp.particlePortReaders = new List<ParticlePortReader>
            {
                new ParticlePortReader
                {
                    particlesParent = exhaust.gameObject,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("RPM_NORMALIZED") : ".RPM_NORMALIZED",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 5, 2.6771793f, 2.6771793f, 1/3f, 1/3f),
                                        new Keyframe(0.35365915f, 7.885854f, 19.569435f, 19.569435f, 1/3f, 1/3f),
                                        new Keyframe(0.6576919f, 12.385969f, 8.384961f, 8.384961f, 1/3f, 1/3f),
                                        new Keyframe(1, 14, 3.3680696f, 3.3680696f, 1/3f, 1/3f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.MAX_PARTICLES,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 70, 130, 130, 1/3f, 1/3f),
                                        new Keyframe(1, 200, 130, 130, 1/3f, 1/3f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.RATE_OVER_TIME,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 60, 110.55776f, 110.55776f, 1/3f, 1/3f),
                                        new Keyframe(1, 200, -5.7477827f, -5.7477827f, 0.34174556f, 1/3f))
                                }
                            }
                        },
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : ".ENGINE_ON",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0.5f, 0, 2, 2, 0, 1/3f),
                                        new Keyframe(1, 1, 2, 2, 1/3f, 0))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = highTemp.gameObject,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = heat != null ? heat.GetFullPortId("TEMPERATURE") : ".TEMPERATURE",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(105, 0, 0.2f, 0.2f, 1/3f, 1/3f),
                                        new Keyframe(120, 5, 0.2f, 0.2f, 1/3f, 1/3f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(105, 0, 2/30f, 2/30f, 1/3f, 1/3f),
                                        new Keyframe(120, 1, 2/30f, 2/30f, 1/3f, 1/3f))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = damage.gameObject,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : ".ENGINE_ON",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0.5f, 0, 2, 2, 0, 1/3f),
                                        new Keyframe(1, 1, 2, 2, 1/3f, 0))
                                }
                            }
                        },
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = de != null ? de.GetFullPortId("ENGINE_HEALTH_STATE_EXT_IN") : ".ENGINE_HEALTH_STATE_EXT_IN",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SIZE,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 3, -12.5f, -12.5f, 0, 1/3f),
                                        new Keyframe(0.2f, 0.5f, -12.5f, -12.5f, 1/3f, 0))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
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

        #endregion

        #region Steam

        [MenuItem("GameObject/CCL/Particles/Steam Template", false, 10)]
        public static void CreateSteamParticles(MenuCommand command)
        {
            var target = (GameObject)command.context;

            var root = new GameObject(CarPartNames.Particles.ROOT);
            root.transform.parent = target.transform;

            var comp = root.AddComponent<ParticlesPortReadersControllerProxy>();

            #region Template Objects

            var steam = new GameObject("SteamExhaust").AddComponent<CopyVanillaParticleSystem>();
            steam.SystemToCopy = VanillaParticleSystem.SteamerExhaustWispy;
            steam = steam.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steam.SystemToCopy = VanillaParticleSystem.SteamerExhaustWave;
            steam = steam.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steam.SystemToCopy = VanillaParticleSystem.SteamerExhaustLeak;

            var steamSmall = new GameObject("SteamExhaust Small").AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallWispy;
            steamSmall = steamSmall.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallWave;
            steamSmall = steamSmall.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallLeak;

            var steamLarge = new GameObject("SteamExhaust Large").AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeWispy;
            steamLarge = steamLarge.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeWave;
            steamLarge = steamLarge.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeLeak;

            CopyVanillaParticleSystem system;

            #endregion

            #region Cylinder Cocks

            var cylCockDrip = new GameObject("CylCockWaterDrip");
            cylCockDrip.transform.parent = comp.transform;
            var dripL = new GameObject("CylCockWaterDrip L").AddComponent<CopyVanillaParticleSystem>();
            dripL.transform.parent = cylCockDrip.transform;
            dripL.SystemToCopy = VanillaParticleSystem.SteamerCylCockWaterDripParticles;
            var dripR = new GameObject("CylCockWaterDrip R").AddComponent<CopyVanillaParticleSystem>();
            dripR.transform.parent = cylCockDrip.transform;
            dripR.SystemToCopy = VanillaParticleSystem.SteamerCylCockWaterDripParticles;

            var cylCockSteam = new GameObject("CylCockSteam").AddComponent<CylinderCockParticlePortReaderProxy>();
            cylCockSteam.transform.parent = comp.transform;

            var ccFR = new GameObject("CylCockSteam FR");
            ccFR.transform.parent = cylCockSteam.transform;
            system = Object.Instantiate(steamSmall, ccFR.transform);
            system.name = steamSmall.name;

            var ccRR = Object.Instantiate(ccFR, cylCockSteam.transform);
            ccRR.name = "CylCockSteam RR";
            var ccFL = Object.Instantiate(ccFR, cylCockSteam.transform);
            ccFL.name = "CylCockSteam FL";
            var ccRL = Object.Instantiate(ccFR, cylCockSteam.transform);
            ccRL.name = "CylCockSteam RL";

            cylCockSteam.ApplyS282Defaults();
            cylCockSteam.cylinderSetups[0].frontParticlesParent = ccFR;
            cylCockSteam.cylinderSetups[0].rearParticlesParent = ccRR;
            cylCockSteam.cylinderSetups[1].frontParticlesParent = ccFL;
            cylCockSteam.cylinderSetups[1].rearParticlesParent = ccRL;

            #endregion

            #region Chimney

            var chimney = new GameObject("SteamSmoke").AddComponent<SteamSmokeParticlePortReaderProxy>();
            chimney.transform.parent = comp.transform;
            chimney.ApplyS282Defaults();

            var smoke = new GameObject("SteamSmoke").AddComponent<CopyVanillaParticleSystem>();
            smoke.SystemToCopy = VanillaParticleSystem.SteamerSteamSmoke;
            smoke = smoke.gameObject.AddComponent<CopyVanillaParticleSystem>();
            smoke.SystemToCopy = VanillaParticleSystem.SteamerSteamSmokeThick;
            smoke.transform.parent = chimney.transform;

            var embers = new GameObject("SmokeEmbers").AddComponent<CopyVanillaParticleSystem>();
            embers.SystemToCopy = VanillaParticleSystem.SteamerEmberClusters;
            embers = embers.gameObject.AddComponent<CopyVanillaParticleSystem>();
            embers.SystemToCopy = VanillaParticleSystem.SteamerEmberSparks;
            embers.transform.parent = chimney.transform;

            chimney.smokeParticlesParent = smoke.gameObject;
            chimney.emberParticlesParent = embers.gameObject;

            #endregion

            # region Other

            var blowdown = new GameObject("Blowdown");
            blowdown.transform.parent = comp.transform;
            blowdown = Object.Instantiate(steamLarge, blowdown.transform).gameObject;
            blowdown.name = steamLarge.name;

            var whistle = new GameObject("Whistle");
            whistle.transform.parent = comp.transform;
            whistle = Object.Instantiate(steamSmall, whistle.transform).gameObject;
            whistle.name = steamSmall.name;

            var safety = new GameObject("SteamSafetyRelease");
            safety.transform.parent = comp.transform;
            safety = Object.Instantiate(steam, safety.transform).gameObject;
            safety.name = steam.name;

            var crack = new GameObject("Crack");
            crack.transform.parent = comp.transform;
            crack = Object.Instantiate(steamLarge, crack.transform).gameObject;
            crack.name = steamLarge.name;

            #endregion

            Object.DestroyImmediate(steam);
            Object.DestroyImmediate(steamSmall);
            Object.DestroyImmediate(steamLarge);

            #region Sim Components

            var exhaust = root.transform.root.GetComponentInChildren<SteamExhaustDefinitionProxy>();
            var boiler = root.transform.root.GetComponentInChildren<BoilerDefinitionProxy>();
            var engine = root.transform.root.GetComponentInChildren<ReciprocatingSteamEngineDefinitionProxy>();

            #endregion

            comp.particlePortReaders = new List<ParticlePortReader>
            {
                new ParticlePortReader
                {
                    particlesParent = whistle,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = exhaust != null ? exhaust.GetFullPortId("WHISTLE_FLOW_NORMALIZED") : ".WHISTLE_FLOW_NORMALIZED",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0),
                                        new Keyframe(0.8f, 0),
                                        new Keyframe(1, 1, 5, 5))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SIZE_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = safety,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = boiler != null ? boiler.GetFullPortId("SAFETY_VALVE_NORMALIZED") : ".SAFETY_VALVE_NORMALIZED",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0),
                                        new Keyframe(0.01f, 0),
                                        new Keyframe(1, 1, 1.010101f, 1.010101f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SIZE_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = blowdown,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = boiler != null ? boiler.GetFullPortId("BLOWDOWN_FLOW_NORMALIZED") : ".BLOWDOWN_FLOW_NORMALIZED",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SIZE_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = crack,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = engine != null ? engine.GetFullPortId("CYLINDER_CRACK_FLOW_NORMALIZED") : ".CYLINDER_CRACK_FLOW_NORMALIZED",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SPEED_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                },
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.START_SIZE_MULTIPLIER,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0.1f, 0.9f, 0.9f),
                                        new Keyframe(1, 1, 0.9f, 0.9f))
                                }
                            }
                        }
                    }
                }
            };

            Undo.RegisterCreatedObjectUndo(root, "Created Steam Particles");
        }

        [MenuItem("GameObject/CCL/Particles/Steam Template", true, 10)]
        public static bool CreateSteamParticlesValidate()
        {
            return Selection.activeGameObject && !Selection.activeGameObject.transform.parent;
        }

        #endregion
    }
}
