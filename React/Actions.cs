using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using WebGLManager;
using Interactions;

namespace React
{
    public class Actions : MonoBehaviour
    {
        /// README
        /// <summary>This class holds functions that React SENDS to Unity</summary>
        /// <see href="https://react-unity-webgl.dev/docs/api/send-message"/>

        // Dispatch JS Events
        [DllImport("__Internal")]
        private static extern void OpenDataPanel(string target);

        void Update()
        {
            // TODO: This doesn't work because the interactions are only dictated by Object.cs
            // Deactivate the functionality using control logic
#if UNITY_WEBGL == true && UNITY_EDITOR == false
            if (Input.GetMouseButtonDown(1))
            {
                OpenDataPanel(gameObject.name);  
            }
#endif
        }

        public void Snap(string name)
        {
            Camera camera = Camera.main;
            GameObject target = GameObject.Find(name);

            camera.GetComponent<CameraOrbit>().origin.position = target.transform.position;
        }

        // This function behaves as if it were static, but React-Unity-WebGL can't access static methods
        // Instead we pull in Scene.HighlightableObjects and iterate over them
        public void HighlightBlock(string name)
        {
            foreach (GameObject go in Scene.HighlightableObjects)
            {
                // Highlight the clicked game object, and fade all others.
                if (go.name != name)
                {
                    go.GetComponent<Serval.Object>().debounce = true;
                    go.GetComponent<Serval.Object>().update = true;
                    go.GetComponent<Serval.Object>().highlighted = false;

                    StartCoroutine(go.GetComponent<Serval.Object>().DebounceUpdate());

                    go.GetComponent<MeshRenderer>().material = go.GetComponent<Serval.Actions>().fade;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else if (go.name == name)
                {
                    go.GetComponent<Serval.Object>().debounce = true;
                    go.GetComponent<Serval.Object>().update = true;
                    go.GetComponent<Serval.Object>().highlighted = true;

                    StartCoroutine(go.GetComponent<Serval.Object>().DebounceUpdate());

                    go.GetComponent<MeshRenderer>().material = go.GetComponent<Serval.Actions>().normal;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}