using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UIElements;

public class Board : MonoBehaviour
{
public int width; // Width of the board
public int height; // Height of the board
public GameObject tilePrefab; // Prefab for the background tiles
private BackgroundTile[,] allTiles; // 2D array to hold all background tiles
public GameObject[] dots; // Array of dot prefabs
public GameObject[,] allDots; // 2D array to hold all dots on the board

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height]; // Initialize tiles array
        allDots = new GameObject[width, height]; // Initialize dots array
        SetUp(); // Set up the board
    }

    private void SetUp() {
        // Loop through each position in the board grid
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                UnityEngine.Vector2 tempPosition = new(i, j); // Set tile position
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
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag) {
                return true;
            }
        } else if (column <= 1 || row <= 1) {
            if (row > 1) {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag) {
                    return true;
                }
            }
            if (column > 1) {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag) {
                    return true;
                }
            }
        }
        return false; // No match found
    }

    // Destroy matched dots at the given position
    private void DestroyMatchesAt(int column, int row) {
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
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
    }
}