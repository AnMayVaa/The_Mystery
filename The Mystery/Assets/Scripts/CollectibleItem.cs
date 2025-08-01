using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // This script should be attached to the collectible item GameObject
    // It will handle the collection of the item by the player
    public ItemData itemData;
    private DialogueStarter dialogueStartert;

    void Start()
    {
        // ดึง DialogueStarter จาก GameObject เดียวกัน
        if (dialogueStartert == null)
            dialogueStartert = GetComponent<DialogueStarter>();

        if (dialogueStartert != null && dialogueStartert.uiPrompt != null)
        {
            dialogueStartert.uiPrompt.SetActive(false); // ซ่อนข้อความ UI ไว้ก่อน
        }
    }

    public void Collect()
    {
        if (itemData != null)
        {
            // Try to access to GameStateManager to collect the item
            // Assuming GameStateManager has a method to collect items
            GameStateManager.Instance.CollectItem(itemData);

            Debug.Log("Item " + itemData.itemName + " collected via 'E' press.");

            // Add a null check here before trying to access dialogueStartert.uiPrompt
            if (dialogueStartert != null && dialogueStartert.uiPrompt != null) // <-- Added null check
            {
                dialogueStartert.uiPrompt.SetActive(false); // ซ่อนข้อความ UI เมื่อเก็บแล้ว
            }
            if (itemData.itemName != "Mention Increase") // ถ้าไอเทมไม่ได้ชื่อ Mention increase ให้ทำลายทิ้งหลังเก็บ
            {
                Destroy(gameObject);
            } 
        }
        else
        {
            Debug.LogWarning("CollectibleItem on " + gameObject.name + " has no ItemData assigned!");
        }
    }
}