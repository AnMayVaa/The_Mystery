using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // This script should be attached to the collectible item GameObject
    // It will handle the collection of the item by the player
    public ItemData itemData;

    void OnTriggerEnter2D(Collider2D other)
    {
        //
        if (other.CompareTag("Player"))
        {
            // Try to access to GameStateManager to collect the item
            // Assuming GameStateManager has a method to collect items
            GameStateManager.Instance.CollectItem(itemData);

            // Optionally, you can destroy the item after collection
            Destroy(gameObject);
        }
    }
}