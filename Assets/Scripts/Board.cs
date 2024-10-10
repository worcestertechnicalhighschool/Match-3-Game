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
    public Dot currentDot;
    private FindMatches findMatches; // Reference to FindMatches script
    public GameObject destroyEffect;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>(); // Get the FindMatches component
        allTiles = new BackgroundTile[width, height]; // Initialize tiles array
        allDots = new GameObject[width, height]; // Initialize dots array
        SetUp(); // Set up the board
    }

    private void SetUp() {
        // Loop through each position in the board grid
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                UnityEngine.Vector2 tempPosition = new(i, j + offSet); // Set tile position
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, UnityEngine.Quaternion.identity); // Create tile
                backgroundTile.transform.parent = this.transform; // Set parent to board
                backgroundTile.name = "( " + i + ", " + j + " )"; // Name tile for debugging
                int dotToUse = Random.Range(0, dots.Length); // Randomly select a dot to use
                int maxIterations = 0;

                // Ensure the dot does not match existing adjacent dots
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100) {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                // Instantiate the selected dot and set its position
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity);
                dot.GetComponent<Dot>().row = j; // Set dot's row
                dot.GetComponent<Dot>().column = i; // Set dot's column
                dot.transform.parent = this.transform; // Set parent to board
                dot.name = "( " + i + ", " + j + " )"; // Name dot for debugging
                allDots[i, j] = dot; // Store the dot in the array
            }
        }
    }

    // Check if the current position has matching dots
    private bool MatchesAt(int column, int row, GameObject piece) {
        // Check horizontally and vertically for matches
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
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
            // How many elements are in the matched pieces list from find matches?
            if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7) {
                findMatches.CheckBombs();
            }
            findMatches.currentMatches.Remove(allDots[column, row]); // Remove from current matches
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, UnityEngine.Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(allDots[column, row]); // Destroy the matched dot
            allDots[column, row] = null; // Set the array position to null
        }
    }

    // Loop through the board and destroy all matched dots
    public void DestroyMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
                    DestroyMatchesAt(i, j); // Check and destroy matches at each position
                }
            }
        }
        StartCoroutine(DecreaseRowCo()); // Start coroutine to handle row adjustments
    }

    // Coroutine to handle decreasing rows when dots are destroyed
    private IEnumerator DecreaseRowCo() {
        int nullCount = 0; // Count of null dots in a column
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    nullCount++; // Increase count of nulls
                } else if (nullCount > 0) {
                    // Move existing dots down by the number of nulls
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null; // Set current position to null
                }
            }
            nullCount = 0; // Reset null count for the next column
        }
        yield return new WaitForSeconds(.4f); // Wait before refilling the board
        StartCoroutine(FillBoardCo()); // Start coroutine to fill the board
    }

    // Refill the board with new dots
    private void RefillBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] == null) {
                    UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length); // Randomly select a dot to use
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity); // Create dot
                    allDots[i, j] = piece; // Store the dot in the array
                    piece.GetComponentInParent<Dot>().row = j; // Set dot's row
                    piece.GetComponentInParent<Dot>().column = i; // Set dot's column
                }
            }
        }
    }

    // Check if there are any matches on the board
    private bool MatchesOnBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i, j] != null) {
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
        RefillBoard(); // Refill the board
        yield return new WaitForSeconds(.2f); // Wait for a short time
        while (MatchesOnBoard()) {
            yield return new WaitForSeconds(.2f); // Wait before checking for matches
            DestroyMatches(); // Destroy any new matches
        }
        findMatches.currentMatches.Clear();
        currentDot = null;
        yield return new WaitForSeconds(.2f); // Wait before allowing moves again
        currentState = GameState.move; // Set state to allow moves
    }
}