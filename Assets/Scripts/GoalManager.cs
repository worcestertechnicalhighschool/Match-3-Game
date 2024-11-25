using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The BlankGoal class holds the goal information like how many are needed, 
// how many are collected, the sprite to represent the goal, and a match value.
[System.Serializable]
public class BlankGoal
{
    public int numberNeeded; // The number of items needed to complete the goal
    public int numberCollected; // The number of items currently collected for the goal
    public Sprite goalSprite; // The sprite that represents the goal visually
    public string matchValue; // A string that holds a matching value or identifier (purpose can vary)
}

// GoalManager is responsible for setting up and managing the goals during both the intro and in-game UI.
public class GoalManager : MonoBehaviour
{
    // Array of BlankGoal objects, representing each goal in the level
    public BlankGoal[] levelGoals;

    // List of current goals in the game (for UI updating purposes)
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    // The prefab used to instantiate goal UI elements
    public GameObject goalPrefab;

    // The parent GameObject for the intro goals UI (before the game starts)
    public GameObject goalIntroParent;

    // The parent GameObject for the in-game goals UI (during the game)
    public GameObject goalGameParent;

    // Reference to the EndGameManager to handle game-ending logic
    private EndGameManager endGame;

    // Start is called before the first frame update.
    // Initializes the game by setting up the goals in both intro and game UI.
    void Start()
    {
        // Find and reference the EndGameManager in the scene
        endGame = FindObjectOfType<EndGameManager>();

        // Calls the SetupGoals method to set up the goals at the start of the game
        SetupGoals();
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