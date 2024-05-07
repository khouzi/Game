using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string cardTag = "Card";
    private string path = "Images/FrontCard";

    [SerializeField] private Sprite backCard;

    private Sprite[] frontCards;
    public List<Sprite> playableCards = new List<Sprite>();

    public List<Button> flippedCard = new List<Button>();

    private bool firstGuess, secondGuess;

    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessCard, secondGuessCard;




    void Awake()
    {
        frontCards = Resources.LoadAll<Sprite>(path);
    }


    void Start()
    {
        GetCards();
        AddCards();
        Shuffle(playableCards);
        gameGuesses = playableCards.Count / 2;
        AddListeners();
    }

    void GetCards()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(cardTag);
        for (int i = 0; i < objects.Length; i++)
        {
            flippedCard.Add(objects[i].GetComponent<Button>());
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

        // If the clicked button is already the first guess, return
        if (clickedIndex == firstGuessIndex)
            return;

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = clickedIndex;
            firstGuessCard = playableCards[firstGuessIndex].name;
            flippedCard[firstGuessIndex].image.sprite = playableCards[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = clickedIndex;
            secondGuessCard = playableCards[secondGuessIndex].name;
            flippedCard[secondGuessIndex].image.sprite = playableCards[secondGuessIndex];
            countGuesses++;
            StartCoroutine(CheckCards());
            firstGuess = secondGuess = false;
        }
    }

    IEnumerator CheckCards()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessCard == secondGuessCard)
        {
            flippedCard[firstGuessIndex].interactable = false;
            flippedCard[secondGuessIndex].interactable = false;

            flippedCard[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            flippedCard[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckEndgame();
        }
        else
        {
            flippedCard[firstGuessIndex].image.sprite = backCard;
            flippedCard[secondGuessIndex].image.sprite = backCard;
        }

    }

    void CheckEndgame()
    {
        countCorrectGuesses++;

        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("Game Finished");
            Debug.Log("It took you" + countGuesses + "to end");
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