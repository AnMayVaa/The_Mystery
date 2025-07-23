using UnityEngine;
using DialogueEditor;

public class DialogueStarter : MonoBehaviour
{

    [SerializeField] private NPCConversation myConversation; // บทสนทนาที่จะเริ่มเมื่อ Interaction zone โดน และกด E จาก ItemDetector

    public GameObject uiPrompt; // UI Prompt ที่จะแสดงเมื่อผู้เล่นอยู่ใกล้ NPC
    // ฟังก์ชันสำหรับเริ่มบทสนทนา    
    public void StartDialogue()
    {
        if (myConversation != null)
        {
            // เรียกใช้ DialogueManager เพื่อเริ่มบทสนทนา
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log("Dialogue started with " + gameObject.name);
        }
        else
        {
            Debug.LogWarning("Dialogue is not assigned in DialogueStarter on " + gameObject.name);
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
