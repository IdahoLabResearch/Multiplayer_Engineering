using System.Collections.Generic;
using UnityEngine;

namespace Serval
{
    public class Actions : MonoBehaviour
    {
        public Material normal;
        public Material fade;
        private List<GameObject> HighlightableObjects = WebGLManager.Scene.HighlightableObjects;
        private string id;

        void Start()
        {
            this.id = gameObject.name;
            this.normal = gameObject.GetComponent<MeshRenderer>().material;
            this.fade = Resources.Load<Material>("Materials/Fade");
        }

        public void HighlightBlock()
        {
            foreach (GameObject go in HighlightableObjects)
            {
                // Highlight the clicked game object, and fade all others.
                if (go.name != this.id)
                {
                    go.GetComponent<MeshRenderer>().material = this.fade;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
                else if (go.name == this.id)
                {
                    go.GetComponent<MeshRenderer>().material = this.normal;
                    for (int i = 0; i < go.transform.childCount; i++)
                    {
                        go.transform.GetChild(i).gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ClearHighlight()
        {
            gameObject.GetComponent<MeshRenderer>().material = normal;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}