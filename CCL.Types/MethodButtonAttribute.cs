using System;
using UnityEngine;

namespace CCL.Types
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MethodButtonAttribute : Attribute
    {
        public readonly string MethodName;
        public readonly string TextOverride;
        public readonly string? Tooltip;

        /// <summary>Render a button that calls the given method</summary>
        /// <param name="methodName">Fully qualified path of the target method, in "Namespace.Class:Method" format.
        /// <para>Must be a static method that takes the enclosing class as an argument, or a 0 parameter instance method on the enclosing class.</para>
        /// </param>
        public MethodButtonAttribute(string methodName, string buttonText)
        {
            MethodName = methodName;
            TextOverride = buttonText;
        }

        /// <summary>Render a button that calls the given method, with a custom tooltip</summary>
        /// <param name="methodName">Fully qualified path of the target method, in "Namespace.Class:Method" format.
        /// <para>Must be a static method that takes the enclosing class as an argument, or a 0 parameter instance method on the enclosing class.</para>
        /// </param>
        public MethodButtonAttribute(string methodName, string buttonText, string tooltip) : this(methodName, buttonText)
        {
            Tooltip = tooltip;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RenderMethodButtonsAttribute : PropertyAttribute
    {

    }
}