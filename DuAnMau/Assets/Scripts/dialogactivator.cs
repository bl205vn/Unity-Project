using System.Xml.Serialization;
using UnityEngine;

public class dialogactivator : MonoBehaviour, IInteractiveable
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueObject dialogueObject;
    [SerializeField] private string npcName = "NPC"; // Tên NPC để debug
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
        {
            player.interactiveable = this;
            if (showDebugInfo)
                Debug.Log($"{npcName}: Player entered dialogue range");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
        {
            if (player.interactiveable is dialogactivator dialogactivator && dialogactivator == this)
            {
                player.interactiveable = null;
                if (showDebugInfo)
                    Debug.Log($"{npcName}: Player left dialogue range");
            }
        }
    }
    
    public void interact(Player player)
    {
        // Kiểm tra null trước khi sử dụng
        if (dialogueObject == null)
        {
            Debug.LogError($"DialogueObject is null in {npcName}! Assign a DialogueObject in the inspector.");
            return;
        }

        if (player.DialogueUI == null)
        {
            Debug.LogError($"DialogueUI is null in Player: {player.gameObject.name}");
            return;
        }

        if (showDebugInfo)
            Debug.Log($"{npcName}: Starting dialogue with player");

        player.DialogueUI.ShowDialogue(dialogueObject);
    }

    // Phương thức để thay đổi dialogue động (có thể gọi từ script khác)
    public void SetDialogue(DialogueObject newDialogue)
    {
        dialogueObject = newDialogue;
        if (showDebugInfo)
            Debug.Log($"{npcName}: Dialogue changed");
    }

    // Phương thức để lấy dialogue hiện tại
    public DialogueObject GetCurrentDialogue()
    {
        return dialogueObject;
    }
}
