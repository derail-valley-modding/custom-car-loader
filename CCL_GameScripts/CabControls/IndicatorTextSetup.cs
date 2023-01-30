using CCL_GameScripts.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace CCL_GameScripts.CabControls
{
    public class IndicatorTextSetup : ComponentInitSpec
    {
        public override string TargetTypeName => "DVCustomCarLoader.Effects.IndicatorText";
        public override bool DestroyAfterCreation => true;

        [ProxyField]
        public TextMeshPro Display;

        [Tooltip("Number of seconds between screen updates")]
        [ProxyField]
        public float RefreshDelay;

        public string FormatString;

        [HideInInspector]
        [ProxyField("FormatString")]
        public string CompiledFormatString;

        [ProxyField("_eventTypes")]
        public SimEventType[] OutputBindings;

        [ProxyField("ControlTypes")]
        public CabInputType[] InputBindings;

        public void OnValidate()
        {
            var bindings = new List<OutputBinding>();
            if (OutputBindings != null)
            {
                bindings.AddRange(OutputBindings.Select(b => new OutputBinding { SimEventType = b }));
            }
            if (InputBindings != null)
            {
                bindings.AddRange(InputBindings.Select(b => new OutputBinding { CabInputType = b }));
            }

            string compiler = FormatString;

            for (int i = 0; i < bindings.Count; i++)
            {
                string pattern = $"{{{bindings[i].GetName()}([^}}]*)}}";
                string replacement = $"{{{i}$1}}";
                compiler = Regex.Replace(compiler, pattern, replacement);
            }

            CompiledFormatString = compiler;

            if (Display)
            {
                try
                {
                    Display.text = string.Format(CompiledFormatString, Enumerable.Repeat(Mathf.PI, bindings.Count).Cast<object>().ToArray());
                }
                catch
                {
                    Display.text = "Format Error";
                }
            }
        }
    }
}