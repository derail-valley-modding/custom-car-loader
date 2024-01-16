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

            public CachedResource(int expectedResources)
            {
                _expectedResources = expectedResources;
            }

            public void BuildCache()
            {
                _cachedResources = Resources.FindObjectsOfTypeAll<T>().ToDictionary(k => k.name, v => v);

                CCLPlugin.Log($"{typeof(T).Name} cache created with {_cachedResources.Count} sounds.");

                if (_cachedResources.Count < SoundGrabber.SoundNames.Length)
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
            Type t;
            FieldInfo fi;

            foreach (SoundGrabber grabber in prefab.GetComponentsInChildren<SoundGrabber>())
            {
                t = grabber.ScriptToAffect.GetType();

                foreach (var replacement in grabber.Replacements)
                {
                    string name = SoundGrabber.SoundNames[replacement.NameIndex];

                    if (s_soundCache.Cache.TryGetValue(name, out AudioClip clip))
                    {
                        fi = t.GetRuntimeField(replacement.FieldName);

                        if (fi == null)
                        {
                            CCLPlugin.Error($"FieldInfo is null in '{prefab.name}' (field name: {replacement.FieldName}), skipping.");
                            continue;
                        }

                        if (replacement.IsArray)
                        {
                            IList<AudioClip> clips = (IList<AudioClip>)fi.GetValue(grabber.ScriptToAffect);
                            clips[replacement.ArrayIndex] = clip;
                        }
                        else
                        {
                            fi.SetValue(grabber.ScriptToAffect, clip);
                        }
                    }
                    else
                    {
                        CCLPlugin.Error($"Could not find sound '{name}' (index {replacement.NameIndex})!");
                    }
                }

                UnityEngine.Object.Destroy(grabber);
            }
        }
    }
}
