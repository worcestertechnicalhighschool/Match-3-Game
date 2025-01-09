using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// The SaveData class is marked as Serializable so it can be serialized for saving/loading purposes.
[Serializable]
public class SaveData
{
    public bool[] isActive;    // Tracks the active state of various game elements (e.g., levels, features).
    public int[] highScores;   // Stores the high scores for the game.
    public int[] stars;        // Stores the number of stars collected by the player in various levels.
}

public class GameData : MonoBehaviour
{
    // A static reference to this class, allowing access to the GameData instance globally.
    public static GameData gameData;
    // The actual data that will be saved/loaded, stored as a SaveData object.
    public SaveData saveData;

    // Called when the script is first loaded or an instance is created
    void Awake()
    {
        // If there's no existing instance of GameData, set this object as the singleton.
        if (gameData == null)
        {
            DontDestroyOnLoad(this.gameObject); // Prevent this object from being destroyed on scene load.
            gameData = this;  // Set the static reference to this instance.
        }
        else
        {
            // If an instance already exists, destroy the new object to maintain a singleton.
            Destroy(this.gameObject);
        }
        Load();  // Load the saved data if any.
    }

    // Save function to serialize the saveData object to a file.
    public void Save()
    {
        // Create a BinaryFormatter to serialize the object into a binary format.
        BinaryFormatter formatter = new BinaryFormatter();
        // Open a file stream to store the saved data in the persistent data path (for cross-platform support).
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create);
        // Create a new SaveData object and assign the current saveData to it.
        SaveData data = new SaveData();
        data = saveData;
        // Serialize the saveData into the file.
        formatter.Serialize(file, data);
        // Close the file stream.
        file.Close();
        // Log a message indicating the data has been saved.
        Debug.Log("Saved! :D");
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    // OnDisable is automatically called when the object is about to be destroyed.
    private void OnDisable()
    {
        Save();  // Save the data when the object is disabled.
    }

    // Load function to deserialize the saved data from the file.
    public void Load()
    {
        // Check if the save file exists before attempting to load.
        if (File.Exists(Application.persistentDataPath + "/player.dat"))
        {
            // Create a BinaryFormatter to deserialize the object from a binary format.
            BinaryFormatter formatter = new BinaryFormatter();
            // Open the saved file stream for reading.
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open);
            // Deserialize the file content back into the saveData object.
            saveData = formatter.Deserialize(file) as SaveData;
            // Close the file stream.
            file.Close();
            // Log a message indicating the data has been loaded.
            Debug.Log("Loaded :D");
        }
    }
}