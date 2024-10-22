using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

// Enum to represent the current state of the game
public enum GameState {
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move; // Current state of the game
    public int width; // Width of the board
    public int height; // Height of the board
    public int offSet; // Offset for positioning tiles
    public GameObject tilePrefab; // Prefab for the background tiles
    private BackgroundTile[,] allTiles; // 2D array to hold all background tiles
    public GameObject[] dots; // Array of dot prefabs
    public GameObject[,] allDots; // 2D array to hold all dots on the board
    private FindMatches findMatches; // Reference to FindMatches script for match detection
    public GameObject destroyEffect; // Effect to show when a dot is destroyed
    public Dot currentDot; // Reference to the currently selected dot

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>(); // Get the FindMatches component from the scene
        allTiles = new BackgroundTile[width, height]; // Initialize the tiles array
        allDots = new GameObject[width, height]; // Initialize the dots array
        SetUp(); // Set up the board with tiles and dots
    }

    // Set up the board by instantiating tiles and randomly placing dots
    private void SetUp() {
        for (int i = 0; i < width; i++) { // Loop through each column
            for (int j = 0; j < height; j++) { // Loop through each row
                // Set tile position based on the current index
                UnityEngine.Vector2 tempPosition = new(i, j + offSet); 
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, UnityEngine.Quaternion.identity); // Create tile
                backgroundTile.transform.parent = this.transform; // Set the parent to the board
                backgroundTile.name = "( " + i + ", " + j + " )"; // Name tile for debugging
                
                int dotToUse = Random.Range(0, dots.Length); // Randomly select a dot prefab
                int maxIterations = 0; // Counter to prevent infinite loops

                // Ensure the selected dot does not match existing adjacent dots
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) {
                    dotToUse = Random.Range(0, dots.Length); // Select a new dot if there's a match
                    maxIterations++;
                }
                maxIterations = 0; // Reset for future use

                // Instantiate the selected dot and set its position
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity);
                dot.GetComponent<Dot>().row = j; // Set dot's row
                dot.GetComponent<Dot>().column = i; // Set dot's column
                dot.transform.parent = this.transform; // Set parent to the board
                dot.name = "( " + i + ", " + j + " )"; // Name dot for debugging
                allDots[i, j] = dot; // Store the dot in the array
            }
        }
    }

    // Check if the current position has matching dots
    private bool MatchesAt(int column, int row, GameObject piece) {
        // Check for horizontal and vertical matches
        if (column > 1 && row > 1) {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag) {
                return true; // Found horizontal match
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag) {
                return true; // Found vertical match
            }
        } else if (column <= 1 || row <= 1) {
            if (row > 1) {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag) {
                    return true; // Found vertical match
                }
            }
            if (column > 1) {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag) {
                    return true; // Found horizontal match
                }
            }
        }
        return false; // No match found
    }

    // Destroy matched dots at the given position
    private void DestroyMatchesAt(int column, int row) {
        // Check if the dot at the position is marked as matched
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
            // Check for special match conditions (bombs, etc.)
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7
            || findMatches.currentMatches.Count == 8 || findMatches.currentMatches.Count == 9) {
                findMatches.CheckBombs(); // Handle bomb effects if applicable
            }
            // Instantiate a destruction effect at the dot's position
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, UnityEngine.Quaternion.identity);
            Destroy(particle, .5f); // Destroy the effect after a short time
            Destroy(allDots[column, row]); // Destroy the matched dot
            allDots[column, row] = null; // Set the array position to null
        }
    }

    // Loop through the board and destroy all matched dots
    public void DestroyMatches() {
        for (int i = 0; i < width; i++) { // Loop through each column
            for (int j = 0; j < height; j++) { // Loop through each row
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j); // Check and destroy matches at each position
                }
            }
        }
        findMatches.currentMatches.Clear(); // Clear the current matches list
        StartCoroutine(DecreaseRowCo()); // Start coroutine to handle row adjustments after destruction
    }

    // Coroutine to handle decreasing rows when dots are destroyed
    private IEnumerator DecreaseRowCo() {
        int nullCount = 0; // Count of null dots in a column
        for (int i = 0; i < width; i++) { // Loop through each column
            for (int j = 0; j < height; j++) { // Loop through each row
                if (allDots[i, j] == null) {
                    nullCount++; // Increase count of nulls
                } else if (nullCount > 0) {
                    // Move existing dots down by the number of nulls above them
                    allDots[i, j].GetComponent<Dot>().row -= nullCount; // Update the row position of the dot
                    allDots[i, j] = null; // Set the current position to null
                }
            }
            nullCount = 0; // Reset null count for the next column
        }
        yield return new WaitForSeconds(.4f); // Wait before refilling the board
        StartCoroutine(FillBoardCo()); // Start coroutine to fill the board with new dots
    }

    // Refill the board with new dots
    private void RefillBoard() {
        for (int i = 0; i < width; i++) { // Loop through each column
            for (int j = 0; j < height; j++) { // Loop through each row
                if (allDots[i, j] == null) { // Check for null positions
                    UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(i, j + offSet); // Set the new position
                    int dotToUse = Random.Range(0, dots.Length); // Randomly select a dot to use
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity); // Create new dot
                    allDots[i, j] = piece; // Store the new dot in the array
                    piece.GetComponentInParent<Dot>().row = j; // Set dot's row
                    piece.GetComponentInParent<Dot>().column = i; // Set dot's column
                }
            }
        }
    }

    // Check if there are any matches on the board
    private bool MatchesOnBoard() {
        for (int i = 0; i < width; i++) { // Loop through each column
            for (int j = 0; j < height; j++) { // Loop through each row
                if (allDots[i, j] != null) { // Check for a valid dot
                    if (allDots[i, j].GetComponent<Dot>().isMatched) {
                        return true; // Found a match
                    }
                }
            }
        }
        return false; // No matches found
    }

    // Coroutine to fill the board after destroying matches
    private IEnumerator FillBoardCo() {
        RefillBoard(); // Refill the board with new dots
        yield return new WaitForSeconds(.2f); // Wait for a short time
        while (MatchesOnBoard()) { // Continuously check for new matches
            yield return new WaitForSeconds(.2f); // Wait before checking for matches again
            DestroyMatches(); // Destroy any new matches found
        }
        findMatches.currentMatches.Clear(); // Clear the matches list after filling
        yield return new WaitForSeconds(.2f); // Wait before allowing moves again
        currentState = GameState.move; // Set state to allow player moves
    }
}