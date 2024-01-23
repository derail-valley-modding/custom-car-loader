using CCL.Types.Proxies.Ports;
using System.Linq;

namespace CCL.Creator.Editor
{
    public enum PortOptionType
    {
        None = 0,
        Port,
        PortReference,
        PortIdField,
    }

    public abstract class PortOptionBase
    {
        public readonly PortOptionType Type;
        public readonly string PrefabName;
        public readonly string? ID;
        public readonly DVPortValueType PortValueType;

        public PortOptionBase(PortOptionType type, string prefabName, string compId, string portId, DVPortValueType valueType)
        {
            Type = type;
            PrefabName = prefabName;
            ID = $"{compId}.{portId}";
            PortValueType = valueType;
        }

        public PortOptionBase(PortOptionType type, string? fullId, DVPortValueType valueType, string prefabName)
        {
            Type = type;
            PrefabName = prefabName;
            ID = fullId;
            PortValueType = valueType;
        }

        public string Description => $"{Code} {ID} ({PrefabName})";

        public string Code => Type switch
        {
            PortOptionType.Port => "P",
            PortOptionType.PortReference => "R",
            PortOptionType.PortIdField => "F",
            _ => "?"
        };
    }

    public class PortReferenceOption : PortOptionBase
    {
        public PortReferenceOption(string prefabName, string compId, string portId, DVPortValueType valueType)
            : base(PortOptionType.PortReference, prefabName, compId, portId, valueType)
        { }
        
        public PortReferenceOption(string? fullId, string prefabName = "Unknown")
            : base(PortOptionType.PortReference, fullId, DVPortValueType.GENERIC, prefabName)
        { }
    }

    public class PortOption : PortOptionBase
    {
        public readonly DVPortType PortType;

        public PortOption(string prefabName, string compId, string portId, DVPortType portType, DVPortValueType valueType)
            : base(PortOptionType.Port, prefabName, compId, portId, valueType)
        {
            PortType = portType;
        }

        public PortOption(string? fullId, string prefabName = "Unknown")
            : base(PortOptionType.Port, fullId, DVPortValueType.GENERIC, prefabName)
        {
            PortType = DVPortType.IN;
        }

        public bool MatchesType(DVPortType[]? filters)
        {
            return (filters == null) || (filters.Length == 0) || filters.Contains(PortType);
        }

        public bool MatchesValueType(DVPortValueType[]? filters)
        {
            return (filters == null) || (filters.Length == 0) || filters.Contains(PortValueType);
        }
    }

    public class PortIdFieldOption : PortOptionBase
    {
        public readonly PortIdField Field;

        public PortIdFieldOption(string prefabName, PortIdField field, DVPortValueType valueType)
            : base(PortOptionType.PortIdField, field.FullName, valueType, prefabName)
        {
            Field = field;
        }
    }
}
