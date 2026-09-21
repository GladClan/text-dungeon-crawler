namespace GameServer.Domain.Map.Scene.InitialReleaseScenes;

public class CryptSceneState: SceneState
{
    public bool DisturbedTheDead { get; set; } = false;
    public int DisturbedTheCists { get; set; } = 0;         // In a fight, this is the number of weak skeletons
    public int DisturbedSarcophagi { get; set; } = 0;       // In a fight, this is the number of supre-strong skeletons
    public bool KeepersDoorOpen { get; set; } = false;
    public bool KeeperDefeated { get; set; } = false;
    public bool KeeperInPary { get; set; } = false;
    public bool KeeperHesitantToJoinParty { get; set; } = false;
    public int PesterAlsandair { get; set; } = 0;
    public bool GotItemsFromKeeper { get; set; } = false;
    public bool AlsandairRequestedAid { get; set; } = false;
}

/*
public SceneContainer CryptScene = new(
    events: [
        { 0, new CryptSceneTestEntry() },
    ],
    state: new CryptSceneTESTState()
);

CryptSceneEntry
    Dialogue
        narrator
            The party moves into the crypt, warily looking at the carved stone walls.
            The yellow stones are inscribed with intricate patterns.
            Torchlight flickers up ahead, casting flickering shadows on the walls and floor.
            There is no other way to go than forward.
    Choices
        Proceed
            // Go to: CryptEntryHall
        Turn back
            // Return to the previous event
                // In testing, warn that there is no other way than forward.

CryptEntryHall
    Dialogue
        narrator
            The path shortly opens into a broad chamber of the same yellow stone.
            The walls of this chamber are hollow, containing rows and rows of srcophagi. There are also several stone chests on the floor, some open to reveal boes inside.
            There are three doorways in this room: one you entered the crypt from, one to the left of the entry, and one straight across.
            The doorway to the left is in shadow, while the one stright across is lit by the torches.
    Choices
        Inspect the chests
            // Go to: CistInspection
        Inspect the sarcophagi
            // InspectSarcophagus
        Take the left door toward the shadows
            // Go to: ShadowedDoorway
        Take the straight door toward the light
            // Go to: LightedDoorway

CryptReEntryHall
    Dialogue
        narrator
            You retrace your steps back into the chamber lined with sarcophagi.
            The three openings await you: the one you entered the crypt from, the one to the left of the entry, shadowed and now ominous, and the one straight across lit with brightly guttering torches.
    Choices
        Inspect the chests
        Inspect the sarcophagi
        Take the left door toward the shadows
        Take the straight door toward the light
        Turn back

CistInspection
    Dialogue
        narrator
            The open chests you can see are simply full of bones, and peeking under the lids of several others, you can see that the contents are just bones.
    Choices
        Rifle through the chests
            // CONDITION: DisturbedTheCists < 4 && KeeperInParty == false
            // EFFECT: DisturbedTheDead = true
            // Go to: RifleThroughCists
        Inspect the sarcophagi
        Take the left door toward the shadows
        Take the straight door toward the light

RifleThroughCists
    Dialogue
        narrator
            Against your better judgement, you go digging through the cists full of bones.
            It seems these boned have been stored with their trinkets: a ring from a dead man's finger, a necklace to be buried with the dead, a smidgen of coin, whatever the dead thought worth taking with them.
            You could keep them. After all, what are trinkets to the dead?
    Choices
        Keep the trinkets
            // EFFECT:
                // DisturbedTheCists++;
                // GiveItem("cist-trinket"); // trinkets grant some small bonus to stats. Each one generates a random trinket from a small selection, such as a ring or a necklace or some other bauble.
                // GiveItem("cist-trinket);
                // GiveGold(random(10))
            // Go to: StolenTrinket
        Perhaps it would be better to leave the dead well enough alone...
            // Go to: LeaveTheDeadAlone

StolenTrinket
    Dialogue
        narrator
            After pilfering trinkets from the dead, don't you think it might be time to move on?
    Choices
        Rifle through more chests
            // Condition: DisturbedTheCists < 4
            // Go to: RifleThroughCists
        Inspect the sarcophagi
        Take the left door toward the shadows
        Take the straight door toward the light

InspectSarcophagus
    Dialogue
        narrator
            The grey stone sarcophagi stand out in stark contrast to the yellow stone of the rest of the room.
            They appears to be glyphs engraven upon each sarcophagus, likely the name of the one who has been entombed.
            Unfortunately, you can't read them.
    Choices
        Open one up. Who knows what we'll find?
            // CONDITION: DisturbedSarcophagi < 2 && KeeperInParty == false
            // DisturbedTheDead = true
            // Go to: RifleThroughSarcophagus
        Perhaps it would be better to leave the dead well enough alone...
        Ask Alsandair what the glyphs mean
            // CONDITION: KeeperInParty == true
            // Go to: AlsandairExplainsTheSarcophagiGlyphs
        Inspect the chests
        Take the left door toward the shadows
        Take the straight door toward the light

RifleThroughSarcophagus
    Dialogue
        narrator
            Curious what you'll find, you heave open the lid of a sarcophagus.
            You are shocked to discover that this dead person was evidently a warrior of great renown.
            They have been buried with their weapons and quite some treasure.
    Choices
        Take some for yourself. What use are gold and weapons to the dead?
            // EFFECT:
                // DisturbedSarcophagi++;
                // GiveItem("dead-mans-weapon"); // All of these weapons are mostly normal, but they deal a small amound of dark or necrotic damage, being cursed after removing them from the dead.
                // GiveGold(random(7) * random(3, 9))
            // Go to: StolenWeapons
        Perhaps it would be better to leave the dead well enough alone...

StolenWeapons
    Dialogue
        narrator
            What a prize! This weapon surely holds quite the story, and the gold will be quite helpful to you in your own quest.
            You might even thank the dead warrior for their aid.
            It surely won't do them any good.
            Now, don't you think it might be time to move on?
    Choices
        Rifle through another sarcophagus
            // Condition: DisturbedSarcophagi < 2
            // Go to: RifleThroughSarcophagus
        Inspect the chests
        Take the left door toward the shadows
        Take the straight door toward the light

LeaveTheDeadAlone
    Dialogue
        narrator
            Perhaps indeed.
    Choices
        Inspect the chests
        Inspect the sarcophagi
        Take the left door toward the shadows
        Take the straight door toward the light

LightedDoorway
    Condition
        KeepersDoorOpen == false
    IfNotMetRequirements
        // Go to: LightedDoorwayOpen
    Dialogue
        narrator
            The doorway leads into another short hallway like the first, which ends in a solid wooden door.
            There is a handle, or an ornate knocker set into the middle, in the shape of a lion. Classic.
    Choices
        Knock
            // EFFECT: KeepersDoorOpen = true
            // Maybe change the title of "Take the straight door toward the light" to "Go to the keeper's chambers"
            // Go to: KeepersChambersKnock
        Try to open the door
            // EFFECT: KeepersDoorOpen = true; KeeperHesitantToJoinParty = true
            // Go to: KeepersChamberBargeIn
        Return to the burial chamber
            // Go to: CryptReEntryHall

KeepersChamberKnock
    Dialogue
        narrator
            The knocker thumps loudly on the door.
            You hear movement inside, like a chair being scraped across the floor.
            The door swings open to reveal a tall, dark-skinned man.
            He is wearing a red, sleeveless robes with a deep purple hood. His head is uncovered and bald, and he has a black mustache and beard.
            The man looks at you, surprise and almost disapproval. He speaks:
        crypt keeper (Alsandair)
            Greetings travellers. I am curious to know how you entered in this crypt.
            There is an evil lurking, which has recently taken up barring entry herein.
            No matter. I feel it is still there, so you have not destroyed it to gain access.
            This is why I was not present to greet you as you entered this crypt.
            I am unable to tame the evil, and thus trapped in my chambers.
        narrator
            The man pauses for a moment, then says as an afterthought:
        crypt keeper (Alsandair)
            My apologies, I have not introduced myself. I am Alsandair, the keeper of this crypt.
    Choices
        Ask about the evil
            // Go to: AlsandairExplainsLitch
        Ask about the buried dead
            // Go to: AlsandairExplainsCadavers
        Go back to the burial chamber
            // CONDITION: AlsandairRequestedAid == false;
            // EFFECT: AlsandairRequestedAid = true
            // Go to: AlsandairStopsTheParty

KeepersChamberBargeIn
    Dialogue
        narrator
            You take the handle and pull open the door. It isn't locked.
            The room is brightly lit, with plenty of candles scattered around several desks and counters bordering the room.
            In the middle of the room, a man was sitting at the table working on something, but has swung around to face you.
            He is dark-skinned, wearing a red, sleeveless robe with a deep purple hood. His head is uncovered and bald, and he has a black mustache and beard.
            He is now satnding menacingly, his hands curled like claws, elbows bent, about to strike.
            He is looking at you breathing hard, but you see he is calming down with each breath.
        crypt keeper (Alsandair)
            How did you get into this place? There is an evil lurking, barring the entry!
            No matter. I feel it is still there, so you have not destroyed it to gain access.
            Pardon my agressive recation to your entry, but you did not give me any warning to prepare for your entering.
            I have been trapped in my chambers to keep the evil at bay.
        narrator
            The man pauses for a moment, as if considering.
        crypt keeper (Alsandair)
            I am Alsandair, the keeper of this crypt.
    Choices
        Ask about the evil
        Ask about the buried dead
        Go back to the burial chamber

AlsandairStopsTheParty
    Condition
        DisturbedTheCists == 0 && DisturbedSarcophagi == 0
    IfNotMetRequirements
        // Go to: AlsandairDiscoversGraveRobbers
    Dialogue
        narrator
            As you turn to leave, the man moves as if to stop you.
        Alsandair
            Please, wait a moment.
            You look familiar with battle, I would ask your aid.
            The evil I speak of, it must be destroyed. Can you do this for me?
    Choices
        Ask about the evil
        Ask about the buried dead
        Will Alsandair help defeat the evil?
            // CONDITION: KeeperInPary == false && KeeperDefeated == false && AlsandairRequestedAid == true && PesterAlsandair < 3
            // EFFECT: PesterAlsandair++
            // Go to: GetAidFromAlsandair
        Go back to the burial chamber
        Leave Alsandair's chambers
            // CONDITION: AlsandairRequestedAid == true
            // Go to: CryptReEntryHall

AlsandairDiscoversGraveRobbers
    Dialogue
        Alsandair
            One moment.
            What is that you carry?
        narrator
            Alsandair seems to sense the items stolen from the dead.
            Perhaps it would have been better to not disturb them.
        Alsandair
            Robbers! Thieves!
            How dare you pilfer these noble warriors and people of renown!
            You shall pay for your insolence!
    Choices
        Alsandair is attacking. Defend yourself!
            // Fight the Crypt Keeper
            // [OnBattleEnd]
                // KeeperDefeated = true
                // AlsandairRequestedAid = true
                // Go to: AlsandairDefeated

AlsandairDefeated
    Dialogue
        narrator
            Alsandair lies on the ground, weak and barely conscious.
        Alsandair
            Please...
            The evil must be defeated.
            In the darkened chamber...
            The seal of containment...
            Take this key and press it to the door...
            Defeat the evil therein...
        narrator
            Alsandair holds out his hand, holding a square of cloth.
            His eyes slide closed.
            Hopefully the man is simply unconscious.
    Choices
        Take the cloth
            // CONDITION: HasItem("seal-breaker") = false
            // EFFECT: GiveItem("seal-breaker")         // a sqare of cloth with an ornate "X" inscribed on it. On further investigation, many intricate runes are visible around the outside edges.
            // Go to: GetSealBreaker
        Investigate the room
            // Go to: InvestigateCryptKeeperChamber
        Leave Alsandair's chambers

GetSealBreaker
    Condition
        KeeperDefeated == false
    IfNotMetRequirements
        // Go to: TakeSealBreaker
    Dialogue
        narrator
            You take the cloth from Alsandair's hand.
    Choices
        Leave Alsandair's chambers
        Ask about the evil
        Ask about the buried dead
        Will Alsandair help defeat the evil?

TakeSealBreaker
    Dialogue
        narrator
            You take the cloth from Alsandair's hand.
    Choices
        Leave Alsandair's chambers
        Investigate the room

AlsandairExplainsLitch
    Condition
        DisturbedTheCists == 0 && DisturbedSarcophagi == 0
    IfNotMetRequirements
        // Go to: AlsandairDiscoversGraveRobbers
    Dialogue
        Alsandair
            It arrived with some of the recent cadavers, may they rest in peace.
            I was giving aid to the priests delivering them when it arose from the back of the cart, as if it was forming out of the souls of the deceased.
            It was a dark shadow, but it gained form in but a few moments.
            The litch.
        narrator
            Alsandair shakes himself slightly, his face darkening.
        Alsandair
            The creature slew the priests quickly. It seemed to consume their souls.
    Choices
        Why is he stuck in this room then?
            // Go to: AlsandairExplainsSealing
        Why can't he destroy the litch?
            // Go to: AlsandairExplainsWeakness
        Ask about the buried dead
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers

AlsandairExplainsSealing
    Dialogue
        Alsandair
            After the evil slew the priests, I fled to this room to obtain some relics which would help me. I quickly drew up a circle to seal it away.
            When I left my chambers, I found it in the lower rooms, down the darkened hallway.
            I sealed it in that forbidden room, but I must maintain the power of the circle to hold it there.
            I cannot leave this circle for long or the evil will break loose of this place.
            I cannot allow that.
    Choices
        Why can't he destroy the litch?
        Ask about the buried dead
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers

AlsandairExplainsWeakness
    Dialogue
        Alsandair
            It is more than I can do alone.
            To face that evil alone would certainly destroy me. I am able to seal it away, but to banish it is beyond my power.
            I must remain here to maintain the power of the seal placed upon the evil.
    Choices
        Ask about the buried dead
        Will Alsandair help defeat the evil?
        Accept this task
            // CONDITION: HasItem("seal-breaker") == false
            // EFFECT: GiveItem("seal-breaker")
            // Go to: AlsandairGivesSealBreaker
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers

AlsandairGivesSealBreaker
    Dialogue
        narrator
            Alsandair gives you a square of cloth with an ornate "X" inscribed on it.
        Alsandair
            To open the darkened room wherein the evil lies, you will need this key.
            Take this and press it to the door of the darkened room, and it shall open for you.
            The seal of containment will remain, but the evil will not be able to exit unless I cease to power this circle.
            Thank you for your willingness to aid me in this.
    Choices
        Leave Alsandair's chambers
        Ask about the evil
        Ask about the buried dead
        Will Alsandair help defeat the evil?


GetAidFromAlsandair
    Condition
        DisturbedTheCists == 0 && DisturbedSarcophagi == 0
        // if (Skill check: persuasion[difficulty = {DisturbedTheDead ? medium : easy + KeeperHesitantToJoinParty ? easy : trivial}] == success)
    IfNotMetRequirements
        // Go to: AlsandairRefusesToHelpParty
    Dialogue
        narrator
            Alsandair pauses for a long moment, considering.
            Finally, you see a determination in his eyes.
            He looks at the table where he was working.
        Alsandair
            When I part from this circle, the evil may overpower its seal and be loosed.
            I cannot allow that.
            But I cannot stay here forever, I have needs that must be met if I am to remain.
            Very well.
        narrator
            He turns to your party and offers his hand.
        Alsandair
            I will indeed join you. We shall destroy this evil together.
    Choices
        Alsandair joins your party
            // EFFECT:
                // AddMemberToParty(Alsandair)
                // KeeperInParty = true
            // Go to: AlsandairJoinsTheParty

AlsandairJoinsTheParty
    Dialogue
        Alsandair
            Wery well.
            Let us go.
    Choices
        Ask about the buried dead
        Ask about the evil
        Leave Alsandair's chambers

AlsandairRefusesToHelpParty
    Dialogue
        narrator
            Alsandair pauses as if considering.
            He doesn't look convinced.
        Alsandair
            I fear to leave this seal. If I cease to rejuvenate it, the evil may overpower it and be loosed.
            I cannot allow that.
            To aid you, I have items that may help you in destroying the evil.
    Choices
        Ask about the items
            // CONDITION: GotItemsFromKeeper = false
            // EFFECT: 
                // GotItemsFromKeeper = true
                // RemoveItemsFromKeeper([
                    // "enchanted-dagger",
                    // "silver-rod",
                    // "healing-scroll",
                    // "health-potion",
                    // "health-potion"
                // ])
                // GiveItemsArray([
                    // "enchanted-dagger" // "Enchanted dagger" or "crypt keepers dagger"
                    // "silver-rod", // Deals extra holy damage
                    // "healing-scroll", 
                    // "health-potion", 
                    // "health-potion"
                // ])
            // Go to: GetKeepersItems
        Accept this task
        Ask about the buried dead
        Ask about the evil
        Leave Alsandair's chambers

GetKeepersItems
    Dialogue
        narrator
            Alsandair gestures to a small chest by the table he was working at.
            Opening it, he reveals a dagger, a silvery staff, a rolled up scroll, and two bottles of goopy red substance.
        Alsandair
            Take these.
            The dagger is enchanted to aid in its prowess against this sort of evil, and the silver rod is something of a wand, channeling heavenly light.
            The scroll contains a ward of health. It can be used to bind your wounds, and the potions are similar concoctions.
    Choices
        Accept this task
        Ask about the buried dead
        Ask about the evil
        Leave Alsandair's chambers

AlsandairExplainsCadavers
    Condition
        DisturbedTheCists == 0 && DisturbedSarcophagi == 0
    IfNotMetRequirements
        // Go to: AlsandairDiscoversGraveRobbers
    Dialogue
        Alsandair
            This crypt holds the bodies of fallen warriors.
            Each being buried here was of great renown, doing great works during their mortal travail.
            Every one has earned their rest.
            I believe The evil that has overshadowed this place seeks their expertise in battle, or perhaps it is simply nourished by their nobility.
            Their souls are powerful I believe, and the evil seeks to control that power.
    Choices
        Ask about the evil
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers


LightedDoorwayOpen  // if the door is already open and 
    Condition
        HasItem("seal-breaker") && KeeperDefeated == false
    IfNotMetRequirements
        // Go to: EmergencyGetSealBreaker
    Dialogue
        narrator
            Alsandair's door stands open. The man is once again seated at the table in the center of his room, the numerous candles providing a near-constant light.
            The room is bordered by several wooden desks and stone counters, set with an impressive array of golwing candles.
            Other than candles, the surfaces are mostly uncluttered except for a couple of books and scrolls, a few pots, and some crates and casks evidently containing food and drink.
    Choices
        Take the cloth
        Ask about the evil
        Ask about the buried dead
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers


EmergencyGetSealBreaker
    Condition
        KeeperDefeated == false     // Does not have the seal breaker
    IfNotMetRequirements
        // Go to: ReturnToAlsandairDefeated
    Dialogue
        Alsandair
            I am glad to see you back.
            I had forgotten to tell you, the door is sealed to the darkened room where the evil lies.
            To open it, you will need this key.
        narrator
            Alsandair holds out his hand, holding a square of cloth.
    Choices
        Take the cloth
        Ask about the evil
        Ask about the buried dead
        Will Alsandair help defeat the evil?
        Go back to the burial chamber
        Leave Alsandair's chambers

ReturnToAlsandairDefeated
    Dialogue
        narrator
            The room is brightly lit, with plenty of candles scattered around several desks and counters bordering the room.
            In the middle of the room is the table the crypt keeper was working at.
            Aslandair lies on the floor.
            Perhaps you should abide by his final wish and defeat the litch he spoke of.
            His words to you ring in your mind: 
            In the darkened chamber.
            The seal of containment.
            Take this key and press it to the door.
            Defeat the evil therein.
            ...
            If only you had not taken the items from the dead.
    Choices
        Investigate the room
        Take the cloth
        Leave Alsandair's chambers

InvestigateCryptKeeperChamber
    Dialogue
        narrator
            The room is bordered by several wooden desks and stone counters, set with an impressive array of golwing candles.
            Other than candles, the surfaces are mostly uncluttered except for a couple of books and scrolls, a few pots, and some crates and casks evidently containing food and drink.
            The desk Alsandair was working at stands in the middle of the room, a circle of some substance written out on it, giving of a subtle, almost unnoticable glow.
            Next to the circle are several books and scrolls that the crypt keeper was apparently perusing.
    Choices
        Take the cloth
        Investigate the circle
            // Go to: AlsandairSealDescription
        Rifle through the crates
            // CONDITION: LootKeepersRoom == false
            // Go to: GuiltTrip
        Peruse the books
            // Go to: PeruseTheBooks
        Leave Alsandair's chambers

AlsandairSealDescription
    Dialogue
        narrator
            You can't tell what substance the circle is made out of.
            Looking at it closer, you see that the circle is intricately detailed with thin swirls and finely penned glyphs.
            As you watch, it seems to vibrate faintly, blurring some of the swirls.
            The circle is slowly fading.
            It'll take a couple of minutes, but shortly it will just e a circle of whatever substance it is made of.
    Choices
        Take the cloth
        Rifle through the crates
        Peruse the books
        Leave Alsandair's chambers

GuiltTrip
    Dialogue
        narrator
            Well, yes. I suppose Alsandair will have no more need of his sustinance seeing that you've beaten him to a pulp.
            Wouldn't you say then, that you are deserving some sort of reward for knocking him out?
            By all means, take the man's food.
    Choices
        Yes, I earned this fair and square
            // EFFECT: 
                // GiveItem("sustinance")
                // LootKeepersRoom = true
            // Go to: LootKeepersFood
        ... what?
            // EFFECT:
                // GiveItem("stat-increase-token")
                // LootKeepersRoom = true
            // Go to: LeaveKeepersFood

LootKeepersFood
    Dialogue
        narrator
            Turns out, the crypt keeper had some pretty good grub.
            Had.
    Choices
        Take the cloth
        Investigate the circle
        Peruse the books
        Leave Alsandair's chambers

LeaveKeepersFood
    Dialogue
        narrator
            That was... unselfish of you. Nigh respectable, not to just take the dying man's food.
            Very well.
    Choices
        Take the cloth
        Investigate the circle
        Peruse the books
        Leave Alsandair's chambers

PeruseTheBooks
    Dialogue
        narrator
            Many of the texts are written in languages you don't understand, but there are some in common writ and other familiar languages.
            There is one scroll that has many circles written in it, similar to the one displayed on the table. Unfortunately, there doesn't seem to be any text in the scroll, just pictographs and odd symbols.
    Choices
        What do the books say?
            // Go to: ReadTheBooks
        Take the cloth
        Investigate the circle
        Rifle through the crates
        Peruse the books
        Leave Alsandair's chambers

ReadTheBooks
    Dialogue
        narrator
            One talks about a warrior which led a group of adventurers into an ancient, forgotten city.
            It seems to infer that the city could fly? But the city was also buried under a mountain.
            It is unclear whether the events actually occured or not.
    Choices
        Keep reading
            // Go to: PromptToReadTheStory
        Take the cloth
        Investigate the circle
        Rifle through the crates
        Leave Alsandair's chambers

PromptToReadTheStory
    Dialogue
        narrator
            Would you like to simply read the book?
    Choices
        Yes
            // Go to: ReadTheStory
        Take the cloth
        Investigate the circle
        Rifle through the crates
        Leave Alsandair's chambers

ReadTheStory
    Dialogue
        narrator
            Kain took a step into the darkness of the gateway, leading his party from the sunny wooded grotto, the gurgling sound of the stream fading quickly into echoing silence. He wanted to sigh in relief as the humid heat gave way to cool and he started feeling better about the platemail armor he customarily wore. The armor, encrested with the King’s Lion, was good for discouraging highway marauders or getting them through checkpoints more promptly, but it made him prone to overheat on a warm summer day such as it was.
            He strode forward down the steps, not looking back to see if his eight companions were following. He had hand-picked each one of them for this venture, and he trusted each of them to follow suit. Kain peered into the darkness, trying to see anything in the gloom, when he heard clear, unintelligible words behind him, and tendrils of silvery-blue lights snaked out from Tel, his wizard companion. He had known Tel since they were both teenagers, each trying to find his own way in the rugged kingdom. Kain’s first impression of Tel was of a rich boy, overindulged by his parents. But truly, Tel was a diligent and faithful companion who worked hard to master the wizard arts through training and in his own studies. A strong bond had grown between him and Kain in the past several months they had been working together, and Kain had grown to rely on the wisdom and insights of his stoic fellow.
            With the tendrils of illumination revealing more and more of the steps before him, Kain tread softly down the smooth stone steps, wary of what might be hiding in the shadows ahead. He held his shield ready, but did not draw his sword, seeing no evident threat in the darkness. It could easily be drawn from its specialty scabbard strapped to his back. He counted the steps as he descended—fifteen, then twenty, then thirty, then fifty. He heard Nathan, the freshest member of his party with the least mission experience, begin to breathe more heavily.
            ...
            It goes on, here's a bit of the good part:
            Kain stood facing Inimicus on the balcony overlooking the grand palace square. Sounds of fighting rose up from below as the band of misfits Kain had assembled fought against Turpis’ minions. The rattling of skeletons mixed with the clash of swords and the explosion of pistols and rifles as well as the crashing of hammers and polearms. Kain sized up Turpis, measuring him and considering the best way to strike. Inimicus stared at Kain with the same intensity, though his gaze was much more menacing and unholy.
            Without warning, the door behind Inimicus crashed open as Turpis tumbled out, crashing to the ground to the left of Inimicus, his body glowing with the radiant heat of some sort of hastily cast shield spell. Kain took this in without glancing away from his adversary, nor did Inimicus waver his glowering toward Kain. An incomprehensible shout came from within the shadowy room and three tiny bolts of blue light shot out towards Turpis’ prone form. Turpis rolled and bounded up, but the bolts followed him. Tel stepped out of the shadows of the doorway, his wand held ready to defend against counter strikes or cast offensively again. He cast a look around the balcony and saw the stand-off, then to Turpis, who had dashed to the side of the balcony and began murmuring a spell that seemed to slow the burning bolts of magic, as if they were sinking through syrup toward him.
            Kain took a tentative step to his right, turning Inimicus away from his minion and Tel as they engaged again. Kain's and Inimicus’ eyes remained fixed on each other as Kain stepped to the side, moving toward the side of Inimicus’ towering shield. Inimicus did not move, except for his head, which turned slightly to follow Kain.
            Without warning, Inimicus struck. Faster than a striking snake, his sword leapt, as if an extension of the monster’s own arm, toward Kain’s throat. Kain raised his own shield to parry the blow, but the sword pivoted into a stab into his helmet. Kain desperately struck out with his own sword, aiming for the arm that held Inimicus’ blade. With an odd ringing clash, the two magical swords collided and the air filled with buzzing heat as Ustrina fizzled, glowing, and Eximus crackled and flashed. Kain felt the heat rising higher and pulled on his sword to move it toward Inimicus’ head, but the sword wouldn’t budge. It was as if the two blades had welded together where they met, and would not be separated. Kain tensed to bring his knee around into Inimicus’ side, but instead he felt a great wrenching in his shoulder as if the swords had been tied to a boulder rolling away. Inimicus yanked the two swords, trying to pull his sword out of the bind, almost ripping Ustrina out of Kain’s grip with his incredible strength.
            Out of the corner of his eye, Kain saw a menacing purple flash from where Tel stood. As his focus wavered with concern for his friend, he felt another jerk on his sword accompanied by a dark red glow. Kain shot his gaze down just in time to see Inimicis’ fingers, curled like claws around a bloody red glow,  shoot out like a blow to Kain's abdomen. It felt like being hit by a landslide. Kain almost dropped his sword and shield as he stumbled back toward the wall, his body aching and trembling with whatever was in the magic Inimicus had hit him with.
            Inimicus strode toward Kain with his sword raised to strike, but he moved slowly, as if surprised. Kain lifted his suddenly aching arms, holding his shield and sword ready, but paused as he caught view of his sword. 
            ...
            Basically, Inimicus is the lord of the Hidden City, a sorcerer and alchemist held to life by his own means, still somewhat living from the glory days when the city still flew. He wields Eximus the Thunderbrand, an immensely powerful and dangerous magical sword. He seeks to restore the Hidden City to what it once was, by any means necessary.
            Anciently, when the city was flying, its power was linked to a city floating upon the sea. Legend says that a tidal wave destroyed the floating city, cutting power to the flying city. And that was the day it fell.
            Tel is a wizard and Kain's closest friend. After the battle, he finds a staff which makes him evil. The staff contains the soul of an evil wizard and must be destroyed and the curse broken for Tel to return to his former goodness.
            There is also a djinn, a fiery-haired princess, a stooped healer that's pretty cool, a foolish wizard who is Inimicus' sidekick, and a bunch of other characters.
            I'm sure there's subtle lore in the story as well.
            ...
            Satisfied?
            No?
            Just go read a book, why don't you.
    Choices
        Take the cloth
        Investigate the circle
        Rifle through the crates
        Leave Alsandair's chambers

AlsandairExplainsTheSarcophagiGlyphs            // The glyphs tell of the warrior, but also some of their stories. He gives an example of one story, telling somewhat of the hero's adventure.
    Dialogue
        narrator
            Alsandair pauses next to a sarcophagus.
        Alsandair
            Each sarcophagus is marked with the name of the hero inside.
            Beneath each name is their story, or the moments of most import.
            Like this one, a great warrior who dies in battle defending her company.
            She lived to bring joy to otehrs, but was called to war in the cause of an unjust ruler.
            She fought for the freedom to find joy. True joy, not that riotous living that is enjoyed by the rich and foolish.
        narrator
            Alsandair trails off, lost in thought looking at the sarcophagus.
        Alsandair
            But we must keep going. We must not give time for the evil to break frre of the seal I placed upon it.
    Choices
        Inspect the chests
        Inspect the sarcophagi
        Take the left door toward the shadows
        Take the straight door toward the light



-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
If KeeperDefeated or KeeperInPary, the litch is more powerful (because Alsandair hasn't been maintaining the circle of sealing)
The chamber leading to the litch is wehre the skeletons attack the party... if you disturbed cists or sarcophagi
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -


ShadowedDoorway     // Where the Litch is. Can't go in without a key from Alsandair, either with him in your party, or when he gives it to you after you defeat him
    Dialogue
        narrator
            |
*/