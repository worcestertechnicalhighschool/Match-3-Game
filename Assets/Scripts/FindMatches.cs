using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board; // Reference to the game board
    public List<GameObject> currentMatches = new List<GameObject>(); // List to hold currently matched dots

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); // Find and reference the Board object
    }

    // Public method to initiate the match finding process
    public void FindAllMatches() {
        StartCoroutine(FindAllMatchesCo()); // Start the coroutine to find matches
    }

    // Check if any of the given dots is a row bomb and get all pieces in that row
    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb) {
            currentMatches = currentMatches.Union(GetRowPieces(dot1.row)).ToList(); // Add all dots in the row to matches
        }
        if (dot2.isRowBomb) {
            currentMatches = currentMatches.Union(GetRowPieces(dot2.row)).ToList(); // Add all dots in the upper row
        }
        if (dot3.isRowBomb) {
            currentMatches = currentMatches.Union(GetRowPieces(dot3.row)).ToList(); // Add all dots in the lower row
        }
        return currentDots;
    }

    // Check if any of the given dots is a column bomb and get all pieces in that column
    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3) {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnBomb) {
            currentMatches = currentMatches.Union(GetColumnPieces(dot1.column)).ToList(); // Add all dots in the column to matches
        }
        if (dot2.isColumnBomb) {
            currentMatches = currentMatches.Union(GetColumnPieces(dot2.column)).ToList(); // Add all dots in the upper column
        }
        if (dot3.isColumnBomb) {
            currentMatches = currentMatches.Union(GetColumnPieces(dot3.column)).ToList(); // Add all dots in the lower column
        }
        return currentDots;
    }

    // Add a dot to the current matches list and mark it as matched
    private void AddToListAndMatch(GameObject dot) {
        if (!currentMatches.Contains(dot)) {
            currentMatches.Add(dot); // Add dot to matches if not already included
        }
        dot.GetComponent<Dot>().isMatched = true; // Mark the dot as matched
    }

    // Add nearby dots to the matches list
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3) {
        AddToListAndMatch(dot1); // Add the first dot
        AddToListAndMatch(dot2); // Add the second dot
        AddToListAndMatch(dot3); // Add the third dot
    }

    // Coroutine to find all matches in the board
    private IEnumerator FindAllMatchesCo() {
        yield return new WaitForSeconds(.1f); // Wait briefly before checking for matches
        for (int i = 0; i < board.width; i++) { // Loop through each column
            for (int j = 0; j < board.height; j++) { // Loop through each row
                GameObject currentDot = board.allDots[i, j]; // Get the current dot
                if (currentDot != null) { // Ensure the current dot exists
                    Dot currentDotDot = currentDot.GetComponent<Dot>();
                    // Check for horizontal matches
                    if (i > 0 && i < board.width - 1) { // Ensure indices are within bounds
                        GameObject leftDot = board.allDots[i - 1, j]; // Dot to the left
                        GameObject rightDot = board.allDots[i + 1, j]; // Dot to the right
                        if (leftDot != null && rightDot != null) {
                            Dot rightDotDot = rightDot.GetComponent<Dot>();
                            Dot leftDotDot = leftDot.GetComponent<Dot>();
                            // Check if both adjacent dots match the current dot
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {
                                currentMatches = currentMatches.Union(IsRowBomb(leftDotDot, currentDotDot, rightDotDot)).ToList();
                                currentMatches = currentMatches.Union(IsColumnBomb(leftDotDot, currentDotDot, rightDotDot)).ToList();
                                GetNearbyPieces(leftDot, currentDot, rightDot); // Add to matches list
                            }
                        }
                    }
                    // Check for vertical matches
                    if (j > 0 && j < board.height - 1) { // Ensure indices are within bounds
                        GameObject upDot = board.allDots[i, j + 1]; // Dot above
                        GameObject downDot = board.allDots[i, j - 1]; // Dot below
                        if (upDot != null && downDot != null) {
                            Dot downDotDot = downDot.GetComponent<Dot>();
                            Dot upDotDot = upDot.GetComponent<Dot>();
                            // Check if both adjacent dots match the current dot
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag) {
                                currentMatches = currentMatches.Union(IsRowBomb(upDotDot, currentDotDot, downDotDot)).ToList();
                                currentMatches = currentMatches.Union(IsColumnBomb(upDotDot, currentDotDot, downDotDot)).ToList();
                                GetNearbyPieces(upDot, currentDot, downDot); // Add to matches list
                            }
                        }
                    }
                }
            }
        }
    }

    // Mark all pieces of the specified color as matched
    public void MatchPiecesOfColor(string color) {
        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                if (board.allDots[i, j] != null) {
                    if (board.allDots[i, j].tag == color) {
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true; // Mark the dot as matched
                    }
                }
            }
        }
    }

    // Get all dots in the specified column and mark them as matched
    List<GameObject> GetColumnPieces(int column) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++) {
            if (board.allDots[column, i] != null) {
                dots.Add(board.allDots[column, i]); // Add the dot to the list
                board.allDots[column, i].GetComponent<Dot>().isMatched = true; // Mark the dot as matched
            }
        }
        return dots; // Return the list of dots
    }

    // Get all dots in the specified row and mark them as matched
    List<GameObject> GetRowPieces(int row) {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++) {
            if (board.allDots[i, row] != null) {
                dots.Add(board.allDots[i, row]); // Add the dot to the list
                board.allDots[i, row].GetComponent<Dot>().isMatched = true; // Mark the dot as matched
            }
        }
        return dots; // Return the list of dots
    }

    // Check if the current or other dot is a bomb and create a bomb if matched
    public void CheckBombs() {
        if (board.currentDot != null) {
            if (board.currentDot.isMatched) {
                board.currentDot.isMatched = false; // Reset the matched status
                // Determine the type of bomb based on swipe direction
                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                    board.currentDot.MakeRowBomb(); // Create a row bomb
                } else {
                    board.currentDot.MakeColumnBomb(); // Create a column bomb
                }
            } else if (board.currentDot.otherDot != null) {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                if (otherDot.isMatched) {
                    otherDot.isMatched = false; // Reset the matched status
                    // Determine the type of bomb based on swipe direction
                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||
                    (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135)) {
                        otherDot.MakeRowBomb(); // Create a row bomb
                    } else {
                        otherDot.MakeColumnBomb(); // Create a column bomb
                    }
                }
            }
        }
    }
}