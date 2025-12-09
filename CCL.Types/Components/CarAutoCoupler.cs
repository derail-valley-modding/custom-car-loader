using CCL.Types.Proxies.Ports;
using System.Collections.Generic;
using UnityEngine;

namespace CCL.Types.Components
{
    [AddComponentMenu("CCL/Components/Car Auto Coupler")]
    public class CarAutoCoupler : MonoBehaviour, IHasPortIdFields
    {
        [Tooltip("The coupler of this car to connect")]
        public CouplerDirection Direction;
        [Tooltip("The coupler of the other car to connect")]
        public CouplerDirection OtherDirection;

        public bool AlwaysCouple = false;
        [CarKindField]
        public string[] CarKinds = new string[0];
        public string[] CarTypes = new string[0];
        public string[] CarLiveries = new string[0];

        [Header("Optional")]
        [PortId(required = false), Tooltip("Optional port that can disable automatic coupling\n" +
            "Will also uncouple the vehicle when active")]
        public string DisablerPort = string.Empty;

        public IEnumerable<PortIdField> ExposedPortIdFields => new[]
        {
            new PortIdField(this, nameof(DisablerPort), DisablerPort, required: false)
        };
    }
}
