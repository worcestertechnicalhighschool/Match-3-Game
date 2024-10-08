using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board; // Reference to the game board
    public List<GameObject> currentMatches = new List<GameObject>(); // List to hold current matched dots

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); // Find and reference the Board object
    }

    // Public method to find all matches
    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCo()); // Start the coroutine to find matches
    }

    // Coroutine to find all matches in the board
    private IEnumerator FindAllMatchesCo() {
        yield return new WaitForSeconds(.2f); // Wait for a brief moment before checking for matches
        for (int i = 0; i < board.width; i++) { // Loop through each column
            for (int j = 0; j < board.height; j++) { // Loop through each row
                GameObject currentDot = board.allDots[i, j]; // Get the current dot
                if (currentDot != null) { // Check if the current dot exists
                    // Check for horizontal matches
                    if (i > 0 && i < board.width - 1) { // Ensure indices are within bounds
                        GameObject leftDot = board.allDots[i - 1, j]; // Dot to the left
                        GameObject rightDot = board.allDots[i + 1, j]; // Dot to the right
                        // Check if both left and right dots exist and match
                        if (leftDot != null && rightDot != null) {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {
                                // Add left dot to current matches if not already added
                                if (!currentMatches.Contains(leftDot)) {
                                    currentMatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true; // Mark left dot as matched
                                
                                // Add right dot to current matches if not already added
                                if (!currentMatches.Contains(rightDot)) {
                                    currentMatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true; // Mark right dot as matched
                                
                                // Add current dot to matches if not already added
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true; // Mark current dot as matched
                            }
                        }
                    }
                    // Check for vertical matches
                    if (j > 0 && j < board.height - 1) { // Ensure indices are within bounds
                        GameObject upDot = board.allDots[i, j + 1]; // Dot above
                        GameObject downDot = board.allDots[i, j - 1]; // Dot below
                        // Check if both up and down dots exist and match
                        if (upDot != null && downDot != null) {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) {
                                upDot.GetComponent<Dot>().isMatched = true; // Mark upper dot as matched
                                // Add upper dot to current matches if not already added
                                if (!currentMatches.Contains(upDot)) {
                                    currentMatches.Add(upDot);
                                }
                                
                                downDot.GetComponent<Dot>().isMatched = true; // Mark lower dot as matched
                                // Add lower dot to current matches if not already added
                                if (!currentMatches.Contains(downDot)) {
                                    currentMatches.Add(downDot);
                                }
                                
                                currentDot.GetComponent<Dot>().isMatched = true; // Mark current dot as matched
                                // Add current dot to matches if not already added
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}