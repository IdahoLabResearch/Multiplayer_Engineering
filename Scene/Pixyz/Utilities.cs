using UnityEngine;
using System.Collections.Generic;
using Utility;
using Pixyz.ImportSDK;

namespace Pixyz
{
    public class Utilities : MonoBehaviour
    {
        public List<string> pixyz = new List<string>();

        void Start()
        {
            GameObject[] gameObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject go in gameObjects)
            {
                if (go.TryGetComponent<Metadata>(out Metadata metadata))
                {
                    pixyz.Add(go.name);
                    Debug.Log("Pixyz object: " + go.name);
                }
            }
        }
    }
}