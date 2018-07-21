using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour
{
	//создал экземпляр класса SnakeQuest, который всегда равен null
	private SnakeQuest snakeQuest; //нормально именуй! зачем ты квест спидом обозвал ????
	private Vector3 fp;   //Первая позиция касания
	private Vector3 lp;   //Последняя позиция касания
	private float dragDistance;  //Минимальная дистанция для определения свайпа
	private List<Vector3> touchPositions = new List<Vector3>(); //Храним все позиции касания в списке


	public delegate void OnSwipeEnded(int x, int y);
	public event OnSwipeEnded OnSwipeEndedEvent;

	private void Start()
	{
		dragDistance = Screen.height * 5 / 100; //dragDistance это 20% высоты экрана
	}
	private void Update()
	{
		//if (snakeQuest == null)
		//{
			//snakeQuest = FindObjectOfType<SnakeQuest>();
		//}
		//else
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
								if (OnSwipeEndedEvent != null)
								{
									OnSwipeEndedEvent.Invoke(1, 0);
								}
								//далее меняем переменную speed в классе SnakeQuest в зависимости от свайпа
								//snakeQuest.speed = new Vector2(1, 0);
							}
							else
							{   //Свайп влево
								Debug.Log("Left Swipe");
								if (OnSwipeEndedEvent != null)
								{
									OnSwipeEndedEvent.Invoke(-1, 0);
								}
								//snakeQuest.speed = new Vector2(-1, 0);
							}
						}
						else
						{   //Если вертикальное движение больше, чнм горизонтальное движение
							if (lp.y > fp.y)  //Если движение вверх
							{   //Свайп вверх
								Debug.Log("Up Swipe");
								if (OnSwipeEndedEvent != null)
								{
									OnSwipeEndedEvent.Invoke(0, 1);
								}
								//snakeQuest.speed = new Vector2(0, 1);
							}
							else
							{   //Свайп вниз
								Debug.Log("Down Swipe");
								if (OnSwipeEndedEvent != null)
								{
									OnSwipeEndedEvent.Invoke(0, -1);
								}
								//snakeQuest.speed = new Vector2(0, -1);
							}
						}
					}

					else
					{   //Это ответвление, как расстояние перемещения составляет менее 20% от высоты экрана

					}
				}
			}
		}
	}
}