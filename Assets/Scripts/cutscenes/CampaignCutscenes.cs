/*
 * Giant file implementing all of the cutscenes in the campaign
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CampaignCutscenes
{
    // ===========================================================
    // TEMPLATE FOR MAKING CUTSCENES (copy, uncomment, and change)
    // ===========================================================

    // public static readonly List<(string, string)> sampleCutscene = new List<(string, string)> {
    //     ("Belle", "Here's some example dialogue"),
    //     ("Pepper", "Example dialogue 2"),
    //     ("Egg", "third speaker says this")
    // };

    public static readonly List<(string, string)> testCutscene = new List<(string, string)> {
        ("SampleSpeakerName", "Here's some example dialogue"),
        ("Speaker2", "Example dialogue 2")
    };
}
