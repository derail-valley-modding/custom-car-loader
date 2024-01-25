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
        private readonly PortIdField? _localIdField;
        private readonly bool _localIsReference;
        private readonly PortDirection _direction;
        private readonly float _width;

        private readonly ConnectionResult _currentConnection;
        private readonly ConnectionResultType _multiplicity;
        private readonly List<PortOptionBase> _options;

        public PortConnectionSelector(SimConnectionsDefinitionProxy host, float width, PortDefinition port, string localId, 
            PortDirection direction, ConnectionResult currentConn)
        {
            _simConn = host;
            _width = width * 1.2f;
            _currentConnection = currentConn;
            _localId = localId;
            _direction = direction;
            _localIsReference = false;

            var sources = PortOptionHelper.GetAvailableSources(host, false);
            IEnumerable<PortOptionBase> options;
            
            if (direction == PortDirection.Input)
            {
                options = PortOptionHelper.GetInputPortConnectionOptions(port.valueType, sources);
                _multiplicity = ConnectionResultType.Single;
            }
            else
            {
                options = PortOptionHelper.GetOutputPortConnectionOptions(port.type, port.valueType, sources);
                _multiplicity = ConnectionResultType.Multiple;
            }

            _options = options.ToList();
        }

        public PortConnectionSelector(SimConnectionsDefinitionProxy host, float width, PortReferenceDefinition reference, string localId,
            ConnectionResult currentConn)
        {
            _simConn = host;
            _width = width * 1.2f;
            _currentConnection = currentConn;
            _multiplicity = ConnectionResultType.Single;
            _localId = localId;
            _direction = PortDirection.Input;
            _localIsReference = true;

            var sources = PortOptionHelper.GetAvailableSources(host, false);
            var options = PortOptionHelper.GetPortReferenceInputOptions(reference.valueType, sources);
            _options = options.ToList();
        }

        public PortConnectionSelector(SimConnectionsDefinitionProxy host, float width, PortIdField idField, ConnectionResult currentConn)
        {
            _simConn = host;
            _width = width * 1.2f;
            _currentConnection = currentConn;
            _multiplicity = idField.IsMultiValue ? ConnectionResultType.Multiple : ConnectionResultType.Single;
            _localId = null!;
            _localIdField = idField;
            _direction = PortDirection.Input;
            _localIsReference = false;

            var sources = PortOptionHelper.GetAvailableSources(host, false);
            var options = PortOptionHelper.GetPortOptions(sources, idField.TypeFilters, idField.ValueFilters);
            _options = options.ToList();
        }

        private const int LINE_HEIGHT = 20;
        private static GUIStyle? _buttonStyle = null;
        private static GUIStyle ButtonStyle
        {
            get
            {
                _buttonStyle ??= new GUIStyle(GUI.skin.button)
                {
                    alignment = TextAnchor.MiddleLeft,
                };
                return _buttonStyle;
            }
        }

        private static readonly Color _unlinkColor = new Color32(255, 153, 153, 255);

        private Vector2 _scrollPosition;
        private const int MAX_OPTIONS = 9;
        private const int SCROLL_PADDING = 15;

        public override Vector2 GetWindowSize()
        {
            if (_options == null || _options.Count == 0) return new Vector2(_width, LINE_HEIGHT);

            if (_options.Count > MAX_OPTIONS)
            {
                return new Vector2(_width + SCROLL_PADDING, LINE_HEIGHT * (MAX_OPTIONS + 1));
            }
            return new Vector2(_width, LINE_HEIGHT * (_options.Count + 1));
        }

        public override void OnGUI(Rect rect)
        {
            if (_options.Count > MAX_OPTIONS)
            {
                var windowRect = new Rect(0, 0, _width + SCROLL_PADDING, LINE_HEIGHT * (MAX_OPTIONS + 1));
                var viewRect = new Rect(0, 0, _width, LINE_HEIGHT * (_options.Count + 1));
                _scrollPosition = GUI.BeginScrollView(windowRect, _scrollPosition, viewRect);
            }

            using (new GUIColorScope(_unlinkColor))
            {
                string unlinkText = (_multiplicity == ConnectionResultType.Multiple) ? "Unlink All" : "Unlink";
                if (GUI.Button(new Rect(0, 0, _width, LINE_HEIGHT), unlinkText, ButtonStyle))
                {
                    _currentConnection.DestroyAll();
                    editorWindow.Close();
                    return;
                }
            }

            float yOffset = 0;

            foreach (var option in _options)
            {
                yOffset += LINE_HEIGHT;
                var optRect = new Rect(0, yOffset, _width, LINE_HEIGHT);

                var connector = _currentConnection.Matches.FirstOrDefault(cd => cd.Id == option.ID);

                bool wasConnected = connector != null;
                bool nowConnected = GUI.Toggle(optRect, wasConnected, option.Description);

                if (nowConnected && !wasConnected)
                {
                    // new connection
                    if ((_multiplicity == ConnectionResultType.Single) && (option.Type != PortOptionType.PortIdField))
                    {
                        // destroy existing connections
                        foreach (var link in _currentConnection.Matches.Where(m => m.ConnectionType != PortOptionType.PortIdField).ToList())
                        {
                            _currentConnection.Destroy(link);
                        }
                    }

                    ConnectionDescriptor newLink;
                    if (_localIdField != null)
                    {
                        newLink = ConnectionDescriptor.CreatePortIdLink(_simConn, _localIdField, option.ID!);
                    }
                    else
                    {
                        newLink = ConnectionDescriptor.CreateNewLink(_simConn, _localId, _localIsReference, option, _direction);
                    }
                    _currentConnection.AddMatch(newLink);
                }
                else if (!nowConnected && wasConnected)
                {
                    // delete connection
                    _currentConnection.Destroy(connector!);
                }
            }

            if (_options.Count > MAX_OPTIONS)
            {
                GUI.EndScrollView();
            }
        }
    }
}
