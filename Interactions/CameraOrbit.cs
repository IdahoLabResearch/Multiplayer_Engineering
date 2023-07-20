using UnityEngine;

namespace Interactions
{
    public class CameraOrbit : MonoBehaviour
    {
        /// README
        /// <summary>A script used to orbit the camera around a moveable target origin in your scene.</summary>
        /// <param name="origin">This is the starting position of the origin, the object around which your camera should rotate.</param>
        /// <param name="distance">This is the camera's starting distance from the origin.</param>
        /// <param name="min">This is the camera's minimum distance from the origin.</param>
        /// <param name="max">This is the camera's maximum distance from the origin.</param>
        /// <remarks>An empty gameobject named "Origin" must be the PARENT of the Main Camera object to use this script. Attach this script to the Main Camera.</remarks>

        public Transform origin;
        private Transform _camera;
        private Vector3 rotation;
        private Vector3 position;

        // Camera distance
        [SerializeField]
        public float distance = 22f;
        [SerializeField]
        public float min = 1f;
        [SerializeField]
        public float max = 22f;

        // Start is called before the first frame update
        void Start()
        {
            this._camera = this.transform;
            this.origin = this.transform.parent;
            this.rotation = new Vector3(0f, 30f, 0f);

            this.AlignCamera();
        }

        public void Reset()
        {
            this.distance = 22f;
            this.origin.position = new Vector3(0f, 4f, 0f);
            this.rotation = new Vector3(0f, 30f, 0f);
        }

        void Update()
        {
            // The camera ALWAYS looks at Origin
            this._camera.LookAt(origin.transform);
        }

        // LateUpdate is called once per frame, after Update()
        void LateUpdate()
        {
            // Rotation
            if (Input.GetMouseButton(0))
            {
                // Magnitude controls how quickly the camera orbits
                float magnitude = 5f;

                // Determine whether the mouse is moving in a given frame
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    this.rotation.x += Input.GetAxis("Mouse X") * magnitude;
                    this.rotation.y -= Input.GetAxis("Mouse Y") * magnitude;

                    // Clamp the y rotation between 0 and 50 degrees
                    this.rotation.y = Mathf.Clamp(this.rotation.y, 0f, 50f);
                }
            }

            // Zoom
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {

                // Magnitude controls how quickly the camera zooms
                float magnitude = 5f;

                float scroll = Input.GetAxis("Mouse ScrollWheel") * magnitude;

                // This computation dampens the scroll speed as you're closer to the origin
                // Multiply by -1 makes scroll up = zoom in, and scroll down = zoom out
                scroll *= this.distance * -1f;

                // (Scroll up to zoom in, scroll down to zoom out)
                this.distance += scroll;

                // Clamp the camera distance between min and max distance (meters)
                this.distance = Mathf.Clamp(this.distance, this.min, this.max);
            }

            // Translation
            if (Input.GetMouseButton(2))
            {
                // Magnitude controls how quickly the camera translates
                float magnitude = 15f;

                // Determine whether the mouse is moving in a given frame
                if (Input.GetAxis("Mouse X") > 0)
                {
                    this.origin.transform.Translate(Vector3.left * magnitude * Time.smoothDeltaTime);
                }
                if (Input.GetAxis("Mouse X") < 0)
                {
                    this.origin.transform.Translate(Vector3.right * magnitude * Time.smoothDeltaTime);
                }
                if (Input.GetAxis("Mouse Y") > 0)
                {
                    this.origin.transform.Translate(Vector3.down * magnitude * Time.smoothDeltaTime);
                }
                if (Input.GetAxis("Mouse Y") < 0)
                {
                    this.origin.transform.Translate(Vector3.up * magnitude * Time.smoothDeltaTime);
                }

                this.origin.transform.position = new Vector3(
                    Mathf.Clamp(this.origin.transform.position.x, -10, 10f),
                    Mathf.Clamp(this.origin.transform.position.y, 0f, 15f),
                    Mathf.Clamp(this.origin.transform.position.z, -10f, 10f)
                );
            }

            this.AlignCamera();
        }

        void AlignCamera()
        {
            /*
            Orbit and scroll generally control how quickly 
            1. The movement starts and stops, and 
            2. How smooth the movement looks

            Recommended values:
            Orbit = 10f
            Scroll = 5f
            */

            float orbit = 10f;

            float scroll = 16f;

            // Camera rig transformations
            Quaternion QT = Quaternion.Euler(rotation.y, rotation.x, 0);
            this.origin.rotation = Quaternion.Lerp(this.origin.rotation, QT, Time.deltaTime * orbit);

            if (this._camera.localPosition.z != this.distance * -1f)
            {
                this._camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._camera.localPosition.z, this.distance * -1f, Time.deltaTime * scroll));
            }
        }
    }

}