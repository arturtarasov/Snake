using UnityEngine;

public class QuestController : MonoBehaviour
{
	public QuestState currentQuestState;
	public SnakeQuest snakeQuest;
	public void StartQuest()
	{
		currentQuestState = QuestState.Started;
		snakeQuest.StartQuest();
	}

	public void Update()
	{
		if (currentQuestState == QuestState.Started)
		{
			snakeQuest.UpdateQuest();
		}
	}
	public void StopQuest()
	{
		currentQuestState = QuestState.Stopped;
		snakeQuest.StopQuest();
	}
	public void PauseQuest()
	{
		currentQuestState = QuestState.Paused;
		snakeQuest.PauseQuest();
	}
}