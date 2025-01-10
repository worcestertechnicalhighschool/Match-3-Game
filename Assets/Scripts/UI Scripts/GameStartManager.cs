using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager : MonoBehaviour
{
    // Reference to the UI panel that is shown at the start of the game
    public GameObject startPanel;

    // Reference to the UI panel that is shown when the level selection is active
    public GameObject levelPanel;

    // Start is called before the first frame update
    void Start()
    {
        // Set the startPanel to active and the levelPanel to inactive when the game starts
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }

    // Method to switch to the level selection screen when the "Play Game" button is clicked
    public void PlayGame()
    {
        // Deactivate the startPanel and activate the levelPanel when the player is ready to play
        startPanel.SetActive(false);
        levelPanel.SetActive(true);
    }

    // Method to return to the start screen when the "Home" button is clicked
    public void Home()
    {
        // Reactivate the startPanel and deactivate the levelPanel to return to the home screen
        startPanel.SetActive(true);
        levelPanel.SetActive(false);
    }
}