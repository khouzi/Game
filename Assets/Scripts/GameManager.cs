using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string cardTag = "Card";
    private string path = "Images/FrontCard";

    [SerializeField] private Sprite backCard;
    [SerializeField] private float flipDuration = 0.3f;

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

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = clickedIndex;
            firstGuessCard = playableCards[firstGuessIndex].name;

            StartCoroutine(FlipCard(flippedCard[firstGuessIndex], playableCards[firstGuessIndex]));
        }
        else if (!secondGuess && clickedIndex != firstGuessIndex)
        {
            secondGuess = true;
            secondGuessIndex = clickedIndex;
            secondGuessCard = playableCards[secondGuessIndex].name;

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
                CheckEndgame();
            }
            else
            {
                StartCoroutine(FlipCard(flippedCard[firstGuessIndex], backCard));
                StartCoroutine(FlipCard(flippedCard[secondGuessIndex], backCard));
            }

            firstGuess = secondGuess = false;
        }
    }

    void CheckEndgame()
    {
        countCorrectGuesses++;

        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("Game Finished");
            Debug.Log("It took you " + countGuesses + " to end");
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
