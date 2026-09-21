namespace GameServer.Domain.Map.Scene.InitialReleaseScenes;

public class UnintegratedPathwayState: SceneState
{
    public bool UsedKey { get; set; } = false;
    public bool CrystalBugsSlain { get; set; } = false;
    public int TakenGems { get; set; } = 0;
    public bool GuardsAlerted { get; set; } = false;
    public bool WizardInParty { get; set; } = false;
    public bool CellUnlocked { get; set; } = false;
    public bool KingsChestOpened { get; set; } = false;
}


/*
public class UnintegratedPathwayScene
{
    public SceneContainer UnintegratedPathwaySceneContainer = new(
        events: [
            { 0, new PathEntry() },
        ],
        state: new UnintegratedPathwayState()
    );
}

PathEntry
    Dialogue
        You walk along a bleak, grey path. It is lifeless here.
        There is no scenery here, just a formless void.
        You have been walking for days when finally, you come to a fork in the road.
        There are three choices.
    Choices
        Go forward
            // Go to: ForwardPath
        Go left
            // Go to: LeftwardPath
        Go right
            // Go to: RightwardPath

ForwardPath
    Dialogue
        This way seems as bland and uneventful as the path you were on.
        You continue walking for days, it seems
    Choices
        Go back
            // Go to: EnterCave
        Keep goig forward
            // Go to: BackToTheEntry

BackToTheEntry
    Dialogue
        As you continue walking forward, you somehow end up back at the fork in the road.
    Choices
        Go forward
        Go left
        Go right

EnterCave
    Dialogue
        Somehow, rather than going back to the fork in the road, you turn around to see the path is gone.
        In its place is a cave.
        It is not totally dark, you see glimmers of light from within.
        The glimmers come from gemstones encased within the rock walls.
        You look behind you for the path, but it is no longer there.
        Behind you is a stone wall.
        No way to go but forward.
    Choices
        Enter the cave
            // Go to: CaveEntry

CaveEntry
    Dialogue
        The cave shortly opens up into a large chamber, lit with flickering candles that make the walls sparkle and glitter.
        In this chamber, there is a small door to the left with light streaming from a window set into the door.
        The cave also continues on.
    Choices
        Investigate the side room
            // EFFECT: ChangeTitle("Investigate the side room", "Enter Nathandriel's room")
            // Go to: DwarfsCove
        Examine the gem-covered walls
            // CONDITION: TakenGems < 3
            // Go to: SparklyGems
        Continue down the cave
            // Go to: Mineshaft

DwarfsCove
    Dialogue
        narrator
            The door opens without a squeak.
            Inside the room is a short figure. A dwarf.
            He turns to look at you as you enter
        Nathandriel
            Ho there! You seem to have wandered astray.
            Need ye any help gettin back to where ya be comin from?
    Choices
        Ask about the cave
            // Go to: NathandrielExplainsCave
        Go back to the cave
            // Go to: ReEnterCave

ReEnterCave
    Dialouge
        narrator
            You go back into the main chamber.
            The candles give plenty of light, making the gems in the walls sparkle and glitter.
            There is the small door, the way you cam, or more cave ahead.
    Choices
        Investigate the side room
        Examine the gem-covered walls
        Continue down the cave

NathandrielExplainsCave
    Dialogue
        Nathaniel
            This cave? Not much to it. It's a gemstone mine, or course!
            Be awary, thoguh, there be crawlies about.
            Gotta watch out fer them crystalbug, now, don't ya!
    Choices
        Ask about the crystalbug
            // Go to: NathanielExplainsCrystalbug
        Go back to the cave

NathanielExplainsCrystalbug
    Dialogue
        Nathaniel
            Well, them buggers. They're shaped a wee bit like an ant, but they live off crystals, don't they!
            They eat em, eh?
            Their hides are hard as stone, and they be quite the pest.
            Watch out if ye be carrying gemstones around, they'll come for ya.
    Choices
        How to fight the crystalbug
            // CONDITION: HasItem("war-hammer") == false
            // EFFECT GiveItem("war-hammer")
            // Go to: NathanielGivesHammer
        Go back to the cave

NathanielGivesHammer
    Dialogue
        Nathaniel
            Well, best way to rid yerself of them buggers it ta hit em with a hammer, I'd say.
            Take this one, it's mah' spare.
    Choices
        Go back to the cave

SparklyGems
    Dialogue
        The walls are scattered with glittering crystals of all sizes.
        A couple of them are almost as large as your fist.
        You notice some of the bigger gems in the walls look loose, almost as if you could pry them out with your bare hands.
    Choices
        Pry one loose
            // EFFECT:
                // TakeGems++
                // GiveItem("gemstone")
            // Go to: TakenGemstone
        Investigate the side room
        Continue down the cave

TakenGemstone
    Dialogue
        The gemstone is beautiful, it glitters in the light of the flickering candles.
        It's got to be worth a lot.
    Choices
        Investigate the side room
        Continue down the cave

Mineshaft
    Dialogue
        narrator
            You continue down the cave.
            The candles become more sparse as you go further along, making the way ahead darker.
            Shortly, you come upon tracks for a minecart.
            The cave goes on for a ways before you come to a divide.
            One direction follows the railway, the other seems to be much smoother stone.
    Choices
        Follow the rails
            // Go to: CrystalbugEncounter
        Follow the smooth path
            // Go to: BackToTheEntry

CrystalbugEncounter
    Condition
        CrystalBugsSlain == false
    IfNotMetRequirements
        // Go to:CrystalbugsDefeated
    Dialogue
        narrator
            As you continue forward, you hear the sound of scuttling...
            Suddenly from ahead in the tunnel, you see movement.
            There are creatures, like giant ants!
            They seem to sprakle and glitter and you realize they're covered in gems.
            And they're heading rapidly toward you!
    Choices
        Crystalbugs are attacking, defend yourself!
            // CONDITION: TakenGems > 0
            // Fight Crystalbugs * (TakenGems + 3)
            // [OnBattleEnd]
                // CrystalbugsSlain = true
                // Go to: BattleEndEvent
        Defend yourself against the bugs!
            // Go to: BugsRushPast

BattleEndEvent
    Condition
        HasItem("war-hammer")
    IfNotMetRequirements
        // Go to: BattleEndEvent2
    Dialogue
        narrator
            That could have been much worse.
            Fortunate you talked to that dwarf to get the hammer, it was very effective against those bugs.
            You see from where the bugs came from is a pile of gems.
            Judgine from the shape of the pile...
            It seems they don't just eat the gems.
    Choices
        Take the gems
            // EFFECT: GiveItems(["gemstone", "life-gem", "healing-gem", "fortify-gem", "mana-gem"])
            // Go to: GemReward
        Continue down the tunnel
            // Go to: BackToTheEntry

BattleEndEvent2
    Dialogue
        narrator
            Those bugs were nasty
            But, on the bright side, you see from where the bugs came from is a pile of gems.
            Then again...
            Judgine from the shape of the pile...
            It seems they don't just eat the gems.
    Choices
        Take the gems
        Continue down the tunnel

GemReward
    Dialogue
        narrator
            It seems the waste of the crystalbug is quite different from what they ingest.
            That's a hearty pile of gems you've collected, surely it shall be quite useful.
            You look up...
            And the cave is gone.
            Suddenly you're back on the path in the dreary grey waste.
    Choices
        Keep going forward

BugsRushPast
    Dialogue
        narrator
            You stand ready to attack, but the bugs don't stop.
            They continue rushing past you, toward the bend in the tunnel.
    Choices
        Continue down the tunnel


RightwardPath
    Condition
        HasItem("courtyard-dungeon-key") == false
    IfNotMetRequirements
        // Go to: RightAgain
    Dialogue
        narrator
            As you walk forward, the path disappears before you, simply vanishing.
            In its place is a wall, seemingly hewn out of a blue stone.
            Ther is not source of light, but you can see perfectly clear.
            Behind you is an opening, nearly half your height. You would need to crawl through to exit.
    Choices
        Crawl through the opening
            // Go to: DungeonKeyRing

DungeonKeyRing
    Dialogue
        narrator
            You come out into a room hewn of the same blue stone.
            On the wall in front of you is a key ring holding a single, ornate key.
            You feel like the key should not be here.
            It's a very uncomfortable feeling.
    Choices
        Tke the key
            // CONDITION: HasItem("courtyard-dungeon-key") == false
            // EFFECT: GiveItem("courtyard-dungeon-key")
            // Go to: TookTheKey
        Crawl back
            // Go to: BackToThePath

TookTheKey
    Dialogue
        narrator
            You pocket the key.
            The room is empty.
            There is just the empty room and the opening to the other empty room.
    Choices
        Crawl back

RightAgain
    Condition
        WizardInParty == false
    IfNotMetRequirements
        // Go to: RightWithWizard
    Dialogue
        narrator
            You take the right path once more, and come to a different room.
            It is made of red stone this time, but opening behind you is of a normal height.
            Somehow you can't see into the room. It is as though it has a curtain in front of it
    Choices
        Walk through the opening
            // Go to: WizardSlayer

WizardSlayer
    Condition
        HasItem("sword-wizard-slayer") == false
    IfNotMetRequirements
        // Go to: TookTheSabre
    Dialogue
        narrator
            As your vision clears, there on the wall is a message, carved into the red stone.
            It reads:
            \"The wizard is not to be trusted\"
            \"But you cannot succeed without him\"
            \"It's dangerous to go alone. Take this.\"
            Below the message is a ruby sabre.
    Choices
        Take the sword
            // EFFECT: GiveItem("sword-wizard-slayer")
            // Go to: AcquireWizardSlayer
        Crawl back

AcquireWizardSlayer
    Dialogue
        narrator
            You pick up the curved sword.
            It feels warm in your hands, and you feel like it contains a power that compliments its graceful workmanship.
            As you turn your attention back to your surroundings, you find yourself back on the grey path.
    Choices
        Keep going forward

TookTheSabre
    Dialogue
        You walk through the blurry curtain again, but find yourself back on the path.
    Choices
        Keep going forward

RightWithWizard
    Dialogue
        narrator
            Ethrandil seems to fade in and out as you continue along the right path.
            It is as if he isn't really there at all.
            Eventually, you realize that the path you have been walking along is growing wider, turning into a field of grey.
            You see bumps on the path growing as you continue walking.
            They look like tombstones, rising from the earth.
            The path turns dark, almost black.
            Suddenly, the world snaps like a rubber band.
            You find yourself back where you started.
    Choices
        Keep going forward

BackToThePath
    Dialogue
        narrator
            You crawl back through, but rather than back in the same stone room, you find yourself on the grey path once more.
    Choices
        Keep going forward

LeftwardPath
    Dialogue
        narrator
            You take the leftward path, hoping for a change in scenery.
            You look back to the fork in the road and see it is no longer there.
            Instead, you see a broad cobblestone courtyard, surrounded by sheer stone walls.
            Ahead of you is a thick wooden door, and to one side is a large, open gate.
    Choices
        Go to the door
            // Go to: CourtyardDoor
        Go through the gate
            // Go to: CastleProper

CourtyardDoor
    Condition
        DoorIsOpen == false
    IfNotMetRequirements
        // Go to: CourtyardDoorOpen
    Dialogue
        narrator
            The door is indeed thick, and unfortunately it is locked.
    Choices
        Unlock the door
            // CONDITION: HasItem("courtyard-dungeon-key")
            // EFFECT: DoorIsOpen = true
            // Go to: CourtyardDoorOpen
        Bash it in
            // CONDITION: HasItem("war-hammer")
            // EFFECT: GuardsAlerted = true
            // Go to: DestroyCourtyardDoor
        Go through the gate

NotDestroyCourtyardDoor
    Condition
        if (skill check: Proficiency.crushing[difficulty = {strength > 17 ? trivial : strength > 13 ? easy : strength > 10 medium : hard}]) == false
    IfRequirementsNotMet
        // EFFECT: DoorIsOpen = true; ChangeTitle("Go to the door", "Approach the remains of the door")
        // Go to: DestroyCourtyardDoor
    Dialogue
        narrator
            You make a racket bashing on the door, and there are quite a few dents in the wood.
            Unfortunately, the door is still as shut as it was before.
            You could try again, but it seems pretty solid.
    Choices
        Unlock the door
        Bash it in
        Go through the gate

DestroyCourtyardDoor
    Dialogue
        narrator
            It takes a couple of hits, but you manage to smash the door open, making quite a racket in the process.
            In any case, the door is open.
            The interior is musty and gloomy, lit by oily torchlight.
    Choices
        Enter
        Go through the gate

CourtyardDoorOpen
    Dialogue
        narrator
            You've opened the door.
            The interior is musty and gloomy, lit by oily torchlight.
    Choices
        Enter
            // Go to: CourtyardDungeon
        Go through the gate

CourtyardDungeon
    Dialogue
        narrator
            The interior is just as foreboding as it looked from the outside.
            You can see many barred cells along the wall.
            It appears this places is a dungeon.
            The hallway isn't very long. In just a  couple of steps you've reached the end of it.
    Choices
        Return to the courtyard
            // Go to: CourtyardAgain
        Explore the dungeon
            // Go to: EncounterWizard

CourtyardAgain
    Dialogue
        narrator
            Back into the bright sunlit courtyard.
    Choices
        Go to the door
        Go through the gate

EncounterWizard
    Condition
        // WizardInParty == false && CellUnlocked == false
    IfRequirementsNotMet
        // Go to: NotWizardJoinsParty
    Dialogue
        narrator
            Peering into the cells, you notice one has an occupant.
            It's a man, cloaked and sitting on a bench in the back
        Cloaked man
            Finally! About time someone's come with my food—wait.
            You don't work here.
            Aha! Great escape!
            I am Ethrandil, I have been imprisoned unjustly by the wicked king.
            If you would let me out, I will reward you handsomely!
    Choices
        Unlock the cell
            // CONDITION: HasItem("courtyard-dungeon-key")
            // Go to: EthrandilOffersToJoin
        Bash open the cell
            // CONDITION: HasItem("war-hammer")
            // EFFECT: GuardsAlerted = true
            // Go to: BarsLikeGongs
        Return to the courtyard

BarsLikeGongs
    Dialogue
        narrator
            You raise the hammer and hit the bars
        Ethrandil
            No! No! No!
            Stop that, you'll alert the guards and they'll come running in.
            There is a key, you imeciles.
            How did you get in here, anyway?
    Choices
        Unlock the cell
        Return to the courtyard

EthrandilOffersToJoin
    Dialogue
        Ethrandil
            Thank you kindly for freeing me!
            I would be pleased to accompany you, if you would.
            My magic would be quite a boon to you, I am sure.
            And I know just where they have taken my effects, as well as how to open the chest!
            I would be happy to reward you with payment or enchanted goods if you would aid me in reaquiring my effects.
    Choices
        Bring Ethrandil along
            // EFFECT: AddMemberToParty(Ethrandil)
            // Go to: WizardJoinsParty
        Decline
            // Go to: NotWizardJoinsParty

NotWizardJoinsParty
    Condition
        // WizardInParty == false
    IfRequirementsNotMet
        // Go to: WizardHurriesYouOn
    Dialogue
        Ethrandil
            Oh, very well.
            If you reconsider, I shall await here until you depart.
    Choices
        Bring Ethrandil along
        Return to the courtyard

WizardJoinsParty
    Dialogue
        Ethrandil
            Wonderful!
            My things are in the keep.
            It is through the gate you likely passed by to come in here.
            Watch out for guards, though. They're vicious.
    Choices
        Return to the courtyard

WizardHurriesYouOn
    Condition
        // MemberInParty(Ethrandil)
    IfRequirementsNotMet
        // Go to: EthrandilOffersToJoin
    Dialogue
        Ethrandil
            Let's get a move on, nothing more to see in this dungeon.
    Choices
        Return to the courtyard


CastleProper
    Condition
        // GuardsAlerted == false
    IfRequirementsNotMet
        // Go to: FightGuards
    Dialogue
        narrator
            You're in a brief path that goes to a keep of some sort.
            Ahead of you are stairs that lead up to an open doorway.
            It's hard to see inside with it being so bright out here, but you can surely see some tables and a grand entry hall through the doorway.
            There are palisade walls either side of the path. There are only two ways to go.
    Choices
        Enter the keep
            // Go to: Castle
        Return to the courtyard

FightGuards
    Dialogue
        narrator
            You go through the gate, but just as ou're walking through, a group of guards spring from either side of the gate!
            They must have heard you banging around with that hammer of yours!
    Choices
        Defend yourself!
            // Fight CastleGuards
            // [OnBattleEnd]
                // Go to: AfterGuardFight

AfterGuardFght
    Condition
        // WizardInParty == true
    IfRequirementsNotMet
        // Go to: NotWizardWarned
    Dialogue
        Ethrandil
            Let's hurry on, the king seems to have an endless supply of these goons.
            They've all surely been alerted by your incessant need to smash things with that hammer of yours!
        narrator
            You're in a brief path that goes to a keep of some sort.
            Ahead of you are stairs that lead up to an open doorway.
            It's hard to see inside with it being so bright out here, but you can surely see some tables and a grand entry hall through the doorway.
            There are palisade walls either side of the path. There are only two ways to go.
    Choices
        Enter the keep
        Return to the courtyard

NotWizardWarned
    Dialogue
        narrator
            After fighting the guards off, you have a chance to look around.
            You're in a brief path that goes to a keep of some sort.
            Ahead of you are stairs that lead up to an open doorway.
            It's hard to see inside with it being so bright out here, but you can surely see some tables and a grand entry hall through the doorway.
            There are palisade walls either side of the path. There are only two ways to go.
    Choices
        Enter the keep
        Return to the courtyard

Castle
    Dialogue
        narrator
            You ascend the staris to the keep, and as you walk through the doorway, the entry hall fades from your view, turning to grey.
            Instead, you walk into what looks to be the throne room.
            Scarlet curtains cover the walls and there is an enormous plush-cushioned chair in front of you.
            There is a large gilded chest to one side of the chair, and a grand painting of what seems to be a tusked ogre being fought back by a man in golden armor on the wall opposite.
            The room seems a bit excessive, fit for a king of a rich country.
    Choices
        Examine the chest
            // Go to: KingsChest
        Go back out the door
            // Go to: ThroneRoomToPath

KingsChest
    Condition
        // KingsChestOpened == false
    IfRequirementsNotMet
        // Go to: KingsChestAfterOpened
    Choices
        Have Ethrandil open the chest
            // CONDITION: WizardInParty == true
            // EFFECT:
                // KingsChestOpened = true
                // GiveItem("kings-sceptre")
                // GiveGold(random(10, 20) * random(9, 20))
                // RemoveFromParty(Ethrandil)
            // Go to: WizardsReward
        Open it with the key
            // CONDITION: UsedKey == false && HasItem("courtyard-dungeon-key")
            // EFFECT: UsedKey = true
            // Go to: KeyNotRight
        Smash open the chest
            // CONDITION: HasItem("war-hammer")
            // Go to: ChestWarded

ChestWarded
    Condition
        // WizardInParty == true
    IfRequirementsNotMet
        // Go to: ChestWardedUnclear
    Dialogue
        narrator
            The hammer bounces off the chest like a magnet bouncing off another magnet of the same polarity.
        Ethrandil
            Well that was silly. I told you that you'd need me to open it!
            It's sealed shut by magic, of course!
            What king would be foolish enough to leave their chest in the open without protecting it from a couple smites of a hammer?
    Choices
        Have Ethrandil open the chest
        Open it with the key
        Go back out the door

ChestWardedUnclear
    Dialogue
        narrator
            The hammer bounces off the chest like a magnet bouncing off another magnet of the same polarity.
            The chest glows a soft purple for a moment.
            It seems it's been warded against smashing.
    Choices
        Open it with the key
        Go back out the door

KeyNotRight
    Dialogue
        narrator
            A splendid idea!
            The key...
            The key doesn't fit.
    Choices
        Have Ethrandil open the chest
        Open it with the key
        Smash open the chest
        Go back out the door

KingsChestAfterOpened
    Dialogue
        narrator
            The chest stands open, the ward broken, as well as your friendship with the widard (short though it was).
    Choices
        Go back out the door

ThroneRoomToPath
    Dialogue
        narrator
            You leave the room, but as you walk through the doorway to the stairs, the world returns to that familiar grey.
            You are back on the familiar grey pathway, only the pathway is now black and the surrounding grey has streaks of blue and red.
    Choices
        Keep going forward

WizardsReward
    Dialogue
        narrator
            Ethrandil stoops down and mutters some words over the chest, waving his hands in strange motions.
            In a moment, he stands and taps the chest lightly with his foot.
            It springs open to reveal a pile of gold coins!
            The wizard reaches in and pulls out a white sceptre, tipped with a glowing turquoise jewel.
            He hands you the sceptre and gestures to the chest.
        Ethrandil
            This is for you, and the gold as well.
            A thanks for freeing me from the dungeon.
        narrator
            As you take the gold and the scepter, you hear clomping footsteps from the doorway behind you.
            A group of guards burst into the room, spreading out around you.
            You ready yourself to fight once more, but the group parts in the middle for someone else coming up the stairs.
            It is a man wearing a light brown cloak, similar to the wizard's.
            He has a silver circlet on his head and a bronze scepter in his hand, like the one you now hold.
            He looks at the wizard.
        The king?
            Ethrandil, I see you have been set free.
        narrator
            His gaze turning to you, the man continues.
        The king?
            You have made a grave error, sirs.
            I do not know what this wizard has told you, but I assure you it is naugh but lies.
            I do not believe you understand the evil you have set free.
        narrator
            Ethrandil's mouth turns up in a leering grin.
            When he speaks again, his voice is harsher.
        Ethrandil
            It was foolish to keep me locked up, Nadnruil.
            But then, these fools have righted your error.
        narrator
            He turns to look at you, a strange, crazed fire in his eyes.
        Ethrandil
            Goodbye.
            I thank you again for your aid.
        narrator
            With a wave of his hand, the scene vanishes from before you and you return to the path.
            Was it a mistake to free Ethrandil?
    Choices
        Walk along the path
            // Go to: TheNewPath
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
This time, the path only has two branches, both leading to the same spot. You have to take both before you can head back, which takes you to the final encounter and fight with Ethrandil
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -
-   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -   -

TheNewPath
    Dialogue
        narrator
            The path is different now, its color an inky black, and the void is darker, streaked with blue and red.
            Theres not much to do but walk.
            Eventually you come across a branching pathway.
            Are you up for more exploration?
            Hopefully nothing like that wizard.
    Choices
        Turn onto the new path
            // Go to: NewBranch
        Keep going forward
            // Go to: OnTheNewPath

OnTheNewPath
    Dialogue
        narrator
            The path continues on, without turn, the blackness seeming to get darker and deeper.
            Eventually you find yourself back at the same choice.
    Choices
        Turn onto the new path
        Keep going forward
        Retrace your steps
            // Go to: CaveSecondTime

CaveSecondTime
    Dialogue
        narrator
            You begin walking back the way you came.
            Eventually you see ahead the same split in the path you left from, looking in the opposite direction.
            As you approach it, it seems to get hazy and blurry.
            The path suddenly becomes insubstantial, and you start to fall through it.
            You feel the wind rush by, a remarkable feeling, considering you see nothing but blue-and-red streaked black and the shrinking grey line of the path as you fall away from it.
            Suddenly you're not falling anymore, but floating in the nothingness.
            The blue and red grow smaller, fainter, until there is nothing but black.
            You feel something solid under your feet.
            Your body grows heavier and your weight presses into your feet.
            Suddenly you realize, the world isn't black; your eyes are closed.
            You open your eyes, and to your dismay you see the path the same as you had left it: dark grey surrounded by void black smeared with blue and red.
    Choices
        Turn onto the new path
        Keep going forward
        Retrace your steps

NewBranch
    Dialogue
        narrator
            You don't even start walking. As soon as you turn towards the branch, you feel as if you're being sucked into the sky thorugh a tube.
            You see the throne room zooming past, the guards scattered on the floor and the other man—Nadrundil—standing alone before Ethrandil.
            The two are locked in what can only be described as a wizard battle
            Suddenly you stop.
            You stand before Ethrandil, the bodies of the guards nowhere to be seen.
            But Nadrundil lies on the floor.
            You fear the man is dead, but you see his chest rise and fall. He is sill alive.
            Suddenly you are standing in the room facing Ethrandil.
            Nadrundil is half crouching, half standing, holding his side where he has been badly burned.
        Nadrundil
            Help me!
            With you by my side, surely we can defeat Ethrandil and end his reign of terror!
        narrator
            Ethrandil smirks and looks at you as well.
        Ethrandil
            You could join this man in his mmiserable end.
            Or you could join me and take part in my new reign.
            What will it be?
    Choices
        Help Nadrundil defeat Ethrandil
            // AddMemberToParty(Nadrundil)
            // Fight Ethrandil and his crytalline golems
                // [OnBattleEnd]
                // Go to: EthrandilDefeated
        Join Ethrandil in his rise to power
            // AddMemberToParty(Ethrandil)
            // Fight Nadrundil and his royal gatekeepers
                // [OnBattleEnd]
                // Go to: NadrundilDefeated


EthrandilDefeated
    Dialogue
        narrator
            Ethrandil is no more. The ashes of his destruction lie scattered on the floor, along with that of the two golems he summoned.
            Nadrundil turns to you, looking exhausted and worn out.
        Nadrundil
            Thank you for your service.
            You have saved much more than this kingdom, you have given me another chance.
            You have my unending gratitude — I could not have done that alone.
            I grant you a favor.
            Anything up to one half of this kingdom will I bestow upon you, or a service of any kind.
    // End

NadrundilDefeated
    Dialogue
        narrator
            Nadrundil is destroyed.
            Ethrandil grins evilly toward you.
        Ethrandil
            You have made a wise decision.
            Come! Let us begin!
    // End
*/