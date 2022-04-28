/*
 * Giant file implementing all of the cutscenes in the campaign
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CampaignCutscenes
{
    private static readonly Dictionary<string, Cutscene> cutsceneDict = new Dictionary<string, Cutscene>
    {
        // ===========================================================
        // TEMPLATE FOR MAKING CUTSCENES (copy, uncomment, and change)
        // ===========================================================

        // { "cutsceneIdentifier", new Cutscene(new List<(string, string)> {
        //     ("Belle", "Here's some example dialogue"),
        //     ("Pepper", "Example dialogue 2"),
        //     ("Egg", "third speaker says this")
        // })},

        { "narrative1", new Cutscene(new List<(string, string)> {
            ("Pepper", "I'll be honest. When I said \"Let's get WcDonald's\" today, this isn't what I meant."),
            ("Pepper", "I wanted some Chicken WcNuggets and fries. However much they're the enemy, WdDonald's makes good fries. You can't deny that."),
            ("Pepper", "Gotta say, if that recipe we're nabbing today doesn't explain their secret, I'm gonna be a bit... salty."),
            ("Belle", "...Are you serious?"),
            ("Pepper", "Sorry, sorry. It was a good pun, though. Surely, you can give me that."),
            ("Belle", "I'll take you to Burger Queen if we make it out of here without any trouble, how's that?"),
            ("Pepper", "It's not my ideal, but I suppose I must make do. Egg, we'll be relying on you to help us out here."),
            ("Egg", "lol nothing new"),
            ("Belle", "You could join us sometimes, you know? Getting out from behind that screen is healthy. Get some exercise and sunlight."),
            ("Egg", "ew"),
            ("Pepper", "Well, maybe this is a discussion for later. Do you want us to bring you anything from Burger Queen?"),
            ("Egg", "1x chicken sandwich no green stuff")
        })},

        { "interlude1", new Cutscene(new List<(string, string)> {
            ("Belle", "Pepper?"),
            ("Pepper", "Yeah? Are you doing alright?"),
            ("Belle", "Yeah. I just... wanted to say thanks."),
            ("Pepper", "Oh. You're welcome. ...For what?"),
            ("Belle", "Thanks for pudding up with me."),
            ("Pepper", "..."),
            ("Egg", "rofl"),
            ("Pepper", "I'm so proud of you. And I ap-peach-eate you too."),
            ("Belle", "I wonder if I'll regret this one day.")
        })},

        { "narrative2", new Cutscene(new List<(string, string)> {
            ("Egg", "escape secure, u good"),
            ("Belle", "Hah! Take that, Donald WcDonald's or whatever your name is! I've waited for this moment for so many years..."),
            ("Pepper", "Phew. There were a few close calls there. I'm glad we made it out safely."),
            ("Belle", "I was never in doubt."),
            ("Egg", "lol lie detected"),
            ("Belle", "Hey! When have my plans ever failed, huh? Tell me. When have I ever steered us wrong?"),
            ("Pepper", "How about that time we got locked in that one meat freezer?"),
            ("Egg", "i have a list on my desktop"),
            ("Belle", "...No one takes my side."),
            ("Pepper", "We <i>each</i> get a side, Belle. You promised us Burger Queen, right?"),
            ("Belle", "Fine. That's our next heist."),
            ("Egg", "but my chicken sandwich")
        })},

        { "testCutscene", new Cutscene(new List<(string, string)> {
            ("Belle", "Hmm... so. Since when did we get these names?"),
            ("Pepper", "It's quite punny. To be honest, I think they're good names. Don't you? Where's your sense of humor, Belle?"),
            ("Belle", "I'm not <i>against</i> these names... I just want to know where they came from. Isn't it strange that just one prototype ago, we were called \"Player 1\" and \"Player 2\" instead?"),
            ("Pepper", "Well, we were renamed to fit the food theme of this game. It's so the developers can pretend the theme actually matters."),
            ("Belle", "But why Belle and Pepper? Why not Basil and Chili?"),
            ("Pepper", "...Those were our alternative set of names, actually. But Lilian decided to stick with Belle and Pepper because they were the names of her cousin's cats."),
            ("Belle", "We're named after cats?"),
            ("Pepper", "Yeah. Could be worse, admittedly."),
            ("Egg", "I second that.")
        })}
    };

    public static Cutscene GetCutscene(string cutsceneIdentifier) => cutsceneDict[cutsceneIdentifier];
    public static bool CutsceneExists(string cutsceneIdentifier) => cutsceneDict.ContainsKey(cutsceneIdentifier);
}
