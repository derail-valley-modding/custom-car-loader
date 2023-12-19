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

        public MethodButtonAttribute(string methodName, string buttonText)
        {
            MethodName = methodName;
            TextOverride = buttonText;
        }

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