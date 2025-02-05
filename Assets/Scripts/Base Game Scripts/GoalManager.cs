using System.Collections.Generic;
using UnityEngine;

// The BlankGoal class holds the goal information like how many are needed, 
// how many are collected, the sprite to represent the goal, and a match value.
[System.Serializable]
public class BlankGoal
{
    // The number of items needed to complete this goal (e.g., collect X items)
    public int numberNeeded;

    // The number of items currently collected toward completing this goal
    public int numberCollected;

    // The sprite that visually represents this goal (used in UI)
    public Sprite goalSprite;

    // A string identifier for matching the goal (could be item type or category)
    public string matchValue;
}

// GoalManager is responsible for setting up and managing the goals during both the intro and in-game UI.
// This includes initializing the goals and updating their status during gameplay.
public class GoalManager : MonoBehaviour
{
    // Array of BlankGoal objects, representing each goal for the current level.
    public BlankGoal[] levelGoals;

    // List of current goal UI elements, used to track and update displayed goals in the game.
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    // Prefab for the goal UI elements, used to instantiate goal representations in the UI.
    public GameObject goalPrefab;

    // The parent GameObject in the intro scene that holds the goal UI elements before gameplay starts.
    public GameObject goalIntroParent;

    // The parent GameObject in the game scene that holds the goal UI elements during gameplay.
    public GameObject goalGameParent;

    // Reference to the EndGameManager, used for handling game-ending conditions and logic.
    private EndGameManager endGame;

    // Reference to the Board, used to track level and world information for goal setup.
    private Board board;

    // Start is called before the first frame update.
    // Initializes the game by setting up the goals in both intro and in-game UI.
    void Start()
    {
        // Find the Board object, which contains the game level and world information.
        board = FindObjectOfType<Board>();

        // Find the EndGameManager, which handles logic when the game ends.
        endGame = FindObjectOfType<EndGameManager>();

        // Retrieve the goals for the current level based on the Board's current world and level.
        GetGoals();

        // Calls SetupGoals method to prepare goal-related UI and logic for gameplay.
        SetupGoals();
    }

    // Retrieves the goals for the current level based on the Board's current world and level.
    void GetGoals()
    {
        // Check if the Board object is not null
        if (board != null)
        {
            // Check if the World object inside the Board is not null
            if (board.world != null)
            {
                // Check if the current level is valid (within the bounds of the world's levels array)
                if (board.level < board.world.levels.Length)
                {
                    // If the current level is valid, set the levelGoals array to the goals of the current level
                    if (board.world.levels[board.level] != null)
                    {
                        // Set levelGoals to the goals defined for the current level
                        levelGoals = board.world.levels[board.level].levelGoals;

                        // Loop through each goal in the levelGoals array
                        for (int i = 0; i < levelGoals.Length; i++)
                        {
                            // Initialize the numberCollected for each goal to 0 (reset the collection count)
                            levelGoals[i].numberCollected = 0;
                        }
                    }
                }
            }
        }
    }


    // This method sets up the goal UI for both the intro screen and the game screen.
    void SetupGoals()
    {
        // Loop through each goal in the levelGoals array to set up the UI for each one
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // Instantiate a new goal for the intro screen using the goalPrefab
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, UnityEngine.Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform); // Set the parent of the goal to goalIntroParent

            // Get the GoalPanel component from the instantiated goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();

            // Set the sprite for the goal in the UI
            panel.thisSprite = levelGoals[i].goalSprite;

            // Set the initial goal string showing 0/numberNeeded in the UI (e.g. 0/3)
            panel.thisString = "0/" + levelGoals[i].numberNeeded;

            // Instantiate a new goal for the game screen using the goalPrefab
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, UnityEngine.Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform); // Set the parent of the goal to goalGameParent

            // Get the GoalPanel component again for the second goal (game screen version)
            panel = gameGoal.GetComponent<GoalPanel>();

            // Add the newly created goal panel to the currentGoals list for future reference
            currentGoals.Add(panel);

            // Set the sprite and initial goal string for the in-game version of the goal
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    // Updates the displayed goals in both the intro and in-game UI.
    public void UpdateGoals()
    {
        int goalsCompleted = 0;

        // Loop through all the goals to update the displayed progress
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // Update the goal text in the UI to reflect the collected/needed values
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;

            // If the goal is completed (numberCollected >= numberNeeded), update the display to show the goal as complete
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }

        // If all goals are completed, trigger the win logic
        if (goalsCompleted >= levelGoals.Length)
        {
            // Ensure that the endGame reference is valid before calling WinGame()
            if (endGame != null)
            {
                endGame.WinGame(); // Call the WinGame method from EndGameManager
            }

            // Log a victory message to the console
            Debug.Log("You win! Hooray!");
        }
    }

    // Compares the goal provided with the goals in the levelGoals array and increments the collected count if it matches
    public void CompareGoal(string goalToCompare)
    {
        // Loop through each goal to check if it matches the provided goalToCompare string
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // If the goal match value matches the goalToCompare string, increment the collected count for that goal
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}