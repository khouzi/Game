using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Transform panel;

    [SerializeField] private GameObject card;
    [SerializeField] private int grid;


    void Awake()
    {
        for (int i = 0; i < grid; i++)
        {
            GameObject button = Instantiate(card);
            button.name = "" + i;
            button.transform.SetParent(panel, false);
        }
    }
}
