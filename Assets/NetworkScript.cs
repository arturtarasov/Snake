using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SnakeTypes
{
    public GameObject prefab;
    public int id;
}


public class NetworkScript : MonoBehaviour
{
	private int MaxX = 25;
	private int MaxY = 41;
	public int MinX = 0;
	public int MinY = 0;

	public GameObject barierPrefab;
	private List<GameObject> borderGameObjects;

	public GameObject foodPrefab;

	private GameObject foodGameObject;

	public List<SnakeTypes> snakeTypes;

	public List<PlayerElement> playersArray = new List<PlayerElement>();

	// public string connectionIP = "192.168.43.98";
	public string connectionIP = "127.0.0.1";
	public int portNumber = 8632;
	private bool connected = false;

	private string clientLabel;

	private float timer;

	private float difficultTimer;
	private int difficultKoef;

	#region Standart Client-Server events

	private void OnServerInitialized()
	{
		connected = true;
	}

	private void OnConnectedToServer()
	{
		connected = true;
	}

	private void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		connected = false;
	}

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

		PlayerElement pe = new PlayerElement
		{
			networkPlayer = networkPlayer,
			Id = playersArray.Count
		};
		playersArray.Add(pe);

	}

	private void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
	}

	#endregion


	private void StartGame()
	{
		if (borderGameObjects != null)
		{
			foreach (var border in borderGameObjects)
			{
				Network.Destroy(border);
			}
		}


		borderGameObjects = new List<GameObject>();
		var pos = new Obj();
		//создание барьеров
		for (int i = MinX; i < MaxX + 1; i++)
		{
			pos = new Obj
			{
				x = MinX + i,
				y = MinY
			};
			pos.SpawnedObject =
				(GameObject)
					Network.Instantiate(barierPrefab,
						new Vector2(pos.x, pos.y), Quaternion.identity, 0);
			borderGameObjects.Add(pos.SpawnedObject);
		}
		for (int i = MinX; i < MaxX + 1; i++)
		{
			pos = new Obj
			{
				x = MinX + i,
				y = MaxY
			};
			pos.SpawnedObject =
				(GameObject)
					Network.Instantiate(barierPrefab,
						new Vector2(pos.x, pos.y), Quaternion.identity, 0);
			borderGameObjects.Add(pos.SpawnedObject);
		}
		for (int i = MinY; i < MaxY + 1; i++)
		{
			pos = new Obj
			{
				x = MinX,
				y = MinY + i
			};
			pos.SpawnedObject =
				(GameObject)
					Network.Instantiate(barierPrefab,
						new Vector2(pos.x, pos.y), Quaternion.identity, 0);
			borderGameObjects.Add(pos.SpawnedObject);
		}
		for (int i = MinY; i < MaxY + 1; i++)
		{
			pos = new Obj
			{
				x = MaxX,
				y = MinY + i
			};
			pos.SpawnedObject =
				(GameObject)
					Network.Instantiate(barierPrefab,
						new Vector2(pos.x, pos.y), Quaternion.identity, 0);
			borderGameObjects.Add(pos.SpawnedObject);
		}

		foreach (var player in playersArray)
		{
			if (player.SnakeElements != null)
			{
				foreach (var element in player.SnakeElements)
				{
					Network.Destroy(element.gameObject);
				}
			}

			player.SnakeElements = new List<GameObject>();
			player.SnakeElements.Add((GameObject)Network.Instantiate(snakeTypes[player.Id].prefab, new Vector2(10, 10), Quaternion.identity, 0));
			player.Length = 3;
		}

		SpawnFood();

		difficultTimer = 0.0f;

	}


	#region RPC
	[RPC]
	private void GetClientLabel(NetworkMessageInfo info)
	{
		if (Network.isServer)
		{
			foreach (var player in playersArray)
			{
				if (player.networkPlayer == info.sender)
				{
					GetComponent<NetworkView>().RPC("SetClientLabel", info.sender, player.SnakeElements == null ? 0 : player.SnakeElements.Count - 3);
				}
			}
		}
	}

	[RPC]
	private void SetClientLabel(int count)
	{
		if (Network.isClient)
		{
			clientLabel = count.ToString();
		}
	}


	[RPC]
	private void ServerStartGame(NetworkMessageInfo info)
	{
		if (Network.isServer)
		{
			StartGame();
		}
	}

	[RPC]
	private void ClientSendMove(int x, int y, NetworkMessageInfo info)
	{
		if (Network.isServer)
		{
			foreach (var player in playersArray)
			{
				if (player.networkPlayer == info.sender)
				{
					player.speed = new Vector2(x, y);

					Debug.Log("Player GUID = " + player.networkPlayer.guid + " and speed = (" + player.speed.x + ";" + player.speed.y + ")");
				}
			}
		}
	}
	#endregion


	private void SpawnFood()
	{
		var pos = new Obj();
		pos.x = Random.Range(MinX + 1, MaxX);
		pos.y = Random.Range(MinY + 1, MaxY);

		if (foodGameObject != null)
		{
			Network.Destroy(foodGameObject);

		}
		pos.SpawnedObject =
			(GameObject)
				Network.Instantiate(foodPrefab, new Vector2(pos.x, pos.y),
					Quaternion.identity, 0);

		foodGameObject = pos.SpawnedObject;
	}

	private void GetPlayerInfo()
	{
		if (Network.isServer)
		{
			string temporary = "";
			foreach (var player in playersArray)
			{
				if (player.SnakeElements != null)
				{
					if (player.SnakeElements.Count > 0)
					{
						if (player.SnakeElements[0] != null)
						{
							if (player.SnakeElements[0].GetComponent<MeshRenderer>() != null)
							{
								temporary = temporary + player.Length +
									"|" + player.SnakeElements[0].GetComponent<MeshRenderer>().material.color.r +
									"|" + player.SnakeElements[0].GetComponent<MeshRenderer>().material.color.g +
									"|" + player.SnakeElements[0].GetComponent<MeshRenderer>().material.color.b +
									"|" + player.SnakeElements[0].GetComponent<MeshRenderer>().material.color.a + ";";
							}
						}
					}
				}



			}

			if (isGameEnded)
				GetComponent<NetworkView>().RPC("SetPlayerInfo", RPCMode.Others, temporary);
			else
			{

				GetComponent<NetworkView>().RPC("SetPlayerInfo", RPCMode.Others, "");
			}
		}
	}

	private string playerInfos;
	[RPC]
	private void SetPlayerInfo(string val)
	{
		playerInfos = val;
	}

	private int GetMaxPlayerSnakesLength()
	{
		int max = 1;
		foreach (var player in playersArray)
		{
			if (player.SnakeElements != null)
			{
				if (player.SnakeElements.Count > max)
				{
					max = player.SnakeElements.Count;
				}
			}
		}
		return max < 3 ? 3 : max;
	}

	#region Standart Unity functions
	//создал экземпляр класса SnakeQuest, который всегда равен null
	private SnakeQuest snakeQuest; //нормально именуй! зачем ты квест спидом обозвал ????
	private Vector3 fp;   //Первая позиция касания
	private Vector3 lp;   //Последняя позиция касания
	private float dragDistance;  //Минимальная дистанция для определения свайпа
	private List<Vector3> touchPositions = new List<Vector3>(); //Храним все позиции касания в списке
	private void Start()
	{
		dragDistance = Screen.height * 5 / 100; //dragDistance это 20% высоты экрана

	}
	private void Update()
	{

		if (Network.isClient)
		{
			MoveSnakeClientSwipe();
			if (Input.GetKeyDown(KeyCode.W))
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, 1);
			}
			if (Input.GetKeyDown(KeyCode.S))
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, -1);
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 1, 0);
			}
			if (Input.GetKeyDown(KeyCode.A))
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, -1, 0);
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, 0);
			}
			GetComponent<NetworkView>().RPC("GetClientLabel", RPCMode.Server);
		}
		if (Network.isServer)
		{

			timer += Time.deltaTime;
			//difficultTimer += Time.deltaTime;

			//if (difficultTimer > 5.0f)
			{
				difficultKoef = GetMaxPlayerSnakesLength() / 3;
				//Debug.Log(difficultKoef);
				//difficultTimer = 0.0f;
			}
			GetPlayerInfo();
			if (timer > 0.1f / Mathf.Sqrt(difficultKoef))
			{
				foreach (var player in playersArray)
				{
					if (player.SnakeElements != null && player.SnakeElements.Count > 0 && player.speed != Vector2.zero)
					{
						GameObject gameObject =
							Network.Instantiate(snakeTypes[player.Id].prefab,
								player.SnakeElements[0].transform.position +
								new Vector3(player.speed.x, player.speed.y),
								Quaternion.identity, 0) as GameObject;
						player.SnakeElements.Insert(0, gameObject);

						if (player.SnakeElements.Count > player.Length)
						{
							Network.Destroy(player.SnakeElements[player.SnakeElements.Count - 1]);
							player.SnakeElements.RemoveAt(player.SnakeElements.Count - 1);

						}

					}
				}


				foreach (var player in playersArray)
				{
					if (player.SnakeElements != null)
					{
						foreach (var snakeElement in player.SnakeElements)
						{
							if (snakeElement.transform.position == foodGameObject.transform.position)
							{
								player.Length++;
								SpawnFood();
							}
						}
					}
				}

				foreach (var player in playersArray)
				{
					if (player.SnakeElements != null)
					{
						foreach (var snakeElement in player.SnakeElements)
						{
							foreach (var borderObject in borderGameObjects)
							{
								if (snakeElement.transform.position == borderObject.transform.position)
								{
									foreach (var playerr in playersArray)
									{
										playerr.speed = Vector2.zero;
										isGameEnded = true;
									}
								}
							}


						}
					}
				}

				timer = 0;
			}
		}
	}
	private Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i)
		{
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

	public GUIStyle guiLabel = new GUIStyle();
	public GUIStyle guiTextField = new GUIStyle();
	public GUIStyle guiButton = new GUIStyle();
	private bool isGameEnded;
	private void OnGUI()
	{
		//guiLabel.fontSize = 50;
		//guiTextField.fontSize = 50;

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		if (Network.isServer)
		{
			
			GUILayout.Label("Работает как сервер", guiLabel);
			GUILayout.Label("Количество подключений: " + Network.connections.Length.ToString(), guiLabel);
		}
		else if (Network.isClient)
		{
			GUILayout.Label("Работает как клиент", guiLabel);
		}
		GUILayout.EndVertical();
		if (!connected)
		{
			GUILayout.BeginVertical();
			connectionIP = GUILayout.TextField(connectionIP, guiTextField);
			int.TryParse(GUILayout.TextField(portNumber.ToString(), guiTextField), out portNumber);
			GUILayout.EndVertical();
			if (GUILayout.Button("Подключиться как клиент"))
			{
				Network.Connect(connectionIP, portNumber);
			}
			if (GUILayout.Button("Создать сервер"))
			{
				Network.InitializeServer(4, portNumber, true);
			}
		}
		else
		{
			if (GUILayout.Button("Отключить"))
			{
				Network.Disconnect();
			}
		}
		if (Network.isClient)
		{
			if (GUILayout.Button("Начать новую игру"))
			{
				GetComponent<NetworkView>().RPC("ServerStartGame", RPCMode.Server);
			}
		}
		GUILayout.EndHorizontal();
		if (connected)
		{
			if (Network.isClient)
			{
				GUILayout.Label("Количество очков: " + clientLabel, guiLabel);


				{
					var array = playerInfos.Split(';');


					foreach (var arr in array)
					{
						var t = arr.Split('|');
						GUIStyle gs = new GUIStyle();
						if (t.Length == 5)
						{


							Color c = new Color();

							gs.normal.background = MakeTex(2, 2, new Color(int.Parse(t[1]), int.Parse(t[2]), int.Parse(t[3]), int.Parse(t[4])));

							GUILayout.BeginHorizontal();

							GUILayout.Label("Очков: " + t[0] + " ; Игрок: " /*+ t[1]+ t[2]+ t[3]+ t[4]*/);
							GUILayout.Box("", gs, GUILayout.Width(100), GUILayout.Height(25));

							GUILayout.EndHorizontal();
						}
					}
				}






			}
			if (GUI.Button(new Rect(0, 1430, 1074, 557), "", guiLabel))//down
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, -1);
			}

			if (GUI.Button(new Rect(0, 0, 1074, 557), "", guiLabel)) //up
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, 1);
			}

			if (GUI.Button(new Rect(540, 564, 540, 861), "", guiLabel)) //right
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 1, 0);
			}

			if (GUI.Button(new Rect(0, 564, 540, 861), "", guiLabel)) //left
			{
				GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, -1, 0);
			}
		}
	}
	#endregion
	private void MoveSnakeClientSwipe()
	{
		foreach (Touch touch in Input.touches)  //используем цикл для отслеживания больше одного свайпа
		{ //должны быть закоментированы, если вы используете списки 
		  /*if (touch.phase == TouchPhase.Began) //проверяем первое касание
		  {
		      fp = touch.position;
		      lp = touch.position;

		  }*/

			if (touch.phase == TouchPhase.Moved) //добавляем касания в список, как только они определены
			{
				touchPositions.Add(touch.position);
			}

			if (touch.phase == TouchPhase.Ended) //проверяем, если палец убирается с экрана
			{
				//lp = touch.position;  //последняя позиция касания. закоментируйте если используете списки
				fp = touchPositions[0]; //получаем первую позицию касания из списка касаний
				lp = touchPositions[touchPositions.Count - 1]; //позиция последнего касания

				//проверяем дистанцию перемещения больше чем 20% высоты экрана
				if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
				{//это перемещение
				 //проверяем, перемещение было вертикальным или горизонтальным 
					if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
					{   //Если горизонтальное движение больше, чем вертикальное движение ...
						if ((lp.x > fp.x))  //Если движение было вправо
						{   //Свайп вправо
							Debug.Log("Right Swipe");
							//далее меняем переменную speed в классе SnakeQuest в зависимости от свайпа
							//snakeQuest.speed = new Vector2(1, 0);
							GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 1, 0);
						}
						else
						{   //Свайп влево
							Debug.Log("Left Swipe");
							//snakeQuest.speed = new Vector2(-1, 0);
							GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, -1, 0);
						}
					}
					else
					{   //Если вертикальное движение больше, чнм горизонтальное движение
						if (lp.y > fp.y)  //Если движение вверх
						{   //Свайп вверх
							Debug.Log("Up Swipe");
							//snakeQuest.speed = new Vector2(0, 1);
							GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, 1);
						}
						else
						{   //Свайп вниз
							Debug.Log("Down Swipe");
							//snakeQuest.speed = new Vector2(0, -1);
							GetComponent<NetworkView>().RPC("ClientSendMove", RPCMode.Server, 0, -1);
						}
					}
				}

				else
				{   //Это ответвление, как расстояние перемещения составляет менее 20% от высоты экрана

				}
			}
		}
	}
	private bool TouchScreenMoveSnakeLeft(Vector2 p)
	{
		Vector2 p0 = new Vector2(0f, 0f);
		Vector2 p1 = new Vector2(0f, 1920f);
		Vector2 p2 = new Vector2(960f, 540f);

		var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
		if (A < 0.0)
		{
			s = -s;
			t = -t;
			A = -A;
		}
		return s > 0 && t > 0 && (s + t) <= A;
	}

	private bool TouchScreenMoveSnakeRight(Vector2 p)
	{
		Vector2 p1 = new Vector2(1080f, 1920f);
		Vector2 p2 = new Vector2(1080f, 0f);
		Vector2 p0 = new Vector2(960f, 540f);

		var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
		if (A < 0.0)
		{
			s = -s;
			t = -t;
			A = -A;
		}
		return s > 0 && t > 0 && (s + t) <= A;
	}
	private bool TouchScreenMoveSnakeUp(Vector2 p)
	{
		Vector2 p0 = new Vector2(0f, 1920f);
		Vector2 p1 = new Vector2(1080f, 1920f);
		Vector2 p2 = new Vector2(960f, 540f);

		var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
		if (A < 0.0)
		{
			s = -s;
			t = -t;
			A = -A;
		}
		return s > 0 && t > 0 && (s + t) <= A;
	}
	private bool TouchScreenMoveSnakeDown(Vector2 p)
	{
		Vector2 p0 = new Vector2(0f, 0f);
		Vector2 p2 = new Vector2(1080f, 0f);
		Vector2 p1 = new Vector2(960f, 540f);

		var s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		var t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		var A = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;
		if (A < 0.0)
		{
			s = -s;
			t = -t;
			A = -A;
		}
		return s > 0 && t > 0 && (s + t) <= A;
	}
}