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
            CreateMap<ButtonUseKeyboardInputProxy, ButtonUseKeyboardInput>().AutoCacheAndMap();
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
