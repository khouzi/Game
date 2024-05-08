using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour, IDataPersistence
{
	// Variables related to card management
	private string cardTag = "Card";
	private string path = "Images/FrontCard";
	private Sprite[] frontCards;
	public List<Sprite> playableCards = new List<Sprite>();
	public List<Button> flippedCard = new List<Button>();
	public List<int> indexRemoved = new List<int>();
	[SerializeField] private GameObject continueBtn, Homebtn;

	// UI related variables
	[Header("UI")]
	[SerializeField] private TextMeshProUGUI Turns;
	[SerializeField] private TextMeshProUGUI timer;
	[SerializeField] private TextMeshProUGUI comboUI;
	[SerializeField] private TextMeshProUGUI scoreBoardUI;

	[SerializeField] private TextMeshProUGUI highestcomboUI;
	[SerializeField] private GameObject menu;
	[SerializeField] private GameObject game;
	[SerializeField] private GameObject sidePanel;
	[SerializeField] private GameObject scoreBoard;


	// Audio related variables
	[Header("SFX")]
	[SerializeField] private AudioClip cardFlip;
	[SerializeField] private AudioClip cardMatch;
	[SerializeField] private AudioClip cardMismatch;
	[SerializeField] private AudioClip gameOver;
	[SerializeField] private AudioSource audioSource;

	// Game state variables
	private int comboCount;
	private int highestCombo;
	private float gameTime;
	private bool gameEnded = false;
	private int countGuesses;
	private int countCorrectGuesses;
	private int gameGuesses;
	private int firstGuessIndex, secondGuessIndex;
	private string firstGuessCard, secondGuessCard;
	private bool firstGuess, secondGuess;

	// Animation related variables
	[Header("Animation")]
	[SerializeField] private float delayBeforeFlip = 2;
	[SerializeField] private float flipDuration = 0.3f;
	[SerializeField] private bool isNewGame;
	[SerializeField] private Sprite backCard;


	// IDataPersistence methods
	public void LoadData(GameData gameData)
	{
		countCorrectGuesses = gameData.countCorrectGuesses;
		countGuesses = gameData.countGuesses;
		indexRemoved = new List<int>(gameData.cardIndexRemoved);
		playableCards = new List<Sprite>(gameData.playableCards);
		gameTime = gameData.gameTime;
		gameEnded = gameData.gameIsFinised;
	}

	public void SaveData(ref GameData gameData)
	{
		gameData.countCorrectGuesses = countCorrectGuesses;
		gameData.countGuesses = countGuesses;
		gameData.cardIndexRemoved = new List<int>(indexRemoved);
		gameData.playableCards = new List<Sprite>(playableCards);
		gameData.gameTime = gameTime;
		gameData.gameIsFinised = gameEnded;
	}

	void Awake()
	{
		Debug.Log(gameEnded);
		frontCards = Resources.LoadAll<Sprite>(path);

	}

	void Start()
	{
		if (gameEnded)
			continueBtn.SetActive(false);
	}

	public void Startbtn()
	{
		indexRemoved.Clear();
		playableCards.Clear();
		countCorrectGuesses = countGuesses = 0;
		gameTime = 0f;
		isNewGame = true;
		game.SetActive(true);
		menu.SetActive(false);
		Homebtn.SetActive(false);
		GameStart();
	}

	public void ContinueBtn()
	{
		game.SetActive(true);
		menu.SetActive(false);
		Homebtn.SetActive(false);
		isNewGame = false;
		GameStart();
	}

	//Main game logic
	void GameStart()
	{
		GetCards();

		if (isNewGame)
		{
			AddCards();
			Shuffle(playableCards);
			RevealCard();
			isNewGame = false;
		}

		UpdateUI();

		gameEnded = false;
		StartCoroutine(UpdateTimer());

		gameGuesses = playableCards.Count / 2;
		AddListeners();
	}

	void RevealCard()
	{
		for (int i = 0; i < flippedCard.Count; i++)
		{
			flippedCard[i].image.sprite = playableCards[i];
			StartCoroutine(DelayBeforeFlippingCard());
		}
	}

	
	void UpdateUI()
	{
		Turns.text = "Turns " + countGuesses;
		comboUI.text = "Combo: " + comboCount;
		highestcomboUI.text = "Highest Combo: " + highestCombo;
	}

	//get all card from scene
	void GetCards()
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag(cardTag);
		for (int i = 0; i < objects.Length; i++)
		{
			flippedCard.Add(objects[i].GetComponent<Button>());

			if (indexRemoved.Contains(i))
			{
				flippedCard[i].image.color = new Color(0, 0, 0, 0);
				flippedCard[i].interactable = false;
			}
			else
				flippedCard[i].image.sprite = backCard;
		}
	}

	// Add playable card
	void AddCards()
	{
		int cardCount = flippedCard.Count;
		int index = 0;

		for (int i = 0; i < cardCount; i++)
		{
			if (index == cardCount / 2)
				index = 0;

			playableCards.Add(frontCards[index]);
			index++;
		}
	}

	void AddListeners()
	{
		foreach (Button card in flippedCard)
		{
			card.onClick.AddListener(() => CardPicked());
		}
	}

	public void CardPicked()
	{
		string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;

		int clickedIndex = int.Parse(name);

		if (!firstGuess)
		{
			firstGuess = true;
			firstGuessIndex = clickedIndex;
			firstGuessCard = playableCards[firstGuessIndex].name;

			audioSource.PlayOneShot(cardFlip);
			StartCoroutine(FlipCard(flippedCard[firstGuessIndex], playableCards[firstGuessIndex]));
		}
		else if (!secondGuess && clickedIndex != firstGuessIndex)
		{
			secondGuess = true;
			secondGuessIndex = clickedIndex;
			secondGuessCard = playableCards[secondGuessIndex].name;

			audioSource.PlayOneShot(cardFlip);
			StartCoroutine(FlipCard(flippedCard[secondGuessIndex], playableCards[secondGuessIndex]));

			countGuesses++;

			StartCoroutine(CheckCards());
		}
	}

	//Check Card matches
	IEnumerator CheckCards()
	{
		yield return new WaitForSeconds(0.5f);

		if (firstGuess && secondGuess)
		{
			if (firstGuessCard == secondGuessCard && firstGuessIndex != secondGuessIndex)
			{
				flippedCard[firstGuessIndex].interactable = false;
				flippedCard[secondGuessIndex].interactable = false;
				flippedCard[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
				flippedCard[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

				indexRemoved.Add(firstGuessIndex);
				indexRemoved.Add(secondGuessIndex);

				audioSource.PlayOneShot(cardMatch);

				comboCount++;

				if (comboCount > highestCombo)
					highestCombo = comboCount;

				CheckEndgame();
			}
			else
			{
				audioSource.PlayOneShot(cardMismatch);

				comboCount = 1;

				StartCoroutine(FlipCard(flippedCard[firstGuessIndex], backCard));
				StartCoroutine(FlipCard(flippedCard[secondGuessIndex], backCard));
			}

			firstGuess = secondGuess = false;

			UpdateUI();
		}
	}

	void CheckEndgame()
	{
		countCorrectGuesses++;

		if (countCorrectGuesses == gameGuesses)
		{
			audioSource.PlayOneShot(gameOver);

			UpdateUI();

			Homebtn.SetActive(true);

			gameEnded = true;

			scoreBoard.SetActive(true);
			sidePanel.SetActive(false);
			scoreBoardUI.text = "It took you <color=red>" + countGuesses + " turns</color> to finish";
			StartCoroutine(ScaleScoreBoard());
		}
	}

	IEnumerator UpdateTimer()
	{
		while (!gameEnded)
		{
			gameTime += Time.deltaTime;

			int minutes = Mathf.FloorToInt(gameTime / 60f);
			int seconds = Mathf.FloorToInt(gameTime % 60f);

			timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);

			yield return null;
		}
	}


	IEnumerator DelayBeforeFlippingCard()
	{
		foreach (Button card in flippedCard)
			card.interactable = false;

		yield return new WaitForSeconds(delayBeforeFlip);

		foreach (Button card in flippedCard)
		{
			card.interactable = true;
			StartCoroutine(FlipCard(card, backCard));
		}
	}

	IEnumerator ScaleScoreBoard()
{
    float elapsedTime = 0f;
    Vector3 initialScale = scoreBoard.transform.localScale;
    Vector3 targetScale = Vector3.one;

    while (elapsedTime < 1f)
    {
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / 1f);
        scoreBoard.transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
        yield return null;
    }
}

	IEnumerator FlipCard(Button card, Sprite targetSprite)
	{
		float elapsedTime = 0f;
		Quaternion startRotation = card.transform.rotation;
		Quaternion endRotation = Quaternion.Euler(0f, 90f, 0f);

		while (elapsedTime < flipDuration)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / flipDuration);
			card.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
			yield return null;
		}

		card.image.sprite = targetSprite;

		elapsedTime = 0f;
		startRotation = card.transform.rotation;
		endRotation = Quaternion.Euler(0f, 0f, 0f);

		while (elapsedTime < flipDuration)
		{
			elapsedTime += Time.deltaTime;
			float t = Mathf.Clamp01(elapsedTime / flipDuration);
			card.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
			yield return null;
		}
	}

	//Shuffle playable cards
	void Shuffle(List<Sprite> List)
	{
		for (int i = 0; i < List.Count; i++)
		{
			Sprite temp = List[i];
			int randomIndex = Random.Range(i, List.Count);
			List[i] = List[randomIndex];
			List[randomIndex] = temp;
		}
	}
}
