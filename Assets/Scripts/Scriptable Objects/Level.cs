using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a new ScriptableObject called "Level" that can be added from the Unity editor
[CreateAssetMenu(fileName = "Level", menuName = "Level")]
public class Level : ScriptableObject
{
    // Section for Board Settings
    [Header("Board Settings")]
    public int width;   // The width of the game board (number of columns)
    public int height;  // The height of the game board (number of rows)

    // Section for Tile Settings
    [Header("Tile Settings")]
    public TileType[] boardLayout; // Defines the layout of the board using TileType enums (e.g., walls, paths)

    // Section for Dot Settings
    [Header("Dot Settings")]
    public GameObject[] dots; // Array of GameObjects representing different dot types (e.g., collectible items or obstacles)

    // Section for Level Goal Settings
    [Header("Level Goal Settings")]
    public int[] scoreGoals; // Array of score goals for completing the level (may represent different targets for the player to achieve)
    public EndGameRequirements endGameRequirements; // The conditions under which the game ends (e.g., moves or time-based)
    public BlankGoal[] levelGoals; // Array of goals to be completed during the level (could include tasks such as clearing dots or reaching a certain score)
}