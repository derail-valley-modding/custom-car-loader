using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using CCL_GameScripts.Attributes;

namespace CCL_GameScripts
{
    public abstract class ComponentInitSpec : MonoBehaviour, IProxyScript
    {
        public abstract string TargetTypeName { get; }
        public virtual bool IsOverrideSet( int index ) => false;
        public abstract bool DestroyAfterCreation { get; }
    }
}
