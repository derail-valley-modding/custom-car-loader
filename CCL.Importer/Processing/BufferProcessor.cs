using CCL.Importer.Types;
using CCL.Types;
using DV.MultipleUnit;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.ComponentModel.Composition;
using System.Linq;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class BufferProcessor : ModelProcessorStep
    {
        private static Transform? s_buffersDE2;

        private static Transform BuffersDE2 => Extensions.GetCached(ref s_buffersDE2,
            () => TrainCarType.LocoShunter.ToV2().prefab.transform.Find(CarPartNames.Buffers.ROOT));

        public Vector3 FrontRigPosition;
        public Vector3 RearRigPosition;

        public override void ExecuteStep(ModelProcessor context)
        {
            var basePrefab = context.Car.UseCustomBuffers ?
                TrainCarType.FlatbedEmpty.ToV2().prefab :
                context.Car.BufferType.ToTypePrefab();

            // copy main buffer part cohort
            GameObject bufferRoot = basePrefab.transform.Find(CarPartNames.Buffers.ROOT).gameObject;
            bufferRoot = Object.Instantiate(bufferRoot, context.Car.prefab.transform);
            bufferRoot.name = CarPartNames.Buffers.ROOT;

            // Replace rig with MU rig in case we want the MU cable. Also add the MU component to the root.
            if (context.Car.HasMUCable)
            {
                ReplaceRigWithMU(bufferRoot, context.Car.BufferType);
            }

            PositionPair bufferPositions;
            if (context.Car.UseCustomBuffers)
            {
                bufferPositions = SetupCustomBuffers(context.Car.prefab, context.Car);
            }
            else
            {
                bufferPositions = SetupDefaultBuffers(context.Car.prefab);
            }

            FrontRigPosition = bufferPositions.Front;
            RearRigPosition = bufferPositions.Rear;
        }

        private static void ReplaceRigWithMU(GameObject bufferRoot, BufferType bufferType)
        {
            var ogControllers = bufferRoot.GetComponentsInChildren<TrainBufferController>().OrderBy(x => x.transform.position.z);
            var replacements = BuffersDE2.GetComponentsInChildren<TrainBufferController>().OrderBy(x => x.transform.position.z);

            var mum = bufferRoot.transform.parent.gameObject.AddComponent<MultipleUnitModule>();

            // If there is a front connector...
            if (bufferType != BufferType.S282B)
            {
                // Clone the originals and put them into position.
                var front = Object.Instantiate(replacements.Last(), bufferRoot.transform);
                front.transform.position = ogControllers.Last().transform.position;
                front.name = CarPartNames.Buffers.CHAIN_MU;

                // Assign the models.
                front.bufferModelLeft = bufferRoot.transform.Find(CarPartNames.Buffers.PAD_FL);
                front.bufferModelRight = bufferRoot.transform.Find(CarPartNames.Buffers.PAD_FR);

                mum.frontCableAdapter = front.GetComponentInChildren<CouplingHoseMultipleUnitAdapter>(true);
            }

            // If there is a rear connector...
            if (bufferType != BufferType.S282A)
            {
                var rear = Object.Instantiate(replacements.First(), bufferRoot.transform);
                rear.transform.position = ogControllers.First().transform.position;
                rear.name = CarPartNames.Buffers.CHAIN_MU;

                rear.bufferModelLeft = bufferRoot.transform.Find(CarPartNames.Buffers.PAD_RL);
                rear.bufferModelRight = bufferRoot.transform.Find(CarPartNames.Buffers.PAD_RR);

                mum.rearCableAdapter = rear.GetComponentInChildren<CouplingHoseMultipleUnitAdapter>(true);
            }

            // Destroy the original rigs.
            foreach (var item in ogControllers)
            {
                Object.Destroy(item.gameObject);
            }
        }

        private static PositionPair SetupDefaultBuffers(GameObject newPrefab)
        {
            // yeet the dummy buffer rigs so they aren't duplicated
            Transform frontCouplerRig = newPrefab.transform.Find(CarPartNames.Couplers.RIG_FRONT);
            Vector3 frontRigPosition;
            if (frontCouplerRig)
            {
                frontRigPosition = frontCouplerRig.position;
                Object.Destroy(frontCouplerRig.gameObject);
            }
            else
            {
                frontRigPosition = new Vector3(0, 1.05f, 8.77f);
                CCLPlugin.Error("Missing front coupler rig from prefab!");
            }

            Transform rearCouplerRig = newPrefab.transform.Find(CarPartNames.Couplers.RIG_REAR);
            Vector3 rearRigPosition;
            if (rearCouplerRig)
            {
                rearRigPosition = rearCouplerRig.position;
                Object.Destroy(rearCouplerRig.gameObject);
            }
            else
            {
                rearRigPosition = new Vector3(0, 1.05f, -8.77f);
                CCLPlugin.Error("Missing rear coupler rig from prefab!");
            }

            // get copied buffer part cohort
            GameObject bufferRoot = newPrefab.transform.Find(CarPartNames.Buffers.ROOT).gameObject;

            // adjust transforms of buffer components
            for (int i = 0; i < bufferRoot.transform.childCount; i++)
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim();

                if (CarPartNames.Buffers.CHAIN_RIGS.Contains(childName))
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    child.localPosition = (child.localPosition.z > 0) ? frontRigPosition : rearRigPosition;
                }
                else if (CarPartNames.Buffers.PLATE_FRONT.Equals(childName))
                {
                    // front hook plate
                    child.localPosition = frontRigPosition + CarPartOffset.HOOK_PLATE_F;
                }
                else if (CarPartNames.Buffers.PLATE_REAR.Equals(childName))
                {
                    // rear hook plate
                    child.localPosition = rearRigPosition + CarPartOffset.HOOK_PLATE_R;
                }
                else if (CarPartNames.Buffers.STEMS.Contains(childName))
                {
                    // stems
                    Object.Destroy(child.gameObject);
                }
                else if (CarPartNames.Buffers.FRONT_PADS.Contains(childName))
                {
                    // front buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, frontRigPosition.y, frontRigPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_F;
                }
                else if (CarPartNames.Buffers.REAR_PADS.Contains(childName))
                {
                    // rear buffer pads
                    Vector3 xShiftBase = new Vector3(child.localPosition.x, rearRigPosition.y, rearRigPosition.z);
                    child.localPosition = xShiftBase + CarPartOffset.BUFFER_PAD_R;
                }
                else
                {
                    CCLPlugin.LogVerbose($"Unknown buffer child {childName}");
                }
            }

            return new PositionPair(frontRigPosition, rearRigPosition);
        }

        private static PositionPair SetupCustomBuffers(GameObject newPrefab, CCL_CarVariant carSetup)
        {
            Transform frontCouplerRig = newPrefab.transform.Find(CarPartNames.Couplers.RIG_FRONT);
            Vector3 frontRigPosition = frontCouplerRig.position;

            Transform rearCouplerRig = newPrefab.transform.Find(CarPartNames.Couplers.RIG_REAR);
            Vector3 rearRigPosition = rearCouplerRig.position;

            // get copied buffer part cohort
            GameObject bufferRoot = newPrefab.transform.Find(CarPartNames.Buffers.ROOT).gameObject;

            Transform newFrontBufferRig = null!;
            Transform newRearBufferRig = null!;

            // adjust transforms of buffer components
            for (int i = 0; i < bufferRoot.transform.childCount; i++)
            {
                Transform child = bufferRoot.transform.GetChild(i);
                string childName = child.name.Trim(' ', '1');

                if (CarPartNames.Buffers.CHAIN_RIGS.Contains(childName))
                {
                    // front or rear chain rig
                    // determine whether front or rear chain rig: +z is front
                    if (child.localPosition.z > 0)
                    {
                        child.localPosition = frontRigPosition;
                        newFrontBufferRig = child;
                        CCLPlugin.LogVerbose($"Set newFrontBufferRig {newFrontBufferRig.name}");
                    }
                    else
                    {
                        child.localPosition = rearRigPosition;
                        newRearBufferRig = child;
                        CCLPlugin.LogVerbose($"Set newRearBufferRig {newRearBufferRig.name}");
                    }
                }
                else if (CarPartNames.Buffers.PLATE_FRONT.Equals(childName))
                {
                    // front hook plate
                    if (carSetup.HideFrontCoupler)
                    {
                        Object.Destroy(child.gameObject);
                        CCLPlugin.LogVerbose("Destroy Hook Plate F");
                    }
                    else
                    {
                        child.localPosition = frontRigPosition + CarPartOffset.HOOK_PLATE_F;
                        CCLPlugin.LogVerbose("Adjust Hook Plate F");
                    }
                }
                else if (CarPartNames.Buffers.PLATE_REAR.Equals(childName))
                {
                    // rear hook plate
                    if (carSetup.HideBackCoupler)
                    {
                        Object.Destroy(child.gameObject);
                        CCLPlugin.LogVerbose("Destroy Hook Plate R");
                    }
                    else
                    {
                        child.localPosition = rearRigPosition + CarPartOffset.HOOK_PLATE_R;
                        CCLPlugin.LogVerbose("Adjust Hook Plate R");
                    }
                }
                else if (CarPartNames.Buffers.STEMS.Contains(childName))
                {
                    // destroy stems
                    Object.Destroy(child.gameObject);
                    CCLPlugin.LogVerbose("Destroy buffer stems");
                }
                else if (CarPartNames.Buffers.FRONT_PADS.Contains(childName) || CarPartNames.Buffers.REAR_PADS.Contains(childName))
                {
                    // destroy template buffer pads since we're overriding
                    Object.Destroy(child.gameObject);
                    CCLPlugin.LogVerbose($"Destroy buffer pad {childName}");
                }
                else
                {
                    CCLPlugin.LogVerbose($"Unknown buffer child {childName}");
                }
            }

            // duplicate front rig to replace missing rear
            if (!carSetup.HideBackCoupler && !newRearBufferRig)
            {
                newRearBufferRig = Object.Instantiate(newFrontBufferRig, newFrontBufferRig.parent);
                newRearBufferRig.eulerAngles = new Vector3(0, 180, 0);
                newRearBufferRig.localPosition = rearRigPosition;
            }

            // get rid of unwanted rear rig
            if (carSetup.HideBackCoupler && newRearBufferRig)
            {
                Object.Destroy(newRearBufferRig.gameObject);
            }

            // duplicate rear to replace missing front
            if (!carSetup.HideFrontCoupler && !newFrontBufferRig)
            {
                newFrontBufferRig = Object.Instantiate(newRearBufferRig, newRearBufferRig.parent);
                newFrontBufferRig.eulerAngles = Vector3.zero;
                newFrontBufferRig.localPosition = frontRigPosition;
            }

            if (carSetup.HideFrontCoupler && newFrontBufferRig)
            {
                Object.Destroy(newFrontBufferRig.gameObject);
            }

            // reparent buffer pads to new root & adjust anchor positions
            foreach (Transform rig in new[] { frontCouplerRig, rearCouplerRig })
            {
                if (!rig) continue;

                string lPadName, rPadName;
                Transform newBufferRig;

                if (rig == frontCouplerRig)
                {
                    if (carSetup.HideFrontCoupler) continue;
                    lPadName = CarPartNames.Buffers.PAD_FL;
                    rPadName = CarPartNames.Buffers.PAD_FR;
                    newBufferRig = newFrontBufferRig;
                }
                else
                {
                    if (carSetup.HideBackCoupler) continue;
                    lPadName = CarPartNames.Buffers.PAD_RL;
                    rPadName = CarPartNames.Buffers.PAD_RR;
                    newBufferRig = newRearBufferRig;
                }

                CCLPlugin.LogVerbose($"Adjust pads for {newBufferRig?.name}, rig = {rig != null}: {rig?.name}");

                // Reparent buffer pads
                TrainBufferController bufferController = newBufferRig!.gameObject.GetComponentInChildren<TrainBufferController>(true);
                if (bufferController)
                {
                    Vector3 position;

                    var lPad = rig.FindSafe(lPadName);
                    if (lPad)
                    {
                        position = newPrefab.transform.InverseTransformPoint(lPad!.position);
                        lPad.parent = bufferRoot.transform;
                        lPad.localPosition = position;
                        bufferController.bufferModelLeft = lPad;
                    }

                    var rPad = rig.FindSafe(rPadName);
                    if (rPad)
                    {
                        position = newPrefab.transform.InverseTransformPoint(rPad!.position);
                        rPad.parent = bufferRoot.transform;
                        rPad.localPosition = position;
                        bufferController.bufferModelRight = rPad;
                    }
                }
                else
                {
                    CCLPlugin.Warning($"No buffer controller, newBufferRig={newBufferRig} {rig!.name}");
                    continue;
                }

                // Adjust new anchors to match positions in prefab
                Transform? bufferChainRig = rig.FindSafe(CarPartNames.Buffers.CHAIN_REGULAR);

                CCLPlugin.LogVerbose($"Adjust anchors for {newBufferRig.name} - {bufferChainRig?.name}");

                foreach (string anchorName in CarPartNames.Buffers.ANCHORS)
                {
                    var anchor = bufferChainRig.FindSafe(anchorName);
                    var newAnchor = newBufferRig.Find(anchorName);

                    if (anchor)
                    {
                        newAnchor.localPosition = anchor!.localPosition;
                    }
                }

                // Adjust air hose & MU connector positions
                if (carSetup.UseCustomHosePositions)
                {
                    CCLPlugin.LogVerbose($"Adjust hoses for {newBufferRig?.name}");

                    var hoseRoot = bufferChainRig.FindSafe(CarPartNames.Couplers.HOSES_ROOT);
                    var newHoseRoot = newBufferRig.FindSafe(CarPartNames.Couplers.HOSES_ROOT);

                    Transform? airHose = hoseRoot.FindSafe(CarPartNames.Couplers.AIR_HOSE);
                    CCLPlugin.LogVerbose($"Air hose = {!!airHose}");
                    if (airHose)
                    {
                        var newAir = newHoseRoot.FindSafe(CarPartNames.Couplers.AIR_HOSE);
                        if (newAir)
                        {
                            newAir!.localPosition = airHose!.localPosition;
                        }
                    }

                    Transform? muHose = hoseRoot.FindSafe(CarPartNames.Couplers.MU_CONNECTOR);
                    CCLPlugin.LogVerbose($"MU hose = {!!muHose}");
                    if (muHose)
                    {
                        var newMU = newHoseRoot.FindSafe(CarPartNames.Couplers.MU_CONNECTOR);
                        if (newMU)
                        {
                            newMU!.localPosition = muHose!.localPosition;
                        }
                    }
                }
            }

            Object.Destroy(frontCouplerRig.gameObject);
            Object.Destroy(rearCouplerRig.gameObject);

            return new PositionPair(frontRigPosition, rearRigPosition);
        }
    }
}
