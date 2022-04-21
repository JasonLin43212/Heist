using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIScript : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI nameText, dialogueText;

    private CutsceneController cutsceneController;

    // Start is called before the first frame update
    void Start()
    {
        cutsceneController = GameState.Instance.CutsceneController;
        SetDialogueUI("Egg", "This is a test dialogue message. This is a test dialogue message. This is a test dialogue message. This is a test dialogue message.");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetDialogueUI(string speaker, string dialogue)
    {
        CharacterData charData = cutsceneController.GetCharacterData(speaker);
        portrait.sprite = charData.sprite;
        nameText.text = charData.displayName;
        dialogueText.text = dialogue;
    }
}
