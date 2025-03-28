using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

// Enum to represent the current state of the game
public enum GameState
{
    wait, // The game is waiting for a player action
    move,  // The game is ready for player movement
    win, // The player has won the game
    lose, // The player has lost the game
    pause // The game is paused
}

// Enum to define different types of tiles on the board
public enum TileKind
{
    Breakable, // A tile that can be destroyed by the player
    Blank,     // An empty space on the board (no tile)
    Normal,     // A standard, unbreakable tile
    Lock,
    Dirt,
    Chocolate
}

[System.Serializable]
public class MatchType {
    public int type;
    public string color;
}

// Class to represent a tile with its position and type
[System.Serializable]
public class TileType
{
    public int x; // X coordinate of the tile
    public int y; // Y coordinate of the tile
    public TileKind tileKind; // Type of the tile (Breakable, Blank, or Normal)
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Object Settings")]
    public int[] scoreGoals; // The score goals required to pass the level
    public World world; // Reference to the World object that contains all levels

    [Header("Board Settings")]
    // The current state of the game, determining whether the player can interact or not
    public GameState currentState = GameState.move; // The game is ready for player movement by default
    public int width; // The number of columns on the board
    public int height; // The number of rows on the board
    public int offSet; // Offset used to position tiles correctly on the board

    [Header("Prefab Settings")]
    public GameObject tilePrefab; // Prefab used to create background tiles
    public GameObject breakableTilePrefab; // Prefab used to create breakable tiles
    public GameObject lockTilePrefab;
    public GameObject dirtTilePrefab;
    public GameObject chocolateTilePrefab;
    public GameObject destroyEffect; // The effect (e.g., animation, particle) shown when a dot is destroyed
    public GameObject[] dots; // Array of dot prefabs, each representing a different type of dot
    public GameObject[,] allDots; // 2D array that holds all the dots currently placed on the board

    [Header("Dot Settings")]
    public Dot currentDot; // Reference to the currently selected dot on the board
    public int basePieceValue = 20; // Base score value for each dot, used for scoring

    [Header("Match Settings")]
    public MatchType matchType;
    public TileType[] boardLayout; // Defines the layout of the board (including background tiles, blank spaces, etc.)
    private FindMatches findMatches; // Reference to the FindMatches script used for detecting matching dots
    private int streakValue = 1; // Multiplier for consecutive matches (combo streak)
    private ScoreManager scoreManager; // Reference to the ScoreManager used to manage and update the player's score
    private SoundManager soundManager; // Reference to the SoundManager, used to play sound effects
    private GoalManager goalManager; // Reference to the GoalManager responsible for managing and updating the game goals
    private bool[,] blankSpaces; // 2D array used to track which spaces are blank (empty)
    private BackgroundTile[,] breakableTiles; // 2D array used to hold references to breakable tiles
    public BackgroundTile[,] lockTiles;
    private BackgroundTile[,] dirtTiles;
    private BackgroundTile[,] chocolateTiles;
    public float refillDelay = 0.2f; // Time delay between refilling the board after a match is destroyed

    [Header("Level Settings")]
    public int level; // The current level the player is on

    // This method runs when the Board script is first initialized
    private void Awake()
    {
        // Check if the PlayerPrefs has a stored key for "Current Level"
        if (PlayerPrefs.HasKey("Current Level"))
        {
            // If the key exists, retrieve the level number from PlayerPrefs and assign it to the 'level' variable
            level = PlayerPrefs.GetInt("Current Level");
        }

        // Initialize the board layout if a world and level are assigned
        if (world != null)
        {
            // Check if the current level is within the bounds of the world's levels array
            if (level < world.levels.Length)
            {
                // If a valid level is defined, initialize the board settings based on the level data
                if (world.levels[level] != null)
                {
                    // Set the width of the board based on the current level
                    width = world.levels[level].width;

                    // Set the height of the board based on the current level
                    height = world.levels[level].height;

                    // Set the available dots for the current level
                    dots = world.levels[level].dots;

                    // Set the score goals for the current level
                    scoreGoals = world.levels[level].scoreGoals;

                    // Set the board layout for the current level
                    boardLayout = world.levels[level].boardLayout;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize references and arrays
        goalManager = FindObjectOfType<GoalManager>(); // Find and store the GoalManager instance to manage level goals
        soundManager = FindObjectOfType<SoundManager>(); // Find and store the SoundManager for playing sounds
        scoreManager = FindObjectOfType<ScoreManager>(); // Find and store the ScoreManager to track the player's score
        breakableTiles = new BackgroundTile[width, height]; // Initialize a 2D array to hold the background tiles (e.g., breakable, unbreakable)
        lockTiles = new BackgroundTile[width, height];
        dirtTiles = new BackgroundTile[width, height];
        chocolateTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>(); // Retrieve the FindMatches component that will handle match logic
        blankSpaces = new bool[width, height]; // Initialize a 2D array to track which spaces are blank (no dots)
        allDots = new GameObject[width, height]; // Initialize a 2D array to hold references to the dots in each space on the board
        // Call SetUp() to initialize the board, place tiles, and randomly populate it with dots
        SetUp();
        currentState = GameState.pause; // Initially pause the game (waiting for player to start or resume)
    }

    // Generate blank spaces on the board based on the board layout
    public void GenerateBlankSpaces()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true; // Mark space as blank
            }
        }
    }

    // Generate breakable tiles on the board based on the board layout
    public void GenerateBreakableTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                // Set the position for the breakable tile
                UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(boardLayout[i].x, boardLayout[i].y);
                // Instantiate the breakable tile prefab at the designated position
                GameObject tile = Instantiate(breakableTilePrefab, tempPosition, UnityEngine.Quaternion.identity);
                // Store a reference to the BackgroundTile component
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateLockTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                // Set the position for the lock tile
                UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(boardLayout[i].x, boardLayout[i].y);
                // Instantiate the breakable tile prefab at the designated position
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, UnityEngine.Quaternion.identity);
                // Store a reference to the BackgroundTile component
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateDirtTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Dirt)
            {
                // Set the position for the lock tile
                UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(boardLayout[i].x, boardLayout[i].y);
                // Instantiate the breakable tile prefab at the designated position
                GameObject tile = Instantiate(dirtTilePrefab, tempPosition, UnityEngine.Quaternion.identity);
                // Store a reference to the BackgroundTile component
                dirtTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void GenerateChocolateTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Chocolate)
            {
                // Set the position for the lock tile
                UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(boardLayout[i].x, boardLayout[i].y);
                // Instantiate the breakable tile prefab at the designated position
                GameObject tile = Instantiate(chocolateTilePrefab, tempPosition, UnityEngine.Quaternion.identity);
                // Store a reference to the BackgroundTile component
                chocolateTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    // Set up the board by instantiating background tiles and randomly placing dots
    private void SetUp()
    {
        // Generate the blank spaces on the board (spaces where no dots will be placed)
        GenerateBlankSpaces();

        // Generate breakable tiles on the board (if applicable, e.g., destructible tiles)
        GenerateBreakableTiles();

        // Generate lock tiles on the board (if applicable, e.g., locking a fruit from being moved)
        GenerateLockTiles();

        // Generate dirt tiles on the board (if applicable, e.g., dirt taking up board space)
        GenerateDirtTiles();

        // Generate dirt tiles on the board (if applicable, e.g., dirt taking up board space)
        GenerateChocolateTiles();

        // Loop through each column (i) and each row (j) to place tiles and dots
        for (int i = 0; i < width; i++)
        { // Loop through each column
            for (int j = 0; j < height; j++)
            { // Loop through each row
              // Check if the current space is not a blank space (i.e., it can hold a tile and dot)
                if (!blankSpaces[i, j] && !dirtTiles[i, j] && !chocolateTiles[i, j])
                {
                    // Calculate the position for the tile and dot based on the column (i) and row (j) indices
                    // Offset is added to vertical positioning to ensure proper placement
                    UnityEngine.Vector2 tempPosition = new(i, j + offSet);
                    UnityEngine.Vector2 tilePosition = new UnityEngine.Vector2(i, j);

                    // Instantiate a new background tile at the calculated position
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, UnityEngine.Quaternion.identity);

                    // Set the parent of the background tile to the board for hierarchy organization in the scene
                    backgroundTile.transform.parent = this.transform;

                    // Name the background tile for easier identification and debugging in the hierarchy (coordinates of the tile)
                    backgroundTile.name = "( " + i + ", " + j + " )";

                    // Randomly select a dot prefab from the available array of dots
                    int dotToUse = Random.Range(0, dots.Length);

                    // Max iterations to prevent potential infinite loops when checking for adjacent dots
                    int maxIterations = 0;

                    // Ensure that the randomly selected dot does not create a match with adjacent dots
                    // Keep selecting a new dot if it causes a match with neighboring dots
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length); // Select a new dot if it causes a match
                        maxIterations++; // Increment the iteration counter
                    }

                    // Reset the iteration counter to zero for potential future use
                    maxIterations = 0;

                    // Instantiate the selected dot at the calculated position
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity);

                    // Set the row and column for the dot to help with grid management
                    dot.GetComponent<Dot>().row = j; // Set the row index for the dot
                    dot.GetComponent<Dot>().column = i; // Set the column index for the dot

                    // Set the parent of the dot to the board for hierarchy organization in the scene
                    dot.transform.parent = this.transform;

                    // Name the dot for easier identification and debugging in the hierarchy (coordinates of the dot)
                    dot.name = "( " + i + ", " + j + " )";

                    // Store the newly instantiated dot in the allDots array at the corresponding position
                    allDots[i, j] = dot;
                }
            }
        }
    }

    // Check if the current position has matching dots
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        // Check for horizontal and vertical matches
        if (column > 1 && row > 1)
        {
            // Check for a horizontal match with the two dots to the left
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true; // Found a horizontal match
                }
            }
            // Check for a vertical match with the two dots above
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true; // Found a vertical match
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            // Handle edge cases where the dot is near the edge of the board
            if (row > 1)
            {
                // Check for a vertical match with the two dots above
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true; // Found a vertical match
                    }
                }
            }
            if (column > 1)
            {
                // Check for a horizontal match with the two dots to the left
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true; // Found a horizontal match
                    }
                }
            }
        }
        return false; // No match found at the specified position
    }

    // Determine if the current matches are in a single row or column
    private MatchType ColumnOrRow()
    {
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;
        matchType.type = 0;
        matchType.color = "";
        for (int i = 0; i < matchCopy.Count; i++)
        {
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;
            for (int j = 0; j < matchCopy.Count; j++)
            {
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatch++;
                }
                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }
        matchType.type = 0;
        matchType.color = "";
        return matchType;
    }

    // Check if any bombs need to be created based on current matches
    private void CheckToMakeBombs()
    {
        if (findMatches.currentMatches.Count > 3)
        {
            MatchType typeOfMatch = ColumnOrRow();
            if (typeOfMatch.type == 1)
            {
                // If matches are in a line
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    // Create a color bomb if the current dot is matched and not already a color bomb
                    currentDot.isMatched = false; // Reset matched status
                    currentDot.MakeColorBomb(); // Create a color bomb
                } 
                else
                { // Check the other dot in case it is matched
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false; // Reset matched status
                            otherDot.MakeColorBomb(); // Create a color bomb
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                // If matches are not in a line, check for adjacent bomb creation
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    // Create an adjacent bomb if the current dot is matched and not already an adjacent bomb
                    currentDot.isMatched = false; // Reset matched status
                    currentDot.MakeAdjacentBomb(); // Create an adjacent bomb
                }
                else if (currentDot.otherDot != null)
                { // Check the other dot for adjacent bomb creation
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                    {
                        otherDot.isMatched = false; // Reset matched status
                        otherDot.MakeAdjacentBomb(); // Create an adjacent bomb
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                findMatches.CheckBombs(typeOfMatch); // Check for any bomb effects based on matches
            }
        }
    } 

    public void BombRow(int row)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (dirtTiles[i, j] != null)
                {
                    dirtTiles[i, row].TakeDamage(1);
                    if (dirtTiles[i, row].hitPoints <= 0)
                    {
                        dirtTiles[i, row] = null;
                    }
                }
            }
        }
    }

    public void BombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (dirtTiles[i, j] != null)
                {
                    dirtTiles[column, i].TakeDamage(1);
                    if (dirtTiles[column, i].hitPoints <= 0)
                    {
                        dirtTiles[column, i] = null;
                    }
                }
            }
        }
    }

    // Destroy matched dots at the specified column and row
    private void DestroyMatchesAt(int column, int row)
    {
        // Check if the dot at the given position is marked as "matched"
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            // Check if there is a breakable tile at the current position
            if (breakableTiles[column, row] != null)
            {
                // Apply damage to the breakable tile (e.g., it may get destroyed after several hits)
                breakableTiles[column, row].TakeDamage(1);

                // If the breakable tile has no hit points left, remove the reference to it from the board
                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null; // Nullify the reference if the tile is destroyed
                }
            }
            // Check if there is a lock tile at the current position
            if (lockTiles[column, row] != null)
            {
                // Apply damage to the lock tile (e.g., it may get destroyed after several hits)
                lockTiles[column, row].TakeDamage(1);

                // If the breakable tile has no hit points left, remove the reference to it from the board
                if (lockTiles[column, row].hitPoints <= 0)
                {
                    lockTiles[column, row] = null; // Nullify the reference if the tile is destroyed
                }
            }
            if (dirtTiles[column, row] != null)
            {
                DamageDirt(column , row);
            }
            if (chocolateTiles[column, row] != null)
            {
                DamageChocolate(column, row);
            }

            // If a GoalManager is present, check if the current dot's tag matches any goals
            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString()); // Compare the dot's tag to the goal
                goalManager.UpdateGoals(); // Update the goal progress (e.g., increment goal counter)
            }
            // Play a sound effect for the destruction (if the sound manager exists)
            if (soundManager != null)
            {
                soundManager.PlayRandomDestroyNoise(); // Play a random destruction sound from the SoundManager
            }

            // Instantiate the destruction effect at the position of the matched dot
            GameObject particle = Instantiate(destroyEffect, allDots[column, row].transform.position, UnityEngine.Quaternion.identity);

            // Destroy the particle effect after 0.5 seconds to clean up the scene
            Destroy(particle, 0.5f);

            // Remove the matched dot from the board by destroying its GameObject
            Destroy(allDots[column, row]);

            // Update the score by multiplying the base score value by the streak value for combos
            scoreManager.IncreaseScore(basePieceValue * streakValue);

            // Set the array position to null for cleanup, ensuring the space is free for new dots
            allDots[column, row] = null;
        }
    }

    // Loop through the entire board and destroy all matched dots
    public void DestroyMatches()
    {
        // If there are 4 or more matched dots (a special match), check if bombs should be created
        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs(); // Create bombs or power-ups for larger matches
        }        
        for (int i = 0; i < width; i++)
        { // Iterate through each column
            for (int j = 0; j < height; j++)
            { // Iterate through each row
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j); // Check and destroy matches at each position
                }
            }
        }
        findMatches.currentMatches.Clear(); // Clear the list of current matches
        StartCoroutine(DecreaseRowCo2()); // Start coroutine to handle row adjustments after destruction
    }

    private void DamageDirt(int column, int row)
    {
        if (column > 0)
        {
            dirtTiles[column - 1, row].TakeDamage(1);
            if (dirtTiles[column - 1, row])
            {
                
                if (dirtTiles[column - 1, row].hitPoints <= 0)
                {
                    dirtTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            dirtTiles[column + 1, row].TakeDamage(1);
            if (dirtTiles[column + 1, row])
            {
                if (dirtTiles[column + 1, row].hitPoints <= 0)
                {
                    dirtTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            dirtTiles[column, row - 1].TakeDamage(1);
            if (dirtTiles[column, row - 1])
            {
                if (dirtTiles[column, row - 1].hitPoints <= 0)
                {
                    dirtTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            dirtTiles[column, row + 1].TakeDamage(1);
            if (dirtTiles[column, row + 1])
            {
                if (dirtTiles[column, row + 1].hitPoints <= 0)
                {
                    dirtTiles[column, row + 1] = null;
                }
            }
        }
    }

    private void DamageChocolate(int column, int row)
    {
        if (column > 0)
        {
            if (chocolateTiles[column - 1, row])
            {
                chocolateTiles[column - 1, row].TakeDamage(1);
                if (chocolateTiles[column - 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column - 1, row] = null;
                }
            }
        }
        if (column < width - 1)
        {
            chocolateTiles[column + 1, row].TakeDamage(1);
            if (chocolateTiles[column + 1, row])
            {
                if (chocolateTiles[column + 1, row].hitPoints <= 0)
                {
                    chocolateTiles[column + 1, row] = null;
                }
            }
        }
        if (row > 0)
        {
            chocolateTiles[column, row - 1].TakeDamage(1);
            if (chocolateTiles[column, row - 1])
            {
                if (chocolateTiles[column, row - 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row - 1] = null;
                }
            }
        }
        if (row < height - 1)
        {
            chocolateTiles[column, row + 1].TakeDamage(1);
            if (chocolateTiles[column, row + 1])
            {
                if (chocolateTiles[column, row + 1].hitPoints <= 0)
                {
                    chocolateTiles[column, row + 1] = null;
                }
            }
        }
    }

    // Coroutine to handle adjusting rows when dots are destroyed
    private IEnumerator DecreaseRowCo2()
    {
        // Loop through each column
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Check for blank spaces and null dots
                if (!blankSpaces[i, j] && !dirtTiles[i, j] && allDots[i, j] == null && !chocolateTiles[i, j])
                {
                    // Move dots down to fill the empty space
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allDots[i, k] != null)
                        {
                            // Update the dot's row to the new position
                            allDots[i, k].GetComponent<Dot>().row = j;
                            allDots[i, k] = null; // Set the original position to null
                            break; // Exit the loop once a dot is moved
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f); // Wait for 0.4 seconds before refilling the board
        StartCoroutine(FillBoardCo()); // Start coroutine to fill the board with new dots
    }

    // Refill the board with new dots in null positions
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        { // Iterate through each column
            for (int j = 0; j < height; j++)
            { // Iterate through each row
                // Check for empty positions that are not blank spaces
                if (allDots[i, j] == null && !blankSpaces[i, j] && !dirtTiles[i, j] && !chocolateTiles[i, j])
                {
                    // Set the new position for the dot
                    UnityEngine.Vector2 tempPosition = new UnityEngine.Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length); // Randomly select a dot prefab
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length); // Randomly select a dot prefab
                    }
                    maxIterations = 0;
                    // Instantiate a new dot at the calculated position
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, UnityEngine.Quaternion.identity);
                    allDots[i, j] = piece; // Store the new dot in the array
                    piece.GetComponentInParent<Dot>().row = j; // Set the dot's row
                    piece.GetComponentInParent<Dot>().column = i; // Set the dot's column
                }
            }
        }
    }

    // Check if there are any matches currently present on the board
    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();
        for (int i = 0; i < width; i++)
        { // Iterate through each column
            for (int j = 0; j < height; j++)
            { // Iterate through each row
                if (allDots[i, j] != null)
                { // Check for a valid dot
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true; // A match has been found
                    }
                }
            }
        }
        return false; // No matches found on the board
    }

    // Coroutine to refill the board with new dots after destroying any matches
    private IEnumerator FillBoardCo()
    {
        // Wait for a short period (0.2 seconds) to allow the board to settle before checking for new matches
        yield return new WaitForSeconds(refillDelay);

        // Refill the board with new dots after clearing the matched dots
        RefillBoard();

        // Wait briefly (0.2 seconds) before allowing the player to make moves again
        yield return new WaitForSeconds(refillDelay);

        // Continuously check for new matches on the board until no more are found
        while (MatchesOnBoard())
        {
            // Increment the streak value to reward consecutive matches
            streakValue++;
            // Destroy any new matches found on the board
            DestroyMatches();
            yield break;
        }
        currentDot = null;

        // If the board is deadlocked (no valid moves are available), shuffle the board to create new possibilities
        if (IsDeadlocked())
        {
            ShuffleBoard(); // Shuffle the board to prevent deadlock
            Debug.Log("Deadlocked!!! :D"); // Log that the board was shuffled due to a deadlock situation
        }

        if (currentState != GameState.pause)
        {
            // After all matches are cleared and the board is refilled or shuffled, set the game state back to 'move'
            // This allows the player to start making moves again
            currentState = GameState.move;
        }

        // Reset the streak value to 1, as the player starts a new sequence of moves
        streakValue = 1;
    }


    // Switch the positions of two dots on the board
    private void SwitchPieces(int column, int row, UnityEngine.Vector2 direction)
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            // Temporarily hold the dot at the new position
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            // Move the dot from its original position to the new position
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            // Place the dot from the new position back to the original position
            allDots[column, row] = holder;
        }
    }

    // Check if there are any valid matches (3 or more dots in a row/column)
    private bool CheckForMatches()
    {
        // Loop through each cell on the board to check for horizontal and vertical matches
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Check for horizontal matches (3 or more dots in a row)
                    if (i < width - 2)
                    {
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true; // Match found horizontally
                            }
                        }
                    }
                    // Check for vertical matches (3 or more dots in a column)
                    if (j < height - 2)
                    {
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true; // Match found vertically
                            }
                        }
                    }
                }
            }
        }
        return false; // No matches found
    }

    // Switch two adjacent dots and check if a match occurs
    public bool SwitchAndCheck(int column, int row, UnityEngine.Vector2 direction)
    {
        SwitchPieces(column, row, direction); // Swap the two dots
        // Check if the swap resulted in a match
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction); // If a match is found, revert the swap
            return true; // Return true if the swap resulted in a match
        }
        SwitchPieces(column, row, direction); // If no match, revert the swap
        return false; // Return false if no match was found
    }

    // Check if the board is deadlocked (no possible matches or valid moves)
    private bool IsDeadlocked()
    {
        // Loop through every dot on the board and check if a valid move is possible
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Check for a valid move to the right
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, UnityEngine.Vector2.right))
                        {
                            return false; // A valid move was found, so it's not deadlocked
                        }
                    }
                    // Check for a valid move upwards
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, UnityEngine.Vector2.up))
                        {
                            return false; // A valid move was found, so it's not deadlocked
                        }
                    }
                }
            }
        }
        return true; // No valid moves were found, the board is deadlocked
    }

    // Shuffle the board when it's deadlocked to ensure valid moves can be made
    private void ShuffleBoard()
    {
        List<GameObject> newBoard = new List<GameObject>();
        // Add all non-null dots to the newBoard list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }

        // Shuffle the board by placing new random dots in the available spaces
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j] && !dirtTiles[i, j] && !chocolateTiles[i, j]) // Only place dots in non-blank spaces
                {
                    int pieceToUse = Random.Range(0, newBoard.Count); // Randomly select a new dot
                    int maxIterations = 0; // Counter to prevent infinite loops when checking for adjacent matches
                                           // Ensure the randomly selected dot does not match any adjacent dots
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count); // Select a new dot if it matches an adjacent one
                        maxIterations++; // Increment iteration count to avoid infinite loop
                    }
                    maxIterations = 0; // Reset counter for potential future use
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = newBoard[pieceToUse]; // Place the new dot on the board
                    newBoard.Remove(newBoard[pieceToUse]); // Remove the selected dot from the newBoard list
                }
            }
        }
        // If the board is still deadlocked after shuffling, try shuffling again
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}