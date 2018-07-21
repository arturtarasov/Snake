using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GameInfo : MonoSingleton<GameInfo>
{
	public QuestController questController;
	public UIController uiController;

	public void Awake()
	{
		if (questController == null)
		{
			questController = FindObjectOfType<QuestController>();
		}
		if (uiController == null)
		{
			uiController = FindObjectOfType<UIController>();
		}


		Exec("Main");
	}
	public void Start()
	{
	}
	public void Update()
	{
	}
	public void OnDestroy()
	{
	}


	public void Exec(string parameter)
	{
		if (parameter == "Start")
		{
			uiController.ChangeMenuState(MenuState.None);
			questController.StartQuest();


			FindObjectOfType<NetworkMenu>().HideMenu();
		}
		if (parameter == "Pause")
		{
			uiController.ChangeMenuState(MenuState.Pause);
			questController.StartQuest();
		}
		if (parameter == "Stop")
		{
			uiController.ChangeMenuState(MenuState.Results);
			questController.StopQuest();
		}
		if (parameter == "Main")
		{
			uiController.ChangeMenuState(MenuState.Main);
			questController.StopQuest();
		}
		if (parameter == "Exit")
		{
			uiController.ChangeMenuState(MenuState.None);
			questController.StopQuest();

			if (Application.isEditor)
			{
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif
			}
			else
			{
				Application.Quit();
			}
		}
	}
}