using AutoMapper;
using CCL.Types.Proxies.Controls;
using DV.KeyboardInput;

namespace CCL.Importer.Proxies.Controls
{
    internal class KeyboardInputReplacer : Profile
    {
        public KeyboardInputReplacer()
        {
            CreateMap<AnalogSetValueJoystickInputProxy, AnalogSetValueJoystickInput>().AutoCacheAndMap();
            CreateMap<BinaryDecodeValueInputProxy, BinaryDecodeValueInput>().AutoCacheAndMap()
                .ForMember(d => d.targetLeastSignificant, o => o.MapFrom(s => Mapper.GetFromCache(s.targetLeastSignificant)))
                .ForMember(d => d.targetMostSignificant, o => o.MapFrom(s => Mapper.GetFromCache(s.targetMostSignificant)));
            CreateMap<ButtonUseKeyboardInputProxy, ButtonUseKeyboardInput>().AutoCacheAndMap();
            CreateMap<ButtonSetValueFromAxisInputProxy, ButtonSetValueFromAxisInput>().AutoCacheAndMap();
            CreateMap<FireboxKeyboardInputProxy, FireboxKeyboardInput>().AutoCacheAndMap();
            CreateMap<MouseScrollKeyboardInputProxy, MouseScrollKeyboardInput>().AutoCacheAndMap();
            CreateMap<PhysicsForceKeyboardInputProxy, PhysicsForceKeyboardInput>().AutoCacheAndMap();
            CreateMap<PhysicsTorqueKeyboardInputProxy, PhysicsTorqueKeyboardInput>().AutoCacheAndMap();
            CreateMap<ToggleSwitchUseKeyboardInputProxy, ToggleSwitchUseKeyboardInput>().AutoCacheAndMap();
            CreateMap<ToggleValueKeyboardInputProxy, ToggleValueKeyboardInput>().AutoCacheAndMap();

            CreateMap<AKeyboardInputProxy.ActionReference, AKeyboardInput.ActionReference>();
        }
    }
}
