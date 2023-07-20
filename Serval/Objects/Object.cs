using System.Collections;
using UnityEngine;
using WebGLManager;
#if UWP || UNITY_UWP_EDITOR
using Microsoft.MixedReality.Toolkit.Input;
#endif

namespace Serval
{
    public class Object : MonoBehaviour
#if UWP || UNITY_UWP_EDITOR
, IMixedRealityInputHandler
#endif
    {
        // Object Properties
        [System.NonSerialized]
        public string id;
        private string message;

        // Object Physics
        private Vector3 currentPosition;
        private Vector3 lastPosition;
        private Quaternion currentRotation;
        private Quaternion lastRotation;

        // Mixed Reality Interactions
        private bool buttonDown = false;
        private bool buttonUp = false;

        // Serval Logic
        [System.NonSerialized]
        public bool locked = false;
        [System.NonSerialized]
        public bool highlighted = false;
        [System.NonSerialized]
        public bool debounce;
        [System.NonSerialized]
        public bool update = false;

        void Start()
        {
            this.id = this.transform.name;
        }

        void LateUpdate()
        {
            this.lastPosition = this.transform.position;
        }

        void FixedUpdate()
        {
            // Update object state independent of Unity frames
            this.currentPosition = this.transform.position;
            this.message = this.UpdateObjectState();
        }

        void Update()
        {
            // Right click locks the object
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.gameObject == gameObject && !this.locked)
                    {
                        this.locked = true;
                    }
                    else if (hit.collider.gameObject == gameObject && this.locked)
                    {
                        this.debounce = true;
                        this.locked = false;

                        // Debounce the placement of the object to eliminate disagreement with Serval
                        StartCoroutine(DebounceUpdate());
                    }
                }
            }
            // Left click highlights the object
            else if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.collider.gameObject == gameObject && !this.highlighted)
                    {
                        // Debounce the update
                        this.debounce = true;

                        // Highlight the gameobject
                        this.highlighted = true;
                        StartCoroutine(DebounceUpdate());

                        foreach (GameObject go in Scene.ServalObjects)
                        {
                            if (go.name != this.id)
                            {
                                // Debounce and remove the highlight from all other gameobjects
                                go.GetComponent<Object>().debounce = true;
                                go.GetComponent<Object>().highlighted = false;
                                StartCoroutine(go.GetComponent<Object>().DebounceUpdate());
                            }
                        }

                        gameObject.GetComponent<Actions>().HighlightBlock();
                    }
                    else if (hit.collider.gameObject == gameObject && this.highlighted)
                    {
                        // If the block is already highlighted, restore all gameobjects to their normal material
                        foreach (GameObject go in Scene.HighlightableObjects)
                        {
                            // Debounce the gameobject
                            Object target = go.GetComponent<Object>();
                            target.debounce = true;

                            // Clear its highlight
                            go.GetComponent<Actions>().ClearHighlight();
                            target.highlighted = false;

                            StartCoroutine(target.GetComponent<Object>().DebounceUpdate());
                        }
                    }
                }
            }

            // If the ESC key is pressed, return all gameobjects to their normal material
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Restore all gameobjects to their normal material
                foreach (GameObject go in Scene.HighlightableObjects)
                {
                    // Debounce the gameobject
                    Object target = go.GetComponent<Object>();
                    target.debounce = true;

                    // Let the Serval Handler know updates have been made
                    target.update = true;

                    // Clear its highlight
                    go.GetComponent<Actions>().ClearHighlight();
                    target.highlighted = false;

                    StartCoroutine(target.GetComponent<Object>().DebounceUpdate());
                }
            }
        }

        public IEnumerator DebounceUpdate()
        {
            this.update = true;
            yield return new WaitForSeconds(.5f);
            this.update = false;
            this.debounce = false;
            yield break;
        }

        // HOLOLENS
#if UWP || UNITY_UWP_EDITOR
        void IMixedRealityInputHandler.OnInputUp(InputEventData eventData)
        {
            Debug.Log("Ran OnInputUp");
            buttonUp = true;
            buttonDown = false;
        }

        void IMixedRealityInputHandler.OnInputDown(InputEventData eventData)
        {
            Debug.Log("Ran OnInputDown");
            buttonDown = true;
        }
#endif

        public Vector3 GetPosition()
        {
            return this.transform.position;
        }

        public Quaternion GetRotation()
        {
            return this.transform.rotation;
        }

        public ObjectState GetState()
        {
            Vector3 position = this.GetPosition();
            Quaternion rotation = this.GetRotation();

            return new ObjectState(position, rotation, this.highlighted, this.locked);
        }

        public string UpdateObjectState()
        {
            ObjectState state = this.GetState();

            // Serval requires a valid JSON, but the NativeWebSocket library sends plain text
            return "{\"UpdateObjectState\": [{\"id\": \"" + this.id + "\", " + "\"state\":  {\"position\": \"" + state.position.ToString() + "\"," + "\"rotation\": \"" + state.rotation.eulerAngles.ToString() + "\"," + "\"highlighted\": \"" + state.highlighted.ToString() + "\"," + "\"locked\": \"" + state.locked.ToString() + "\"}}]}";
        }

        public bool CheckUpdates()
        {
            // Don't broadcast object state if it hasn't moved
            float magnitude = new Vector3(currentPosition.x - lastPosition.x, currentPosition.y - lastPosition.y, currentPosition.z - lastPosition.z).magnitude;

            if (magnitude > 0.01f || this.update)
            {
                return true;
            }

            return false;
        }

        public void UpdatePosition(Vector3 position)
        {
            this.transform.position = position;
        }

        public void UpdateRotation(Vector3 rotation)
        {
            this.transform.eulerAngles = rotation;
        }

        public void UpdateHighlight(bool highlighted)
        {
            if (highlighted != this.highlighted && highlighted == true)
            {
                this.highlighted = highlighted;

                gameObject.GetComponent<Actions>().HighlightBlock();

                StartCoroutine(DebounceUpdate());

                foreach (GameObject go in Scene.ServalObjects)
                {
                    if (go.name != this.id)
                    {
                        // Remove the highlight from all other gameobjects
                        go.GetComponent<Object>().update = true;
                        go.GetComponent<Object>().highlighted = false;
                        StartCoroutine(go.GetComponent<Object>().DebounceUpdate());
                    }
                }
            }
            else if (highlighted != this.highlighted && highlighted == false)
            {
                foreach (GameObject go in Scene.HighlightableObjects)
                {
                    // Debounce the gameobject
                    Object target = go.GetComponent<Object>();
                    target.highlighted = false;

                    // Clear its highlight
                    go.GetComponent<Actions>().ClearHighlight();

                    // Let the Serval Handler know updates have been made
                    StartCoroutine(target.GetComponent<Object>().DebounceUpdate());
                }
            }
        }
    }
}