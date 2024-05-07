using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public List<int> cardIndexRemoved = new List<int>();
    public List<Sprite> playableCards = new List<Sprite>();

    public int countGuesses, countCorrectGuesses;

    public GameData()
    {
        countGuesses = countCorrectGuesses = 0;
    }
}
