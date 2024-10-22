using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Class representing each dot on the board
public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; // Current column of the dot
    public int row; // Current row of the dot
    public int previousColumn; // Previous column (for reverting moves)
    public int previousRow; // Previous row (for reverting moves)
    public int targetX; // Target X position for movement
    public int targetY; // Target Y position for movement
    public bool isMatched = false; // Flag to indicate if the dot is part of a match
    private Board board; // Reference to the Board instance
    public GameObject otherDot; // Reference to the dot being swapped with
    private Vector2 firstTouchPosition; // Position of the first touch for swipe detection
    private Vector2 finalTouchPosition; // Position of the final touch for swipe detection
    private Vector2 tempPosition; // Temporary position used for movement
    public float swipeAngle = 0; // Angle of the swipe for direction detection
    public float swipeResist = 1f; // Resistance threshold for swipe detection
    private FindMatches findMatches; // Reference to the FindMatches script for match checking
    
    [Header("Power Up Variables")]
    public bool isColorBomb; // Flag for color bomb power-up
    public bool isColumnBomb; // Flag for column bomb power-up
    public bool isRowBomb; // Flag for row bomb power-up
    public GameObject rowArrow; // Prefab for the visual representation of the row bomb
    public GameObject columnArrow; // Prefab for the visual representation of the column bomb
    public GameObject colorBomb; // Prefab for the visual representation of the color bomb

    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false; // Initialize column bomb status
        isRowBomb = false; // Initialize row bomb status
        board = FindObjectOfType<Board>(); // Find and reference the Board in the scene
        findMatches = FindObjectOfType<FindMatches>(); // Reference to the FindMatches instance
    }

    // Handles right-click to create a color bomb for debugging purposes
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1)) {
            isColorBomb = true; // Set color bomb flag to true
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform; // Set the color bomb as a child of this dot
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the target position based on the current column and row
        targetX = column;
        targetY = row;

        // Move towards the target X position
        if (Mathf.Abs(targetX - transform.position.x) > .1) {
            tempPosition = new Vector2(targetX, transform.position.y); // Set new position for X
            transform.position = Vector2.Lerp(transform.position, tempPosition, .1f); // Smooth movement towards the target
            // Update the board reference if this dot is in the new position
            if (board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject; // Update board array
            }
            findMatches.FindAllMatches(); // Check for matches after moving
        } else {
            // Directly set position if close enough to target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition; // Set to target position
            board.allDots[column, row] = this.gameObject; // Update board reference
        }

        // Move towards the target Y position
        if (Mathf.Abs(targetY - transform.position.y) > .1) {
            tempPosition = new Vector2(transform.position.x, targetY); // Set new position for Y
            transform.position = Vector2.Lerp(transform.position, tempPosition, .1f); // Smooth movement towards target
            // Update the board reference if this dot is in the new position
            if (board.allDots[column, row] != this.gameObject) {
                board.allDots[column, row] = this.gameObject; // Update board array
            }
            findMatches.FindAllMatches(); // Check for matches after moving
        } else {
            // Directly set position if close enough to target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition; // Set to target position
            board.allDots[column, row] = this.gameObject; // Update board reference
        }
    }

    // Coroutine to check the validity of the move
    public IEnumerator CheckMoveCo() {
        if (isColorBomb) {
            findMatches.MatchPiecesOfColor(otherDot.tag); // Match pieces of the same color
            isMatched = true; // Mark current dot as matched
        } else if (otherDot.GetComponent<Dot>().isColorBomb) {
            findMatches.MatchPiecesOfColor(this.gameObject.tag); // Match pieces of the current dot's color
            otherDot.GetComponent<Dot>().isMatched = true; // Mark the other dot as matched
        }
        
        yield return new WaitForSeconds(.2f); // Wait before checking match validity
        
        if (otherDot != null) {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) {
                // If no match is found, revert the move
                otherDot.GetComponent<Dot>().row = row; // Revert the other dot's row
                otherDot.GetComponent<Dot>().column = column; // Revert the other dot's column
                row = previousRow; // Restore previous row
                column = previousColumn; // Restore previous column
                board.currentDot = null; // Clear the current dot reference
                board.currentState = GameState.move; // Allow player to move again
            } else {
                // If there is a match, destroy matched dots
                board.DestroyMatches(); // Trigger match destruction process
                yield return new WaitForSeconds(.2f); // Wait for destruction to complete
            }
        }
    }

    // Handle mouse down event for initiating touch input
    private void OnMouseDown() {
        if (board.currentState == GameState.move) {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get initial touch position
        }
    }

    // Handle mouse up event for finalizing touch input
    private void OnMouseUp() {
        if (board.currentState == GameState.move) {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get final touch position
            CalculateAngle(); // Calculate swipe angle and direction
        }
    }

    // Calculate the angle of the swipe to determine movement direction
    void CalculateAngle() {
        // Check if swipe distance exceeds the resistance threshold
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            // Calculate the swipe angle in degrees
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle); // Log the angle for debugging
            MovePieces(); // Move the pieces based on calculated swipe direction
            board.currentState = GameState.wait; // Change state to wait while processing the move
            board.currentDot = this; // Set current dot reference
        } else {
            board.currentState = GameState.move; // Reset state if swipe is too short
        }
    }

    // Move the pieces based on the swipe direction
    void MovePieces() {
        // Determine the direction of the swipe and move the dots accordingly
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            // Right Swipe
            otherDot = board.allDots[column + 1, row]; // Get the dot to the right
            previousRow = row; // Save previous row
            previousColumn = column; // Save previous column
            otherDot.GetComponent<Dot>().column -= 1; // Move that dot left
            column += 1; // Update current dot's column
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            // Up Swipe
            otherDot = board.allDots[column, row + 1]; // Get the dot above
            previousRow = row; // Save previous row
            previousColumn = column; // Save previous column
            otherDot.GetComponent<Dot>().row -= 1; // Move that dot down
            row += 1; // Update current dot's row
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            // Left Swipe
            otherDot = board.allDots[column - 1, row]; // Get the dot to the left
            previousRow = row; // Save previous row
            previousColumn = column; // Save previous column
            otherDot.GetComponent<Dot>().column += 1; // Move that dot right
            column -= 1; // Update current dot's column
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            // Down Swipe
            otherDot = board.allDots[column, row - 1]; // Get the dot below
            previousRow = row; // Save previous row
            previousColumn = column; // Save previous column
            otherDot.GetComponent<Dot>().row += 1; // Move that dot up
            row -= 1; // Update current dot's row
        }
        StartCoroutine(CheckMoveCo()); // Start the coroutine to check move validity
    }

    // Find matches with adjacent dots
    void FindMatches() {
        // Check for matches with dots to the left and right
        if (column > 0 && column < board.width - 1) {
            GameObject leftDot1 = board.allDots[column - 1, row]; // Dot to the left
            GameObject rightDot1 = board.allDots[column + 1, row]; // Dot to the right
            if (leftDot1 != null && rightDot1 != null) {
                // Check if both adjacent dots match the current dot's tag
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag) {
                    leftDot1.GetComponent<Dot>().isMatched = true; // Mark left dot as matched
                    rightDot1.GetComponent<Dot>().isMatched = true; // Mark right dot as matched
                    isMatched = true; // Mark current dot as matched
                }
            }
        }

        // Check for matches with dots above and below
        if (row > 0 && row < board.height - 1) {
            GameObject upDot1 = board.allDots[column, row + 1]; // Dot above
            GameObject downDot1 = board.allDots[column, row - 1]; // Dot below
            if (upDot1 != null && downDot1 != null) {
                // Check if both adjacent dots match the current dot's tag
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag) {
                    upDot1.GetComponent<Dot>().isMatched = true; // Mark upper dot as matched
                    downDot1.GetComponent<Dot>().isMatched = true; // Mark lower dot as matched
                    isMatched = true; // Mark current dot as matched
                }
            }
        }
    }

    // Method to create a row bomb power-up
    public void MakeRowBomb() {
        isRowBomb = true; // Set row bomb flag
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform; // Set arrow as a child of this dot
    }

    // Method to create a column bomb power-up
    public void MakeColumnBomb() {
        isColumnBomb = true; // Set column bomb flag
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform; // Set arrow as a child of this dot
    }
}