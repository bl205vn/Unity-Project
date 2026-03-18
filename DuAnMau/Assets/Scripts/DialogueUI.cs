using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text TextLabel;

    public bool IsOpen { get; private set; }

    private TypewriterEffect typewriterEffect;

    void Awake()
    {
        // Đảm bảo TypewriterEffect được tạo trong Awake
        EnsureTypewriterEffect();
    }

    void Start()
    {
        IsOpen = false;
        CloseDialogueBox();
    }

    private void EnsureTypewriterEffect()
    {
        // Kiểm tra và tạo TypewriterEffect nếu chưa có
        typewriterEffect = GetComponent<TypewriterEffect>();
        if (typewriterEffect == null)
        {
            typewriterEffect = gameObject.AddComponent<TypewriterEffect>();
            Debug.Log("Auto-added TypewriterEffect component to " + gameObject.name);
        }
    }

    public void ShowDialogue(DialogueObject dialogueObject)
    {
        if (dialogueObject == null)
        {
            Debug.LogError("DialogueObject is null!");
            return;
        }

        // Đảm bảo TypewriterEffect có sẵn trước khi sử dụng
        EnsureTypewriterEffect();

        if (dialogueBox == null)
        {
            Debug.LogError("DialogueBox is null! Assign it in the inspector.");
            return;
        }

        if (TextLabel == null)
        {
            Debug.LogError("TextLabel is null! Assign it in the inspector.");
            return;
        }

        IsOpen = true;
        dialogueBox.SetActive(true);
        
        // Sử dụng TypewriterEffect nếu có, nếu không thì hiển thị text bình thường
        if (typewriterEffect != null)
        {
            StartCoroutine(routine: StepThroughDialogue(dialogueObject));
        }
        else
        {
            Debug.LogWarning("TypewriterEffect is null, using simple text display");
            StartCoroutine(routine: StepThroughDialogueSimple(dialogueObject));
        }
    }

    private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        foreach (string dialogue in dialogueObject.Dialogue)
        {
            yield return typewriterEffect.Run(dialogue, TextLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.V));
        }

        CloseDialogueBox();
    }

    // Phiên bản đơn giản không cần TypewriterEffect
    private IEnumerator StepThroughDialogueSimple(DialogueObject dialogueObject)
    {
        foreach (string dialogue in dialogueObject.Dialogue)
        {
            TextLabel.text = dialogue;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.V));
        }

        CloseDialogueBox();
    }

    private void CloseDialogueBox()
    {
        IsOpen = false;
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
        if (TextLabel != null)
            TextLabel.text = string.Empty;
    }
}
