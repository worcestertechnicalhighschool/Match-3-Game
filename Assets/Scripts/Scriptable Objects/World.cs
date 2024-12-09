using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Creates a new ScriptableObject called "World" that can be added from the Unity editor
[CreateAssetMenu(fileName = "World", menuName = "World")]
public class World : ScriptableObject
{
    // An array of levels in the world
    // Each level contains specific game data such as the game requirements, difficulty, etc.
    public Level[] levels; // List of levels in the world, where each element represents a different level
}