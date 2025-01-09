using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// The GoalPanel class is responsible for setting up the UI elements for a single goal
// including its image (sprite) and the text that shows the goal progress (e.g., 0/10).
public class GoalPanel : MonoBehaviour
{
    public Image thisImage; // The Image component used to display the goal's sprite
    public Sprite thisSprite; // The sprite representing the goal's visual appearance
    public Text thisText; // The Text component used to display the goal's progress
    public string thisString; // The string representing the goal's progress (e.g., "0/10")

    // Start is called before the first frame update.
    // It calls the Setup method to initialize the UI elements.
    void Start()
    {
        Setup(); // Calls the Setup method to configure the panel's image and text
    }

    // The Setup method assigns the goal's sprite to the image and the progress string to the text.
    void Setup()
    {
        thisImage.sprite = thisSprite; // Sets the sprite of the image component
        thisText.text = thisString; // Sets the text of the text component to show the goal's progress
    }
}