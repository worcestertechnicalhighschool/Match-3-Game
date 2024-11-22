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

// GoalManager is responsible for setting up the goals in both the intro and in-game UI.
public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals; // Array of BlankGoal objects, representing each goal in the level
    public GameObject goalPrefab; // The prefab used to instantiate goal UI elements
    public GameObject goalIntroParent; // The parent GameObject for the intro goals UI
    public GameObject goalGameParent; // The parent GameObject for the in-game goals UI

    // Start is called before the first frame update.
    // It's used to initialize the goals at the start of the game.
    void Start()
    {
        SetupIntroGoals(); // Calls the function to setup the goals at the beginning
    }

    // This method sets up the goal UI for both the intro screen and the game screen.
    void SetupIntroGoals()
    {
        // Loop through each goal in the levelGoals array to set up the UI for each one
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // Instantiate a new goal for the intro screen using the goalPrefab
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, UnityEngine.Quaternion.identity);
            goal.transform.SetParent(goalIntroParent.transform); // Set the parent of the goal to goalIntroParent

            // Get the GoalPanel component from the instantiated goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite; // Set the sprite for the goal
            panel.thisString = "0/" + levelGoals[i].numberNeeded; // Set the initial goal string showing 0/numberNeeded

            // Instantiate a new goal for the game screen using the goalPrefab
            GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, UnityEngine.Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform); // Set the parent of the goal to goalGameParent

            // Get the GoalPanel component again for the second goal (game screen version)
            panel = gameGoal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite; // Set the sprite for the goal
            panel.thisString = "0/" + levelGoals[i].numberNeeded; // Set the initial goal string showing 0/numberNeeded
        }
    }
}