using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public GameObject dialogueUI;

    // Character info
    public List<CharacterData> characterList;
    private Dictionary<string, CharacterData> characterDatabase;

    // Cutscene variables
    private bool inCutscene = false;
    private Cutscene currentCutscene = null;
    private int frameIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        LoadCharactersToDictionary();
        dialogueUIScript.cutsceneController = this;
        dialogueUI.transform.GetChild(0).gameObject.SetActive(true);
        dialogueUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isGamePaused && inCutscene && Input.GetMouseButtonDown(0))
        {
            // Clicked during a cutscene; progress the dialogue
            if (dialogueUIScript.FinishRevealDialogue())
            {
                frameIndex++;
                if (frameIndex < currentCutscene.NumFrames) UpdateDialogueUI();
                else
                {
                    EndCutscene();
                    GameState.Instance.GameController.resumeStopwatch();
                }
            }
        }
    }

    public void StartCutscene(Cutscene cutscene)
    {
        if (cutscene.NumFrames < 1) return;
        inCutscene = true;
        currentCutscene = cutscene;
        frameIndex = 0;
        UpdateDialogueUI();
    }

    private void EndCutscene()
    {
        inCutscene = false;
        dialogueUI.SetActive(false);
    }

    private void UpdateDialogueUI()
    {
        if (frameIndex >= currentCutscene.NumFrames) return;
        (string speaker, string dialogue) = currentCutscene[frameIndex];
        dialogueUI.SetActive(true);
        dialogueUIScript.SetDialogueUI(speaker, dialogue);
    }

    private void LoadCharactersToDictionary()
    {
        characterDatabase = new Dictionary<string, CharacterData>();
        foreach (CharacterData charData in characterList)
        {
            characterDatabase[charData.name] = charData;
        }
    }

    public CharacterData GetCharacterData(string characterName)
    {
        if (characterDatabase.ContainsKey(characterName)) return characterDatabase[characterName];
        return null;
    }

    public bool InCutscene => inCutscene;
    private DialogueUIScript dialogueUIScript => dialogueUI.GetComponent<DialogueUIScript>();
}
