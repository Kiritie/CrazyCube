using UnityEngine;

public class CubeChild : MonoBehaviour
{
	public bool bCanRemove = false;
	private GameController game;
	
	void Start()
	{
		game = GameObject.FindWithTag("GameController").GetComponent<GameController>();
	}

	void Update()
	{
	    if (game.bRunning)
		{
			if (bCanRemove)
				PlayRemoveAnimation();
		}
	}

	void PlayRemoveAnimation()
	{
		Color color = GetComponent<SpriteRenderer>().color;
		color.a -= Time.deltaTime * 2.0f;
		GetComponent<SpriteRenderer>().color = color;
	}
}
