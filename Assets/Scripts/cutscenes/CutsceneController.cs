using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public List<CharacterData> characterList;

    private Dictionary<string, CharacterData> characterDatabase;

    // Start is called before the first frame update
    void Start()
    {
        LoadCharactersToDictionary();
    }

    // Update is called once per frame
    void Update()
    {

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
}
