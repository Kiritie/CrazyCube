using UnityEngine;

public class Cube : MonoBehaviour
{
	private GameController game;
	public int xIndex = 0;
	public int yIndex = 0;
	private const float MOVESIZE = 0.3f;
	private float downTimeSize = 0;
	private float lastDownTime = 0;
	public Color color;
	private bool canMoveDown = true;
	private bool canMoveLeft = true;
	private bool canMoveRight = true;
	private bool canModifyModal = true;
	private bool canRemove = false;
	private bool hasMoveDone = false;

	void Start()
	{
		Initialize();
		ModifyModal();
		ModifyColor();
	}

	void Initialize()
	{
		game = GameObject.FindWithTag("GameController").GetComponent<GameController>();
		lastDownTime = Time.time;
		transform.position = new Vector2(1.2f, 0);
		downTimeSize = game.moveSpace;
	}

	void Update()
	{		
		if (game.bRunning)
		{
			MoveDoneCheck();
			Control();
		}
	}

	void NextModal()
	{
		yIndex++;
		if (yIndex >= game.cubeList [xIndex].Count)
			yIndex = 0;
		ModifyModal();
		ModifyColor();
	}

	void ModifyModal()
	{
		if (transform.childCount > 0)
			Destroy(transform.GetChild(0).gameObject);
		GameObject cubeClone = Instantiate<GameObject>(game.cubeList [xIndex] [yIndex]);
		cubeClone.transform.SetParent(transform);
		cubeClone.transform.localPosition = Vector2.zero;
	}

	void ModifyColor()
	{
		for (int a = 0; a < transform.childCount; a++)
		{
			for (int b = 0; b < 4; b++)
			{
				transform.GetChild(a).GetChild(b).GetComponent<SpriteRenderer>().color = color;
			}
		}
	}

	void Control()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow) && canMoveDown)
		{
			downTimeSize = game.speedMoveSpace;
		}
		if (Input.GetKeyUp(KeyCode.DownArrow) && canMoveDown)
		{
			downTimeSize = game.moveSpace;
			lastDownTime = Time.time;
		}
		if (Time.time - lastDownTime >= downTimeSize && !hasMoveDone)
		{
			if (downTimeSize > game.speedMoveSpace)
				downTimeSize = game.moveSpace;
			lastDownTime = Time.time;
			CheckDownCollision();
			if (canMoveDown)
				transform.Translate(Vector2.down * MOVESIZE);
			CheckDownCollision();
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) && !canRemove && !hasMoveDone)
		{
			CheckModifyCollision();
			if (canModifyModal)
				NextModal();
			if (!canMoveDown)
			{
				CheckDownCollision();
				if (canMoveDown)
					lastDownTime = Time.time;
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) && !canRemove && !hasMoveDone)
		{
			CheckLeftCollision();
			if (canMoveLeft)
				transform.Translate(Vector2.left * MOVESIZE);
			if (!canMoveDown)
			{
				CheckDownCollision();
				if (canMoveDown)
					lastDownTime = Time.time;
			}
			CheckLeftCollision();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow) && !canRemove && !hasMoveDone)
		{
			CheckRightCollision();
			if (canMoveRight)
				transform.Translate(Vector2.right * MOVESIZE);
			if (!canMoveDown)
			{
				CheckDownCollision();
				if (canMoveDown)
					lastDownTime = Time.time;
			}
			CheckRightCollision();
		}
	}

	void CheckLeftCollision()
	{
		for (int i = 0; i < 4; i++)
		{
			Vector2 childPos = transform.GetChild(0).GetChild(i).position;
			RaycastHit2D hit;
			hit = Physics2D.Raycast(childPos, Vector2.left, 10.0f);
			if (hit.collider != null)
			{
				if (hit.transform.tag == "BackGround")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						canMoveLeft = false;
						break;
					} else
						canMoveLeft = true;
				}
				if (hit.transform.tag == "Cube")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						canMoveLeft = false;
						break;
					} else
						canMoveLeft = true;
				}
			}
		}
	}

	void CheckRightCollision()
	{
		for (int i = 0; i < 4; i++)
		{
			Vector2 childPos = transform.GetChild(0).GetChild(i).position;
			RaycastHit2D hit;
			hit = Physics2D.Raycast(childPos, Vector2.right, 10.0f);
			if (hit.collider != null)
			{
				if (hit.transform.tag == "BackGround")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						canMoveRight = false;
						break;
					} else
						canMoveRight = true;
				}
				if (hit.transform.tag == "Cube")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						canMoveRight = false;
						break;
					} else
						canMoveRight = true;
				}
			}
		}
	}

	void CheckDownCollision()
	{
		for (int i = 0; i < 4; i++)
		{
			Vector2 childPos = transform.GetChild(transform.childCount - 1).GetChild(i).position;
			RaycastHit2D hit;
			hit = Physics2D.Raycast(childPos, Vector2.down, 10.0f);
			if (hit.collider != null)
			{
				if (hit.transform.tag == "BackGround")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						downTimeSize = game.moveSpace;
						canMoveDown = false;
						PlayRemoveAnimation();
						break;
					} else
						canMoveDown = true;
				}
				if (hit.transform.tag == "Cube")
				{
					if (Vector2.Distance(childPos, hit.point) < MOVESIZE)
					{
						downTimeSize = game.moveSpace;
						canMoveDown = false;
						PlayRemoveAnimation();
						break;
					} else
						canMoveDown = true;
				}
			}
		}
	}

	void CheckModifyCollision()
	{
		int nNextType = yIndex + 1;
		if (nNextType >= game.cubeList [xIndex].Count)
			nNextType = 0;
		GameObject cubeClone = Instantiate<GameObject>(game.cubeList [xIndex] [nNextType]);
		cubeClone.transform.SetParent(transform);
		cubeClone.transform.localPosition = Vector2.zero;
		for (int i = 0; i < 4; i++)
		{
			Vector2 childPos = transform.GetChild(1).GetChild(i).position;
			RaycastHit2D hit;
			hit = Physics2D.Raycast(childPos, Vector2.down, 10.0f);
			if (hit.collider != null)
			{
				if (hit.transform.tag == "BackGround")
				{
					if (Vector2.Distance(childPos, hit.point) == 0)
					{
						canModifyModal = false;
						break;
					} else
						canModifyModal = true;
				}
				if (hit.transform.tag == "Cube")
				{
					if (Vector2.Distance(childPos, hit.point) == 0)
					{
						canModifyModal = false;
						break;
					} else
						canModifyModal = true;
				}
			}
		}
		Destroy(cubeClone);
	}

	void MoveDoneCheck()
	{
		if (!canMoveDown && Time.time - lastDownTime >= downTimeSize)
		{
			hasMoveDone = true;
			if (GetComponent<Cube>() != null)
				Destroy(GetComponent<Cube>());
			for (int i = 0; i < 4; i++)
				transform.GetChild(transform.childCount - 1).GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
			RemoveCheck();
			if (transform.position.y == 0)
				game.GameOver();
			else
				game.CreateCube();
		}
	}

	void PlayRemoveAnimation()
	{
		for (int i = 0; i < 4; i++)
			transform.GetChild(transform.childCount - 1).GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
		for (int i = 0; i < 20; i++)
		{
			RaycastHit2D[] cubes;
			cubes = Physics2D.LinecastAll(new Vector2(0, -(MOVESIZE * i)), new Vector2(MOVESIZE * 9, -(MOVESIZE * i)));
			if (cubes.Length >= 10)
			{
				canRemove = true;
				foreach (RaycastHit2D hit in cubes)
					hit.transform.GetComponent<CubeChild>().bCanRemove = true;
			}
		}
		for (int i = 0; i < 4 && !hasMoveDone; i++)
			transform.GetChild(transform.childCount - 1).GetChild(i).GetComponent<BoxCollider2D>().enabled = false;
	}

	void RemoveCheck()
	{
		for (int i = 0; i < 20; i++)
		{
			RaycastHit2D[] cubes;
			cubes = Physics2D.LinecastAll(new Vector2(0, -(MOVESIZE * i)), new Vector2(MOVESIZE * 9, -(MOVESIZE * i)));
			if (cubes.Length >= 10)
			{
				game.PlusScore();
				MoveToSolid(i - 1);
				foreach (RaycastHit2D hit in cubes)
					Destroy(hit.transform.gameObject);
			}
		}
	}

	void MoveToSolid(int nIndex)
	{
		for (int i = nIndex; i >= 0; i--)
		{
			RaycastHit2D[] cubes;
			cubes = Physics2D.LinecastAll(new Vector2(0, -(MOVESIZE * i)), new Vector2(MOVESIZE * 9, -(MOVESIZE * i)));
			if (cubes.Length > 0)
			{
				foreach (RaycastHit2D hit in cubes)
					hit.transform.Translate(Vector2.down * MOVESIZE);
			}
		}
	}
}