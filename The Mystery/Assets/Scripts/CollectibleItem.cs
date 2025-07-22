using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // This script should be attached to the collectible item GameObject
    // It will handle the collection of the item by the player
    public ItemData itemData;
    public GameObject uiPrompt;

    void Start()
    {
        if (uiPrompt != null)
        {
            uiPrompt.SetActive(false); // ซ่อนข้อความ UI ไว้ก่อน
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

            if (uiPrompt != null)
            {
                uiPrompt.SetActive(false); // ซ่อนข้อความ UI เมื่อเก็บแล้ว
            }
            Destroy(gameObject); // ทำลายไอเท็มหลังจากเก็บ
        }
        else
        {
            Debug.LogWarning("CollectibleItem on " + gameObject.name + " has no ItemData assigned!");
        }
    }

    public void ShowPrompt()
    {
        if (uiPrompt != null && !uiPrompt.activeSelf)
        {
            uiPrompt.SetActive(true);
        }
    }

    // ฟังก์ชันสำหรับซ่อนข้อความ UI
    public void HidePrompt()
    {
        if (uiPrompt != null && uiPrompt.activeSelf)
        {
            uiPrompt.SetActive(false);
        }
    }
}