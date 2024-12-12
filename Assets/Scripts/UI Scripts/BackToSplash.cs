using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSplash : MonoBehaviour
{
    // The name of the scene to load when the OK button is pressed
    public string sceneToLoad;

    // Method to handle the OK button click, which loads the specified scene
    public void OK()
    {
        // Load the scene specified in the sceneToLoad variable
        SceneManager.LoadScene(sceneToLoad);
    }
}