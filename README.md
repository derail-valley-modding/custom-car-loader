# Custom Car Loader (CCL)

A mod for Derail Valley that allows the creation of custom rolling stock and locomotives. The mod is still under development, thus bugs and idiosyncrasies are to be expected. You can use the car creation package to setup a car in Unity and export it as an assetbundle, which can then be loaded into Derail Valley and spawned in-game. The cars can be spawned with the Comms Radio using the standard Car Spawner menu.

The [original project](https://github.com/Freznosis/DVCustomCarLoader) was created by Freznosis.

## Car Creation

Content authors can find a [guide to car creation](https://github.com/katycat5e/DVCustomCarLoader/wiki) in the Wiki.

## Improving CCL

Before opening pull requests, developers should build and test their changes locally to make sure everything is working as expected.

### Environment Setup

After cloning the repository, some setup is required in order to successfully build the mod DLLs. This guide assumes Visual Studio is used for development and the `DVCustomCarLoader` solution has already been opened.

#### Reference Paths for CCL_GameScripts

1. Right click the `CCL_GameScripts` project in the Solution Explorer window and choose `Properties` from the context menu
1. Navigate to the `Reference Paths` tab of the properties window
1. Click `Browse�` next to the `Folder:` input
1. Navigate to `Derail Valley install directory > DerailValley_Data > Managed`
1. Click `Select Folder` in the File Explorer dialog and then `Add Folder` below the `Folder:` input
1. Click `Browse�` again
1. Navigate to `Unity install directory > Editor > Data > Managed`
1. Click `Select Folder` in the File Explorer dialog and then `Add Folder` below the `Folder:` input
1. Save the CCL_GameScripts properties

#### Reference Paths for DVCustomCarLoader

1. Right click the `DVCustomCarLoader` project in the Solution Explorer window and choose `Properties` from the context menu
1. Navigate to the `Reference Paths` tab of the properties window
1. Click `Browse�` next to the `Folder:` input
1. Navigate to `Derail Valley install directory > DerailValley_Data > Managed`
1. Click `Select Folder` in the File Explorer dialog and then `Add Folder` below the `Folder:` input
1. Click `Browse�` again
1. Navigate to `Derail Valley install directory > DerailValley_Data > Managed > UnityModManager`
1. Click `Select Folder` in the File Explorer dialog and then `Add Folder` below the `Folder:` input
1. Save the DVCustomCarLoader properties

> **Note:** The reference paths will be saved locally to `*.csproj.user` files. These files should not be committed as the reference paths may differ between each developer's environment. Git should already be set up to ignore these files.

### Build Output

The output DLLs will need to be copied into `Derail Valley install directory > Mods > DVCustomCarLoader` each time the solution is built. Copy them from `bin\Debug` or `bin\Release` depending on the selected build configuration.
