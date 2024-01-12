﻿using CCL.Types.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CCL.Importer.Processing
{
    [Export(typeof(IModelProcessorStep))]
    internal class SoundProcessor : ModelProcessorStep
    {
        private static bool _isCacheEmpty = true;
        private static Dictionary<string, AudioClip> s_soundCache = new Dictionary<string, AudioClip>();
        private static Dictionary<string, AudioClip> SoundCache
        {
            get
            {
                if (_isCacheEmpty)
                {
                    BuildSoundCache();
                }

                return s_soundCache!;
            }
        }

        private static void BuildSoundCache()
        {
            s_soundCache = Resources.FindObjectsOfTypeAll<AudioClip>().ToDictionary(k => k.name, v => v);
            _isCacheEmpty = false;

            CCLPlugin.Log($"Sound cache created with {s_soundCache.Count} sounds.");
        }

        public static void ClearCache()
        {
            s_soundCache.Clear();
            _isCacheEmpty = true;
        }

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
                    if (SoundCache.TryGetValue(replacement.SoundName, out AudioClip clip))
                    {
                        fi = t.GetField(replacement.FieldName);

                        if (fi == null)
                        {
                            CCLPlugin.Error($"FieldInfo is null in '{prefab.name}' (field name: {replacement.FieldName}), skipping.");
                            continue;
                        }

                        if (replacement.IsArray)
                        {
                            IList<AudioClip> clips = (IList<AudioClip>)fi.GetValue(grabber.ScriptToAffect);
                            clips[replacement.Index] = clip;
                        }
                        else
                        {
                            fi.SetValue(grabber.ScriptToAffect, clip);
                        }
                    }
                    else
                    {
                        CCLPlugin.Error($"Could not find sound '{replacement.SoundName}'!");
                    }
                }

                UnityEngine.Object.Destroy(grabber);
            }
        }
    }
}