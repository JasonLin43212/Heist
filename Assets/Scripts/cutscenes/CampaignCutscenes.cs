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
            ("Pepper", "I wanted some Chicken WcNuggets and fries. However much they're the enemy, WcDonald's makes good fries. You can't deny that."),
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

        // Lol this is why i should never work on dialogue. - Jason
        // Nah this is good!!! - Lilian
        { "keycard", new Cutscene(new List<(string, string)> {
            ("Egg", "keycard.get() == true lmao"),
            ("Belle", "What?"),
            ("Pepper", "What?"),
            ("Egg", "belle, press v near a door to use the keycard"),
            ("Egg", "pepper, press enter near a door to use the keycard"),
            ("Belle", "Understood.")
        })},

        { "button", new Cutscene(new List<(string, string)> {
            ("Egg", "oh no"),
            ("Belle", "Oh no."),
            ("Pepper", "Oh no."),
            ("Kool Aid Man", "OH YEAHHHHHHH!!!!!!!!"),
            ("Belle", "..."),
            ("Pepper", "..."),
            ("Kool Aid Man", ".........I don't think I should be here. Uhhhhhhh... bye!"),
            ("Egg", "i think stepping on the colored buttons toggles the doors of the same colors."),
            ("Belle", "Understood."),
            ("Pepper", "Understood."),
            ("Belle", "Hey! Stop copying me."),
            ("Pepper", "Hey! Stop copying me."),
            ("Belle", "LoSeRsAysWhAt"),
            ("Pepper", "What?"),
            ("Egg", "get rekted lmao")
        })},

        // Additional misc dialogue

        // breaking the fourth wall

        { "awkwardCameraQuestion", new Cutscene(new List<(string, string)> {
            ("Belle", "Egg, I just thought of something."),
            ("Egg", "woAw rare occasion"),
            ("Belle", "Ha ha, very funny. But seriously, I have a question about something. You know how you're able to disable cameras remotely?"),
            ("Egg", "???yea?"),
            ("Belle", "And there's no limit to how many cameras you disable at once. Or how often."),
            ("Egg", "yea? i'm op hehe thanx"),
            ("Belle", "Why can't you just disable all the cameras at once and keep them that way?"),
            ("Pepper", "..."),
            ("Egg", "..."),
            ("Egg", "don't hurt your brain thinking about it too much"),
            ("Belle", "..."),
            ("Egg", "THERE ARE REASONS OKAY")
        })},

        { "guardsAreStupid", new Cutscene(new List<(string, string)> {
            ("Pepper", "<i>\"Left Right Spin Security\"</i>. I guess... that makes sense? Or, it makes a tiny bit more sense than it did before, though that wasn't a high standard."),
            ("Belle", "What are you going on about now?"),
            ("Pepper", "I was reading the logo on these robot guards. They're made by <i>\"Left Right Spin Security\"</i>, whoever that is."),
            ("Pepper", "It really feels like it shouldn't be that hard to program robot guards with some variety. Maybe this company really likes their philosophy, though."),
            ("Belle", "I wouldn't complain, though. If the guards are predictable, it makes them easy to evade."),
            ("Egg", "n00b coder rofl more complicated AI probably had too many bugs"),
            ("Belle", "Another thing is, why does every restaurant seem to use the exact same robot model? You'd think that investing in security tech would be an appealing advantage."),
            ("Pepper", "Let's just assume the brains behind these operations had other priorities."),
            ("Egg", "\"brains\"")
        })},

        { "existentialCrisis", new Cutscene(new List<(string, string)> {
            ("Pepper", "Belle, do you ever get the feeling that the world is much bigger than we think?"),
            ("Belle", "...Not really."),
            ("Pepper", "I read about this theory that free will is actually a fake construct. That what we think are choices are actually just the work of some higher power, like a god. Or a computer simulation."),
            ("Pepper", "What if we don't even actually exist? Like, if you think about it, isn't the way the world works so weirdly, almost unrealistically absurd?"),
            ("Belle", "I've been a bit more concerned about surviving than philosophy, if I have to be honest. We're in the middle of a heist, in case you've forgotten."),
            ("Pepper", "But time literally stops when we're talking! It's fine."),
            ("Belle", "...You know, when you put it that way, I kind of see where you're coming from.")
        })},

        // More punss

        { "cornPuns", new Cutscene(new List<(string, string)> {
            ("Egg", "me bored"),
            ("Pepper", "I can share a joke or two, if you'd like."),
            ("Belle", "Are they puns? If they're puns, I'm going to kindly reject your offer."),
            ("Pepper", "Are you saying my puns are too corny for you?"),
            ("Belle", "...Don't you <i>dare</i>--"),
            ("Pepper", "...'cause while there might be a kernel of truth to that, I personally think my puns are a-maize-ing."),
            ("Belle", "I'm going back to the mission."),
            ("Pepper", "Shucks.")
        })},

        // The Backstory

        { "backstory1", new Cutscene(new List<(string, string)> {
            ("Pepper", "We should have a catchphrase. Something like, \"Crepetomaniacs Assemble!\""),
            ("Belle", "Why does that sound like a trademark violation?"),
            ("Pepper", "Alas, great ideas are always trademarked. I suppose my genius shall once more remain hidden to the world."),
            ("Belle", "If it makes you feel better, we have way more reasons to want to remain hidden over being famous or whatever."),
            ("Pepper", "As if I could forget. It's nice to pretend though, sometimes. Imagine that life is different. Better."),
            ("Belle", "How could I imagine that? This is all I've known. My life has been nothing but risking my life for every little scrap, every single extra day I want to live."),
            ("Pepper", "..."),
            ("Belle", "I'm sorry. I didn't mean to come off... that is, I know you've had your own share of hardships. Different ones for sure, but hardships all the same."),
            ("Pepper", "It's fine. After all, though we fight for different reasons, we're aiming for the same goals, aren't we?"),
            ("Belle", "Right. I think... I'm starting to understand the value of a partner. And trust. I suppose I have you to thank for teaching me."),
            ("Pepper", "I'm here for you. Let's take this one step at a time, together.")
        })},

        { "backstory2", new Cutscene(new List<(string, string)> {
            ("Belle", "Do you ever regret it? Leaving what you had before behind, and just for this?"),
            ("Pepper", "No. I ran away from that life for a reason. I know it's hard to understand."),
            ("Belle", "I don't understand, but I'd like to try to. It's just hard to believe, that you'd prefer having nothing over everything."),
            ("Pepper", "By everything, do you mean money?"),
            ("Belle", "For a start, at least. It seems like money can get you anything. Safety, comfort, influence... the power to go wherever you want. Food you can eat for pleasure instead of survival."),
            ("Pepper", "The thing is, you <i>can't</i> go where you want. Any freedom you think you have... it's just an illusion. Put a single toe out of line and it'll be shoved back."),
            ("Pepper", "I lived thirteen years knowing nothing of the world beyond the kitchen. Once I got that tiny taste of what lay outside that world, I knew I could never be happy being stuck there."),
            ("Belle", "I think I'm starting to see where you're coming from, though it's obvious we think about very different problems."),
            ("Pepper", "No one would argue our situations were anything alike, I'd say we came out of it with a similar spirit of sorts."),
            ("Belle", "Maybe, we're both fighting for our lives in our own way. It doesn't matter if that life is measured in decisions or in heartbeats.")
        })},

        { "backstory3", new Cutscene(new List<(string, string)> {
            ("Pepper", "Robot guards are pretty hardy. Getting brained by a crowbar sounds like it would hurt. I'm glad that never happened to me."),
            ("Belle", "...Are you ever going to forget about that?"),
            ("Pepper", "No way! I'm telling you, a crazy green-haired lady holding a bloody crowbar over your head is the stuff of nightmares."),
            ("Belle", "It was <i>not</i> bloody! You know what <i>was</i> bloody? The <i>knife</i> you were waving back at me!"),
            ("Pepper", "Now you're making things up. No one would mistake tomato juice for blood. At least I was supposed to be there, unlike you."),
            ("Egg", "ah the classic story of love at first sight"),
            ("Pepper", "..."),
            ("Belle", "...Pepper, this is all your fault."),
            ("Pepper", "I think I need to erase that suggestion from my memory."),
            ("Belle", "Maybe being brained with a crowbar is the right move after all.")
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
