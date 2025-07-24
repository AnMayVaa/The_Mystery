using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon; // Icon for the item

    [Header("Status Effects")]
    public int status1Change; // 
    public int status2Change; //

    // [Header("Item Properties")]
    public string itemID; // Unique identifier for the item

    void OnEnable()
    {
        // Ensure itemID is set to a default value if not specified
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = name; // Use the name of the ScriptableObject as the default ID
        }
    }
}