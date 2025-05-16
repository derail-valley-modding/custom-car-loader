using CCL.Types.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace CCL.Types.Proxies
{
    public class CabinRenderOrderingProxy : MonoBehaviour, ICustomSerialized
    {
        [Serializable]
        public class OrderingRenderer
        {
            public SortingGroup group = null!;
            public short initialOrder;
            public short whenInside;
        }

        public CameraTriggerProxy triggerNullable = null!;
        public List<OrderingRenderer> ordering = new List<OrderingRenderer>();

        private string? _json;

        public void OnValidate()
        {
            _json = JSONObject.ToJson(ordering.Where(x => x.group != null).ToList());
        }

        public void AfterImport()
        {
            ordering = JSONObject.FromJson(_json, () => new List<OrderingRenderer>());
        }
    }
}
