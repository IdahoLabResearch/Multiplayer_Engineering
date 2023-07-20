using UnityEngine;
using UnityEngine.SceneManagement;

namespace Serval
{
    public class PlayerManager : MonoBehaviour
    {
        void OnDisable()
        {
            Debug.Log("Scene Unloaded");
        }
    }
}