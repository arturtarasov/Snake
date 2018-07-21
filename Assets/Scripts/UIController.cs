using UnityEngine;

public enum MenuState
{
    None,
    Main,
    Settings,
    Pause,
    Results
}
[System.Serializable]
public class Page
{
    public MenuState menuSate;
    public GameObject menuCanvas;
}

public class UIController : MonoBehaviour
{
	[SerializeField] private Page[] pages;

	[SerializeField] private MenuState currentMenuState;

	public void ChangeMenuState(MenuState newMenuState)
	{
		DisableAll();
		EnablePage(newMenuState);

		currentMenuState = newMenuState;
	}
	public void DisableAll()
	{
		foreach (var page in pages)
		{
			page.menuCanvas.gameObject.SetActive(false);
		}
	}
	private void EnablePage(MenuState newMenuState)
	{
		foreach (var page in pages)
		{
			if (page.menuSate == newMenuState)
			{
				page.menuCanvas.gameObject.SetActive(true);
			}
		}
	}
}