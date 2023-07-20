using UnityEngine;
using System.Linq;
using System;
using NativeWebSocket;
using LitJson;
using System.Collections.Generic;
using System.Text;
using Utility;
using Interactions;
using WebGLManager;

namespace Serval
{
    public class Handler : MonoBehaviour
    {
        // Websocket
        public static WebSocket websocket;
        private bool gaming = false;

        // Player Details
        public Player player;
        public GameObject playerPrefab;
        public GameObject PlayerContainer;
        public static List<Player> PlayerList = new List<Player>();
        private Vector3 startingPosition;
        private Quaternion startingRotation;

        // Object Tracking
        private IEnumerable<GameObject> objects;
        public static List<string> lockedMovementList = new List<string>();

        private void Awake()
        {
            PlayerContainer = GameObject.Find("Players");
        }
        // Start is called before the first frame update
        async void Start()
        {
            websocket = new WebSocket("wss://serval.azuredev.inl.gov/containers/420");

            websocket.OnOpen += () =>
            {
                Debug.Log("Websocket Open");

                // In WebGL, the Camera is the player's gameobject
                player = new Player(Camera.main.gameObject);
                objects = Scene.ServalObjects;

                // Start Gaming
                // Broadcast state much more frequently than request state, so as to not request an old state
                InvokeRepeating("RequestState", 0f, .45f);
                InvokeRepeating("BroadcastState", 0f, .01f);
            };

            websocket.OnError += (e) =>
            {
                Debug.Log("Websocket Error: " + e);
            };

            websocket.OnClose += (e) =>
            {
                Debug.Log("Websocket Closed!");
            };

            websocket.OnMessage += (bytes) =>
            {
                // The first Serval message on connection is the player id
                if (this.player.id == null)
                {
                    this.player.id = Encoding.ASCII.GetString(bytes);
                    PlayerList.Add(this.player);
                }
                else
                {
                    // Parse the message as a string
                    var message = Encoding.UTF8.GetString(bytes);
                    JsonData data = JsonMapper.ToObject(message);

                    // If the message contains information about users, update the Players
                    if (data.ContainsKey("users"))
                    {
                        UpdatePlayers(data);
                    }
                    // If the message contains information about objects, update the Objects
                    if (data.ContainsKey("objects"))
                    {
                        UpdateObjects(data);
                    }

                    // The game state is updated by Serval before broadcasting
                    if (this.gaming == false)
                    {
                        Debug.Log("Starting to Game");
                        this.gaming = true;
                    }
                }
            };

            // waiting for messages
            await websocket.Connect();
        }

        void OnDisable()
        {
            // Remove the player from the scene
            this.player.DeletePlayer();

            // Close the websocket connection
            websocket.Close();
        }

        private void OnApplicationQuit()
        {
            // Remove the player from the scene
            this.player.DeletePlayer();

            // Close the websocket connection
            websocket.Close();
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            websocket.DispatchMessageQueue();
#endif
        }

        void RequestState()
        {
            // Request updates
            // On receipt of these updates, websocket.OnMessage() is called
            this.SendWebSocketMessage("{  \"RequestObjectState\": null}");
            this.SendWebSocketMessage("{  \"RequestUserState\": {}}");
        }

        void BroadcastState()
        {
            // The game instance should wait until it's received state from Serval before broadcasting
            if (this.gaming)
            {
                // Update Player
                string PlayerMessage = this.player.UpdateUserState();
                this.SendWebSocketMessage(PlayerMessage);

                // Update Each Participating GameObject
                foreach (GameObject go in objects)
                {
                    // Only broadcast state if the object has moved
                    if (go.GetComponent<Object>().CheckUpdates())
                    {
                        string ObjectMessage = go.GetComponent<Object>().UpdateObjectState();
                        this.SendWebSocketMessage(ObjectMessage);
                    }
                }
            }
        }

        async void SendWebSocketMessage(string message)
        {
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.SendText(message);
            }
        }

        public void UpdatePlayers(JsonData data)
        {
            // Get the list of players
            List<string> UserIDList = data["users"].Cast<JsonData>().Select(json => (string)json["id"]).ToList();

            // Remove the players who aren't in the game
            foreach (Player player in PlayerList.ToList())
            {
                if (player.id != this.player.id)
                {
                    if (!UserIDList.Contains(player.id))
                    {
                        PlayerList.Remove(player);
                        Debug.Log(player.id + " disconnected");
                        GameObject.Destroy(GameObject.Find(player.id));
                    }
                }
            }

            foreach (string UserID in UserIDList)
            {
                bool PlayerAlreadyExists = PlayerList.FirstOrDefault(Player => Player.id == UserID) != null;

                // If the received Player doesn't exist in the scene, create it
                if (!PlayerAlreadyExists)
                {
                    GameObject PlayerObject = Instantiate(playerPrefab, PlayerContainer.transform);

                    // Create and Instantiate the New Player
                    Player NewPlayer = new Player(PlayerObject);
                    NewPlayer.id = UserID;
                    NewPlayer.go.name = NewPlayer.id;

                    PlayerList.Add(NewPlayer);
                }

                // Get the Player at this index from the PlayerList to update it
                Player Player = PlayerList.FirstOrDefault(player => player.id == UserID);

                // Update the Player
                int index = UserIDList.IndexOf(UserID);
                UpdatePlayerData(Player, index, data);
            }
        }

        public void UpdatePlayerData(Player Player, int index, JsonData data)
        {
            string state = data["users"][index]["state"]?.ToString() ?? "";

            if (state != "")
            {
                if (data["users"][index]["state"].Keys.Contains("position"))
                {
                    Player.go.transform.localPosition = StringToVector3(data["users"][index]["state"]["position"].ToString());
                }

                if (data["users"][index]["state"].Keys.Contains("rotation"))
                {
                    Player.go.transform.localEulerAngles = StringToVector3(data["users"][index]["state"]["rotation"].ToString());
                }
            }
        }

        public void UpdateObjects(JsonData data)
        {
            for (int i = 0; i < data["objects"].Count; i++)
            {
                GameObject go = GameObject.Find(data["objects"][i]["id"].ToString());

                //Exclude lines but allow position
                if (data["objects"][i].Keys.Contains("state")
                    && !data["objects"][i]["id"].ToString().StartsWith("line")
                    && data["objects"][i]["state"].Keys.Contains("position"))
                {
                    try
                    {
                        Transform target = GameObject.Find(data["objects"][i]["id"].ToString()).transform;

                        if (target.GetComponent<Object>().debounce)
                        {
                            // If the object is being debounced, it should ignore Serval
                            continue;
                        }

                        Vector3 position = StringToVector3(data["objects"][i]["state"]["position"].ToString());
                        Vector3 rotation = StringToVector3(data["objects"][i]["state"]["rotation"].ToString());

                        bool highlight = bool.Parse(data["objects"][i]["state"]["highlighted"].ToString());

                        //target.GetComponent<Object>().UpdatePosition(position);
                        target.GetComponent<Object>().UpdateHighlight(highlight);

                    }
                    catch (Exception e)
                    {
                        Debug.Log("Failed to move " + data["objects"][i]["id"].ToString() + " Error: " + e.ToString());
                    }
                }
            }
        }


        public Vector3 StringToVector3(string Vector3String)
        {
            // Remove the parentheses
            if (Vector3String.StartsWith("(") && Vector3String.EndsWith(")"))
            {
                Vector3String = Vector3String.Replace("(", "").Replace(")", "");
            }

            // Split the items
            string[] stringArray = Vector3String.Split(',');

            Vector3 vector3 = new Vector3(
                float.Parse(stringArray[0]),
                float.Parse(stringArray[1]),
                float.Parse(stringArray[2])
                );

            return vector3;
        }
    }

}
