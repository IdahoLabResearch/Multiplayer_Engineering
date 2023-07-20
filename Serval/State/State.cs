using UnityEngine;

namespace Serval
{
    public class State
    {
        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }

        public State(Vector3 position, Vector3 rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }

    }

    public class ObjectState
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public bool highlighted { get; set; }
        public bool locked { get; set; }

        public ObjectState(Vector3 position, Quaternion rotation, bool highlighted, bool locked)
        {
            this.position = position;
            this.rotation = rotation;
            this.highlighted = highlighted;
            this.locked = locked;
        }

    }

}