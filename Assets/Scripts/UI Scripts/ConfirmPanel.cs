using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    [Header("Level Information")]
    public string levelToLoad; // The name of the scene to load when the player confirms
    public int level; // The level number associated with this confirmation panel
    private GameData gameData; // Reference to the GameData script
    private int starsActive; // Number of stars earned by the player for this level
    private int highScore; // Highest score achieved by the player for this level

    [Header("UI Information")]
    // Array of Image components representing the stars to show level completion status
    public Image[] stars;
    // UI Text component to display the high score for the level
    public Text highScoreText;
    // UI Text component to display the number of stars the player earned
    public Text starText;

    // Start is called when the script is enabled before the first frame update
    void OnEnable()
    {
        // Initialize the game data reference
        gameData = FindObjectOfType<GameData>();

        // Load saved data related to the current level
        LoadData();

        // Call the method to initially activate the stars based on the player's progress
        ActivateStars();

        // Update the UI texts (high score and star count)
        SetText();
    }

    // Loads the saved data for stars and high scores from the game data
    void LoadData()
    {
        if (gameData != null)
        {
            // Get the number of stars earned for the current level from saved data
            starsActive = gameData.saveData.stars[level - 1];
            // Get the high score for the current level from saved data
            highScore = gameData.saveData.highScores[level - 1];
        }
    }

    // Activates (enables) the correct number of star images based on the player's progress
    void ActivateStars()
    {
        // Loop through the stars array and enable the images up to the number of stars the player earned
        for (int i = 0; i < starsActive; i++)
        {
            // Enable the star image (set it to visible)
            stars[i].enabled = true;
        }
    }

    // Updates the text components with the correct high score and star count for the current level
    void SetText()
    {
        // Display the high score for the current level
        highScoreText.text = "" + highScore;
        // Display the number of stars earned by the player out of a maximum of 3
        starText.text = "" + starsActive + "/3";
    }

    // Method to handle the cancel action - hides the confirmation panel
    public void Cancel()
    {
        // Disable the game object (confirmation panel), effectively hiding it
        this.gameObject.SetActive(false);
    }

    // Method to handle the play action - sets the current level and loads the corresponding scene
    public void Play()
    {
        // Set the current level in PlayerPrefs to the previous level (level - 1), for tracking purposes
        PlayerPrefs.SetInt("Current Level", level - 1);

        // Load the scene associated with the level to load (the actual gameplay scene)
        SceneManager.LoadScene(levelToLoad);
    }
}