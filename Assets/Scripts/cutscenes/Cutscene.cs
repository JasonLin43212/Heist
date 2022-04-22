using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene
{
    private List<(string, string)> content;

    public Cutscene(List<(string, string)> content)
    {
        this.content = content;
    }

    public Cutscene()
    {
        this.content = new List<(string, string)>();
    }

    public void Add(string speaker, string dialogue)
    {
        content.Add((speaker, dialogue));
    }

    public (string, string) this[int index]
    {
        get { return (content[index].Item1, content[index].Item2); }
    }

    public int NumFrames => content.Count;
}
