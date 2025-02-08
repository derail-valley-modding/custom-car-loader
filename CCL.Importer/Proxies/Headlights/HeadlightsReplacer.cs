using AutoMapper;
using CCL.Types.Proxies.Headlights;
using DV.Simulation.Cars;
using System.Linq;
using UnityEngine;
using VLB;

namespace CCL.Importer.Proxies.Headlights
{
    internal class HeadlightsReplacer : Profile
    {
        public HeadlightsReplacer()
        {
            CreateMap<HeadlightsMainControllerProxy, HeadlightsMainController>().AutoCacheAndMap()
                .ForMember(d => d.headlightSetupsFront, o => o.Ignore())
                .ForMember(d => d.headlightSetupsRear, o => o.Ignore())
                .AfterMap(HeadlightsMainControllerAfter);
            CreateMap<HeadlightsSubControllerBaseProxy, HeadlightsSubControllerBase>()
                .ForMember(d => d.headlights, o => o.MapFrom(s => Mapper.GetFromCache(s.headlights)))
                .IncludeAllDerived();
            CreateMap<HeadlightsSubControllerStandardProxy, HeadlightsSubControllerStandard>().AutoCacheAndMap();

            CreateMap<HeadlightProxy, Headlight>().AutoCacheAndMap()
                .ForMember(d => d.beamData, o => o.MapFrom(s => MapBeam(s.beamData)));

            CreateMap<HeadlightBeamControllerProxy, HeadlightBeamController>().AutoCacheAndMap()
                .ForMember(d => d.headlightsMainController, o => o.MapFrom(s => Mapper.GetFromCache(s.headlightsMainController)));
            CreateMap<VolumetricLightBeamProxy, VolumetricLightBeam>().AutoCacheAndMap()
                .AfterMap(VolumetricLightBeamAfter);

            CreateMap<AutomaticHeadlightsControllerProxy, AutomaticHeadlightsController>().AutoCacheAndMap();
            CreateMap<RearConnectedDualCarAutomaticHeadlightsControllerProxy, RearConnectedDualCarAutomaticHeadlightsController>().AutoCacheAndMap();

            CreateMap<CarLightsOptimizerProxy, CarLightsOptimizer>().AutoCacheAndMap()
                .ForMember(d => d.beamController, o => o.MapFrom(s => Mapper.GetFromCache(s.beamController)))
                .ForMember(d => d.cabLightDisableSqrDistance, o => o.MapFrom(s => s.cabLightDisableDistance * s.cabLightDisableDistance))
                .ForMember(d => d.headLightGlassDisableSqrDistance, o => o.MapFrom(s => s.headLightGlassDisableDistance * s.headLightGlassDisableDistance))
                .ForMember(d => d.headlightsDisableSqrDistance, o => o.MapFrom(s => s.headlightsDisableDistance * s.headlightsDisableDistance))
                .ForMember(d => d.glaresDisableSqrDistance, o => o.MapFrom(s => s.glaresDisableDistance * s.glaresDisableDistance))
                .ForMember(d => d.beamsDisableSqrDistance, o => o.MapFrom(s => s.beamsDisableDistance * s.beamsDisableDistance));
        }

        private void HeadlightsMainControllerAfter(HeadlightsMainControllerProxy proxy, HeadlightsMainController controller)
        {
            controller.headlightSetupsFront = proxy.headlightSetupsFront.Select(setup => new HeadlightsMainController.HeadlightSetup(
                (HeadlightsMainController.HeadlightSetting)setup.setting,
                setup.subControllers.Select(subcontroller => (HeadlightsSubControllerBase)Mapper.GetFromCache(subcontroller)).ToArray(),
                setup.mainOffSetup)).ToArray();

            controller.headlightSetupsRear = proxy.headlightSetupsRear.Select(setup => new HeadlightsMainController.HeadlightSetup(
                (HeadlightsMainController.HeadlightSetting)setup.setting,
                setup.subControllers.Select(subcontroller => (HeadlightsSubControllerBase)Mapper.GetFromCache(subcontroller)).ToArray(),
                setup.mainOffSetup)).ToArray();

            foreach (var item in proxy.headlightSetupsFront)
            {
                Object.Destroy(item);
            }
            foreach (var item in proxy.headlightSetupsRear)
            {
                Object.Destroy(item);
            }
        }

        private VolumetricBeamControllerBase.VolumetricBeamData MapBeam(VolumetricBeamControllerBaseProxy.VolumetricBeamData data)
        {
            return new VolumetricBeamControllerBase.VolumetricBeamData()
            {
                beam = (VolumetricLightBeam)Mapper.GetFromCache(data.beam),
                intensityOutsideMax = data.intensityOutsideMax,
                intensityInsideMax = data.intensityInsideMax
            };
        }

        private void VolumetricLightBeamAfter(VolumetricLightBeamProxy proxy, VolumetricLightBeam beam)
        {
            beam.gameObject.SetActive(false);

            //beam.colorFromLight
            //beam.colorMode
            //beam.color
            //beam.colorGradient
            //beam.intensityFromLight
            //beam.intensityModeAdvanced
            //beam.intensityInside
            //beam.intensityOutside
            beam.blendingMode = BlendingMode.Additive;
            //beam.spotAngleFromLight
            //beam.spotAngle
            beam.coneRadiusStart = 0.1f;
            beam.shaderAccuracy = ShaderAccuracy.Fast;
            beam.geomMeshType = MeshType.Shared;
            beam.geomCustomSides = 18;
            beam.geomCustomSegments = 5;
            beam.skewingLocalForwardDirection = new Vector3(0.0f, 0.0f, 1.0f);
            beam.clippingPlaneTransform = null!;
            beam.geomCap = true;
            beam.fallOffEndFromLight = true;
            beam.attenuationEquation = AttenuationEquation.Quadratic;
            beam.attenuationCustomBlending = 0.5f;
            beam.fallOffStart = 0.0f;
            beam.fallOffEnd = 70.0f;
            beam.depthBlendDistance = 2.0f;
            beam.cameraClippingDistance = 0.5f;
            //beam.glareFrontal
            beam.glareBehind = 1.0f;
            beam.fresnelPow = 10.0f;
            beam.noiseMode = NoiseMode.Disabled;
            beam.noiseIntensity = 0.5f;
            beam.noiseScaleUseGlobal = true;
            beam.noiseScaleLocal = 0.5f;
            beam.noiseVelocityUseGlobal = true;
            beam.noiseVelocityLocal = new Vector3(0.07f, 0.18f, 0.05f);
            beam.dimensions = Dimensions.Dim3D;
            beam.tiltFactor = Vector2.zero;

            beam.pluginVersion = 1960;

            beam._TrackChangesDuringPlaytime = true;
            beam._SortingLayerID = 0;
            beam._SortingOrder = 0;
            beam._FadeOutBegin = 150;
            beam._FadeOutEnd = 300;

            beam.ValidateProperties();
        }
    }
}
