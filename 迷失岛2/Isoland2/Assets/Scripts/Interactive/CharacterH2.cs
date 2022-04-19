using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class CharacterH2 : Interactive
{
    private DialogueController dialogueController;

    private void Awake()
    {
        dialogueController = GetComponent<DialogueController>();
    }

    public override void EmptyClicked()
    {
        // 对话内容A
        if (isDone)
            dialogueController.ShowDialogueFinish();
        else
            dialogueController.ShowDialogueEmpty();
        
    }

    protected override void OnClickedAction()
    {
        // 对话内容B
        dialogueController.ShowDialogueFinish();
    }
}
