using System;
using UnityEngine;

namespace CCL.Types
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class MethodButtonAttribute : Attribute
    {
        public readonly string MethodName;
        public readonly string TextOverride;

        public MethodButtonAttribute(string methodName, string buttonText)
        {
            MethodName = methodName;
            TextOverride = buttonText;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RenderMethodButtonsAttribute : PropertyAttribute
    {

    }
}