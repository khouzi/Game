using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
	private string dataDirPath = "";
	private string dataFileName = "";

	public FileDataHandler(string dataDirPath, string dataFileName)
	{
		this.dataDirPath = dataDirPath;
		this.dataFileName = dataFileName;
	}

	public GameData Load()
	{
		string fullPath = Path.Combine(dataDirPath, dataFileName);
		GameData loadedData = null;
		if (File.Exists(fullPath))
		{
			try
			{
				// Load the data from the file
				string dataToLoad = "";
				using (FileStream stream = new FileStream(fullPath, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(stream))
						dataToLoad = reader.ReadToEnd();
				}


				// deserialize the data
				loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
			}
			catch (Exception e)
			{
				Debug.LogError("Error loading file: " + fullPath + "\n" + e.Message);
			}
		}
		return loadedData;

	}

	public void Save(GameData data)
	{
		string fullPath = Path.Combine(dataDirPath, dataFileName);
		try
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

			string dataToStore = JsonUtility.ToJson(data, true);


			using (FileStream stream = new FileStream(fullPath, FileMode.Create))
			{
				using (StreamWriter writer = new StreamWriter(stream))
					writer.Write(dataToStore);
			}
		}
		catch (Exception e)
		{
			Debug.LogError("Error saving file: " + fullPath + "\n" + e.Message);
		}

	}
}
