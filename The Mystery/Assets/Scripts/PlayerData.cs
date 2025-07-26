using System.Collections.Generic; // For List<T>
using UnityEngine; // For Vector3

[System.Serializable]
public class PlayerData
{
    public int mental; //
    public int case_progress; //

    // List<string> inventory; // List to hold item IDs
    public List<string> collectedItemIDs;

    // Position and scene that player was last in
    public Vector3 lastPosition;
    public string lastScene;

    public PlayerData()
    {
        // Initialize default values
        mental = 100; // Default value for status1 
        case_progress = 0; // Default value for status2

        collectedItemIDs = new List<string>();

        lastPosition = Vector3.zero; // Default position
        lastScene = "MainMenu"; // Default scene
    }
}