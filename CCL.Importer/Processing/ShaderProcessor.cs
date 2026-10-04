using CCL.Types.Components;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class ShaderProcessor : ModelProcessorStep
    {
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

        public static Shader GetShader(ShaderGrabber.GrabbableShader shader)
        {
            if (s_shaderCache.TryGetValue(shader, out Shader s))
            {
                return s;
            }

            s = shader switch
            {
                ShaderGrabber.GrabbableShader.TransparencyWithFog =>
                        Shader.Find("TransparencyWithFog"),
                ShaderGrabber.GrabbableShader.DVModularBuildings =>
                        Shader.Find("Derail Valley/Modular Buildings"),
                _ => throw new System.ArgumentOutOfRangeException(nameof(shader)),
            };

            s_shaderCache.Add(shader, s);
            return s;
        }

        public override void ExecuteStep(ModelProcessor context)
        {
            foreach (var prefab in context.Car.AllPrefabs)
            {
                ReplaceShaderGrabbers(prefab);
            }
        }
    }
}
