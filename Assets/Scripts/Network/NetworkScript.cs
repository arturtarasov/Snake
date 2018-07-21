//This is a script that creates a Toggle that you enable to start the Server.
//Attach this script to an empty GameObject
//Create a Toggle GameObject by going to Create>UI>Toggle.
//Click on your empty GameObject.
//Click and drag the Toggle GameObject from the Hierarchy to the Toggle section in the Inspector window.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//This makes the GameObject a NetworkManager GameObject
public class Example : NetworkManager
{
	public Toggle m_Toggle;
	Text m_ToggleText;

	void Start()
	{
		//Fetch the Text of the Toggle to allow you to change it later
		m_ToggleText = m_Toggle.GetComponentInChildren<Text>();
		OnOff(false);
	}

	//Connect this function to the Toggle to start and stop the Server
	public void OnOff(bool change)
	{
		//Detect when the Toggle returns false
		if (change == false)
		{
			//Stop the Server
			StopServer();
			//Change the text of the Toggle
			m_ToggleText.text = "Connect Server";
		}

		//Detect when the Toggle returns true
		if (change == true)
		{
			//Start the Server
			StartServer();
			//Change the Toggle Text
			m_ToggleText.text = "Disconnect Server";
		}
	}

	//Detect when the Server starts and output the status
	public override void OnStartServer()
	{
		//Output that the Server has started
		Debug.Log("Server Started!");
	}

	//Detect when the Server stops
	public override void OnStopServer()
	{
		//Output that the Server has stopped
		Debug.Log("Server Stopped!");
	}
	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 50, 50), "Click"))
		{
			Debug.Log("Clicked the button with an image");
		}
		if (GUI.Button(new Rect(10, 70, 50, 30), "Click"))
		{
			Debug.Log("Clicked the button with text");
		}
	}
}