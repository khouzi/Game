using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour, IDataPersistence
{
	private string cardTag = "Card";
	private string path = "Images/FrontCard";

	[SerializeField] private TextMeshProUGUI Turns, timer, comboUI, highestcomboUI;

	[SerializeField] private GameObject menu, game;

	private int comboCount;
	private int highestCombo;

	private float gameTime;
	private bool gameEnded;


	[SerializeField] private AudioClip cardFlip, cardMatch, cardMismatch, gameOver;
	[SerializeField] private AudioSource audioSource;

	[SerializeField] private Sprite backCard;
	[SerializeField] private float flipDuration = 0.3f;

	[SerializeField] private bool isNewGame;

	private Sprite[] frontCards;
	public List<Sprite> playableCards = new List<Sprite>();

	public List<Button> flippedCard = new List<Button>();
	public List<int> indexRemoved = new List<int>();


	private bool firstGuess, secondGuess;


	private int countGuesses;
	private int countCorrectGuesses;
	private int gameGuesses;

	private int firstGuessIndex, secondGuessIndex;

	private string firstGuessCard, secondGuessCard;

	public void LoadData(GameData gameData)
	{
		countCorrectGuesses = gameData.countCorrectGuesses;
		countGuesses = gameData.countGuesses;
		indexRemoved = new List<int>(gameData.cardIndexRemoved);
		playableCards = new List<Sprite>(gameData.playableCards);
		gameTime = gameData.gameTime;
	}

	public void SaveData(ref GameData gameData)
	{
		gameData.countCorrectGuesses = countCorrectGuesses;
		gameData.countGuesses = countGuesses;
		gameData.cardIndexRemoved = new List<int>(indexRemoved);
		gameData.playableCards = new List<Sprite>(playableCards);
		gameData.gameTime = gameTime;

	}

	void Awake()
	{
		frontCards = Resources.LoadAll<Sprite>(path);
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
		GameStart();
	}

	public void ContinueBtn()
	{
		game.SetActive(true);
		menu.SetActive(false);
		isNewGame = false;
		GameStart();
	}

	void GameStart()
	{
		GetCards();

		if (isNewGame)
		{
			AddCards();
			Shuffle(playableCards);
			isNewGame = false;
		}


		UpdateUI();

		gameEnded = false;
		StartCoroutine(UpdateTimer());

		gameGuesses = playableCards.Count / 2;
		AddListeners();
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

	void UpdateUI()
	{
		Turns.text = "Turns " + countGuesses;

		comboUI.text = "Combo: " + comboCount;
		highestcomboUI.text = "Highest Combo: " + highestCombo;
	}

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


			Debug.Log("Game Finished");
			Debug.Log("It took you " + countGuesses + " to end");

			UpdateUI();

			gameEnded = true;
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