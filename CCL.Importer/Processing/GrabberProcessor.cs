using CCL.Types.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class GrabberProcessor : ModelProcessorStep
    {
        /// <summary>
        /// A cache for a Unity resource loading in-game.
        /// </summary>
        /// <typeparam name="T">A Unity resource.</typeparam>
        private class CachedResource<T> where T : UnityEngine.Object
        {
            private Dictionary<string, T>? _cachedResources = null;

            public Dictionary<string, T> Cache
            {
                get
                {
                    if (_cachedResources == null)
                    {
                        BuildCache();
                    }

                    return _cachedResources!;
                }
            }

            private int _expectedResources;

            /// <param name="expectedResources">
            /// The number of resources expected in the cache. This is only
            /// used to check if the cache has been correctly built.
            /// </param>
            public CachedResource(int expectedResources)
            {
                _expectedResources = expectedResources;
            }

            public void BuildCache()
            {
                _cachedResources = Resources.FindObjectsOfTypeAll<T>().ToDictionary(k => k.name, v => v);

                CCLPlugin.Log($"{typeof(T).Name} cache created with {_cachedResources.Count} sounds.");

                // If we have more names that elements in the cache, it's possible one of the required
                // resources has not been loaded, so log an error.
                if (_cachedResources.Count < _expectedResources)
                {
                    CCLPlugin.Error($"Possible miscreation of {typeof(T).Name} cache ({_cachedResources.Count}/{_expectedResources})");
                }
            }

            public void ClearCache()
            {
                if (_cachedResources != null)
                {
                    _cachedResources.Clear();
                    _cachedResources = null;
                }
            }
        }

        private static CachedResource<AudioClip> s_soundCache = new CachedResource<AudioClip>(SoundGrabber.SoundNames.Length);

        public override void ExecuteStep(ModelProcessor context)
        {
            ProcessSoundsOnPrefab(context.Car.prefab);

            if (context.Car.externalInteractablesPrefab)
            {
                ProcessSoundsOnPrefab(context.Car.externalInteractablesPrefab);
            }

            if (context.Car.explodedExternalInteractablesPrefab)
            {
                ProcessSoundsOnPrefab(context.Car.explodedExternalInteractablesPrefab);
            }

            if (context.Car.interiorPrefab)
            {
                ProcessSoundsOnPrefab(context.Car.interiorPrefab);
            }

            if (context.Car.explodedInteriorPrefab)
            {
                ProcessSoundsOnPrefab(context.Car.explodedInteriorPrefab);
            }
        }

        private void ProcessSoundsOnPrefab(GameObject prefab)
        {
            ProcessGrabberOnPrefab<SoundGrabber, AudioClip>(prefab, s_soundCache);
        }

        /// <summary>
        /// Processes a kind of <see cref="VanillaResourceGrabber{T}"/>.
        /// </summary>
        /// <typeparam name="TGrabber">The implemented <see cref="VanillaResourceGrabber{T}"/> for the type of <typeparamref name="TGrabType"/>.</typeparam>
        /// <typeparam name="TGrabType">The type that will be grabbed.</typeparam>
        /// <param name="prefab">The prefab with the components.</param>
        /// <param name="cache">The cache for the type that will be grabbed.</param>
        private void ProcessGrabberOnPrefab<TGrabber, TGrabType>(GameObject prefab, CachedResource<TGrabType> cache)
            where TGrabType : UnityEngine.Object
            where TGrabber : VanillaResourceGrabber<TGrabType>
        {
            Type t;
            FieldInfo fi;

            foreach (TGrabber grabber in prefab.GetComponentsInChildren<TGrabber>())
            {
                t = grabber.ScriptToAffect.GetType();

                foreach (var replacement in grabber.Replacements)
                {
                    string name = grabber.GetNames()[replacement.NameIndex];

                    // Try to find the cached value.
                    if (cache.Cache.TryGetValue(name, out TGrabType grabbed))
                    {
                        // Get the field we are supposed to replace.
                        fi = t.GetRuntimeField(replacement.FieldName);

                        // Field not found, so we can't replace it, log and skip.
                        if (fi == null)
                        {
                            CCLPlugin.Error($"FieldInfo is null in '{prefab.name}' (field name: {replacement.FieldName}), skipping.");
                            continue;
                        }

                        // If we are supposed to assign to an array...
                        if (replacement.IsArray)
                        {
                            // Grab the array reference, and replace the value in it.
                            IList<TGrabType> array = (IList<TGrabType>)fi.GetValue(grabber.ScriptToAffect);
                            array[replacement.ArrayIndex] = grabbed;
                        }
                        else
                        {
                            // Assign the value directly.
                            fi.SetValue(grabber.ScriptToAffect, grabbed);
                        }
                    }
                    else
                    {
                        CCLPlugin.Error($"Could not find cached key '{name}' (index {replacement.NameIndex})!");
                    }
                }

                // Remove the grabber from the prefab since it's no longer needed.
                UnityEngine.Object.Destroy(grabber);
            }
        }
    }
}
