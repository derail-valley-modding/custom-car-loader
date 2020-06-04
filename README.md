# DVCustomCarLoader (Unfinished)

A rudimentary custom car loader for Derail Valley. It works great as is, but is missing many of the planned features I had for it. You can setup a car in Unity and then export it as an assetbundle. After that you can use the Derail Valley mod to load the car. The cars can be spawned with the Comms Radio in game using "Custom Car Spawner".

The unity exporter source/plugin is included in .7z file in release.

## HOW TO EXPORT

1) Open your Unity project. Import the CUSTOM_CAR_LOADER.unitypackage

2) Open CreateCar scene. You should see car_template in the scene. 
***-If not, drag car_template into the scene and zero out the position, rotation, and scale.***

3) Drag your model into the scene, zero out your model as well. Your model should follow the X and Z axis of the car_template. 
***-If you don't have a model you can use the Flatbed_WellCar for this tutorial.***

4) Goto car_template, right click, Unpack Prefab Completely.

5) Make sure your model is a child of an empty gameobject. ***The mesh shouldn't be at the top of the prefab.***

6) Drag TrainCarSetup script to your model and then create a prefab from it.
-***Create AssetBundle***: Once you created your car prefab, goto TrainCarSetup script and click CreateAssetBundleForTrainCar.

7) Now you can align your bogies, couplers, chains, etc. 
***-After aligning bogies, click AlignBogieColliders on TrainCarSetup script to make sure bogie positions and colliders are correctly setup.***

8) After you are done aligning all positions, right click TrainCarSetup script and click Prepare for Export.

9) When ExportTrainCar window pops up. Click "Prepare TrainCar for Export".

10) Setup your settings for the car. 
-***Identifier*** is the name of your car. 
-***Type of car*** is the underlying type of car. Pick the type that matches your car the closest.

11) Click "Finalize TrainCar settings" once you are done entering your cars info.

12) Read *[HOW TO EXPORT]* or *[HOW TO AVOID ERRORS]* within the window if you are having any trouble. 
***-Click "Export Train Car".***

13) The export window should popup to your games directory if it exists within the C:/ directory. If not, goto your game directory and find this folder. 
***-Derail Valley/Mods/DVCustomCarLoader/Cars/***

14) Create a new folder inside of the Cars folder for your new car.

15) Once your car is finished exporting it is ready to use inside of Derail Valley.
