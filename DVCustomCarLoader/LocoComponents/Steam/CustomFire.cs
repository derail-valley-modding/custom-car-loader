using DV.CabControls;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DVCustomCarLoader.LocoComponents.Steam
{
    public class CustomFire : Fire
    {
        protected CustomLocoControllerSteam loco;
        protected ParticleSystem fireParticleSystem;
        protected ParticleSystem sparksParticleSystem;
        protected Collider[] overlapColliders;

        protected MeshRenderer fireboxCoalMeshRenderer;
        protected MeshRenderer fireboxMeshRenderer;
        protected MeshRenderer fireboxDoorFrameMeshRenderer;

        private static readonly FieldInfo isFireOnField = AccessTools.Field(typeof(Fire), "isFireOn");
        protected bool FireWasOn
        {
            get => (bool)isFireOnField.GetValue(this);
            set => isFireOnField.SetValue(this, value);
        }

        protected CustomChuffController chuffController;
        protected float targetChuffDraftLevel = 0f;
        protected float chuffDraftLevel = 0f;
        protected float chuffDraftSlope = 0f;

        protected Coroutine chuffFalloffRoutine = null;
        protected Vector3 originalFireScale;

        public void InitializeFromOther(Fire baseFire)
        {
            fireObj = baseFire.fireObj;
            fireboxObj = baseFire.fireboxObj;
            fireboxCoalObj = baseFire.fireboxCoalObj;
            fireboxDoorFrameObj = baseFire.fireboxDoorFrameObj;
            sparksObj = baseFire.sparksObj;
            helperTriggerVR = baseFire.helperTriggerVR;
        }

        internal void StartOverride(
            ref ParticleSystem fireParticles,
            ref ParticleSystem sparksParticles,
            Collider[] colliders,
            ref MeshRenderer coalRenderer,
            ref MeshRenderer fireboxRenderer,
            ref MeshRenderer doorFrameRenderer)
        {
            loco = TrainCar.Resolve(gameObject).GetComponent<CustomLocoControllerSteam>();

            if (loco.GetComponentInChildren<CustomChuffController>() is CustomChuffController chuff)
            {
                chuffController = chuff;
                chuffController.OnChuff += OnChuff;
            }

            fireboxCoalMeshRenderer = coalRenderer = fireboxCoalObj.GetComponent<MeshRenderer>();
            fireboxMeshRenderer = fireboxRenderer = fireboxObj.GetComponent<MeshRenderer>();
            fireboxDoorFrameMeshRenderer = doorFrameRenderer = fireboxDoorFrameObj.GetComponent<MeshRenderer>();

            fireParticleSystem = fireParticles = fireObj.GetComponent<ParticleSystem>();
            fireParticleSystem.Stop();
            originalFireScale = fireObj.transform.localScale;

            sparksParticleSystem = sparksParticles = sparksObj.GetComponent<ParticleSystem>();
            sparksParticleSystem.Stop();

            isFireOnField.SetValue(this, true);

            if (!VRManager.IsVREnabled())
            {
                helperTriggerVR.SetActive(false);
            }

            overlapColliders = colliders;
        }

        protected void OnDestroy()
        {
            chuffController.OnChuff -= OnChuff;
        }

        internal IEnumerator CheckFireOnOverride(float timeout)
        {
            var wait = WaitFor.Seconds(timeout);
            while (true)
            {
                yield return wait;

                bool fireIsOn = (loco.GetFireOn() == 1);

                if (fireIsOn ^ FireWasOn)
                {
                    FireWasOn = fireIsOn;
                    if (fireIsOn)
                    {
                        fireParticleSystem.Play();
                        sparksParticleSystem.Play();
                    }
                    else
                    {
                        fireParticleSystem.Stop();
                        sparksParticleSystem.Stop();
                    }
                }

                float tempPercent = loco.FireTemp / loco.MaxFireTemp;
                if (fireIsOn)
                {
                    IgniteTerrainTile(IGNITION_STRENGTH);
                    IgniteLeakOrCargo(IGNITION_STRENGTH);

                    SetFlameScale(tempPercent, loco.BlowerFlow);
                }

                SetFireIntensity(tempPercent);
            }
        }

        protected void OnChuff(float power)
        {
            targetChuffDraftLevel = power;
            if (chuffFalloffRoutine == null)
            {
                chuffFalloffRoutine = StartCoroutine(ChuffFalloff());
            }
        }

        protected IEnumerator ChuffFalloff()
        {
            do
            {
                yield return null;
                
                targetChuffDraftLevel -= CHUFF_FALLOFF_PER_FRAME;
                if (targetChuffDraftLevel < 0) targetChuffDraftLevel = 0;

                chuffDraftLevel = Mathf.SmoothDamp(chuffDraftLevel, targetChuffDraftLevel, ref chuffDraftSlope, Time.deltaTime * CHUFF_FALLOFF_INERTIA);

                if (chuffDraftLevel < 0.05f) chuffDraftLevel = 0;
            }
            while (chuffDraftLevel > 0);

            chuffFalloffRoutine = null;
        }

        protected void ApplyChuffScale()
        {
            float scale = Mathf.Lerp(1f, CHUFF_SCALE_MULTIPLIER, chuffDraftLevel);
            fireObj.transform.localScale = Vector3.Scale(originalFireScale, new Vector3(1, scale, 1));

            var emission = fireParticleSystem.emission;
            scale = Mathf.Lerp(1f, CHUFF_RATE_MULTIPLIER, chuffDraftLevel);
            emission.rateOverTimeMultiplier = scale;
        }

        protected void SetFireIntensity(float tempPercent)
        {
            if (fireboxCoalMeshRenderer) fireboxCoalMeshRenderer.material.SetColor("_EmissionColor", Color.white * tempPercent * MAX_EMISSION_INTENSITY);
            if (fireboxMeshRenderer) fireboxMeshRenderer.material.SetColor("_EmissionColor", Color.white * tempPercent * MAX_EMISSION_INTENSITY);
            if (fireboxDoorFrameMeshRenderer) fireboxDoorFrameMeshRenderer.material.SetColor("_EmissionColor", Color.white * tempPercent * MAX_EMISSION_INTENSITY);
        }

        protected void SetFlameScale(float tempPercent, float blower)
        {
            float scale = (tempPercent * 0.7f) + (blower * 0.3f);

            var main = fireParticleSystem.main;
            main.startSizeX = Mathf.Lerp(START_3DSIZE_X_MIN, START_3DSIZE_X_MAX, scale);
            main.startSizeY = Mathf.Lerp(START_3DSIZE_Y_MIN, START_3DSIZE_Y_MAX, scale);
            main.startSizeZ = Mathf.Lerp(START_3DSIZE_Z_MIN, START_3DSIZE_Z_MAX, scale);

            float rateScale = (tempPercent * 0.5f) + (blower * 0.5f);

            var emission = fireParticleSystem.emission;
            emission.rateOverTime = Mathf.Lerp(EMISSION_RATE_MIN, EMISSION_RATE_MAX, rateScale);

            var shape = fireParticleSystem.shape;
            float x = shape.position.x;
            float y = shape.position.y;
            float z = Mathf.Lerp(SHAPE_POSITION_Z_MIN, SHAPE_POSITION_Z_MAX, rateScale);
            shape.position = new Vector3(x, y, z);
        }

        protected void Update()
        {
            ApplyChuffScale();
        }

        internal void HandleTriggerEnter(Collider other)
        {
            Coal coalChunk = other.gameObject.GetComponent<Coal>();
            if (coalChunk)
            {
                if (coalChunk.GetComponent<ItemBase>().IsGrabbed())
                {
                    return;
                }

                loco.AddCoalChunk();
                DV_GameObjectDestructionHandler.RemoveGameObject(coalChunk.gameObject);
                return;
            }
            else
            {
                var lighter = other.gameObject.GetComponent<Lighter>();
                if (lighter)
                {
                    if (lighter.IsFireOn() && loco.FireboxLevel > 0f)
                    {
                        loco.SetFireOn(1);
                    }

                    lighter.GetComponent<LighterFetcher>().TryReturnToInventory();
                    return;
                }
                return;
            }
        }

        new public void IgniteLeakOrCargo(float ignitionStrength)
        {
            float fireDoorOpen = loco.GetFireDoor();
            float igniteRadius = Mathf.Max(1f, fireDoorOpen * FIRE_IGNITION_SPHERE_SIZE);

            int nHazmatInRange = Physics.OverlapSphereNonAlloc(
                fireboxObj.transform.position,
                igniteRadius,
                overlapColliders,
                LayerMask.GetMask("Hazmat"),
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < nHazmatInRange; i++)
            {
                var cargoReaction = overlapColliders[i].GetComponentInParent<ICargoReaction>();
                if (cargoReaction != null && cargoReaction.TryIgniteExternally(ignitionStrength))
                {
                    cargoReaction.PlayIgnitionSound(transform.position);
                }
            }
        }

        private const float IGNITION_STRENGTH = 10;

        private const float START_3DSIZE_X_MIN = 0.5f; //0.8f;
        private const float START_3DSIZE_Y_MIN = 0.5f; //1.3f;
        private const float START_3DSIZE_Z_MIN = 0.5f; //0.8f;

        private const float START_3DSIZE_X_MAX = 1;
        private const float START_3DSIZE_Y_MAX = 2;
        private const float START_3DSIZE_Z_MAX = 1;

        private const float EMISSION_RATE_MIN = 2;//1;
        private const float EMISSION_RATE_MAX = 6;//3;

        private const float SHAPE_POSITION_Z_MIN = -0.4f;
        private const float SHAPE_POSITION_Z_MAX = 0;

        private const float FIRE_IGNITION_SPHERE_SIZE = 7;

        private const float MAX_EMISSION_INTENSITY = 2;

        private const float CHUFF_FALLOFF_PER_FRAME = 0.02f;
        private const float CHUFF_FALLOFF_INERTIA = 0.3f;
        private const float CHUFF_SCALE_MULTIPLIER = 2f;
        private const float CHUFF_RATE_MULTIPLIER = 10f;
    }

    [HarmonyPatch(typeof(Fire))]
    public static class CustomFirePatches
    {
        [HarmonyPatch("Start")]
        [HarmonyPrefix]
        public static void StartPrefix(
            Fire __instance,
            ref ParticleSystem ___fireParticleSystem,
            ref ParticleSystem ___sparksParticleSystem,
            Collider[] ___overlapColliders,
            ref MeshRenderer ___fireboxCoalMeshRenderer,
            ref MeshRenderer ___fireboxMeshRenderer,
            ref MeshRenderer ___fireboxDoorFrameMeshRenderer)
        {
            if (__instance is CustomFire cf)
            {
                cf.StartOverride(
                    ref ___fireParticleSystem,
                    ref ___sparksParticleSystem,
                    ___overlapColliders,
                    ref ___fireboxCoalMeshRenderer,
                    ref ___fireboxMeshRenderer,
                    ref ___fireboxDoorFrameMeshRenderer);
            }
        }

        [HarmonyPatch("CheckFireOn")]
        [HarmonyPrefix]
        public static bool CheckFireOn(Fire __instance, ref IEnumerator __result, float timeout)
        {
            if (__instance is CustomFire cf)
            {
                __result = cf.CheckFireOnOverride(timeout);
                return false;
            }
            return true;
        }

        [HarmonyPatch("OnTriggerEnter")]
        [HarmonyPrefix]
        public static bool OnTriggerEnter(Fire __instance, Collider other)
        {
            if (__instance is CustomFire cf)
            {
                cf.HandleTriggerEnter(other);
                return false;
            }
            return true;
        }
    }
}
