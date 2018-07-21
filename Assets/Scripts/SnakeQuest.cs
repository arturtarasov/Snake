using UnityEngine;
using System.Collections.Generic;

public class SnakeQuest : MonoBehaviour
{
        [SerializeField] private List<Element> gameElements;


        private float timer;
        private int MaxX = 25;
        private int MaxY = 41;
        public int MinX = 0;
        public int MinY = 0;
        public static int score = 0;


        public Element GetElementByType(ElementType elementType)
        {
                foreach (var gameElement in gameElements)
                {
                        if (gameElement.ElementType == elementType)
                        {
                                return gameElement;
                        }
                }
                return null;
        }

        public void StartSnake()
        {
                var pos = new Obj
                {
                        x = 5,
                        y = 6
                };
                pos.SpawnedObject =
                        (GameObject)
                                Network.Instantiate(GetElementByType(ElementType.Snake).Prefab,
                                        new Vector2(pos.x, pos.y), Quaternion.identity, 0);


                GetElementByType(ElementType.Snake).SpawnedObjects = new List<Obj>();

                if (GetElementByType(ElementType.Snake).SpawnedObjects != null)
                {
                        GetElementByType(ElementType.Snake).SpawnedObjects.Add(pos);
                }

                pos = new Obj
                {
                        x = 5,
                        y = 7
                };
                pos.SpawnedObject =
                        (GameObject)
                                Network.Instantiate(GetElementByType(ElementType.Snake).Prefab,
                                        new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                if (GetElementByType(ElementType.Snake).SpawnedObjects != null)
                {
                        GetElementByType(ElementType.Snake).SpawnedObjects.Add(pos);
                }

                pos = new Obj
                {
                        x = 5,
                        y = 8
                };
                pos.SpawnedObject =
                        (GameObject)
                                Network.Instantiate(GetElementByType(ElementType.Snake).Prefab,
                                        new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                if (GetElementByType(ElementType.Snake).SpawnedObjects != null)
                {
                        GetElementByType(ElementType.Snake).SpawnedObjects.Add(pos);
                }
        }

        public void StartQuest()
        {
                StartSnake();

                GetElementByType(ElementType.Border).SpawnedObjects = new List<Obj>();
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
                                        Network.Instantiate(GetElementByType(ElementType.Border).Prefab,
                                                new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                        if (GetElementByType(ElementType.Border).SpawnedObjects != null)
                        {
                                GetElementByType(ElementType.Border).SpawnedObjects.Add(pos);
                        }
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
                                        Network.Instantiate(GetElementByType(ElementType.Border).Prefab,
                                                new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                        if (GetElementByType(ElementType.Border).SpawnedObjects != null)
                        {
                                GetElementByType(ElementType.Border).SpawnedObjects.Add(pos);
                        }
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
                                        Network.Instantiate(GetElementByType(ElementType.Border).Prefab,
                                                new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                        if (GetElementByType(ElementType.Border).SpawnedObjects != null)
                        {
                                GetElementByType(ElementType.Border).SpawnedObjects.Add(pos);
                        }
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
                                        Network.Instantiate(GetElementByType(ElementType.Border).Prefab,
                                                new Vector2(pos.x, pos.y), Quaternion.identity, 0);

                        if (GetElementByType(ElementType.Border).SpawnedObjects != null)
                        {
                                GetElementByType(ElementType.Border).SpawnedObjects.Add(pos);
                        }
                }
                SpawnFood(); //создание еды
        }

        public void StopQuest()
        {

        }

        public void PauseQuest()
        {

        }

        public void UpdateQuest()
        {
                timer += Time.deltaTime;
                if (timer > 0.1f)
                {
                        Debug.Log("Количество игроков " + Object.FindObjectOfType<NetworkMenu>().playersArray.Count);
                        foreach (var playerElement in Object.FindObjectOfType<NetworkMenu>().playersArray)
                        {
                                MoveSnake(playerElement);
                        }

                        timer = 0;



                        //NoFoodInSnake();
                }
        }

        private void MoveSnake(PlayerElement playerElement) //передвижение змейки
        {
                var pos = new Obj();
                pos.x = GetElementByType(ElementType.Snake).SpawnedObjects[0].x + (int) playerElement.speed.x;
                pos.y = GetElementByType(ElementType.Snake).SpawnedObjects[0].y + (int) playerElement.speed.y;
                Debug.Log(playerElement.speed.x + " " + playerElement.speed.y);
                pos.SpawnedObject =
                        (GameObject)
                                Network.Instantiate(GetElementByType(ElementType.Snake).Prefab,
                                        new Vector2(pos.x, pos.y), Quaternion.identity, 0);
                GetElementByType(ElementType.Snake).SpawnedObjects.Insert(0, pos);

                if (pos.x == MinX || pos.x == MaxX || pos.y == MinY || pos.y == MaxY)
                {
                        playerElement.speed = new Vector2(0, 0);
                }
                if (NoFoodInSnake() == false)
                {
                        Network.Destroy(
                                GetElementByType(ElementType.Snake).SpawnedObjects[
                                        GetElementByType(ElementType.Snake).SpawnedObjects.Count - 1].SpawnedObject);
                        GetElementByType(ElementType.Snake)
                                .SpawnedObjects.RemoveAt(GetElementByType(ElementType.Snake).SpawnedObjects.Count - 1);
                }
        }

        private void SpawnFood()
        {
                var pos = new Obj();
                pos.x = Random.Range(MinX + 1, MaxX);
                pos.y = Random.Range(MinY + 1, MaxY);

                if (GetElementByType(ElementType.Food).SpawnedObjects != null &&
                    GetElementByType(ElementType.Food).SpawnedObjects.Count > 0)
                {
                        Network.Destroy(GetElementByType(ElementType.Food).SpawnedObjects[0].SpawnedObject);
                        GetElementByType(ElementType.Food).SpawnedObjects.RemoveAt(0);
                }
                pos.SpawnedObject =
                        (GameObject)
                                Network.Instantiate(GetElementByType(ElementType.Food).Prefab, new Vector2(pos.x, pos.y),
                                        Quaternion.identity, 0);

                if (GetElementByType(ElementType.Food).SpawnedObjects != null)
                {
                        GetElementByType(ElementType.Food).SpawnedObjects.Add(pos);
                }

        }

        private bool NoFoodInSnake()
        {
                for (int i = 0; i < GetElementByType(ElementType.Snake).SpawnedObjects.Count; i++)
                {
                        if (GetElementByType(ElementType.Food).SpawnedObjects[0].x ==
                            GetElementByType(ElementType.Snake).SpawnedObjects[i].x &&
                            GetElementByType(ElementType.Food).SpawnedObjects[0].y ==
                            GetElementByType(ElementType.Snake).SpawnedObjects[i].y)
                        {
                                score++;
                                SpawnFood();
                                return true;
                        }
                }
                return false;
        }
}