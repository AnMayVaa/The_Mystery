using System.Collections.Generic; // For List<T>

[System.Serializable]
public class PlayerData
{
    public int currentStatus1; //
    public int currentStatus2; //

    // List<string> inventory; // List to hold item IDs
    public List<string> collectedItemIDs;

    public PlayerData()
    {
        // Initialize default values
        currentStatus1 = 100; // Default value for status1 
        currentStatus2 = 50; // Default value for status2
        
        collectedItemIDs = new List<string>();
    }
}