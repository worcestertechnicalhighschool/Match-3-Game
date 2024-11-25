using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Enum to define the type of game (Moves or Time)
public enum GameType
{
    Moves, // Game based on the number of moves
    Time   // Game based on time
}

// Serializable class to define the game ending requirements
[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;     // The type of game (Moves or Time)
    public int counterValue;      // The initial counter value (either moves or time limit)
}

public class EndGameManager : MonoBehaviour
{
    // UI Elements for displaying the moves and time
    public GameObject movesLabel;
    public GameObject timeLabel;

    // UI Panels for win and lose states
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;

    // Counter text to display the current value of moves or time
    public Text counter;

    // Requirements for ending the game
    public EndGameRequirements requirements;

    // Current counter value (number of moves or remaining time)
    public int currentCounterValue;

    // Reference to the Board object (used for checking game state)
    private Board board;

    // Timer in seconds (used if the game is time-based)
    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        // Find the Board object in the scene
        board = FindObjectOfType<Board>();

        // Set up the game based on the defined game type
        SetupGame();
    }

    // Update is called once per frame
    void Update()
    {
        // If the game type is Time-based and the counter value is greater than 0
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            // Decrease the timer by the time passed since the last frame
            timerSeconds -= Time.deltaTime;

            // If the timer reaches 0, decrease the counter value
            if (timerSeconds <= 0)
            {
                // Decrease the counter value and reset the timer
                DecreaseCounterValue();
                timerSeconds = 1; // Reset timer to 1 second
            }
        }
    }

    // Setup the game based on the specified game type (Moves or Time)
    void SetupGame()
    {
        // Initialize the counter value from the requirements
        currentCounterValue = requirements.counterValue;

        // If the game type is Moves, show the moves label and hide the time label
        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else // If the game type is Time, show the time label and hide the moves label
        {
            timerSeconds = 1; // Start the timer
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        // Update the counter UI with the initial counter value
        counter.text = "" + currentCounterValue;
    }

    // Method to decrease the counter value (for moves or time)
    public void DecreaseCounterValue()
    {
        // If the game is not paused, decrease the counter
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;

            // If the counter reaches 0, the player loses
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    // Method to handle winning the game
    public void WinGame()
    {
        // Activate the "You Win" panel
        youWinPanel.SetActive(true);

        // Set the board state to "win"
        board.currentState = GameState.win;

        // Log a win message
        Debug.Log("You Win! :D ");

        // Reset the counter to 0 and update the UI
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        // Trigger the Game Over process (such as fading out)
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    // Method to handle losing the game
    public void LoseGame()
    {
        // Activate the "Try Again" panel
        tryAgainPanel.SetActive(true);

        // Set the board state to "lose"
        board.currentState = GameState.lose;

        // Log a loss message
        Debug.Log("You Lost! :( ");

        // Reset the counter to 0 and update the UI
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;

        // Trigger the Game Over process (such as fading out)
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
}