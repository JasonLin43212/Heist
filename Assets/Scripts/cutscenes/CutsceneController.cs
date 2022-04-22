using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public DialogueUIScript dialogueUI;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (inCutscene && Input.GetMouseButtonDown(0))
        {
            // Clicked during a cutscene; progress the dialogue
            frameIndex++;
            if (frameIndex < currentCutscene.NumFrames) UpdateDialogueUI();
            else EndCutscene();
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
        dialogueUI.SetDialogueUIDisplay(false);
    }

    private void UpdateDialogueUI()
    {
        if (frameIndex >= currentCutscene.NumFrames) return;
        (string speaker, string dialogue) = currentCutscene[frameIndex];
        dialogueUI.SetDialogueUI(speaker, dialogue);
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
}
