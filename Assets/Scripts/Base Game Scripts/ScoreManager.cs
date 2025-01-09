using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board; // Reference to the Board component to access game state and score goals
    public Text scoreText; // Reference to the UI Text component that displays the current score
    public int score; // The player's current score
    public Image scoreBar; // Reference to the UI Image component representing the score progress bar
    private GameData gameData; // Reference to the GameData component to save high scores

    // Start is called before the first frame update
    void Start()
    {
        // Find the Board component in the scene to track the score and goals
        board = FindObjectOfType<Board>();

        // Find the GameData component to access and store high scores
        gameData = FindObjectOfType<GameData>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the scoreText UI element to display the current score value
        // Convert the integer score to a string and set it to the scoreText element
        scoreText.text = score.ToString();
    }

    // Method to increase the score by a specified amount
    // Called when a match is made or other events trigger a score increase
    public void IncreaseScore(int amountToIncrease)
    {
        // Increase the current score by the specified amount
        score += amountToIncrease;

        // Check if the gameData is available to save the high score
        if (gameData != null)
        {
            // Retrieve the current high score for the level from the saved game data
            int highScore = gameData.saveData.highScores[board.level];

            // If the new score exceeds the current high score, update the high score in saved data
            gameData.saveData.highScores[board.level] = score;

            // If the current score is greater than the high score, save the new high score
            if (score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }
        }

        // Check if the board and scoreBar references are valid to avoid null reference errors
        if (board != null && scoreBar != null)
        {
            // Get the total number of score goals (used to set up the score bar)
            int length = board.scoreGoals.Length;

            // Update the fill amount of the scoreBar to reflect the player's progress toward the final score goal
            // The fill amount is based on the current score relative to the final score goal
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];
        }
    }
}