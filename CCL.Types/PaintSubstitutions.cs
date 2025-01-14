using System;
using UnityEngine;

namespace CCL.Types
{
    [CreateAssetMenu(menuName = "CCL/Paint Substitutions")]
    public class PaintSubstitutions : ScriptableObject, ICustomSerialized
    {
        [Serializable]
        public class Substitution
        {
            [Tooltip("The original material in the prefab")]
            public Material Original = null!;
            [Tooltip("The new material applied to replace it")]
            public Material Substitute = null!;
        }

        [PaintField]
        public string Paint = string.Empty;
        public Substitution[] Substitutions = new Substitution[0];

        [SerializeField, HideInInspector]
        private Material[] _originals = new Material[0];
        [SerializeField, HideInInspector]
        private Material[] _substitutes = new Material[0];

        public void OnValidate()
        {
            int length = Substitutions.Length;
            _originals = new Material[length];
            _substitutes = new Material[length];

            for (int i = 0; i < length; i++)
            {
                _originals[i] = Substitutions[i].Original;
                _substitutes[i] = Substitutions[i].Substitute;
            }
        }

        public void AfterImport()
        {
            int length = _originals.Length;
            Substitutions = new Substitution[length];

            for (int i = 0; i < length; i++)
            {
                Substitutions[i] = new Substitution
                {
                    Original = _originals[i],
                    Substitute = _substitutes[i]
                };
            }
        }
    }
}
