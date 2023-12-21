# Custom Car Loader (CCL)

A mod for Derail Valley that allows the creation of custom rolling stock and locomotives. The mod is still under development, thus bugs and idiosyncrasies are to be expected. You can use the car creation package to setup a car in Unity and export it as an assetbundle, which can then be loaded into Derail Valley and spawned in-game. The cars can be spawned with the Comms Radio using the standard Car Spawner menu.

The [original project](https://github.com/Freznosis/DVCustomCarLoader) was created by Freznosis.

## Car Creation

Content authors can find a [guide to car creation](https://foxden.cc/articles/read/car-loader) on the FoxDen website.

## Improving CCL

Before opening pull requests, developers should build and test their changes locally to make sure everything is working as expected.

### Environment Setup

After cloning the repository, some setup is required in order to successfully build the mod DLLs. You will need to create a new [Directory.Build.targets](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2022) file to specify your reference paths. This file will be located in the main directory, next to DVCustomCarLoader.sln.

Below is an example of the necessary structure. When creating your targets file, you will need to replace the three reference paths with the corresponding folders on your system. The first two can be found in your Derail Valley install directory, and the third is your Unity Editor install directory under Program Files. Make sure to include the semicolons **between** each of the paths (and no semicolon after the last path). Also note that shortcuts that you might use in file explorer (such as %ProgramFiles%) won't be expanded in these paths - you need to use the full absolute path.
```xml
<Project>
	<PropertyGroup>
		<ReferencePath>
			P:\SteamLibrary\steamapps\common\Derail Valley\DerailValley_Data\Managed\;
			P:\SteamLibrary\steamapps\common\Derail Valley\DerailValley_Data\Managed\UnityModManager\;
			C:\Program Files\Unity\Hub\Editor\2019.4.22f1\Editor\Data\Managed
		</ReferencePath>
		<AssemblySearchPaths>$(AssemblySearchPaths);$(ReferencePath);</AssemblySearchPaths>
	</PropertyGroup>
</Project>
```

### Build Output

The output DLLs will need to be copied into `Derail Valley install directory > Mods > DVCustomCarLoader` each time the solution is built. Copy them from `bin\Debug` or `bin\Release` depending on the selected build configuration.

### Proxy Configuration

The simplest way to bring standard vanilla configurations into CCL is through Proxies.  Proxies are types in CCL.Types that implement the same parameters as the serialized public attributes of a vanilla behaviour.  Once that proxy exists, you can tell importer how to convert it into a vanilla behaviour with a simple class:
```cs
using CCL.Importer.Proxies;
using System.ComponentModel.Composition;

namespace CCL.Importer.Proxies.MyCategoryOfProxies {
  [Export(typeof(IProxyReplacer))]
  public class MyProxyReplacer : ProxyReplacer<ProxyType, VanillaType> {}
}
```
If you need more control but only need to customize the mapper itself (perhaps there are nested proxies) then you can implement the Configure method from ProxyReplacer.  This method receives an AutoMapper `IMappingExpression<ProxyType, VanillaType>` which you can use to customize nested mappings.  You can also implement `AddAdditionalMappings` and you can configure new AutoMapper mappers in this method.  It is guaranteed to be called at the right time to allow you to create mappers.  Finally you can override CanReplace which will get the actual source component at the time of replacing and let you return a boolean to indicate whether or not to actually proceed with replacing - returning false will skip this component.  This can be used to allow a single proxy to map to multiple destination types based on something in the proxy's configuration. 

In the event you need even further control, you can implement IProxyReplacer directly, but remember to include the Export annotation, and remember that it must be a public class.  You also will need to either implement an AutoMapper Profile for your type or manually map your types.  AutoMapper is pretty powerful and can do most of what you are going to need to do. You will also find some utility methods in `Mapper` for replacing components with their proxies - these are implemented as extension methods - so they are simply called on GameObject or MonoBehaviours.
