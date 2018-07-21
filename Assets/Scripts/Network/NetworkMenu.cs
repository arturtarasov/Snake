using UnityEngine;
using System.Collections.Generic;

public class PlayerElement
{
        public int Id;
	public NetworkPlayer networkPlayer;
	public Vector2 speed;
	public GameObject snake;
        public int Length { get; set; }


        public List<GameObject> SnakeElements { get; set; }
}

public class NetworkMenu : MonoBehaviour
{
        public List<PlayerElement> playersArray = new List<PlayerElement>();

        public string connectionIP = "192.168.43.98";
        public int portNumber = 8632;
        private bool connected = false;

        private void Update()
        {
                if (Network.isClient)
                {
                        if (Input.GetKeyDown(KeyCode.W))
                        {
                                GetComponent<NetworkView>().RPC("UpdateSnakeMove", RPCMode.Server, 0, 1);
                        }
                        if (Input.GetKeyDown(KeyCode.S))
                        {
                                GetComponent<NetworkView>().RPC("UpdateSnakeMove", RPCMode.Server, 0, -1);
                        }
                        if (Input.GetKeyDown(KeyCode.D))
                        {
                                GetComponent<NetworkView>().RPC("UpdateSnakeMove", RPCMode.Server, 1, 0);
                        }
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                                GetComponent<NetworkView>().RPC("UpdateSnakeMove", RPCMode.Server, -1, 0);
                        }
                }
        }

        public void HideMenu()
        {
                GetComponent<NetworkView>().RPC("HideMenuRPC", RPCMode.All, null);
        }

        [RPC]
        public void HideMenuRPC(NetworkMessageInfo info)
        {
                FindObjectOfType<UIController>().DisableAll();
                if (Network.isServer)
                {
                        FindObjectOfType<QuestController>().StartQuest();
                }
        }

        public void StartSnakeClient()
        {
                GetComponent<NetworkView>().RPC("StartSnakeRPC", RPCMode.Server, null);
        }

        [RPC]
        public void StartSnakeRPC(NetworkMessageInfo info)
        {
                FindObjectOfType<SnakeQuest>().StartQuest();
        }

        //управление другим клиентом змейкой при подключении к серверу???
        /*
	public void NewPlayerConnected()
	{
	    networkView.RPC("NewPlayerConnectedRPC", RPCMode.All, null);
	}
	[RPC]
	public void NewPlayerConnectedRPC()
	{
	    FindObjectOfType<SnakeQuest>().StartQuest();
	}*/
        /*
	 Срабатывает на клиенте
	     */

        #region Standart functions

        private void OnConnectedToServer()
        {
                connected = true;
        }

        /*
	 Срабатывает на сервере
	     */

        private void OnServerInitialized()
        {
                connected = true;
        }

        /*
	Срабатывает на клиенте
	    */

        private void OnDisconnectedFromServer(NetworkDisconnection info)
        {
                connected = false;
        }

        /*
	 Срабатывает на сервере
	     */

        private void OnPlayerConnected(NetworkPlayer networkPlayer)
        {
                foreach (var player in playersArray)
                {
                        if (player.networkPlayer == networkPlayer)
                        {
                                Debug.Log("Player is exist");
                                playersArray.Remove(player);
                        }
                }

                PlayerElement pe = new PlayerElement();
                pe.networkPlayer = networkPlayer;
                playersArray.Add(pe);

                //после добавления клиента в список создаем змейку
                StartSnakeClient();

                Debug.Log("Player is connected with " + networkPlayer.ipAddress);
        }

        /*
	Срабатывает на сервере
	    */

        private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
        {
        }

        #endregion


        [RPC]
        private void UpdateSnakeMove(int x, int y, NetworkMessageInfo info)
        {
                if (Network.isServer)
                {
                        foreach (var player in playersArray)
                        {
                                if (player.networkPlayer == info.sender)
                                {
                                        player.speed = new Vector2(x, y);

                                        Debug.Log("Player GUID = " + player.networkPlayer.guid +" and speed = ("+ player.speed.x +";"+ player.speed.y+")");
                                }
                        }
                }
        }
        
        private void OnGUI()
        {
                if (Network.isServer)
                {
                        GUILayout.Label("Running as a server");
                        GUILayout.Label("Connections: " + Network.connections.Length.ToString());
                }
                else if (Network.isClient)
                {
                        GUILayout.Label("Running as a client");
                }

                if (!connected)
                {
                        connectionIP = GUILayout.TextField(connectionIP);
                        int.TryParse(GUILayout.TextField(portNumber.ToString()), out portNumber);

                        if (GUILayout.Button("Connect"))
                        {
                                Network.Connect(connectionIP, portNumber);
                        }
                        if (GUILayout.Button("Host"))
                        {
                                Network.InitializeServer(4, portNumber, true);
                        }
                }
                else
                {
                        if (GUILayout.Button("Disconnect"))
                        {
                                Network.Disconnect();
                        }
                }
        }
}