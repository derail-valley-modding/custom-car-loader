using System;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Procedural Materials", order = MenuOrdering.ProcMats)]
    public class ProceduralMaterialDefinitions : ScriptableObject, ICustomSerialized
    {
        public enum MaterialType
        {
            Exploded,
            [Tooltip("Please include a dummy texture in the Detail Albedo x2 field of the material")]
            PaintDetailsOld,
            [Tooltip("Please include a dummy texture in the Detail Albedo x2 field of the material")]
            PaintDetailsNew,
        }

        [Serializable]
        public class GeneratorSet
        {
            public MaterialType Type;
            public Material Original = null!;
        }

        public List<GeneratorSet> Entries = new List<GeneratorSet>();

        [SerializeField, HideInInspector]
        private MaterialType[]? _types;
        [SerializeField, HideInInspector]
        private Material[]? _mats;

        public void OnValidate()
        {
            int length = Entries.Count;
            _types = new MaterialType[length];
            _mats = new Material[length];

            for (int i = 0; i < length; i++)
            {
                _types[i] = Entries[i].Type;
                _mats[i] = Entries[i].Original;
            }
        }

        public void AfterImport()
        {
            Entries.Clear();

            if (_types == null || _mats == null) return;

            for (int i = 0; i < _types.Length; i++)
            {
                Entries.Add(new GeneratorSet { Type = _types[i], Original = _mats[i] });
            }
        }
    }
}
