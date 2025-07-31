using UnityEngine;
using DialogueEditor;

public class DialoguesMentalMax : MonoBehaviour
{
    [SerializeField] private NPCConversation myConversation; // บทสนทนาที่จะเริ่มเมื่อ Interaction zone โดน และกด E จาก ItemDetector

    [Header("Player Control")]
    [SerializeField]
    [Tooltip("If true, player movement will be frozen during this dialogue")]
    public bool freezePlayerDuringDialogue = true; // Default to true for backward compatibility

    // ฟังก์ชันสำหรับเริ่มบทสนทนา    
    public void StartDialogue()
    {
        if (myConversation != null)
        {
            // เรียกใช้ DialogueManager เพื่อเริ่มบทสนทนา
            ConversationManager.Instance.StartConversation(myConversation);
            Debug.Log("Dialogue started with " + gameObject.name);

            GameStateManager.Instance.playerDuringDialogue = true;
            GameStateManager.Instance.freezePlayerDuringDialogue = freezePlayerDuringDialogue;
        }
        else
        {
            Debug.LogWarning("Dialogue is not assigned in DialogueStarter on " + gameObject.name);
        }
    }
}
