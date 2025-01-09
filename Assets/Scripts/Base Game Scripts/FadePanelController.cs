using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FadePanelController handles the animations for the fade-in and fade-out effects
// for the game panels and transitions like game start and game over.
public class FadePanelController : MonoBehaviour
{
    // References to the Animator components that control the panel and game info animations
    public Animator panelAnim;
    public Animator gameInfoAnim;

    // This method is called when the OK button is pressed (usually to start the game).
    public void OK()
    {
        // Check if the animator components are assigned before proceeding
        if (panelAnim != null && gameInfoAnim != null)
        {
            // Set the "Out" parameter to true, triggering the fade-out animation for both panel and game info
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);

            // Start the game after a short delay (triggered by a coroutine)
            StartCoroutine(GameStartCo());
        }
    }

    // This method is called when the game is over, to show the "Game Over" screen
    public void GameOver()
    {
        // Set the "Out" parameter to false (reset fade-out animation), and enable the "Game Over" animation
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);
    }

    // Coroutine that starts the game after a short delay (to allow the fade-out effect to finish)
    IEnumerator GameStartCo()
    {
        // Wait for 1 second before continuing (giving time for fade-out animation)
        yield return new WaitForSeconds(1f);

        // Find the Board object in the scene to access the current game state
        Board board = FindObjectOfType<Board>();

        // Change the game state to "move", indicating that the game has started
        board.currentState = GameState.move;
    }
}