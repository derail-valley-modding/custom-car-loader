﻿using CCL.Types.Components;
using DV;
using DV.ThingTypes;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ShaderProcessor : ModelProcessorStep
    {
        private static Shader? _engineShader = null;

        private static Shader EngineShader => Extensions.GetCached(ref _engineShader, () =>
        {
            var prefab = Globals.G.Types.TrainCarType_to_v2[TrainCarType.LocoShunter].prefab;
            var exterior = prefab.transform.Find("LocoDE2_Body/ext 621_exterior");
            var material = exterior.GetComponent<MeshRenderer>().material;
            return material.shader;
        });

        private static void ApplyDefaultShader(GameObject prefab)
        {
            foreach (var renderer in prefab.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.materials)
                {
                    // replace opaque material shader
                    if ((material.shader.name == "Standard") && (material.GetFloat("_Mode") == 0))
                    {
                        material.shader = EngineShader;
                    }
                }
            }
        }

        public static void ReplaceShaderGrabbers(GameObject newFab)
        {
            var replacers = newFab.GetComponentsInChildren<ShaderGrabber>();

            // Process each replacer.
            foreach (var replacer in replacers)
            {
                // Affect all renderers the same way.
                foreach (var renderer in replacer.RenderersToAffect)
                {
                    // Affect only matching lengths.
                    for (int i = 0; i < replacer.ShadersToReplace.Length; i++)
                    {
                        renderer.sharedMaterials[replacer.ShadersToReplace[i].Index].shader = GetShader(replacer.ShadersToReplace[i].Shader);
                    }
                }

                // No need to keep the replacer anymore.
                Object.Destroy(replacer);
            }
        }

        private static readonly Dictionary<ShaderGrabber.GrabbableShader, Shader> s_shaderCache = new();

        private static Shader GetShader(ShaderGrabber.GrabbableShader shader)
        {
            Shader s;

            if (s_shaderCache.TryGetValue(shader, out s))
            {
                return s;
            }

            switch (shader)
            {
                case ShaderGrabber.GrabbableShader.TransparencyWithFog:
                    s = Shader.Find("TransparencyWithFog");
                    break;
                case ShaderGrabber.GrabbableShader.DVModularBuildings:
                    s = Shader.Find("Derail Valley/Modular Buildings");
                    break;
                default:
                    CCLPlugin.Error($"Invalid shader requested '{shader}'.");
                    return null!;
            }

            s_shaderCache.Add(shader, s);
            return s;
        }

        private static void UpdateShaders(GameObject prefab)
        {
            ApplyDefaultShader(prefab);
            ReplaceShaderGrabbers(prefab);
        }

        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var prefab in context.Car.AllPrefabs)
            {
                UpdateShaders(prefab);
            }
        }
    }
}
