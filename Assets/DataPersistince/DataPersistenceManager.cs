using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DataPersistenceManager : MonoBehaviour
{
	[Header("File Storage Settings")]
	[SerializeField] private string fileName;

	[SerializeField] private GameObject continueBtn;
	private GameData gameData;
	private List<IDataPersistence> dataPersistencesObjects;
	private FileDataHandler dataHandler;

	public static DataPersistenceManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one DataPersistenceManager in the scene");
		}
		Instance = this;

		this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
		this.dataPersistencesObjects = FindAllDataPersistenceObjects();
		LoadGame();
	}
	public void NewGame()
	{
		this.gameData = new GameData();
	}

	public void LoadGame()
	{
		// Load the data from the file
		this.gameData = dataHandler.Load();

		if (this.gameData == null)
		{
			Debug.Log("No data was found");
			continueBtn.SetActive(false);
			NewGame();
		}

		foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
		{
			dataPersistenceObj.LoadData(gameData);
		}

	}

	public void SaveGame()
	{
		foreach (IDataPersistence dataPersistenceObj in dataPersistencesObjects)
		{
			dataPersistenceObj.SaveData(ref gameData);
		}


		dataHandler.Save(gameData);
	}

	private void OnApplicationQuit()
	{
		SaveGame();
	}

	private List<IDataPersistence> FindAllDataPersistenceObjects()
	{
		IEnumerable<IDataPersistence> dataPersistencesObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().OfType<IDataPersistence>();
		return new List<IDataPersistence>(dataPersistencesObjects);
	}
}
