using UnityEngine;
using Utility;

namespace Interactions
{
    public class Draggable : MonoBehaviour
    {
        /// README
        /// <summary>This script enables users to pick up and move gameobjects</summary>
        /// <remarks>The gameobject with this script must also have a Box Collider, and Rigid Body. This was very hard to program for 3D scenes, and so it is recommended you add constraints to the Rigid Body by freezing certain axes of position or rotation to reduce chaotic movement.</remarks>
        /// <see cref="../Utility/CustomTags.cs"/>

        private GameObject selectedObject;
        private Vector3 mPrevPos = Vector3.zero;
        private Vector3 mPosDelta = Vector3.zero;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (selectedObject == null)
                {
                    RaycastHit hit = CastRay();
                    if (hit.collider != null)
                    {
                        CustomTags customTags = hit.collider.gameObject.GetComponent<CustomTags>();
                        if (customTags)
                        {
                            if (customTags.HasTag("Draggable"))
                            {
                                selectedObject = hit.collider.gameObject;
                                Cursor.visible = false;
                            }
                        }

                        return;
                    }
                    return;
                }
                else
                {
                    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

                    selectedObject.transform.position = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
                    selectedObject = null;
                    Cursor.visible = true;
                }
            }

            if (selectedObject != null)
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);

                selectedObject.transform.position = new Vector3(worldPosition.x, selectedObject.transform.position.y, worldPosition.z);
            }

        }

        private RaycastHit CastRay()
        {
            Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
            Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
            Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
            Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);

            RaycastHit hit;
            Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit);

            return hit;
        }
    }

}