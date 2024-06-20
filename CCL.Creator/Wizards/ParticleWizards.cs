using CCL.Types.Components;
using CCL.Types.Proxies.Ports;
using CCL.Types.Proxies.Simulation.Diesel;
using CCL.Types.Proxies.Simulation.Steam;
using CCL.Types.Proxies.Simulation;
using CCL.Types.Proxies.VFX;
using CCL.Types;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
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
                            portId = de != null ? de.GetFullPortId("RPM_NORMALIZED") : "",
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
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : "",
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
                            portId = heat != null ? heat.GetFullPortId("TEMPERATURE") : "",
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
                            portId = de != null ? de.GetFullPortId("ENGINE_ON") : "",
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
                            portId = de != null ? de.GetFullPortId("ENGINE_HEALTH_STATE_EXT_IN") : "",
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
            steam.AllowReplacing = false;
            steam = steam.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steam.SystemToCopy = VanillaParticleSystem.SteamerExhaustWave;
            steam.AllowReplacing = false;
            steam = steam.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steam.SystemToCopy = VanillaParticleSystem.SteamerExhaustLeak;
            steam.AllowReplacing = false;

            var steamSmall = new GameObject("SteamExhaust Small").AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallWispy;
            steamSmall.AllowReplacing = false;
            steamSmall = steamSmall.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallWave;
            steamSmall.AllowReplacing = false;
            steamSmall = steamSmall.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamSmall.SystemToCopy = VanillaParticleSystem.SteamerExhaustSmallLeak;
            steamSmall.AllowReplacing = false;

            var steamLarge = new GameObject("SteamExhaust Large").AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeWispy;
            steamLarge.AllowReplacing = false;
            steamLarge = steamLarge.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeWave;
            steamLarge.AllowReplacing = false;
            steamLarge = steamLarge.gameObject.AddComponent<CopyVanillaParticleSystem>();
            steamLarge.SystemToCopy = VanillaParticleSystem.SteamerExhaustLargeLeak;
            steamLarge.AllowReplacing = false;

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

            var smokeParent = new GameObject("SteamSmoke");
            smokeParent.transform.parent = chimney.transform;
            var smoke = new GameObject("SteamSmoke").AddComponent<CopyVanillaParticleSystem>();
            smoke.SystemToCopy = VanillaParticleSystem.SteamerSteamSmoke;
            smoke.transform.parent = smokeParent.transform;
            var thickSmoke = new GameObject("SteamSmokeThick").AddComponent<CopyVanillaParticleSystem>();
            thickSmoke.SystemToCopy = VanillaParticleSystem.SteamerSteamSmokeThick;
            thickSmoke.transform.parent = smokeParent.transform;

            var embers = new GameObject("SmokeEmbers").AddComponent<CopyVanillaParticleSystem>();
            embers.SystemToCopy = VanillaParticleSystem.SteamerEmberClusters;
            embers.AllowReplacing = false;
            embers = embers.gameObject.AddComponent<CopyVanillaParticleSystem>();
            embers.SystemToCopy = VanillaParticleSystem.SteamerEmberSparks;
            embers.AllowReplacing = false;
            embers.transform.parent = chimney.transform;

            chimney.smokeParticlesParent = smokeParent.gameObject;
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

            var leaksParent = new GameObject("RandomLeaks");
            leaksParent.transform.parent = comp.transform;
            leaksParent.SetActive(false);
            var leaks = new GameObject("SteamLeaks").AddComponent<CopyVanillaParticleSystem>();
            leaks.transform.parent = leaksParent.transform;
            leaks.SystemToCopy = VanillaParticleSystem.SteamerSteamLeaks;

            #endregion

            Object.DestroyImmediate(steam);
            Object.DestroyImmediate(steamSmall);
            Object.DestroyImmediate(steamLarge);

            #region Sim Components

            var exhaust = root.transform.root.GetComponentInChildren<SteamExhaustDefinitionProxy>();
            var boiler = root.transform.root.GetComponentInChildren<BoilerDefinitionProxy>();
            var engine = root.transform.root.GetComponentInChildren<ReciprocatingSteamEngineDefinitionProxy>();
            var cylCock = root.transform.root.GetComponentsInChildren<SimComponentDefinitionProxy>()
                .FirstOrDefault(x => x.ID == "cylinderCock");
            var throttleCalculator = root.transform.root.GetComponentsInChildren<SimComponentDefinitionProxy>()
                .FirstOrDefault(x => x.ID == "throttleCalculator");
            var traction = root.transform.root.GetComponentInChildren<TractionDefinitionProxy>();
            var firebox = root.transform.root.GetComponentInChildren<FireboxDefinitionProxy>();

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
                            portId = exhaust != null ? exhaust.GetFullPortId("WHISTLE_FLOW_NORMALIZED") : "",
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
                            portId = boiler != null ? boiler.GetFullPortId("SAFETY_VALVE_NORMALIZED") : "",
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
                            portId = boiler != null ? boiler.GetFullPortId("BLOWDOWN_FLOW_NORMALIZED") : "",
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
                            portId = engine != null ? engine.GetFullPortId("CYLINDER_CRACK_FLOW_NORMALIZED") : "",
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
                    particlesParent = dripL.gameObject,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = engine != null ? engine.GetFullPortId("WATER_IN_CYLINDERS_NORMALIZED") : "",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                }
                            }
                        },
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = cylCock != null ? cylCock.GetFullPortId("EXT_IN") : "",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = dripR.gameObject,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = engine != null ? engine.GetFullPortId("WATER_IN_CYLINDERS_NORMALIZED") : "",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                }
                            }
                        },
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = cylCock != null ? cylCock.GetFullPortId("EXT_IN") : "",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1, 1),
                                        new Keyframe(1, 1, 1, 1))
                                }
                            }
                        }
                    }
                },
                new ParticlePortReader
                {
                    particlesParent = leaksParent,
                    particleUpdaters = new List<ParticlePortReader.PortParticleUpdateDefinition>
                    {
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = throttleCalculator != null ? throttleCalculator.GetFullPortId("STEAM_CHEST_PRESSURE") : "",
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
                                    propertyType = ParticlePortReader.ParticleProperty.RATE_OVER_TIME,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 0, 1 / 12f, 1 / 12f),
                                        new Keyframe(12, 1, 1 / 12f, 1 / 12f))
                                }
                            }
                        },
                        new ParticlePortReader.PortParticleUpdateDefinition
                        {
                            portId = traction != null ? traction.GetFullPortId("FORWARD_SPEED_EXT_IN") : "",
                            inputModifier = new ValueModifier(),
                            propertiesToUpdate = new List<ParticlePortReader.PropertyChangeDefinition>
                            {
                                new ParticlePortReader.PropertyChangeDefinition
                                {
                                    propertyType = ParticlePortReader.ParticleProperty.ON_OFF,
                                    propertyChangeCurve = new AnimationCurve(
                                        new Keyframe(0, 1, -0.0852168f, -0.0852168f, 0, 0.08717979f),
                                        new Keyframe(1.5f, 0, -0.9947581f, -0.9947581f, 0.041025322f, 0.15235536f))
                                }
                            }
                        }
                    }
                }
            };

            comp.particleColorPortReaders = new List<ParticleColorPortReader>
            {
                new ParticleColorPortReader
                {
                    particlesParent = smoke.gameObject,
                    portId = firebox != null ? firebox.GetFullPortId("SMOKE_DENSITY") : "",
                    inputModifier = new ValueModifier(),
                    changeType = ColorPropertyChange.RGB_ONLY,
                    startColorMin = new Color(1, 1, 1, 1),
                    startColorMax = new Color(0.122641504f, 0.122641504f, 0.122641504f, 1),
                    colorLerpCurve = new AnimationCurve(
                        new Keyframe(0, 0, 1, 1),
                        new Keyframe(1, 1, 1, 1))
                },
                new ParticleColorPortReader
                {
                    particlesParent = smoke.gameObject,
                    portId = exhaust != null ? exhaust.GetFullPortId("AIR_FLOW") : "",
                    inputModifier = new ValueModifier(),
                    changeType = ColorPropertyChange.ALPHA_ONLY,
                    startColorMin = new Color(0.5019608f, 0.5019608f, 0.5019608f, 1),
                    startColorMax = new Color(0.5019608f, 0.5019608f, 0.5019608f, 0.3137255f),
                    colorLerpCurve = new AnimationCurve(
                        new Keyframe(0, 0, 0.05f, 0.05f),
                        new Keyframe(20, 1, 0.05f, 0.05f))
                },
                new ParticleColorPortReader
                {
                    particlesParent = thickSmoke.gameObject,
                    portId = firebox != null ? firebox.GetFullPortId("SMOKE_DENSITY") : "",
                    inputModifier = new ValueModifier(),
                    changeType = ColorPropertyChange.ALL,
                    startColorMin = new Color(1, 1, 1, 0),
                    startColorMax = new Color(0.12156863f, 0.12156863f, 0.12156863f, 1),
                    colorLerpCurve = new AnimationCurve(
                        new Keyframe(0, 0, -0.0076235156f, -0.0076235156f, 0, 0.14615384f),
                        new Keyframe(0.3f, 0, 0.0056568207f, 0.0056568207f, 1 / 3f, 0.17435893f),
                        new Keyframe(0.7f, 1, 0.020209895f, 0.020209895f, 0.21227589f, 1 / 3f),
                        new Keyframe(1.01f, 1.0027869f, 0.036841f, 0.036841f, 0.38064513f, 0))
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
