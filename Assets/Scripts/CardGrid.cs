using UnityEngine;
using TMPro;

public class CardGrid : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform panel;
    [SerializeField] private GameObject card;
    [SerializeField] private TMP_InputField rowsInputField;
    [SerializeField] private TMP_InputField columnsInputField;
    public int rowsValue, columnsValue;

    private int grid;

    public void LoadData(GameData data)
    {
        rowsValue = data.rows;
        columnsValue = data.columns;
    }

    public void SaveData(ref GameData data)
    {
        data.rows = rowsValue;
        data.columns = columnsValue;
    }

    public void CreateGrid()
    {
        if (!int.TryParse(rowsInputField.text, out rowsValue))
            rowsValue = 2;

        if (!int.TryParse(columnsInputField.text, out columnsValue))
            columnsValue = 2;

        CreateOrUpdateGrid();
    }

    public void ContineFomGrid()
    {
        CreateOrUpdateGrid();
    }

    private void CreateOrUpdateGrid()
    {
        grid = rowsValue * columnsValue;

        // Remove excess buttons
        while (panel.childCount > grid)
        {
            Destroy(panel.GetChild(0).gameObject);
        }

        // Add or update buttons
        for (int i = 0; i < grid; i++)
        {
            GameObject button;
            if (i < panel.childCount)
            {
                button = panel.GetChild(i).gameObject;
            }
            else
            {
                button = Instantiate(card);
                button.transform.SetParent(panel, false);
            }
            button.name = i.ToString();
        }
    }
}
