using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.HUD;

namespace CCL.Importer.Proxies.Controls
{
    internal class HUDElementReplacer : Profile
    {
        public HUDElementReplacer()
        {
            CreateMap<NotchedControlCustomNamesProxy, NotchedControlCustomNames>().AutoCacheAndMap();
            CreateMap<NotchedControlNumberedNamesProxy, NotchedControlNumberedNames>().AutoCacheAndMap();
            CreateMap<PercentageControlNamesProxy, PercentageControlNames>().AutoCacheAndMap();

            CreateMap<LocoControlsReaderProxy, LocoControlsReader>().AutoCacheAndMap();
            CreateMap<LocoIndicatorReaderProxy, LocoIndicatorReader>().AutoCacheAndMap()
                .ForMember(d => d.speed, o => o.MapFrom(s => Mapper.GetFromCache(s.speed)))
                .ForMember(d => d.tmTemp, o => o.MapFrom(s => Mapper.GetFromCache(s.tmTemp)))
                .ForMember(d => d.oilTemp, o => o.MapFrom(s => Mapper.GetFromCache(s.oilTemp)))
                .ForMember(d => d.amps, o => o.MapFrom(s => Mapper.GetFromCache(s.amps)))
                .ForMember(d => d.sand, o => o.MapFrom(s => Mapper.GetFromCache(s.sand)))
                .ForMember(d => d.oil, o => o.MapFrom(s => Mapper.GetFromCache(s.oil)))
                .ForMember(d => d.fuel, o => o.MapFrom(s => Mapper.GetFromCache(s.fuel)))
                .ForMember(d => d.battery, o => o.MapFrom(s => Mapper.GetFromCache(s.battery)))
                .ForMember(d => d.engineRpm, o => o.MapFrom(s => Mapper.GetFromCache(s.engineRpm)))
                .ForMember(d => d.turbineRpmMeter, o => o.MapFrom(s => Mapper.GetFromCache(s.turbineRpmMeter)))
                .ForMember(d => d.brakePipe, o => o.MapFrom(s => Mapper.GetFromCache(s.brakePipe)))
                .ForMember(d => d.mainReservoir, o => o.MapFrom(s => Mapper.GetFromCache(s.mainReservoir)))
                .ForMember(d => d.brakeCylinder, o => o.MapFrom(s => Mapper.GetFromCache(s.brakeCylinder)))
                .ForMember(d => d.voltage, o => o.MapFrom(s => Mapper.GetFromCache(s.voltage)))
                .ForMember(d => d.availablePower, o => o.MapFrom(s => Mapper.GetFromCache(s.availablePower)))
                .ForMember(d => d.tenderCoalLevel, o => o.MapFrom(s => Mapper.GetFromCache(s.tenderCoalLevel)))
                .ForMember(d => d.tenderWaterLevel, o => o.MapFrom(s => Mapper.GetFromCache(s.tenderWaterLevel)))
                .ForMember(d => d.steam, o => o.MapFrom(s => Mapper.GetFromCache(s.steam)))
                .ForMember(d => d.locoWaterLevel, o => o.MapFrom(s => Mapper.GetFromCache(s.locoWaterLevel)))
                .ForMember(d => d.locoCoalLevel, o => o.MapFrom(s => Mapper.GetFromCache(s.locoCoalLevel)))
                .ForMember(d => d.fireTemperature, o => o.MapFrom(s => Mapper.GetFromCache(s.fireTemperature)))
                .ForMember(d => d.steamChest, o => o.MapFrom(s => Mapper.GetFromCache(s.steamChest)))
                .ForMember(d => d.waterInCylinder, o => o.MapFrom(s => Mapper.GetFromCache(s.waterInCylinder)))
                .ForMember(d => d.cylinderTemperature, o => o.MapFrom(s => Mapper.GetFromCache(s.cylinderTemperature)));
            CreateMap<LocoLampReaderProxy, LocoLampReader>().AutoCacheAndMap()
                .ForMember(d => d.fuel, o => o.MapFrom(s => Mapper.GetFromCache(s.fuel)))
                .ForMember(d => d.battery, o => o.MapFrom(s => Mapper.GetFromCache(s.battery)))
                .ForMember(d => d.oil, o => o.MapFrom(s => Mapper.GetFromCache(s.oil)))
                .ForMember(d => d.sandLow, o => o.MapFrom(s => Mapper.GetFromCache(s.sandLow)))
                .ForMember(d => d.sandDeploying, o => o.MapFrom(s => Mapper.GetFromCache(s.sandDeploying)))
                .ForMember(d => d.engineTemp, o => o.MapFrom(s => Mapper.GetFromCache(s.engineTemp)))
                .ForMember(d => d.oilTemp, o => o.MapFrom(s => Mapper.GetFromCache(s.oilTemp)))
                .ForMember(d => d.rpm, o => o.MapFrom(s => Mapper.GetFromCache(s.rpm)))
                .ForMember(d => d.turbineRpm, o => o.MapFrom(s => Mapper.GetFromCache(s.turbineRpm)))
                .ForMember(d => d.voltage, o => o.MapFrom(s => Mapper.GetFromCache(s.voltage)))
                .ForMember(d => d.availablePower, o => o.MapFrom(s => Mapper.GetFromCache(s.availablePower)))
                .ForMember(d => d.amp, o => o.MapFrom(s => Mapper.GetFromCache(s.amp)))
                .ForMember(d => d.wheelSlip, o => o.MapFrom(s => Mapper.GetFromCache(s.wheelSlip)))
                .ForMember(d => d.electronics, o => o.MapFrom(s => Mapper.GetFromCache(s.electronics)))
                .ForMember(d => d.cabLight, o => o.MapFrom(s => Mapper.GetFromCache(s.cabLight)))
                .ForMember(d => d.tmOffline, o => o.MapFrom(s => Mapper.GetFromCache(s.tmOffline)))
                .ForMember(d => d.headlightsFront, o => o.MapFrom(s => Mapper.GetFromCache(s.headlightsFront)))
                .ForMember(d => d.headlightsRear, o => o.MapFrom(s => Mapper.GetFromCache(s.headlightsRear)))
                .ForMember(d => d.wipers, o => o.MapFrom(s => Mapper.GetFromCache(s.wipers)))
                .ForMember(d => d.brakePipe, o => o.MapFrom(s => Mapper.GetFromCache(s.brakePipe)))
                .ForMember(d => d.brakeCyl, o => o.MapFrom(s => Mapper.GetFromCache(s.brakeCyl)))
                .ForMember(d => d.mainRes, o => o.MapFrom(s => Mapper.GetFromCache(s.mainRes)));
            CreateMap<LocoFuseBoxReferenceProxy, LocoFuseBoxReference>().AutoCacheAndMap();
        }
    }
}
