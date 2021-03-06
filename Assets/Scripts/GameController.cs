using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public List<GameObject> cubeList0, cubeList1, cubeList2, cubeList3, cubeList4, cubeList5, cubeList6;
	public List<List<GameObject>> cubeList = new List<List<GameObject>>();
	public GameObject cubeObject;
	public Text score, best, pauseText;
	public Button pause;
	public Transform next;
	public GameObject none;
	public bool bRunning = false;
	private int nextXIndex = 0;
	private int nextYIndex = 0;
	private Color nextColor;
	private List<Color> colorList = new List<Color>();
	public GameObject optionPlane, gameLabel;
	public Toggle musicTog, audioTog;
	public Slider volume;
	public AudioSource audioSource;
	public AudioClip startClip, destroyClip;
	public float moveSpace = 0;
	public float speedMoveSpace = 0.04f;
	private int nDestroyCount = 0;

	void Start()
	{
		Initialize();
	}

	void Initialize()
	{
		cubeList.Add(cubeList0);
		cubeList.Add(cubeList1);
		cubeList.Add(cubeList2);
		cubeList.Add(cubeList3);
		cubeList.Add(cubeList4);
		cubeList.Add(cubeList5);
		cubeList.Add(cubeList6);
		colorList.Add(Color.red);
		colorList.Add(Color.yellow);
		colorList.Add(Color.green);
		colorList.Add(Color.white);
		colorList.Add(Color.cyan);
		colorList.Add(new Color(0, 0.5f, 1, 1));
		colorList.Add(new Color(0.5f, 0, 1, 1));
		colorList.Add(new Color(1, 1, 0, 1));
		colorList.Add(new Color(1, 0, 1, 1));
		colorList.Add(new Color(1, 0.5f, 0, 1));
		if (PlayerPrefs.GetString("best") != "")
		{
			best.text = PlayerPrefs.GetString("best");
			musicTog.isOn = PlayerPrefs.GetInt("music") == 1;
			audioTog.isOn = PlayerPrefs.GetInt("audio") == 1;
			volume.value = PlayerPrefs.GetFloat("volume");
		}
		if (Screen.width != 500 || Screen.height != 620 || Screen.fullScreen)
			Screen.SetResolution(500, 620, false);
		audioSource.volume = volume.value;
		if (musicTog.isOn)
			audioSource.Play();
	}

	void Update()
	{
		if (bRunning)
			MakeHard();
	}

	void MakeHard()
	{
		if (moveSpace > speedMoveSpace)
			moveSpace -= Time.deltaTime * 0.001f;
	}

	public void CreateCube()
	{
		nDestroyCount = 0;
		GameObject cubeClone = Instantiate<GameObject>(cubeObject);
		cubeClone.transform.SetParent(transform.Find("Cubes"));
		cubeClone.GetComponent<Cube>().xIndex = nextXIndex;
		cubeClone.GetComponent<Cube>().yIndex = nextYIndex;
		cubeClone.GetComponent<Cube>().color = nextColor;
		ShowNext();
	}

	public void ShowNext()
	{
		if (next.childCount > 1)
			DestroyImmediate(next.GetChild(1).gameObject);
		nextXIndex = Random.Range(0, cubeList.Count);
		nextYIndex = Random.Range(0, cubeList [nextXIndex].Count);
		int nIndex = Random.Range(0, colorList.Count);
		nextColor = colorList [nIndex];
		GameObject cubeClone = Instantiate<GameObject>(cubeList [nextXIndex] [nextYIndex]);
		cubeClone.transform.SetParent(next);
		cubeClone.transform.localPosition = Vector2.zero;
		for (int i = 0; i < 4; i++)
			next.GetChild(1).GetChild(i).GetComponent<SpriteRenderer>().color = nextColor;
	}

	public void StartGame()
	{
		foreach (Transform obj in transform.FindChild("Cubes"))
			Destroy(obj.gameObject);
		ShowNext();
		CreateCube();
		bRunning = true;
		pause.interactable = true;
		pauseText.text = "暂停";
		none.SetActive(false);
		score.text = "00000";
		gameLabel.SetActive(false);
		moveSpace = 0.5f;
		nDestroyCount = 0;
		if (audioTog.isOn)
			audioSource.PlayOneShot(startClip);
	}

	public void GamePause()
	{
		bRunning = false;
		pauseText.text = "继续";
	}

	public void GameContinue()
	{
		bRunning = true;
		pauseText.text = "暂停";
	}

	public void GameOver()
	{
		pause.interactable = false;
		pauseText.text = "暂停";
		none.SetActive(true);
		if (int.Parse(score.text) > int.Parse(best.text))
			best.text = score.text;
		score.text = "00000";
		gameLabel.SetActive(true);
		Destroy(next.GetChild(next.childCount - 1).gameObject);
		foreach (Transform obj in transform.FindChild("Cubes"))
			Destroy(obj.gameObject);
	}

	public void PlusScore()
	{
		string text = score.text;
		text = (int.Parse(text) + 10).ToString();
		score.text = string.Format("{0:D5}", int.Parse(text));
		if (audioTog.isOn && nDestroyCount == 0)
		{
			audioSource.PlayOneShot(destroyClip);
			nDestroyCount++;
		}
	}

	void OnDestroy()
	{
		PlayerPrefs.SetString("best", best.text);
		PlayerPrefs.SetInt("music", musicTog.isOn ? 1 : 0);
		PlayerPrefs.SetInt("audio", audioTog.isOn ? 1 : 0);
		PlayerPrefs.SetFloat("volume", volume.value);
	}
}
