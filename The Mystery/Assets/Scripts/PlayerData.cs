using System.Collections.Generic; // For List<T>

[System.Serializable]
public class PlayerData
{
    public int mental; //
    public int case_progress; //

    // List<string> inventory; // List to hold item IDs
    public List<string> collectedItemIDs;

    public PlayerData()
    {
        // Initialize default values
        mental = 100; // Default value for status1 
        case_progress = 0; // Default value for status2
        
        collectedItemIDs = new List<string>();
    }
}