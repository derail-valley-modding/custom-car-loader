﻿using CCL.Creator.Utility;
using CCL.Types.Proxies.VFX;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Inspector
{
    [CustomEditor(typeof(CylinderCockParticlePortReaderProxy))]
    internal class CylinderCockParticlePortReaderProxyEditor : Editor
    {
        private int _index;
        private CylinderCockParticlePortReaderProxy _proxy = null!;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _proxy = (CylinderCockParticlePortReaderProxy)target;

            EditorHelpers.DrawHeader("Quick curve setup");

            _index = Mathf.Clamp(EditorGUILayout.IntField("Cylinder setup index", _index), 0, _proxy.cylinderSetups.Length - 1);
            var setup = _proxy.cylinderSetups[_index];

            if (GUILayout.Button("Apply 0 degrees curves"))
            {
                setup.frontActivityCurve = CylinderCockParticlePortReaderProxy.Curve0;
                setup.rearActivityCurve = CylinderCockParticlePortReaderProxy.Curve180;
            }

            if (GUILayout.Button("Apply 90 degrees curves"))
            {
                setup.frontActivityCurve = CylinderCockParticlePortReaderProxy.Curve90;
                setup.rearActivityCurve = CylinderCockParticlePortReaderProxy.Curve270;
            }

            if (GUILayout.Button("Apply 180 degrees curves"))
            {
                setup.frontActivityCurve = CylinderCockParticlePortReaderProxy.Curve180;
                setup.rearActivityCurve = CylinderCockParticlePortReaderProxy.Curve0;
            }

            if (GUILayout.Button("Apply 270 degrees curves"))
            {
                setup.frontActivityCurve = CylinderCockParticlePortReaderProxy.Curve270;
                setup.rearActivityCurve = CylinderCockParticlePortReaderProxy.Curve90;
            }

            EditorHelpers.DrawLocoDefaultsButtons(target);
        }
    }
}
