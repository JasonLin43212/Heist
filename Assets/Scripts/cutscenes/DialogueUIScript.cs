using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIScript : MonoBehaviour
{
    // References
    public Image portrait;
    public TextMeshProUGUI nameText, dialogueText;

    public CutsceneController cutsceneController;

    // Text display variables
    [Min(0f)]
    public float characterRevealDelay;

    private const string START_HIDDEN = "<alpha=#00>", END_HIDDEN = "<alpha=#FF>";
    private string targetDialogueText = "";
    private int showDialogueIndex = 0, nextSpaceIndex = 0;
    private bool revealCoroutineGoing = false;

    public void SetDialogueUI(string speaker, string dialogue)
    {
        CharacterData charData = cutsceneController.GetCharacterData(speaker);
        portrait.sprite = charData.sprite;
        nameText.text = charData.displayName;

        // Start text reveal
        dialogueText.text = "";
        targetDialogueText = dialogue;
        showDialogueIndex = 0;
        nextSpaceIndex = 0;

        if (!revealCoroutineGoing) StartCoroutine(DelayTextReveal());
    }

    private bool ShowNewLetter()
    {
        if (showDialogueIndex >= targetDialogueText.Length) return false;  // We've already finished revealing the dialogue
        else if (showDialogueIndex == targetDialogueText.Length - 1)
        {
            dialogueText.text = targetDialogueText;
            showDialogueIndex++;
            return false;  // We just finished revealing the dialogue
        }

        // See if we need to update the next space
        if (nextSpaceIndex < showDialogueIndex)
        {
            nextSpaceIndex = showDialogueIndex;
            while (nextSpaceIndex < targetDialogueText.Length && targetDialogueText[nextSpaceIndex] != ' ')
                nextSpaceIndex++;
        }

        // Compose text string
        string textOutput = targetDialogueText.Substring(0, showDialogueIndex + 1);
        int extraMargin = nextSpaceIndex - showDialogueIndex - 1;
        if (extraMargin > 0)
        {
            string hiddenText = targetDialogueText.Substring(showDialogueIndex + 1, extraMargin);
            textOutput = textOutput + START_HIDDEN + hiddenText + END_HIDDEN;
        }
        dialogueText.text = textOutput;

        showDialogueIndex++;
        return true;
    }

    public bool FinishRevealDialogue()
    {
        // If still revealing dialogue, fast-forward to completion and return false; else return true.
        if (showDialogueIndex >= targetDialogueText.Length) return true;
        showDialogueIndex = targetDialogueText.Length;
        dialogueText.text = targetDialogueText;
        return false;
    }

    private IEnumerator DelayTextReveal()
    {
        revealCoroutineGoing = true;
        while (ShowNewLetter())
        {
            float start = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup < start + characterRevealDelay) yield return null;
        }
        revealCoroutineGoing = false;
    }
}
