using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
                                if (currentDot.GetComponent<Dot>().isRowBomb || leftDot.GetComponent<Dot>().isRowBomb
                                || rightDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j)); // Get all dots in the row
                                }
                                // Check for column bombs and add affected pieces to currentMatches
                                if (currentDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i)); // Get all dots in the column
                                }
                                if (leftDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i - 1)); // Get all dots in the left column
                                }
                                if (rightDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i + 1)); // Get all dots in the right column
                                }
                                // Add left dot to current matches if not already added
                                if (!currentMatches.Contains(leftDot)) {
                                    currentMatches.Add(leftDot);
                                    currentMatches.Add(leftDot); // Add left dot if not already present
                                }
                                leftDot.GetComponent<Dot>().isMatched = true; // Mark left dot as matched
                                
                                // Add right dot to current matches if not already added
                                if (!currentMatches.Contains(rightDot)) {
                                    currentMatches.Add(rightDot);
                                    currentMatches.Add(rightDot); // Add right dot if not already present
                                }
                                rightDot.GetComponent<Dot>().isMatched = true; // Mark right dot as matched
                                
                                // Add current dot to matches if not already added
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                    currentMatches.Add(currentDot); // Add current dot if not already present
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
                                // Check for column bombs and add affected pieces to currentMatches
                                if (currentDot.GetComponent<Dot>().isColumnBomb || upDot.GetComponent<Dot>().isColumnBomb
                                || downDot.GetComponent<Dot>().isColumnBomb) {
                                    currentMatches.Union(GetColumnPieces(i)); // Get all dots in the column
                                }
                                // Check for row bombs and add affected pieces to currentMatches
                                if (currentDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j)); // Get all dots in the row
                                }
                                if (upDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j + 1)); // Get all dots in the upper row
                                }
                                if (downDot.GetComponent<Dot>().isRowBomb) {
                                    currentMatches.Union(GetRowPieces(j - 1)); // Get all dots in the lower row
                                }
                                upDot.GetComponent<Dot>().isMatched = true; // Mark upper dot as matched
                                // Add upper dot to current matches if not already added
                                if (!currentMatches.Contains(upDot)) {
                                    currentMatches.Add(upDot);
                                    currentMatches.Add(upDot); // Add upper dot if not already present
                                }
                                
                                downDot.GetComponent<Dot>().isMatched = true; // Mark lower dot as matched
                                // Add lower dot to current matches if not already added
                                if (!currentMatches.Contains(downDot)) {
                                    currentMatches.Add(downDot);
                                    currentMatches.Add(downDot); // Add lower dot if not already present
                                }
                                
                                currentDot.GetComponent<Dot>().isMatched = true; // Mark current dot as matched
                                // Add current dot to matches if not already added
                                if (!currentMatches.Contains(currentDot)) {
                                    currentMatches.Add(currentDot);
                                    currentMatches.Add(currentDot); // Add current dot if not already present
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void MatchPiecesOfColor(string color) {
        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                if (board.allDots[i, j] != null) {
                    if (board.allDots[i, j].tag == color) {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }
    List<GameObject> GetColumnPieces(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++) {
            if (board.allDots[column, i] != null) {
                dots.Add(board.allDots[column, i]);
                board.allDots[column, i].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++) {
            if (board.allDots[i, row] != null) {
                dots.Add(board.allDots[i, row]);
                board.allDots[i, row].GetComponent<Dot>().isMatched = true;
            }
        }
        return dots;
    }
    public void CheckBombs() {
        if (board.currentDot != null) {
            if (board.currentDot.isMatched) {
                board.currentDot.isMatched = false;
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                    board.currentDot.MakeRowBomb();
                } else {
                    board.currentDot.MakeColumnBomb();
                }
            } else if (board.currentDot.otherDot != null) {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched) {
                    otherDot.isMatched = false;
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                    (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                        otherDot.MakeRowBomb();
                    } else {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}