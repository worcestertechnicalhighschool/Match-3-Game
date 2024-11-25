using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    // The health points of the background tile, which determines how much damage it can take
    public int hitPoints;

    // Reference to the SpriteRenderer component that controls the tile's appearance
    private SpriteRenderer sprite;

    // Reference to the GoalManager, which tracks and updates game goals
    private GoalManager goalManager;

    // Method to apply damage to the tile, reducing its hit points
    public void TakeDamage(int damage)
    {
        hitPoints -= damage; // Reduce the tile's hit points by the specified damage value
        MakeLighter(); // Change the tile's color to visually indicate it has taken damage
    }

    // Start is called before the first frame update
    // Initializes the tile and retrieves necessary references for gameplay
    void Start()
    {
        // Find and reference the GoalManager in the scene (to update goals when the tile is destroyed)
        goalManager = FindObjectOfType<GoalManager>();

        // Get the SpriteRenderer component attached to this GameObject (tile)
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    // Continuously checks if the tile's health has reached zero and handles its destruction
    public void Update()
    {
        // Check if the tile's hit points are zero or less, indicating it is destroyed
        if (hitPoints <= 0)
        {
            // If the GoalManager is found, update the goals based on this tile's tag
            if (goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag); // Inform the GoalManager about the tile's destruction
                goalManager.UpdateGoals(); // Update the goals UI to reflect the progress
            }

            // Destroy this GameObject (the tile) when its hit points are depleted
            Destroy(this.gameObject);
        }
    }

    // Method to lighten the tile's color, giving a visual effect when it takes damage
    void MakeLighter()
    {
        // Get the current color of the tile's sprite (using SpriteRenderer)
        Color color = sprite.color;

        // Reduce the alpha value to make the tile appear lighter (transparency is increased)
        float newAlpha = color.a * 0.5f;

        // Set the tile's sprite color to the new, lighter color with the same RGB values but reduced alpha
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}