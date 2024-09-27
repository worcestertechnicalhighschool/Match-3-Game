using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; // Current column of the dot
    public int row; // Current row of the dot
    public int previousColumn; // Previous column (for reverting moves)
    public int previousRow; // Previous row (for reverting moves)
    public int targetX; // Target X position for movement
    public int targetY; // Target Y position for movement
    public bool isMatched = false; // Flag for matching status
    private Board board; // Reference to the board
    private GameObject otherDot; // Reference to the dot being swapped with
    private Vector2 firstTouchPosition; // Position of the first touch
    private Vector2 finalTouchPosition; // Position of the final touch
    private Vector2 tempPosition; // Temporary position for movement
    public float swipeAngle = 0; // Angle of the swipe
    public float swipeResist = 1f; // Resistance threshold for swipe detection

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>(); // Find and reference the Board
        targetX = (int)transform.position.x; // Initialize target position
        targetY = (int)transform.position.y;
        row = targetY;
        column = targetX;
        previousRow = row;
        previousColumn = column;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches(); // Check for matches in the current position
        if (isMatched) {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f); // Change color for matched dots
        }
        // Update target position
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) {
            // Move towards target X position
            tempPosition = new Vector2(targetX, transform.position.y); // Set new position
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f); // Smooth movement
        } else {
            // Directly set position if close enough
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;  // Update board reference
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) {
            // Move towards target Y position
            tempPosition = new Vector2(transform.position.x, targetY); // Set new position
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f); // Smooth movement
        } else {
            // Directly set position if close enough
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject; // Update board reference
        }
    }

    // Coroutine to check the validity of the move
    public IEnumerator CheckMoveCo() {
        yield return new WaitForSeconds(.5f); // Wait before checking
        if (otherDot != null) {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched) {
                // If no match, revert the move
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            } else {
                // If there is a match, destroy matched dots
                board.DestroyMatches();
            }
            otherDot = null; // Reset otherDot reference
        }
    }

    // Handle mouse down event for touch input
    private void OnMouseDown() {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    // Handle mouse up event for touch input
    private void OnMouseUp() {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    // Calculate the angle of the swipe
    void CalculateAngle() {
        // Calculate the angle of the swipe based on touch movement
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist) {
            // Calculate the angle in degrees
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            Debug.Log(swipeAngle); // Log the angle for debugging
            MovePieces(); // Move the pieces based on swipe direction
        }
    }

    void MovePieces() {
        // Determine the direction of the swipe and move the dots accordingly
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            // Right Swipe
            otherDot = board.allDots[column + 1, row]; // Get the dot to the right
            otherDot.GetComponent<Dot>().column -= 1; // Move that dot left
            column += 1; // Update current dot's column
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            // Up Swipe
            otherDot = board.allDots[column, row + 1]; // Get the dot above
            otherDot.GetComponent<Dot>().row -= 1; // Move that dot down
            row += 1; // Update current dot's row
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            // Left Swipe
            otherDot = board.allDots[column - 1, row]; // Get the dot to the left
            otherDot.GetComponent<Dot>().column += 1; // Move that dot right
            column -= 1; // Update current dot's column
        } else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) {
            // Down Swipe
            otherDot = board.allDots[column, row - 1]; // Get the dot below
            otherDot.GetComponent<Dot>().row += 1; // Move that dot up
            row -= 1; // Update current dot's row
        }
        StartCoroutine(CheckMoveCo()); // Start the coroutine to check the move validity
    }

    void FindMatches() {
        // Check for matches with adjacent dots
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
}