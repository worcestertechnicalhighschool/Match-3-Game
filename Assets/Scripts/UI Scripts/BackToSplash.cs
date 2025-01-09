using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load when the OK button is pressed (can be a splash screen or another scene)
    private GameData gameData; // Reference to the GameData script to access and save game data
    private Board board; // Reference to the Board script to get the current level

    // Method to handle the OK button click, which is triggered after a win
    public void WinOK()
    {
        // Check if gameData exists before attempting to save progress
        if (gameData != null)
        {
            // Mark the next level as unlocked in the game's saved data
            gameData.saveData.isActive[board.level + 1] = true;

            // Save the updated game data (unlock next level)
            gameData.Save();
        }

        // Load the scene specified in the sceneToLoad variable (typically a splash screen or another UI screen)
        SceneManager.LoadScene(sceneToLoad);
    }

    // Method to handle the OK button click after a loss (typically loading a retry or game over screen)
    public void LoseOK()
    {
        // Load the scene specified in the sceneToLoad variable (e.g., game over screen, retry option)
        SceneManager.LoadScene(sceneToLoad);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Get the GameData object in the scene to access the saved game data
        gameData = FindObjectOfType<GameData>();

        // Get the Board object in the scene to access the current level
        board = FindObjectOfType<Board>();
    }
}