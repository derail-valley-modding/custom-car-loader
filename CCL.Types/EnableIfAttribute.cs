using UnityEngine;

namespace CCL.Types
{
    public class EnableIfAttribute : PropertyAttribute
    {
        public string Target { get; }
        public bool Invert { get; set; }

        /// <summary>
        /// Enables or disables the property in the inspector based on a condition.
        /// </summary>
        /// <param name="target">The name of the field, property, or method that decides when to enable/disable. Must be/return a bool.</param>
        /// <param name="invert">If true, the result of <paramref name="target"/> will have the opposite effect.</param>
        public EnableIfAttribute(string target, bool invert = false)
        {
            Target = target;
            Invert = invert;
        }
    }
}
