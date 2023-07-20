using UnityEngine;

namespace Serval
{
    public class Player
    {
        public string id;
        public GameObject go;

        public Player(GameObject go)
        {
            this.go = go;
        }

        public Vector3 GetPosition()
        {
            return this.go.transform.localPosition;
        }

        public Vector3 GetRotation()
        {
            return this.go.transform.localEulerAngles;
        }

        public State GetState()
        {
            Vector3 position = this.GetPosition();
            Vector3 rotation = this.GetRotation();
            return new State(position, rotation);
        }

        public string UpdateUserState()
        {
            State state = this.GetState();
            // Serval requires a valid JSON, but the NativeWebSocket library sends plain text
            return "{\"UpdateUserState\": {\"state\":  {\"position\": \"" + state.position.ToString() + "\"," + "\"rotation\": \"" + state.rotation.ToString() + "\"}}}";
        }

        public void DeletePlayer()
        {
            Serval.Handler.PlayerList.Remove(this);
        }
    }
}