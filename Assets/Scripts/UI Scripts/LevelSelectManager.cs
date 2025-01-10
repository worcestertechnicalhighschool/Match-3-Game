using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour
{
    // Array of panels representing different pages in the level selection UI
    public GameObject[] panels;

    // The currently active panel for the level selection UI
    public GameObject currentPanel;

    // The current page of levels being viewed
    public int page;

    // Reference to the GameData, which stores saved game data
    private GameData gameData;

    // The level that the player is currently on
    public int currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Find the GameData object in the scene
        gameData = FindObjectOfType<GameData>();

        // Deactivate all panels initially
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }

        // Check if gameData is found and set currentLevel based on saved data
        if (gameData != null)
        {
            for (int i = 0; i < gameData.saveData.isActive.Length; i++)
            {
                // If a level is marked as active, set it as the current level
                if (gameData.saveData.isActive[i])
                {
                    currentLevel = i;
                }
            }
        }

        // Determine the page number based on the current level
        page = (int)Mathf.Floor(currentLevel / 27f);

        // Set the initial active panel to the appropriate page
        currentPanel = panels[page];
        panels[page].SetActive(true);
    }

    // Method to move to the next page (right)
    public void PageRight()
    {
        // Check if we are not already on the last page
        if (page < panels.Length - 1)
        {
            // Deactivate the current panel and move to the next one
            currentPanel.SetActive(false);
            page++;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }

    // Method to move to the previous page (left)
    public void PageLeft()
    {
        // Check if we are not already on the first page
        if (page > 0)
        {
            // Deactivate the current panel and move to the previous one
            currentPanel.SetActive(false);
            page--;
            currentPanel = panels[page];
            currentPanel.SetActive(true);
        }
    }
}