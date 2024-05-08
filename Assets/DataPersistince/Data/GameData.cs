using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<int> cardIndexRemoved = new List<int>();
    public List<Sprite> playableCards = new List<Sprite>();
    public float gameTime;
    public bool gameIsFinised;

    public int rows;
    public int columns;

    public int countGuesses, countCorrectGuesses;

    public GameData()
    {
        countGuesses = countCorrectGuesses = 0;
    }
}
