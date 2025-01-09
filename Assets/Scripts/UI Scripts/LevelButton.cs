using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    // Header to define active state and sprite settings
    [Header("Active Stuff")]
    public bool isActive;              // Determines if the button is active (clickable)
    public Sprite activeSprite;        // Sprite for the active (clickable) button
    public Sprite lockedSprite;        // Sprite for the locked (non-clickable) button
    private Image buttonImage;         // Reference to the button's Image component (for changing sprite)
    private Button myButton;           // Reference to the Button component (for enabling/disabling)
    private int starsActive;

    // Header to define UI elements
    [Header("UI Stuff")]
    public Image[] stars;              // Array of star images for level completion stars
    public Text levelText;             // Text field for displaying the level number
    public int level;                  // The level number represented by this button
    public GameObject confirmPanel;    // Reference to a confirmation panel (shown when a level is selected)

    private GameData gameData;

    // Start is called before the first frame update
    void Start()
    {
        // Get references to components on this GameObject
        gameData = FindObjectOfType<GameData>();
        buttonImage = GetComponent<Image>();
        myButton = GetComponent<Button>();
        
        // Call methods to initialize the button state and UI elements
        LoadData();
        ActivateStars();
        ShowLevel();
        DecideSprite();
    }

    // Deactivates all stars initially, to be activated based on the level progress
    void ActivateStars()
    {
        // Loop through all the stars and disable them
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    // Decides which sprite should be shown based on the 'isActive' status
    void DecideSprite()
    {
        // If the level button is active (clickable)
        if (isActive)
        {
            buttonImage.sprite = activeSprite;  // Set the active button sprite
            myButton.enabled = true;            // Enable the button interaction
            levelText.enabled = true;           // Make the level text visible
        }
        // If the level button is locked (non-clickable)
        else
        {
            buttonImage.sprite = lockedSprite; // Set the locked button sprite
            myButton.enabled = false;          // Disable the button interaction
            levelText.enabled = false;         // Hide the level text
        }
    }

    void LoadData()
    {
        if (gameData != null)
        {
            if (gameData.saveData.isActive[level - 1])
            {
                isActive = true;
            } else
            {
                isActive = false;
            }
            starsActive = gameData.saveData.stars[level - 1];
        }
    }

    // Displays the level number on the button's text field
    void ShowLevel()
    {
        if (level == 1)
        {
            levelText.text = "1 Minute Blitz";
        } else
        {
            levelText.text = "" + (level - 1);  // Set the level number as text
        }
    }

    // This method is called when a player selects a level, displaying a confirmation panel
    public void ConfirmPanel(int level)
    {
        // Set the level number in the confirmation panel (presumably for confirmation or level load)
        confirmPanel.GetComponent<ConfirmPanel>().level = level;

        // Activate the confirmation panel (make it visible to the user)
        confirmPanel.SetActive(true);
    }
}