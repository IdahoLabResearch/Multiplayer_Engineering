using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;
using Serval;

namespace WebGLManager
{
    public class Scene : MonoBehaviour
    {
        // Scenes
        [DllImport("__Internal")]
        private static extern void SendScenes(string response);
        public static List<string> Scenes = new List<string>();

        // DeepLynx
        [DllImport("__Internal")]
        private static extern void SendNodes(string response);

        // These tagged objects are used throughout the package
        public static List<GameObject> TaggedGameObjects = new List<GameObject>();
        public static List<GameObject> HighlightableObjects = new List<GameObject>();
        public static List<GameObject> DraggableObjects = new List<GameObject>();
        public static List<GameObject> ServalObjects = new List<GameObject>();
        public static List<GameObject> ReactObjects = new List<GameObject>();

        void Start()
        {
            TaggedGameObjects = CustomTags.GetGameObjects();
            HighlightableObjects = CustomTags.GetGameObjectsWithTag("Highlight");
            DraggableObjects = CustomTags.GetGameObjectsWithTag("Draggable");
            ServalObjects = CustomTags.GetGameObjectsWithTag("Serval");
            ReactObjects = CustomTags.GetGameObjectsWithTag("React");

            Setup();
        }

        public void Setup()
        {
            // Dynamically attach scripts to their respective tagged gameobjects
            foreach (GameObject go in TaggedGameObjects)
            {
                if (ReactObjects.Contains(go))
                {
                    go.AddComponent<React.Actions>();
                }
                if (ServalObjects.Contains(go))
                {
                    go.AddComponent<Serval.Object>();
                    go.AddComponent<Serval.Actions>();
                }
                if (DraggableObjects.Contains(go))
                {
                    go.AddComponent<Interactions.Draggable>();
                }
            }

#if UNITY_WEBGL == true && UNITY_EDITOR == false
            // Send nodes to React
            string nodes = DeepLynx.Utilities.ListNodes();
            SendNodes(nodes);

            // Send scenes to React
            string scenes = string.Join(",", this.ListScenes());
            SendScenes(scenes);
#endif
        }

        public List<string> ListScenes()
        {
            List<string> Scenes = new List<string>();
            int SceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < SceneCount; i++)
            {
                Scenes.Add(SceneUtility.GetScenePathByBuildIndex(i));
            }

            return Scenes;
        }

#if UNITY_WEBGL == true && UNITY_EDITOR == false
        public void RetrieveScenes()
        {
            string message = string.Join(",", Scenes);
            SendScenes(message);
        }

        public void OpenScene(string name)
        {
            // Load the new scene
            StartCoroutine(LoadSceneAsync(name));
        }
#endif

        IEnumerator LoadSceneAsync(string scene)
        {
            // The Application loads the Scene in the background as the current Scene runs
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

}