using UnityEngine;

public class GameUI : MonoBehaviour
{
	private GameController game;

	void Start()
	{
		game = GameObject.FindWithTag("GameController").GetComponent<GameController>();
	}

	public void ClickStart()
	{
		game.StartGame();
	}

	public void ClickPause()
	{
		if (game.pauseText.text == "暂停")
			game.GamePause();
		else
			game.GameContinue();
	}

	public void ClickOption()
	{
		game.optionPlane.SetActive(true);
	}

	public void ClickExit()
	{
		Application.Quit();
	}

	public void ClickBack()
	{
		game.optionPlane.SetActive(false);
	}

	public void ClickClear()
	{
		game.best.text = "00000";
	}

	public void ChangedMusicTog()
	{
		if (game != null)
		{
			if (game.musicTog.isOn)
				game.audioSource.Play();
			else
				game.audioSource.Stop();
		}
	}

	public void ChangedVolume()
	{
		if (game != null)
			game.audioSource.volume = game.volume.value;
	}
}
