using UnityEngine;


[CreateAssetMenu(menuName = "Game/Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField][TextArea] private string[] dialogue;

    public string[] Dialogue => dialogue;
}
