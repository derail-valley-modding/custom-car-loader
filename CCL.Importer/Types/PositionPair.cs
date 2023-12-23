using UnityEngine;

namespace CCL.Importer.Types
{
    internal struct PositionPair
    {
        public Vector3 Front;
        public Vector3 Rear;

        public PositionPair(Vector3 front, Vector3 rear)
        {
            Front = front;
            Rear = rear;
        }
    }
}
