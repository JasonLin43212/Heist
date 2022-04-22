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

    // public static readonly Cutscene sampleCutscene = new Cutscene(new List<(string, string)> {
    //     ("Belle", "Here's some example dialogue"),
    //     ("Pepper", "Example dialogue 2"),
    //     ("Egg", "third speaker says this")
    // });

    public static readonly Cutscene testCutscene = new Cutscene(new List<(string, string)> {
        ("Belle", "Hmm... so. Since when did we get these names?"),
        ("Pepper", "It's quite punny. To be honest, I think they're good names. Don't you? Where's your sense of humor, Belle?"),
        ("Belle", "I'm not <i>against</i> these names... I just want to know where they came from. Isn't it strange that just one prototype ago, we were called \"Player 1\" and \"Player 2\" instead?"),
        ("Pepper", "Well, we were renamed to fit the food theme of this game. It's so the developers can pretend the theme actually matters."),
        ("Belle", "But why Belle and Pepper? Why not Basil and Chili?"),
        ("Pepper", "...Those were our alternative set of names, actually. But Lilian decided to stick with Belle and Pepper because they were the names of her cousin's cats."),
        ("Belle", "We're named after cats?"),
        ("Pepper", "Yeah. Could be worse, admittedly."),
        ("Egg", "I second that.")
    });
}
