using CCL.Types.Proxies.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CCL.Creator.Editor
{
    public class PortConnectionSelector : PopupWindowContent
    {
        private readonly SimConnectionsDefinitionProxy _simConn;
        private readonly string _localId;
        private readonly bool _localIsReference;
        private readonly PortDirection _direction;
        private readonly float _width;

        private readonly ConnectionDescriptor? _currentConnection;
        private readonly List<PortOptionBase> _options;

        public PortConnectionSelector(SimConnectionsDefinitionProxy host, float width, PortDefinition port, string localId, PortDirection direction, ConnectionDescriptor? currentConn = null)
        {
            _simConn = host;
            _width = width;
            _currentConnection = currentConn;
            _localId = localId;
            _direction = direction;
            _localIsReference = false;

            var sources = PortOptionHelper.GetAvailableSources(host, false);
            IEnumerable<PortOptionBase> options;
            
            if (direction == PortDirection.Input)
            {
                options = PortOptionHelper.GetInputPortConnectionOptions(port.valueType, sources);
            }
            else
            {
                options = PortOptionHelper.GetOutputPortConnectionOptions(port.valueType, sources);
            }

            _options = options.ToList();
        }

        public PortConnectionSelector(SimConnectionsDefinitionProxy host, float width, PortReferenceDefinition reference, string localId, ConnectionDescriptor? currentConn = null)
        {
            _simConn = host;
            _width = width;
            _currentConnection = currentConn;
            _localId = localId;
            _direction = PortDirection.Output;
            _localIsReference = true;

            var sources = PortOptionHelper.GetAvailableSources(host, false);
            var options = PortOptionHelper.GetPortReferenceInputOptions(reference.valueType, sources);
            _options = options.ToList();
        }

        private const int LINE_HEIGHT = 20;

        public override Vector2 GetWindowSize()
        {
            int nOptions = _options != null ? _options.Count : 0;
            return new Vector2(_width, LINE_HEIGHT * (nOptions + 1));
        }

        public override void OnGUI(Rect rect)
        {
            if (_options == null || _options.Count == 0)
            {
                GUI.Label(new Rect(0, 0, _width, LINE_HEIGHT), "No Options Found");
                return;
            }

            if (GUI.Button(new Rect(0, 0, _width, LINE_HEIGHT), "Unlink"))
            {
                _currentConnection?.DestroyConnection();
                editorWindow.Close();
                return;
            }

            float yOffset = 0;

            foreach (var option in _options)
            {
                yOffset += LINE_HEIGHT;
                var optRect = new Rect(0, yOffset, _width, LINE_HEIGHT);

                if (GUI.Button(optRect, option.Description))
                {
                    if (_currentConnection != null)
                    {
                        _currentConnection.ChangeLink(option);
                    }
                    else
                    {
                        bool needsRefConnection = _localIsReference || (option.Type == PortOptionType.PortReference);
                        ConnectionDescriptor.CreateNewLink(_simConn, _localId, option.ID!, _direction, needsRefConnection);
                    }

                    editorWindow.Close();
                    return;
                }
            }
        }
    }
}
