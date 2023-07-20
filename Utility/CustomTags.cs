using UnityEngine;
using System.Collections.Generic;

namespace Utility
{
    public class CustomTags : MonoBehaviour
    {

        /// README
        /// <summary></summary>
        /// <param name="tags">An array of tags, corresponding to the scripts you intend to attach to the gameobject</param>
        /// <remarks>Attach this script to any gameobject that you'd like the framework to dynamically attach scripts to at runtime.</remarks>
        /// <see cref="../Scene/Scene.cs"/>

        [SerializeField]
        public List<string> tags = new List<string>();

        public void AddTag(string tag)
        {
            tags.Add(tag);
        }
        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }

        public List<string> GetTags()
        {
            return this.tags;
        }

        public static List<GameObject> GetGameObjects()
        {
            List<GameObject> taggedGameObjects = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject go in objects)
            {
                if (go.TryGetComponent<CustomTags>(out CustomTags component))
                {
                    if (go.GetComponent<CustomTags>())
                    {
                        taggedGameObjects.Add(go);
                    }
                }
            }
            return taggedGameObjects;
        }

        public static List<GameObject> GetGameObjectsWithTag(string tag)
        {
            List<GameObject> taggedGameObjects = new List<GameObject>();

            GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject go in objects)
            {
                if (go.TryGetComponent<CustomTags>(out CustomTags component))
                {
                    if (go.GetComponent<CustomTags>().HasTag(tag))
                    {
                        taggedGameObjects.Add(go);
                    }
                }
            }
            return taggedGameObjects;
        }

        public void Rename(int index, string tagName)
        {
            tags[index] = tagName;
        }

        public string GetAtIndex(int index)
        {
            return tags[index];
        }

        public int Count
        {
            get { return tags.Count; }
        }
    }
}