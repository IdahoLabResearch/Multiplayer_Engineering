using UnityEngine;
using System.Collections.Generic;
using Utility;

namespace DeepLynx
{
    public class Utilities : MonoBehaviour
    {
        // This script returns a string of gameobjects tagged as DeepLynx
        public static string ListNodes()
        {
            List<string> nodes = new List<string>();

            List<GameObject> gameObjects = CustomTags.GetGameObjectsWithTag("DeepLynx");

            foreach (GameObject go in gameObjects)
            {
                nodes.Add(go.name);
            }

            return string.Join(",", nodes);
        }
    }
}