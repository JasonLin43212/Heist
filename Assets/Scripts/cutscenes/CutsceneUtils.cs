using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CharacterData
{
    public string name, displayName;
    public Sprite sprite;

    public CharacterData(string name, string displayName, Sprite sprite)
    {
        this.name = name;
        this.displayName = displayName;
        this.sprite = sprite;
    }
}
